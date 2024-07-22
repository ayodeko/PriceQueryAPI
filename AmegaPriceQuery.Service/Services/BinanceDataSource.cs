using System.Net.WebSockets;
using System.Text;
using AmegaPriceQuery.Core.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AmegaPriceQuery.Service.Services;

public class BinanceDataSource : IDataSource
{
    private readonly IConfiguration _configuration;
    private readonly IPriceUtility _priceUtility;
    private readonly ILogger<BinanceDataSource> _logger;
    private CancellationTokenSource _cancellationTokenSource;
    private ClientWebSocket _webSocket;


    public BinanceDataSource(IPriceUtility priceUtility, IConfiguration configuration, ILogger<BinanceDataSource> logger, ClientWebSocket clientWebSocket)
    {
        _priceUtility = priceUtility;
        _configuration = configuration;
        _webSocket = clientWebSocket;
        _logger = logger;
    }

    public event Action<string>? OnMessageReceived;

    public async Task DisconnectFromSocketAsync()
    {
        try
        {
            _cancellationTokenSource.Cancel();
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            _webSocket.Dispose();
            _logger.LogInformation("Disconnected from Binance WebSocket.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disconnecting from Binance WebSocket.");
        }
    }

    public async Task BeginPriceDataStreamingAsync(string instrument)
    {
        await SendSubscriptionMessageAsync(instrument, "SUBSCRIBE");
    }

    public async Task StopPriceDataStreamingAsync(string instrument)
    {
        await SendSubscriptionMessageAsync(instrument, "UNSUBSCRIBE");
    }

    public async Task ConnectToSocketAsync()
    {
        if (_webSocket.State == WebSocketState.Open)
        {
            return; // Already connected
        }
        _cancellationTokenSource = new CancellationTokenSource();
        var url = new Uri(_configuration["WebSocketSettings:BinanceWebSocketUrl"] ??
                          throw new InvalidOperationException("Binance websocket url is null"));
        try
        {
            await _webSocket.ConnectAsync(url, _cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task SendSubscriptionMessageAsync(string instrument, string action)
    {
        var subscriptionMessage = new
        {
            method = action,
            @params = new[] { $"{instrument}@aggTrade" },
            id = 1
        };

        var message = JsonConvert.SerializeObject(subscriptionMessage);
        var bytes = Encoding.UTF8.GetBytes(message);

        try
        {
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
                CancellationToken.None);
            _logger.LogInformation("Sent {Action} message for {Instrument} to Binance WebSocket.", action, instrument);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending {Action} message for {Instrument}.", action, instrument);
        }
    }

    public async Task ReceiveMessageFromSocket(CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];
        try
        {
            while (_webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine("Socket Message " + message);
                        HandleMessage(message);
                        break;
                    }
                    case WebSocketMessageType.Close:
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing",
                            CancellationToken.None);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while receiving message from socket.");
        }
    }

    private void HandleMessage(string message)
    {
        try
        {
            var json = JObject.Parse(message);

            // Check if the message is a subscription response
            if (json["result"] != null && json["id"] != null)
            {
                _logger.LogInformation("Received subscription response: {Message}", message);
            }
            else
            {
                OnMessageReceived?.Invoke(message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling received message: {Message}", message);
        }
    }
}