using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieInfoBackend.Auth;
using MovieInfoBackend.Helpers;
using Serilog;

namespace MovieInfoBackend.Endpoints;

public class AuthEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/logout", async (SignInManager<ApplicationUser> signInManager,
            [FromBody] EmptyBodyRequest emptyBodyRequest,
            ClaimsPrincipal user) =>
            {
                try {
                    if (emptyBodyRequest != null)
                    {
                        await signInManager.SignOutAsync();
                        return Results.Ok();
                    }
                    return Results.Unauthorized();
                }
                catch (Exception e)
                {
                    string username = user.Identity?.Name ?? "<no username found>";

                    Log.ForContext("Username", username)
                        .Error(e, $"An error occurred while logging out the user.");

                    throw;
                }
            }
        )
        .WithSummary("Logout")
        .WithDescription("Logs out the current user and deletes their session cookie. This function is user-defined, "
                            + "as it is not included in ASP.NET Core Identity in .NET 10.0, oddly. See https://github.com/dotnet/aspnetcore/issues/52834 for details, and code at "
                            + "https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-10.0&preserve-view=true#log-out ."
                            + "NOTE: A JSON body must be specified for this request to work properly, but all content in it is ignored.")
        .RequireAuthorization(ProgramConstants.LoggedInUsersOnlyPolicyName);
    }
}

public class EmptyBodyRequest
{
    
}