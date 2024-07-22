namespace AmegaPriceQuery.Core.Interfaces;

public interface IPriceChannel
{
    Task SubscribeAsync(string instrument);
    Task UnsubscribeAsync(string instrument);
    void BroadcastPrice(string message);
}