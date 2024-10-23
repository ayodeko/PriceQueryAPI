namespace PriceQuery.Core.Interfaces;

/// <summary>
/// Interface for handling price requests.
/// </summary>
public interface IRequestHandler
{
    /// <summary>
    /// Get the price of an instrument using REST.
    /// </summary>
    /// <param name="instrument">The instrument to get the price for.</param>
    /// <returns>Response containing the price.</returns>
    Task<RestResponse> GetPriceRest(string instrument);
    
    /// <summary>
    /// Get the list of available instruments using REST.
    /// </summary>
    /// <returns>Response containing the list of instruments.</returns>
    Task<RestResponse> GetInstrumentsRest();
    
    /// <summary>
    /// Get the price of an instrument using WebSocket.
    /// </summary>
    /// <param name="instrument">The instrument to get the price for.</param>
    /// <returns>Tuple containing the instrument and the response.</returns>
    Task<Tuple<string, WebSocketResponse>> GetPriceWebSocket(string instrument);
}