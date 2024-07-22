using AmegaPriceQuery.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AmegaPriceQuery.API;

public static class Endpoints
{
    public static WebApplication BuildEndpoints(this WebApplication app)
    {
        //Rest Endpoints
        app.MapGet("api/GetInstruments", ([FromServices] IRequestHandler handler)
            => handler.GetInstrumentsRest());
        app.MapGet("api/GetPrice", ([FromServices] IRequestHandler handler, string instrument)
            => handler.GetPriceRest(instrument));

        //Create the WebSocket Endpoint
        var webSocketHandler = app.Services.GetRequiredService<ISocketOutlet>();
        webSocketHandler.MapEndpoint(app);
        return app;
    }
}