// Here we have multiple concrete class dependencies which are abstracted by same interface and we are resolving dependencies based on string passed

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<RainyService>();
builder.Services.AddScoped<SunnyService>();

builder.Services.AddScoped<Func<string, IWeatherService>>(serviceProvider => key =>
{
    switch (key)
    {
        case "sunny": return serviceProvider.GetRequiredService<SunnyService>();
        case "rainy": return serviceProvider.GetRequiredService<RainyService>();
         default: throw new ArgumentException($"Unknown weather type: {key}");
    }
    ;
});


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
app.MapGet("/{type}", (Func<string, IWeatherService> factory, string type) =>
{
    var service = factory(type); // Resolve the correct service
    return service.GetForecast(); // Call method
});


app.Run();


public interface IWeatherService
{
    object GetForecast();
}

public class RainyService : IWeatherService
{
    public object GetForecast()
    {
        return new { weather = "Rainy" }; 
    }
}
public class SunnyService : IWeatherService
{
    public object GetForecast()
    {
        return new { weather = "Sunny" }; 
    }
}

