namespace AmegaPriceQuery.Core.Interfaces;

public interface IDataSource
{
    Task BeginPriceDataStreaming();
    void StopPriceDataStreaming();
}