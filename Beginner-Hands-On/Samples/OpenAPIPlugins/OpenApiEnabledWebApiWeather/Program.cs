
using System.ComponentModel;
using System.Text.Json;
using WebApiWeather;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddAuthorization()
    .AddLogging()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

var jsonOptions = new JsonSerializerOptions { WriteIndented = true };

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)]
        })
        .ToArray(); // Materialize the result

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"""
    ================================================
    Response body: 
            
    {JsonSerializer.Serialize(forecast, jsonOptions)}
    ================================================
    """);
    Console.ResetColor();

    return forecast;
})
.WithDescription("Get a weather forecast for the next 5 days.")
.WithSummary("Get Weather Forecast")
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();
