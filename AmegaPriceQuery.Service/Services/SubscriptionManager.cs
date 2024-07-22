using System.Collections.Concurrent;
using AmegaPriceQuery.Core.Interfaces;
using Newtonsoft.Json.Linq;

namespace AmegaPriceQuery.Service.Services;

public class SubscriptionManager : IPriceChannel
{
    private readonly ILogger<SubscriptionManager> _logger;
    private readonly ConcurrentDictionary<string, int> _subscriptions;
    private readonly object _lock = new object();
    private readonly IDataSource _dataSource;
    private readonly IPriceUtility _priceUtility;
    

    public SubscriptionManager(IDataSource dataSource, IPriceUtility priceUtility, ILogger<SubscriptionManager> logger)
    {
        _dataSource = dataSource;
        _logger = logger;
        _subscriptions = new ConcurrentDictionary<string, int>();
        _priceUtility = priceUtility;
        _dataSource.OnMessageReceived += BroadcastPrice;
    }


    public async Task SubscribeAsync(string instrument)
    {
        instrument = instrument.ToLower();
        if (_subscriptions.ContainsKey(instrument))
        {
            _subscriptions[instrument]++;
        }
        else
        {
            _subscriptions.TryAdd(instrument, 1);
            await _dataSource.ConnectToSocketAsync();
            // Start receiving messages in the background
            _ = Task.Run((Func<Task>)(async () => await _dataSource.ReceiveMessageFromSocket(new CancellationTokenSource().Token)));

            await _dataSource.BeginPriceDataStreamingAsync(instrument);
        }
    }

    public async Task UnsubscribeAsync(string instrument)
    {
        instrument = instrument.ToLower();
        if (_subscriptions.ContainsKey(instrument))
        {
            _subscriptions[instrument]--;
            if (_subscriptions[instrument] == 0)
            {
                _subscriptions.TryRemove(instrument, out _);
                await _dataSource.StopPriceDataStreamingAsync(instrument);
            }
        }
        
        if (_subscriptions.IsEmpty)
        {
            await _dataSource.DisconnectFromSocketAsync();
        }
    }

    public void BroadcastPrice(string message)
    {
        var response = ParsePrice(message);
        _priceUtility.UpdatePrice(response.Item1, response.Item2);
    }

    Tuple<string, decimal> ParsePrice(string message)
    {
        try
        {
            var json = JObject.Parse(message);

            // Ensure the message is a trade update
            if (json["e"]?.ToString() != "aggTrade")
            {
                return new System.Tuple<string, decimal>("error", 0);
            }

            var symbol = json["s"].ToString();
            var price = decimal.Parse(json["p"]?.ToString() ?? "0");

            return new Tuple<string, decimal>(symbol.ToLower(), price);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error encountered during attempt to parse price");
            return new System.Tuple<string, decimal>("error", 0);
        }
    }

}