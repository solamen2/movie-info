using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieInfoBackend.Helpers;
using static MovieInfoBackend.Helpers.ProgramConstants;  // for ApiRoutePrefix
using MovieInfoBackend.ViewModels;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace MovieInfoBackend.Endpoints;

[ExcludeFromCodeCoverage]
public class OscarsEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet($"{ApiRoutePrefix}/oscars", [Authorize]
            async (
                ClaimsPrincipal user,
                [FromServices] OscarsHttpClient oscarsHttpClient) =>
            {
                try
                {
                    OscarsViewModel? oscarsViewModel = await GetOscarsViewModel(oscarsHttpClient);
                    if (oscarsViewModel == null)
                    {
                        return Results.Problem(detail: "The Academy Awards data could not be loaded.", statusCode: StatusCodes.Status503ServiceUnavailable);
                    }

                    string username = user?.Identity?.Name ?? "<no username found>";
                    Log.Debug($"Username: {username}");

                    return Results.Json(oscarsViewModel.Years);
                }
                catch (Exception e)
                {
                    string username = user.Identity?.Name ?? "<no username found>";

                    Log.ForContext("Username", username)
                        .Error(e, "An error occurred while processing the /oscars request.");

                    throw;
                }
            }
        )
        .WithSummary("Oscars")
        .WithDescription("Lists the Academy Award winners and nominees of every ceremony, by year.")
        .RequireAuthorization(ProgramConstants.LoggedInUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireAuthorization(ProgramConstants.SearchUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireRateLimiting(ProgramConstants.TokenRateLimiterPolicyName);
    }

    // Null when the dataset couldn't be downloaded or parsed, which the callers treat as the Oscars being unavailable
    // rather than as an error
    public async static Task<OscarsViewModel?> GetOscarsViewModel(OscarsHttpClient oscarsHttpClient)
    {
        OscarsViewModel? oscarsViewModel;
        try
        {
            oscarsViewModel = await oscarsHttpClient.GetOscars();
        }
        catch (Exception e) when (e is HttpRequestException || e is TaskCanceledException || e is Polly.CircuitBreaker.BrokenCircuitException)
        {
            Log.Warning(e, "Downloading the Academy Awards data failed.");
            return null;
        }

        if (oscarsViewModel == null)
        {
            Log.Warning("Oscars data unavailable: the Academy Awards dataset could not be downloaded or parsed.");
        }
        else
        {
            Log.Debug($"Oscars ceremonies: {oscarsViewModel.Years.Count}");
        }

        return oscarsViewModel;
    }
}
