namespace AmegaPriceQuery.Core.Interfaces;

public interface IPriceUtility
{
    Task<List<string>> GetInstruments();
    Task<decimal> GetPrice(string instrument);
    Task UpdatePrice(string instrument);
}