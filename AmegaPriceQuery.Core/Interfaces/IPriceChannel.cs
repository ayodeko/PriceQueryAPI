namespace AmegaPriceQuery.Core.Interfaces;

public interface IPriceChannel
{
    Task Subscribe(string instrument);
    Task Unsubscribe(string instrument);
    Task BroadcastPrice(string instrument, decimal price);
}