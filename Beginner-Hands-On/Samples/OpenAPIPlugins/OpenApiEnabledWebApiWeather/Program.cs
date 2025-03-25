using Microsoft.OpenApi.Models;
using System.Text.Json;
using WebApiWeather;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddAuthorization()
    .AddLogging()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Weather API",
            Version = "v1",
            Description = "A simple weather forecast API"
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather API v1");
    });
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
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();
