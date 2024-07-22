namespace AmegaPriceQuery.Core.Interfaces;

/// <summary>
/// Interface for data source communication.
/// </summary>
public interface IDataSource
{
    /// <summary>
    /// Event triggered when a message is received.
    /// </summary>
    event Action<string> OnMessageReceived;
    
    /// <summary>
    /// Connect to the WebSocket.
    /// </summary>
    Task ConnectToSocketAsync();
    
    /// <summary>
    /// Disconnect from the WebSocket.
    /// </summary>
    Task DisconnectFromSocketAsync();
    
    /// <summary>
    /// Receive messages from the WebSocket.
    /// </summary>
    /// <param name="token">Cancellation token for the operation.</param>
    Task ReceiveMessageFromSocket(CancellationToken token);
    
    /// <summary>
    /// Start streaming price data for a given instrument.
    /// </summary>
    /// <param name="instrument">The instrument to start streaming data for.</param>
    Task BeginPriceDataStreamingAsync(string instrument);
    
    /// <summary>
    /// Stop streaming price data for a given instrument.
    /// </summary>
    /// <param name="instrument">The instrument to stop streaming data for.</param>
    Task StopPriceDataStreamingAsync(string instrument);
}