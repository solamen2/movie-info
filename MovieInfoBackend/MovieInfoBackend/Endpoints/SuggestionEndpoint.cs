using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MovieInfoBackend.DataModels;
using MovieInfoBackend.Helpers;
using static MovieInfoBackend.Helpers.ProgramConstants;  // for ApiRoutePrefix
using MovieInfoBackend.ViewModels;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace MovieInfoBackend.Endpoints;

[ExcludeFromCodeCoverage]
public class SuggestionEndpoint
{
    public static string CachePrefix = "suggestions-";

    public static void Map(WebApplication app)
    {
        app.MapGet($"{ApiRoutePrefix}/search", [Authorize]
            async (
                string searchQuery,
                ClaimsPrincipal user,
                [FromServices] SuggestionHttpClient suggestionHttpClient,
                [FromServices] IMemoryCache cache) =>
            {
                try
                {
                    IResult? suggestionViewModelsJson;
                    string suggestionsCacheKey = CachePrefix + searchQuery;  // NOTE: We may not hit the cache that often for suggestions, but being a bit paranoid here to minimize impact

                    if (!cache.TryGetValue(suggestionsCacheKey, out suggestionViewModelsJson))
                    {
                        SuggestionsResponseDataModel? suggestionsResponse = await suggestionHttpClient.GetSuggestions(searchQuery);

                        List<SuggestionViewModel> suggestionViewModels = new List<SuggestionViewModel>();
                        if (suggestionsResponse == null || suggestionsResponse.Suggestions == null || suggestionsResponse.Suggestions.Length <= 0)
                            return null;  // TODO: return proper HTML error codes
                        foreach (SuggestionDataModel suggestionDataModel in suggestionsResponse.Suggestions)
                        {
                            suggestionViewModels.Add(new SuggestionViewModel(suggestionDataModel));
                        }

                        string username = user?.Identity?.Name ?? "<no username found>";
                        Log.Debug($"Username: {username}");
                        Log.Debug($"Suggestions:\n\n{suggestionsResponse}\n\n");   // NOTE: Not destructuring using @ operator because Serilog doesn't let you configure output easily
                                                                                   // (and Seq doesn't support Azure Container Apps, so it's not used in this app)


                        suggestionViewModelsJson = Results.Json(suggestionViewModels);
                        
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                            .SetSlidingExpiration(TimeSpan.FromHours(1));
                        cache.Set(suggestionsCacheKey, suggestionViewModelsJson, cacheEntryOptions);
                    }

                    return suggestionViewModelsJson;
                }
                catch (Exception e)
                {
                    string username = user.Identity?.Name ?? "<no username found>";

                    Log.ForContext("Username", username)
                        .Error(e, $"An error occurred while processing the /search request '{searchQuery}'.");

                    throw;
                }
            }
        )
        .WithSummary("Search")
        .WithDescription("Searches IMDB for people, movies, and many other media types, and returns basic information on them.")
        .RequireAuthorization(ProgramConstants.LoggedInUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireAuthorization(ProgramConstants.SearchUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireRateLimiting(ProgramConstants.TokenRateLimiterPolicyName);
    }

    // WARNING: This function should only ever be used in local development to generate test case data
    [ExcludeFromCodeCoverage]
    private class SuggestionEndpointHelper
    {
        private static SuggestionsResponseDataModel? LoadMockData()
        {
            string suggestionHttpClientResponse;
            string testDataFilename = "SuggestionHttpClientResponse2.json";

            using (StreamReader sr = File.OpenText($"../TestMovieInfoBackend/TestData/{testDataFilename}"))
            {
                suggestionHttpClientResponse = sr.ReadToEnd();
            }
            if (String.IsNullOrWhiteSpace(suggestionHttpClientResponse))
            {
                throw new ArgumentException($"{testDataFilename} is not valid test data.");
            }

            return SuggestionHttpClient.GetModelFromResponse(suggestionHttpClientResponse);
        }
    }
}