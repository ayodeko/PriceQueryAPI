using AmegaPriceQuery.Core;
using AmegaPriceQuery.Core.Interfaces;

namespace AmegaPriceQuery.Service.Services;


public class RequestHandler(IPriceChannel channel, IPriceUtility utility, ILogger<RequestHandler> logger)
    : IRequestHandler
{
    public async Task<RestResponse> GetInstrumentsRest()
    {
        try
        {
            var result = await utility.GetInstruments();
            var response = new RestResponse(result, 
                "Retrieved", ResponseCode.Successful);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error encountered while getting price");
            var response = new RestResponse(null, 
                "An error occured while retrieving the instruments", ResponseCode.Failed);
            return response;
        }
    }

    public async Task<Tuple<string, WebSocketResponse>> GetPriceWebSocket(string instrument)
    {
        
        try
        {
            var price = await utility.GetPrice(instrument);
            if (price != null)
            {
                
                var response = new WebSocketResponse(new { instrument, price }, 
                    "Retrieved", ResponseCode.Successful);
                return new Tuple<string, WebSocketResponse>(instrument, response);
            }
            else
            {
                await channel.SubscribeAsync(instrument);
                logger.LogError("Instrument {Instrument} not found", instrument);
                var response = new WebSocketResponse(null, 
                    $"Price for {instrument} not found, please try again later", ResponseCode.Failed);
                return new Tuple<string, WebSocketResponse>(instrument, response);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error encountered while getting price");
            throw;
        }
    }

    public async Task<RestResponse> GetPriceRest(string instrument)
    {
        try
        {
            var price = await utility.GetPrice(instrument);
            if (price != null)
            {
                
                var response = new RestResponse(new { instrument, price }, 
                    "Retrieved", ResponseCode.Successful);
                return response;
            }
            else
            {
                await channel.SubscribeAsync(instrument);
                logger.LogError("Instrument {Instrument} not found", instrument);
                var response = new RestResponse(null, 
                    $"Price for {instrument} not found, please try again later", ResponseCode.Failed);
                return response;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error encountered while getting price");
            throw;
        }
    }
}