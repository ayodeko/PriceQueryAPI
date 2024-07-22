using System.Net.WebSockets;
using AmegaPriceQuery.Core.Interfaces;
using AmegaPriceQuery.Service.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
// Configure logging to use the console
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IPriceChannel, SubscriptionManager>();
builder.Services.AddSingleton<IDataSource, BinanceDataSource>();
builder.Services.AddSingleton<IPriceUtility, PriceUtility>();
builder.Services.AddScoped<IRequestHandler, RequestHandler>();
builder.Services.AddSingleton<ClientWebSocket>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("api/GetInstruments", ([FromServices] IRequestHandler handler)
    => handler.GetInstrumentsRest());

app.MapGet("api/GetPrice", ([FromServices] IRequestHandler handler, string instrument)
    => handler.GetPriceRest(instrument));

app.Run();
