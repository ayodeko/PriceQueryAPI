using System.Collections.Concurrent;
using PriceQuery.Core.Interfaces;
using Newtonsoft.Json.Linq;

namespace PriceQuery.Service.Services;

public class SubscriptionManager : IPriceChannel
{
    private readonly ILogger<SubscriptionManager> _logger;
    private readonly IDataSource _dataSource;
    private readonly IPriceUtility _priceUtility;
    public event Action<string, decimal>? BroadcastPriceEvent;

    public SubscriptionManager(IDataSource dataSource, IPriceUtility priceUtility, ILogger<SubscriptionManager> logger)
    {
        _dataSource = dataSource;
        _logger = logger;
        subscriptions = new ConcurrentDictionary<string, int>();
        _priceUtility = priceUtility;
        _dataSource.OnMessageReceived += ReceivePriceFromSource;
    }
    
    public ConcurrentDictionary<string, int> subscriptions { get; }

    public async Task SubscribeAsync(string instrument)
    {
        instrument = instrument.ToLower();
        if (subscriptions.ContainsKey(instrument))
        {
            // Increment the subscription count for the instrument
            subscriptions[instrument]++;
            _logger.LogInformation("Incremented subscription count for {Instrument}.", instrument);
        }
        else
        {
            await _dataSource.ConnectToSocketAsync();
            _logger.LogInformation("Connected to WebSocket for subscribing to {Instrument}.", instrument);

            // Start receiving messages in the background
            _ = Task.Run((Func<Task>)(async () => await _dataSource.ReceiveMessageFromSocket(new CancellationTokenSource().Token)));

            await _dataSource.BeginPriceDataStreamingAsync(instrument);
            subscriptions.TryAdd(instrument, 1);
            _logger.LogInformation("Subscribed to {Instrument}.", instrument);
        }
    }

    public async Task UnsubscribeAsync(string instrument)
    {
        instrument = instrument.ToLower();
        if (subscriptions.ContainsKey(instrument))
        {
            subscriptions[instrument]--;
            _logger.LogInformation("Decremented subscription count for {Instrument}.", instrument);
            if (subscriptions[instrument] == 0)
            {
                subscriptions.TryRemove(instrument, out _);
                await _dataSource.StopPriceDataStreamingAsync(instrument);
                _logger.LogInformation("Unsubscribed from {Instrument}.", instrument);
            }
        }

        if (subscriptions.IsEmpty)
        {
            await _dataSource.DisconnectFromSocketAsync();
            _logger.LogInformation("Disconnected from WebSocket as there are no active subscriptions.");
        }
    }

    public void ReceivePriceFromSource(string message)
    {
        var response = ParsePrice(message);
        _priceUtility.UpdatePrice(response.Item1, response.Item2);
        BroadcastPrice(response.Item1, response.Item2);
    }

    public void BroadcastPrice(string instrument, decimal price)
    {
        BroadcastPriceEvent?.Invoke(instrument, price);
        _logger.LogInformation("Broadcasted price update for {Instrument}: {Price}.", instrument, price);
    }

    Tuple<string, decimal> ParsePrice(string message)
    {
        try
        {
            var json = JObject.Parse(message);

            // Ensure the message is a trade update
            if (json["e"]?.ToString() != "aggTrade")
            {
                _logger.LogWarning("Received non-trade message: {Message}.", message);
                return new Tuple<string, decimal>("error", 0);
            }

            var symbol = json["s"].ToString();
            var price = decimal.Parse(json["p"]?.ToString() ?? "0");

            return new Tuple<string, decimal>(symbol.ToLower(), price);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error encountered during attempt to parse price");
            return new Tuple<string, decimal>("error", 0);
        }
    }
}