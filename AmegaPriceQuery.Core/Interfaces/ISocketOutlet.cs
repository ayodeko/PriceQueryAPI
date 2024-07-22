namespace AmegaPriceQuery.Core.Interfaces;

public interface ISocketOutlet
{
    Task Subscribe(string instrument);
    Task UnSubscribe(string instrument);
    Task Connect(string instrument);
    Task Disconnect(string instrument);
    event Action OnMessageReceived;
    void MapEndpoint(WebApplication app);
}