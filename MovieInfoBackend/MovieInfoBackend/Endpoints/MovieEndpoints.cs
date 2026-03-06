using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Http;
using MovieInfoBackend.DataModels;
using MovieInfoBackend.Helpers;
using MovieInfoBackend.ViewModels;
using Serilog;

namespace MovieInfoBackend.Endpoints;

public class MovieEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/api/search", [Authorize]
            async (
                string searchQuery,
                ClaimsPrincipal user,
                [FromServices] IMemoryCache cache,
                [FromServices] IHttpClientFactory clientFactory,
                [FromServices] ITypedHttpClientFactory <MovieHttpClient> typedHttpClientFactory) =>
                {
                    try
                    {
                        HttpClient httpClient = clientFactory.CreateClient(nameof(MovieHttpClient));
                        MovieHttpClient movieHttpClient = typedHttpClientFactory.CreateClient(httpClient);

                        MovieSuggestionsResponseDataModel? suggestionsResponse;
                        string suggestionsCacheKey = MovieHttpClient.CachePrefix + searchQuery;
                        if (!cache.TryGetValue(suggestionsCacheKey, out suggestionsResponse))
                        {
                            suggestionsResponse = await movieHttpClient.GetSuggestions(searchQuery);

                            var cacheEntryOptions = new MemoryCacheEntryOptions()
                                .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                                .SetSlidingExpiration(TimeSpan.FromHours(1));

                            cache.Set(suggestionsCacheKey, suggestionsResponse, cacheEntryOptions);
                        }

                        string username = user?.Identity?.Name ?? "<no username found>";

                        Log.Debug($"Username: {username}");
                        Log.Debug($"Suggestions:\n\n{suggestionsResponse}\n\n");   // NOTE: Not destructuring using @ operator because Serilog doesn't
                                                                                   // let you configure output easily (and Seq doesn't support Azure Container Apps)

                        List<SuggestionViewModel> suggestionViewModels = new List<SuggestionViewModel>();
                        if (suggestionsResponse == null || suggestionsResponse.Suggestions == null)
                            return null;  // TODO: return proper HTML error codes
                        foreach (SuggestionDataModel suggestionDataModel in suggestionsResponse.Suggestions)
                        {
                            suggestionViewModels.Add(new SuggestionViewModel(suggestionDataModel));
                        }

                        return Results.Json(suggestionViewModels);
                    }
                    catch (Exception e)
                    {
                        string username = user.Identity?.Name ?? "<no username found>";

                        Log.ForContext("Username", username)
                            .Error(e, "An error occurred while processing the request");

                        throw;
                    }
                })
        .WithName("Search")
        .RequireAuthorization(ProgramConstants.LoggedInUsersOnlyPolicyName)
        .RequireAuthorization(ProgramConstants.SearchUsersOnlyPolicyName);
    }
}