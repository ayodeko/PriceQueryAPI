using System.Collections.Concurrent;
using PriceQuery.Core;
using PriceQuery.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace PriceQuery.Service.Services;

public class PriceHub(IPriceChannel priceChannel, ILogger<PriceHub> logger) : Hub
{
    
    public async Task Subscribe(string instrument)
    {
        var connectionId = Context.ConnectionId;

        // Add the client to the SignalR group for the instrument
        await Groups.AddToGroupAsync(connectionId, instrument);

        // Subscribe to the instrument using the price channel
        await priceChannel.SubscribeAsync(instrument);

        await Clients.Caller.SendAsync("Subscribed", instrument);
        logger.LogInformation("Client {ConnectionId} subscribed to {Instrument}.", connectionId, instrument);
    }

    public async Task Unsubscribe(string instrument)
    {
        var connectionId = Context.ConnectionId;

        // Remove the client from the SignalR group for the instrument
        await Groups.RemoveFromGroupAsync(connectionId, instrument);

        // Unsubscribe from the instrument using the price channel
        await priceChannel.UnsubscribeAsync(instrument);

        await Clients.Caller.SendAsync("Unsubscribed", instrument);
        logger.LogInformation("Client {ConnectionId} unsubscribed from {Instrument}.", connectionId, instrument);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;

        // Remove the client from all groups they were subscribed to
        foreach (var groupName in Context.Items.Values)
        {
            await Groups.RemoveFromGroupAsync(connectionId, groupName?.ToString());
        }

        await base.OnDisconnectedAsync(exception);
        logger.LogInformation("Client {ConnectionId} disconnected.", connectionId);
    }

    public static async Task BroadcastPrice(IHubContext<PriceHub> hubContext, string instrument, WebSocketResponse response)
    {
        // Broadcast price updates to subscribed clients
        await hubContext.Clients.Group(instrument).SendAsync("ReceivePrice", response);
    }
}
