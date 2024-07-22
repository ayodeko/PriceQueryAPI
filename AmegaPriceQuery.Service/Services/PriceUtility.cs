using System.Collections.Concurrent;
using AmegaPriceQuery.Core.Interfaces;

namespace AmegaPriceQuery.Service.Services;

public class PriceUtility : IPriceUtility
{
    private readonly ILogger<PriceUtility> _logger;
    private readonly ConcurrentDictionary<string, decimal> _prices;

    public PriceUtility(ILogger<PriceUtility> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _prices = new ConcurrentDictionary<string, decimal>();
    }
    
    public Task<List<string>> GetInstruments()
    {
        var instruments = new List<string> { "BTCUSD", "ETHUSD", "EURUSD" };
        return Task.FromResult(instruments);
    }

    public async Task<decimal?> GetPrice(string instrument)
    {
        if (_prices.TryGetValue(instrument, out var price))
        {
            return price;
        }
        return null;
    }

    public async Task UpdatePrice(string instrument, decimal price)
    {
        if (string.IsNullOrWhiteSpace(instrument))
        {
            throw new ArgumentException("Instrument cannot be null or whitespace.", nameof(instrument));
        }

        if (price < 0)
        {
            throw new ArgumentException("Price cannot be negative.", nameof(price));
        }

        _prices[instrument] = price;
        _logger.LogInformation("Updated price for instrument {Instrument}: {Price}", instrument, price);
    }
}