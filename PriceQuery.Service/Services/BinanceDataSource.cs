using System.Net.WebSockets;
using System.Text;
using PriceQuery.Core.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PriceQuery.Service.Services;

public class BinanceDataSource(
    IConfiguration configuration,
    ILogger<BinanceDataSource> logger,
    ClientWebSocket clientWebSocket)
    : IDataSource
{
    private CancellationTokenSource _cancellationTokenSource;
    private ClientWebSocket? _webSocket = clientWebSocket;

    public event Action<string>? OnMessageReceived;

    public async Task DisconnectFromSocketAsync()
    {
        try
        {
            // Cancel the current operations
            await _cancellationTokenSource.CancelAsync();
            // Close the WebSocket connection
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            logger.LogInformation("Disconnected from Binance WebSocket.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error disconnecting from Binance WebSocket.");
        }
        finally
        {
            _webSocket?.Dispose();
            _webSocket = null;
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
        _webSocket ??= new ClientWebSocket();
        if (_webSocket.State == WebSocketState.Open)
        {
            logger.LogInformation("WebSocket already connected.");
            return; // Already connected
        }
        _cancellationTokenSource = new CancellationTokenSource();
        var url = new Uri(configuration["WebSocketSettings:BinanceWebSocketUrl"] ??
                          throw new InvalidOperationException("Binance websocket url is null"));
        try
        {
            // Connect to the WebSocket
            await _webSocket.ConnectAsync(url, _cancellationTokenSource.Token);
            logger.LogInformation("Connected to Binance WebSocket.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error connecting to Binance WebSocket.");
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
            // Send subscription message to the WebSocket
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
                CancellationToken.None);
            logger.LogInformation("Sent {Action} message for {Instrument} to Binance WebSocket.", action, instrument);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while sending {Action} message for {Instrument}.", action, instrument);
        }
    }

    public async Task ReceiveMessageFromSocket(CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];
        try
        {
            // Receive messages from the WebSocket
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
            logger.LogError(ex, $"Error while receiving message from socket.");
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
                logger.LogInformation("Received subscription response: {Message}", message);
            }
            else
            {
                // Trigger the event for the received message
                OnMessageReceived?.Invoke(message);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling received message: {Message}", message);
        }
    }
}
