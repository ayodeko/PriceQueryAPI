﻿using AmegaPriceQuery.Core;
using AmegaPriceQuery.Core.Interfaces;
using AmegaPriceQuery.Service.Services;
using Microsoft.AspNetCore.SignalR;

namespace AmegaPriceQuery.Service;


public class SignalRWebSocketHandler : ISocketOutlet
{
    private readonly IHubContext<PriceHub> _hubContext;
    private readonly ILogger<SignalRWebSocketHandler> _logger;
    private readonly IPriceChannel _priceChannel;
    private readonly IPriceUtility _utility;

    public event Action? OnMessageReceived;

    public SignalRWebSocketHandler(IHubContext<PriceHub> hubContext, ILogger<SignalRWebSocketHandler> logger, IPriceChannel priceChannel,
        IPriceUtility utility)
    {
        _hubContext = hubContext;
        _logger = logger;
        _priceChannel = priceChannel;
        _utility = utility;
        _priceChannel.BroadcastPriceEvent += OnPriceReceived;
    }

    

    private async void OnPriceReceived(string instrument, decimal inputPrice)
    {
        try
        {
            var price = await _utility.GetPrice(instrument);
            if (price != null)
            {
                var response = new WebSocketResponse(new { instrument, price }, 
                    "Retrieved", ResponseCode.Successful);
                await PriceHub.BroadcastPrice(_hubContext, instrument, response);
            }
            else
            {
                _logger.LogError("Instrument {Instrument} not found", instrument);
                var response = new WebSocketResponse(null, 
                    $"Price for {instrument} not found, please try again later", ResponseCode.Failed);
                await PriceHub.BroadcastPrice(_hubContext, instrument, response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error broadcasting price update to SignalR Hub.");
        }
    }
    
    
    
    //SignalR already handles these methods
    public Task Subscribe(string instrument)
    {
        throw new NotImplementedException();
    }

    public Task UnSubscribe(string instrument)
    {
        throw new NotImplementedException();
    }

    public Task Connect(string instrument)
    {
        throw new NotImplementedException();
    }

    public Task Disconnect(string instrument)
    {
        throw new NotImplementedException();
    }

    public void MapEndpoint(WebApplication  app)
    {
        app.MapHub<PriceHub>("/signalrhub");
    }
}