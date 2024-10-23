using System.Collections.Concurrent;

namespace PriceQuery.Core.Interfaces;

/// <summary>
/// Interface for price channel operations.
/// </summary>
public interface IPriceChannel
{
    /// <summary>
    /// Dictionary to manage subscriptions.
    /// </summary>
    ConcurrentDictionary<string, int> subscriptions { get; }

    /// <summary>
    /// Subscribe to a given instrument.
    /// </summary>
    /// <param name="instrument">The instrument to subscribe to.</param>
    Task SubscribeAsync(string instrument);
    
    /// <summary>
    /// Unsubscribe from a given instrument.
    /// </summary>
    /// <param name="instrument">The instrument to unsubscribe from.</param>
    Task UnsubscribeAsync(string instrument);
    
    /// <summary>
    /// Broadcast price updates.
    /// </summary>
    /// <param name="instrument">The instrument being broadcasted.</param>
    /// <param name="price">The price of the instrument.</param>
    void BroadcastPrice(string instrument, decimal price);
    
    /// <summary>
    /// Event triggered when a price is broadcasted.
    /// </summary>
    event Action<string, decimal> BroadcastPriceEvent;
    
    /// <summary>
    /// Receive price updates from the data source.
    /// </summary>
    /// <param name="message">The message received from the data source.</param>
    void ReceivePriceFromSource(string message);
}