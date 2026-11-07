using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieInfoBackend.DataModels;
using MovieInfoBackend.Helpers;
using static MovieInfoBackend.Helpers.ProgramConstants;  // for ApiRoutePrefix
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace MovieInfoBackend.Endpoints;

[ExcludeFromCodeCoverage]
public class InflationEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet($"{ApiRoutePrefix}/inflation", [Authorize]
            async (
                ClaimsPrincipal user,
                [FromServices] InflationHttpClient inflationHttpClient) =>
            {
                try
                {
                    InflationDataModel? inflationDataModel = await GetInflationDataModel(inflationHttpClient);
                    if (inflationDataModel == null)
                    {
                        return Results.Problem(detail: "The inflation data could not be loaded.", statusCode: StatusCodes.Status503ServiceUnavailable);
                    }

                    string username = user?.Identity?.Name ?? "<no username found>";
                    Log.Debug($"Username: {username}");

                    return Results.Json(inflationDataModel);
                }
                catch (Exception e)
                {
                    string username = user.Identity?.Name ?? "<no username found>";

                    Log.ForContext("Username", username)
                        .Error(e, "An error occurred while processing the /inflation request.");

                    throw;
                }
            }
        )
        .WithSummary("Inflation")
        .WithDescription("The Consumer Price Index series (from the Bureau of Labor Statistics) that movie budgets and revenues are adjusted for inflation with.")
        .RequireAuthorization(ProgramConstants.LoggedInUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireAuthorization(ProgramConstants.SearchUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireRateLimiting(ProgramConstants.TokenRateLimiterPolicyName);
    }

    // Null when either series couldn't be downloaded or parsed, which the callers treat as the inflation data being
    // unavailable rather than as an error
    public async static Task<InflationDataModel?> GetInflationDataModel(InflationHttpClient inflationHttpClient)
    {
        InflationDataModel? inflationDataModel;
        try
        {
            inflationDataModel = await inflationHttpClient.GetInflation();
        }
        catch (Exception e) when (e is HttpRequestException || e is TaskCanceledException || e is Polly.CircuitBreaker.BrokenCircuitException)
        {
            Log.Warning(e, "Downloading the inflation data failed.");
            return null;
        }

        if (inflationDataModel == null)
        {
            Log.Warning("Inflation data unavailable: the BLS Consumer Price Index series could not be downloaded or parsed.");
        }
        else
        {
            Log.Debug($"Inflation series years: {inflationDataModel.SeasonallyAdjusted.Rows.Count} seasonally adjusted, {inflationDataModel.NotSeasonallyAdjusted.Rows.Count} not seasonally adjusted");
        }

        return inflationDataModel;
    }
}
