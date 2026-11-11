using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MovieInfoBackend.Helpers;
using static MovieInfoBackend.Helpers.ProgramConstants;  // for ApiRoutePrefix
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace MovieInfoBackend.Endpoints;

[ExcludeFromCodeCoverage]
public class AspectRatiosEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet($"{ApiRoutePrefix}/aspectratios", [Authorize]
            (ClaimsPrincipal user) =>
            {
                string username = user?.Identity?.Name ?? "<no username found>";
                Log.Debug($"Username: {username}");

                return Results.Json(AspectRatiosHelper.All());
            }
        )
        .WithSummary("Aspect Ratios")
        .WithDescription("Lists every aspect ratio the app has a name or description for, narrowest first (see AspectRatiosHelper).")
        .RequireAuthorization(ProgramConstants.LoggedInUsersOnlyPolicyName)
        .RequireAuthorization(ProgramConstants.SearchUsersOnlyPolicyName)
        .RequireRateLimiting(ProgramConstants.TokenRateLimiterPolicyName);
    }
}
