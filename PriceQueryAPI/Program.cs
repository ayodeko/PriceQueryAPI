using PriceQuery.API;

var builder = WebApplication.CreateBuilder(args);
// Configure logging to use the console
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

//Inject all Services
builder.InjectServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Create Rest and WebSocket Endpoints
app.BuildEndpoints();

app.Run();