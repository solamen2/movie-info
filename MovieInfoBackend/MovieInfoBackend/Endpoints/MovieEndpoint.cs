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
using static MovieInfoBackend.DataModels.TmdbConfigurationCountriesResponseDataModel;
using static MovieInfoBackend.DataModels.TmdbConfigurationLanguagesResponseDataModel;

namespace MovieInfoBackend.Endpoints;

[ExcludeFromCodeCoverage]
public class MovieEndpoint
{
    public static string CachePrefix = "movie-";

    public static void Map(WebApplication app)
    {
        app.MapGet($"{ApiRoutePrefix}/movie", [Authorize]
            async (
                string? imdbId,
                int? tmdbId,
                ClaimsPrincipal user,
                [FromServices] SuggestionHttpClient suggestionHttpClient,
                [FromServices] OmdbHttpClient omdbHttpClient,
                [FromServices] TmdbHttpClient tmdbHttpClient,
                [FromServices] WikipediaHttpClient wikipediaHttpClient,
                [FromServices] IMemoryCache cache) =>
            {
                IResult? movieViewModelJson;
                
                try
                {
                    if (imdbId == null)
                    {
                        if (tmdbId == null)
                        {
                            return Results.BadRequest("Either imdbId or tmdbId must be provided.");
                        }
                        imdbId = await GetImdbId(tmdbId.Value, tmdbHttpClient, cache);
                        if (imdbId == null)
                        {
                            Log.Debug($"Movie IMDB ID for TMDB ID '{tmdbId}' was null!");
                            return Results.NotFound($"Movie with TMDB ID '{tmdbId}' was not found in the TMDB, or has no IMDB ID.");
                        }
                    }

                    string movieCacheKey = CachePrefix + imdbId;  // NOTE: We may not hit the cache that often for suggestions, but being a bit paranoid here to minimize impact

                    if (!cache.TryGetValue(movieCacheKey, out movieViewModelJson))
                    {
                        string username = user?.Identity?.Name ?? "<no username found>";
                        Log.Debug($"Username: {username}");

                        Task<SuggestionViewModel?> suggestionTask = GetSuggestionViewModel(imdbId, suggestionHttpClient, cache);
                        Task<OmdbResponseDataModel?> omdbMovieTask = GetOmdbResponseDataModel(imdbId, omdbHttpClient);
                        Task<ConfigurationCountriesDictionary?> tmdbCountriesTask = GetConfigurationCountriesDictionary(tmdbHttpClient);
                        Task<ConfigurationLanguagesDictionary?> tmdbLanguagesTask = GetConfigurationLanguagesDictionary(tmdbHttpClient);
                        int? tmdbMovieIdNullable = tmdbId ?? await GetTmdbId(imdbId, tmdbHttpClient);
                        if (tmdbMovieIdNullable == null)
                        {
                            Log.Debug($"Movie TMDB ID for search '{imdbId}' was null!");
                            return Results.NotFound($"Movie for search '{imdbId}' was not found in the TMDB.");
                        }
                        int tmdbMovieId = tmdbMovieIdNullable.GetValueOrDefault();
                        Task<TmdbMovieResponseDataModel?> tmdbMovieTask = GetTmdbMovieResponseDataModel(tmdbMovieId, tmdbHttpClient);
                        Task<TmdbMovieCreditsResponseDataModel?> tmdbMovieCreditsTask = GetTmdbMovieCreditsResponseDataModel(tmdbMovieId, tmdbHttpClient);
                        Task<TmdbWatchProvidersResponseDataModel?> tmdbMovieWatchProvidersTask = GetWatchProvidersResponseDataModel(tmdbMovieId, tmdbHttpClient);
                        // Wikipedia is searched by the title and year from the suggestion (see WikipediaHttpClient.GetMovieLink), so that lookup can only start once the suggestion is in
                        SuggestionViewModel? movieSuggestionViewModel = await suggestionTask;
                        Task<string?> wikipediaLinkTask = GetWikipediaLink(movieSuggestionViewModel, wikipediaHttpClient);

                        // NOTE: Strictly speaking, this is not needed, but it's a good marker for when all tasks have been started
                        await Task.WhenAll(omdbMovieTask, tmdbCountriesTask, tmdbLanguagesTask, tmdbMovieTask, tmdbMovieCreditsTask, tmdbMovieWatchProvidersTask, wikipediaLinkTask);

                        OmdbResponseDataModel? omdbMovieResponseDataModel = await omdbMovieTask;
                        ConfigurationCountriesDictionary? tmdbConfigurationCountriesDictionary = await tmdbCountriesTask;
                        ConfigurationLanguagesDictionary? tmdbConfigurationLanguagesDictionary = await tmdbLanguagesTask;
                        TmdbMovieResponseDataModel? tmdbMovieResponseDataModel = await tmdbMovieTask;
                        TmdbMovieCreditsResponseDataModel? tmdbMovieCreditsResponseDataModel = await tmdbMovieCreditsTask;
                        TmdbWatchProvidersResponseDataModel? tmdbMovieWatchProvidersResponseDataModel = await tmdbMovieWatchProvidersTask;
                        string? wikipediaLink = await wikipediaLinkTask;

                        if (movieSuggestionViewModel == null)
                        {
                            Log.Debug($"Movie SuggestionViewModel for search '{imdbId}' was null!");
                        }
                        if (omdbMovieResponseDataModel == null)
                        {
                            Log.Debug($"Movie OmdbResponseDataModel for search '{imdbId}' was null!");
                        }
                        if (tmdbConfigurationCountriesDictionary == null)
                        {
                            Log.Debug($"Configuration countries dictionary while searching for '{imdbId}' was null!");
                        }
                        if (tmdbConfigurationLanguagesDictionary == null)
                        {
                            Log.Debug($"Configuration languages dictionary while searching for '{imdbId}' was null!");
                        }
                        if (tmdbMovieResponseDataModel == null)
                        {
                            Log.Debug($"Movie TmdbMovieResponseDataModel for search '{imdbId}' and TMDB ID '{tmdbMovieId}' was null!");
                        }
                        if (tmdbMovieCreditsResponseDataModel == null)
                        {
                            Log.Debug($"Movie TmdbMovieCreditsResponseDataModel for search '{imdbId}' and TMDB ID '{tmdbMovieId}' was null!");
                        }
                        if (tmdbMovieWatchProvidersResponseDataModel == null)
                        {
                            Log.Debug($"Movie TmdbWatchProvidersResponseDataModel for search '{imdbId}' and TMDB ID '{tmdbMovieId}' was null!");
                        }
                        if (wikipediaLink == null)
                        {
                            Log.Debug($"Movie Wikipedia link for search '{imdbId}' was null!");
                        }

                        MovieViewModel movieViewModel;
                        // Null checks all happened above, but there's no good way to let the compiler know about that, so recheck here
                        if (movieSuggestionViewModel != null
                            && tmdbMovieResponseDataModel != null
                            && tmdbMovieCreditsResponseDataModel != null
                            && tmdbMovieWatchProvidersResponseDataModel != null
                            && tmdbConfigurationCountriesDictionary != null
                            && tmdbConfigurationLanguagesDictionary != null)
                        {
                            if (omdbMovieResponseDataModel == null)  // Still lots of good info if this is null, so use an empty object
                            {
                                omdbMovieResponseDataModel = OmdbResponseDataModel.GetEmptyOmdbResponseDataModel();
                            }
                            
                            movieViewModel = new MovieViewModel(movieSuggestionViewModel,
                                                                omdbMovieResponseDataModel,
                                                                tmdbMovieResponseDataModel,
                                                                tmdbMovieCreditsResponseDataModel,
                                                                tmdbMovieWatchProvidersResponseDataModel,
                                                                tmdbConfigurationCountriesDictionary,
                                                                tmdbConfigurationLanguagesDictionary,
                                                                wikipediaLink);
                        }
                        else
                        {
                            return null;  // TODO: return proper HTML error codes
                        }

                        movieViewModelJson = Results.Json(movieViewModel);

                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                            .SetSlidingExpiration(TimeSpan.FromHours(1));
                        cache.Set(movieCacheKey, movieViewModelJson, cacheEntryOptions);
                    }  // end cache block
                }
                catch (Exception e)
                {
                    string username = user.Identity?.Name ?? "<no username found>";

                    Log.ForContext("Username", username)
                        .Error(e, $"An error occurred while processing the /movie request '{imdbId}'.");

                    throw;
                }

                return movieViewModelJson;
            }
        )
        .WithSummary("Movie")
        .WithDescription("Searches IMDB, TMDB, OMDB, and Wikipedia for detailed information on movies, looked up by IMDB ID or by TMDB ID.")
        .RequireAuthorization(ProgramConstants.LoggedInUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireAuthorization(ProgramConstants.SearchUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireRateLimiting(ProgramConstants.TokenRateLimiterPolicyName);
    }

    public async static Task<SuggestionViewModel?> GetSuggestionViewModel(string imdbId, SuggestionHttpClient suggestionHttpClient, IMemoryCache cache)
    {
        SuggestionViewModel? movieSuggestionViewModel;
        string movieSuggestionCacheKey = SuggestionEndpoint.CachePrefix + imdbId;  // NOTE: We may not hit the cache that often for suggestions, but being a bit paranoid here to minimize impact
        
        if (!cache.TryGetValue(movieSuggestionCacheKey, out movieSuggestionViewModel))
        {
            SuggestionsResponseDataModel? suggestionsResponse = await suggestionHttpClient.GetSuggestions(imdbId);

            if (suggestionsResponse == null || suggestionsResponse.Suggestions == null || suggestionsResponse.Suggestions.Length <= 0)
                return null;
            Log.Debug($"Suggestions:\n\n{suggestionsResponse}\n\n");   // NOTE: Not destructuring using @ operator because Serilog doesn't let you configure output easily
                                                                       // (and Seq doesn't support Azure Container Apps, so it's not used in this app)

            SuggestionDataModel? suggestionDataModel = suggestionsResponse.FindByItemId(imdbId);
            if (suggestionDataModel == null)
            {
                Log.Warning($"Movie suggestions for '{imdbId}' did not contain that IMDB ID.");
                return null;
            }
            movieSuggestionViewModel = new SuggestionViewModel(suggestionDataModel);
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                .SetSlidingExpiration(TimeSpan.FromHours(1));
            cache.Set(movieSuggestionCacheKey, movieSuggestionViewModel, cacheEntryOptions);
        }
        return movieSuggestionViewModel;
    }

    public async static Task<OmdbResponseDataModel?> GetOmdbResponseDataModel(string imdbId, OmdbHttpClient omdbHttpClient)
    {
        OmdbResponseDataModel? omdbMovieDataModelResponse = await omdbHttpClient.GetMedia(imdbId);
        if (omdbMovieDataModelResponse == null)
            return null;
        
        Log.Debug($"OMDB movie data response:\n\n{omdbMovieDataModelResponse}\n\n");

        return omdbMovieDataModelResponse;
    }

    public async static Task<ConfigurationCountriesDictionary?> GetConfigurationCountriesDictionary(TmdbHttpClient tmdbHttpClient)
    {
        ConfigurationCountriesDictionary? tmdbConfigurationCountriesDictionary = await tmdbHttpClient.GetCountries();
        
        Log.Debug($"TMDB configuration countries dictionary:\n\n{tmdbConfigurationCountriesDictionary}\n\n");

        return tmdbConfigurationCountriesDictionary;
    }

    public async static Task<ConfigurationLanguagesDictionary?> GetConfigurationLanguagesDictionary(TmdbHttpClient tmdbHttpClient)
    {
        ConfigurationLanguagesDictionary? tmdbConfigurationLanguagesDictionary = await tmdbHttpClient.GetLanguages();
        
        Log.Debug($"TMDB configuration languages dictionary:\n\n{tmdbConfigurationLanguagesDictionary}\n\n");

        return tmdbConfigurationLanguagesDictionary;
    }

    public async static Task<string?> GetImdbId(int tmdbId, TmdbHttpClient tmdbHttpClient, IMemoryCache cache)
    {
        string imdbIdCacheKey = $"{CachePrefix}imdb-{tmdbId}";

        if (!cache.TryGetValue(imdbIdCacheKey, out string? imdbId) || imdbId == null)
        {
            TmdbMovieExternalIdsResponseDataModel? externalIdsResponseDataModel = await tmdbHttpClient.GetMovieExternalIds(tmdbId);
            if (externalIdsResponseDataModel == null || string.IsNullOrEmpty(externalIdsResponseDataModel.ImdbId))
                return null;

            Log.Debug($"TMDB movie external IDs response:\n\n{externalIdsResponseDataModel}\n\n");

            imdbId = externalIdsResponseDataModel.ImdbId;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                .SetSlidingExpiration(TimeSpan.FromHours(1));
            cache.Set(imdbIdCacheKey, imdbId, cacheEntryOptions);
        }
        return imdbId;
    }

    public async static Task<int?> GetTmdbId(string imdbId, TmdbHttpClient tmdbHttpClient)
    {
        TmdbIdResponseDataModel? tmdbIdResponseDataModel = await tmdbHttpClient.GetFindByImdbIdResults(imdbId);
        if (tmdbIdResponseDataModel == null || tmdbIdResponseDataModel.MovieResults == null || tmdbIdResponseDataModel.MovieResults.Length <= 0)
            return null;
        
        Log.Debug($"TMDB movie find by IMDB Id response:\n\n{tmdbIdResponseDataModel}\n\n");

        return tmdbIdResponseDataModel.MovieResults[0].TmdbId;
    }

    public async static Task<TmdbMovieResponseDataModel?> GetTmdbMovieResponseDataModel(int tmdbId, TmdbHttpClient tmdbHttpClient)
    {
        TmdbMovieResponseDataModel? tmdbMovieResponseDataModel = await tmdbHttpClient.GetMovie(tmdbId);
        if (tmdbMovieResponseDataModel == null)
            return null;
        
        Log.Debug($"TMDB movie details response:\n\n{tmdbMovieResponseDataModel}\n\n");

        return tmdbMovieResponseDataModel;
    }

    public async static Task<TmdbMovieCreditsResponseDataModel?> GetTmdbMovieCreditsResponseDataModel(int tmdbId, TmdbHttpClient tmdbHttpClient)
    {
        TmdbMovieCreditsResponseDataModel? tmdbMovieCreditsResponseDataModel = await tmdbHttpClient.GetMovieCredits(tmdbId);
        if (tmdbMovieCreditsResponseDataModel == null)
            return null;
        
        Log.Debug($"TMDB movie credits response:\n\n{tmdbMovieCreditsResponseDataModel}\n\n");

        return tmdbMovieCreditsResponseDataModel;
    }

    public async static Task<TmdbWatchProvidersResponseDataModel?> GetWatchProvidersResponseDataModel(int tmdbId, TmdbHttpClient tmdbHttpClient)
    {
        TmdbWatchProvidersResponseDataModel? tmdbMovieWatchProvidersResponseDataModel = await tmdbHttpClient.GetMovieWatchProviders(tmdbId);
        if (tmdbMovieWatchProvidersResponseDataModel == null)
            return null;
        
        Log.Debug($"TMDB movie watch providers response:\n\n{tmdbMovieWatchProvidersResponseDataModel}\n\n");

        return tmdbMovieWatchProvidersResponseDataModel;
    }

    // Returns the movie's Wikipedia page link, "" when Wikipedia has no page for it, or null when the lookup failed (or had nothing to search for)
    public async static Task<string?> GetWikipediaLink(SuggestionViewModel? movieSuggestionViewModel, WikipediaHttpClient wikipediaHttpClient)
    {
        if (movieSuggestionViewModel == null)
            return null;

        string? wikipediaLink = await wikipediaHttpClient.GetMovieLink(movieSuggestionViewModel.Name, movieSuggestionViewModel.Year);
        if (wikipediaLink == null)
            return null;
        
        Log.Debug($"Wikipedia movie link response:\n\n{wikipediaLink}\n\n");

        return wikipediaLink;
    }
}