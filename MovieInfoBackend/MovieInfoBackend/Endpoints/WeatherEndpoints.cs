using Microsoft.AspNetCore.Authorization;
using MovieInfoBackend.Helpers;

namespace MovieInfoBackend.Endpoints;

// TODO: Remove me when any real endpoints are added, this is just for testing
public class WeatherEndpoints
{
    public static void Map(WebApplication app)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/api/weatherforecast",[Authorize] () =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                        new WeatherForecast
                        (
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            summaries[Random.Shared.Next(summaries.Length)]
                        ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .RequireAuthorization(ProgramConstants.LoggedInUsersOnlyPolicyName) // TODO: Move these policies to a real endpoint and get rid of this test endpoint
            .RequireAuthorization(ProgramConstants.SearchUsersOnlyPolicyName);
    }
    
    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}