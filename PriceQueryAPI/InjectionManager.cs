using System.Net.WebSockets;
using PriceQuery.Core.Interfaces;
using PriceQuery.Service;
using PriceQuery.Service.Services;

namespace PriceQuery.API;

public static class InjectionManager
{
    public static WebApplicationBuilder InjectServices(this WebApplicationBuilder builder)
    {
        //Add swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        //Inject services to app
        builder.Services.AddSingleton<IPriceChannel, SubscriptionManager>();
        builder.Services.AddSingleton<IDataSource, BinanceDataSource>();
        builder.Services.AddSingleton<IPriceUtility, PriceUtility>();
        builder.Services.AddSingleton<ISocketOutlet, SignalRWebSocketHandler>();
        builder.Services.AddScoped<IRequestHandler, RequestHandler>();
        builder.Services.AddSingleton<ClientWebSocket>();
        
        //Use SignalR
        builder.Services.AddSignalR();
        return builder;
    }
}