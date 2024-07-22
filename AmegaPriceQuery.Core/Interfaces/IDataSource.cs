namespace AmegaPriceQuery.Core.Interfaces;

public interface IDataSource
{
    event Action<string> OnMessageReceived;
    Task ConnectToSocketAsync();
    Task DisconnectFromSocketAsync();
    Task ReceiveMessageFromSocket(CancellationToken token);
    Task BeginPriceDataStreamingAsync(string instrument);
    Task StopPriceDataStreamingAsync(string instrument);
}