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
    // TODO: Add endpoints for people, TV series, TV seasons, and TV episodes
    // These endpoints will have to call multiple TMDB APIs (and sometimes the OMDB API), and await on all of them at the end using Task.WhenAll()

    public static string CachePrefix = "movie-";

    public static void Map(WebApplication app)
    {
        app.MapGet($"{ApiRoutePrefix}/movie", [Authorize]
            async (
                string imdbId,
                ClaimsPrincipal user,
                [FromServices] SuggestionHttpClient suggestionHttpClient,
                [FromServices] OmdbHttpClient omdbHttpClient,
                [FromServices] TmdbHttpClient tmdbHttpClient,
                [FromServices] IMemoryCache cache) =>
            {
                IResult? movieViewModelJson;
                
                try
                {
                    string movieCacheKey = CachePrefix + imdbId;  // NOTE: We may not hit the cache that often for suggestions, but being a bit paranoid here to minimize impact

                    if (!cache.TryGetValue(movieCacheKey, out movieViewModelJson))
                    {
                        string username = user?.Identity?.Name ?? "<no username found>";
                        Log.Debug($"Username: {username}");

                        Task<SuggestionViewModel?> suggestionTask = GetSuggestionViewModel(imdbId, suggestionHttpClient, cache);
                        Task<OmdbResponseDataModel?> omdbMovieTask = GetOmdbResponseDataModel(imdbId, omdbHttpClient);
                        Task<ConfigurationCountriesDictionary?> tmdbCountriesTask = GetConfigurationCountriesDictionary(tmdbHttpClient);
                        Task<ConfigurationLanguagesDictionary?> tmdbLanguagesTask = GetConfigurationLanguagesDictionary(tmdbHttpClient);
                        int? tmdbMovieIdNullable = await GetTmdbId(imdbId, tmdbHttpClient);
                        if (tmdbMovieIdNullable == null)
                        {
                            Log.Debug($"Movie TMDB ID for search '{imdbId}' was null!");
                            return null;
                        }
                        int tmdbMovieId = tmdbMovieIdNullable.GetValueOrDefault();
                        Task<TmdbMovieResponseDataModel?> tmdbMovieTask = GetTmdbMovieResponseDataModel(tmdbMovieId, tmdbHttpClient);
                        Task<TmdbMovieCreditsResponseDataModel?> tmdbMovieCreditsTask = GetTmdbMovieCreditsResponseDataModel(tmdbMovieId, tmdbHttpClient);
                        Task<TmdbWatchProvidersResponseDataModel?> tmdbMovieWatchProvidersTask = GetWatchProvidersResponseDataModel(tmdbMovieId, tmdbHttpClient);

                        // NOTE: Strictly speaking, this is not needed, but it's a good marker for when all tasks have been started
                        await Task.WhenAll(suggestionTask, omdbMovieTask, tmdbCountriesTask, tmdbLanguagesTask, tmdbMovieTask, tmdbMovieCreditsTask, tmdbMovieWatchProvidersTask);

                        SuggestionViewModel? movieSuggestionViewModel = await suggestionTask;
                        OmdbResponseDataModel? omdbMovieResponseDataModel = await omdbMovieTask;
                        ConfigurationCountriesDictionary? configurationCountriesDictionary = await tmdbCountriesTask;
                        ConfigurationLanguagesDictionary? configurationLanguagesDictionary = await tmdbLanguagesTask;
                        TmdbMovieResponseDataModel? tmdbMovieResponseDataModel = await tmdbMovieTask;
                        TmdbMovieCreditsResponseDataModel? tmdbMovieCreditsResponseDataModel = await tmdbMovieCreditsTask;
                        TmdbWatchProvidersResponseDataModel? tmdbMovieWatchProvidersResponseDataModel = await tmdbMovieWatchProvidersTask;

                        if (movieSuggestionViewModel == null)
                        {
                            Log.Debug($"Movie SuggestionViewModel for search '{imdbId}' was null!");
                        }
                        if (omdbMovieResponseDataModel == null)
                        {
                            Log.Debug($"Movie OmdbResponseDataModel for search '{imdbId}' was null!");
                        }
                        if (configurationCountriesDictionary == null)
                        {
                            Log.Debug($"Configuration countries dictionary while searching for '{imdbId}' was null!");
                        }
                        if (configurationLanguagesDictionary == null)
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

                        MovieViewModel movieViewModel;
                        // Null checks all happened above, but there's no good way to let the compiler know about that, so recheck here
                        if (movieSuggestionViewModel != null
                            && omdbMovieResponseDataModel != null
                            && tmdbMovieResponseDataModel != null
                            && tmdbMovieCreditsResponseDataModel != null
                            && tmdbMovieWatchProvidersResponseDataModel != null
                            && configurationCountriesDictionary != null
                            && configurationLanguagesDictionary != null)
                        {
                            movieViewModel = new MovieViewModel(movieSuggestionViewModel,
                                                                omdbMovieResponseDataModel,
                                                                tmdbMovieResponseDataModel,
                                                                tmdbMovieCreditsResponseDataModel,
                                                                tmdbMovieWatchProvidersResponseDataModel,
                                                                configurationCountriesDictionary,
                                                                configurationLanguagesDictionary);
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
        .WithDescription("Searches IMDB, TMDB, and OMDB for detailed information on movies.")
        .RequireAuthorization(ProgramConstants.LoggedInUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireAuthorization(ProgramConstants.SearchUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireRateLimiting(ProgramConstants.TokenRateLimiterPolicyName);
    }

    public async static Task<SuggestionViewModel?> GetSuggestionViewModel(string imdbId, SuggestionHttpClient suggestionHttpClient, IMemoryCache cache)
    {
        SuggestionViewModel? movieSuggestionViewModel;
        string movieSuggestionCacheKey = CachePrefix + imdbId;  // NOTE: We may not hit the cache that often for suggestions, but being a bit paranoid here to minimize impact
        
        if (!cache.TryGetValue(movieSuggestionCacheKey, out movieSuggestionViewModel))
        {
            SuggestionsResponseDataModel? suggestionsResponse = await suggestionHttpClient.GetSuggestions(imdbId);

            if (suggestionsResponse == null || suggestionsResponse.Suggestions == null || suggestionsResponse.Suggestions.Length <= 0)
                return null;
            Log.Debug($"Suggestions:\n\n{suggestionsResponse}\n\n");   // NOTE: Not destructuring using @ operator because Serilog doesn't let you configure output easily
                                                                       // (and Seq doesn't support Azure Container Apps, so it's not used in this app)

            movieSuggestionViewModel = new SuggestionViewModel(suggestionsResponse.Suggestions[0]);
            
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
        ConfigurationCountriesDictionary? configurationCountriesDictionary = await tmdbHttpClient.GetCountries();
        
        Log.Debug($"TMDB configuration countries dictionary:\n\n{configurationCountriesDictionary}\n\n");

        return configurationCountriesDictionary;
    }

    public async static Task<ConfigurationLanguagesDictionary?> GetConfigurationLanguagesDictionary(TmdbHttpClient tmdbHttpClient)
    {
        ConfigurationLanguagesDictionary? configurationLanguagesDictionary = await tmdbHttpClient.GetLanguages();
        
        Log.Debug($"TMDB configuration languages dictionary:\n\n{configurationLanguagesDictionary}\n\n");

        return configurationLanguagesDictionary;
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

    // WARNING: This function should only ever be used in local development to generate test case data
    [ExcludeFromCodeCoverage]
    private class MovieEndpointHelper
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

        // TODO: Need to do test case data helpers for other calls too
    }
}