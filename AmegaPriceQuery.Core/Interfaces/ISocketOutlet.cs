namespace AmegaPriceQuery.Core.Interfaces;

/// <summary>
/// Interface for managing WebSocket outlets.
/// </summary>
public interface ISocketOutlet
{
    /// <summary>
    /// Subscribe to a given instrument.
    /// </summary>
    /// <param name="instrument">The instrument to subscribe to.</param>
    Task Subscribe(string instrument);
    
    /// <summary>
    /// Unsubscribe from a given instrument.
    /// </summary>
    /// <param name="instrument">The instrument to unsubscribe from.</param>
    Task UnSubscribe(string instrument);
    
    /// <summary>
    /// Connect to a WebSocket.
    /// </summary>
    Task Connect();
    
    /// <summary>
    /// Disconnect from a WebSocket.
    /// </summary>
    Task Disconnect();
    
    /// <summary>
    /// Event triggered when a message is received.
    /// </summary>
    event Action OnMessageReceived;
    
    /// <summary>
    /// Map the WebSocket endpoint.
    /// </summary>
    /// <param name="app">The web application to map the endpoint to.</param>
    void MapEndpoint(WebApplication app);
}