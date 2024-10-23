namespace PriceQuery.Core.Interfaces;

/// <summary>
/// Interface for price utility operations.
/// </summary>
public interface IPriceUtility
{
    /// <summary>
    /// Get the list of available instruments.
    /// </summary>
    /// <returns>List of instruments.</returns>
    Task<List<string>> GetInstruments();
    
    /// <summary>
    /// Get the price of a given instrument.
    /// </summary>
    /// <param name="instrument">The instrument to get the price for.</param>
    /// <returns>The price of the instrument.</returns>
    Task<decimal?> GetPrice(string instrument);
    
    /// <summary>
    /// Update the price of a given instrument.
    /// </summary>
    /// <param name="instrument">The instrument to update the price for.</param>
    /// <param name="price">The new price of the instrument.</param>
    Task UpdatePrice(string instrument, decimal price);
}