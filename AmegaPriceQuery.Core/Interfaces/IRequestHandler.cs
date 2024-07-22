namespace AmegaPriceQuery.Core.Interfaces;

public interface IRequestHandler
{
    Task<RestResponse> GetPriceRest(string instrument);
    Task<RestResponse> GetInstrumentsRest();
    Task<Tuple<string, WebSocketResponse>> GetPriceWebSocket(string instrument);
}
