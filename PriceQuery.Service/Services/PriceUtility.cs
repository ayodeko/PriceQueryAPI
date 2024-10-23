using System.Collections.Concurrent;
using PriceQuery.Core.Interfaces;

namespace PriceQuery.Service.Services;

public class PriceUtility(ILogger<PriceUtility> logger) : IPriceUtility
{
    private readonly ILogger<PriceUtility> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ConcurrentDictionary<string, decimal> _prices = new();

    public Task<List<string>> GetInstruments()
    {
        var instruments = new List<string> { "BTCUSD", "ETHUSD", "EURUSD" };
        return Task.FromResult(instruments);
    }

    public async Task<decimal?> GetPrice(string instrument)
    {
        // Try to get the price for the given instrument
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

        // Update the price for the instrument
        _prices[instrument] = price;
        _logger.LogInformation("Updated price for instrument {Instrument}: {Price}", instrument, price);
    }
}