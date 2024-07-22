using System.Collections.Concurrent;

namespace AmegaPriceQuery.Core.Interfaces;

public interface IPriceChannel
{
    ConcurrentDictionary<string, int> subscriptions { get; }


    Task SubscribeAsync(string instrument);
    Task UnsubscribeAsync(string instrument);
    void BroadcastPrice(string instrument, decimal price);
    public event Action<string, decimal> BroadcastPriceEvent;
    void ReceivePriceFromSource(string message);
}