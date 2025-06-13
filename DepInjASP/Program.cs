using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Register services in the DI container => whenever someone asks IWeatherService, create instance of WeatherService 
builder.Services.AddScoped<IWeatherService, WeatherService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Injecting IWeatherService
app.MapGet("/", (IWeatherService service) =>
{
    return service.GetForecast();
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();


public interface IWeatherService
{
    object GetForecast();
}

public class WeatherService : IWeatherService
{
    public object GetForecast()
    {
        return new { weather = "Sunny" }; 
    }
}
