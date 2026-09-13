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
public class TvSeriesEndpoint
{
    public static string CachePrefix = "tv_series-";

    public static void Map(WebApplication app)
    {
        app.MapGet($"{ApiRoutePrefix}/tvseries", [Authorize]
            async (
                string imdbId,
                ClaimsPrincipal user,
                [FromServices] SuggestionHttpClient suggestionHttpClient,
                [FromServices] OmdbHttpClient omdbHttpClient,
                [FromServices] TmdbHttpClient tmdbHttpClient,
                [FromServices] IMemoryCache cache) =>
            {
                IResult? tvSeriesViewModelJson;
                
                try
                {
                    string tvSeriesCacheKey = CachePrefix + imdbId;  // NOTE: We may not hit the cache that often for suggestions, but being a bit paranoid here to minimize impact

                    if (!cache.TryGetValue(tvSeriesCacheKey, out tvSeriesViewModelJson))
                    {
                        string username = user?.Identity?.Name ?? "<no username found>";
                        Log.Debug($"Username: {username}");

                        Task<SuggestionViewModel?> suggestionTask = GetSuggestionViewModel(imdbId, suggestionHttpClient, cache);
                        Task<OmdbResponseDataModel?> omdbTvSeriesTask = GetOmdbResponseDataModel(imdbId, omdbHttpClient);
                        Task<ConfigurationCountriesDictionary?> tmdbCountriesTask = GetConfigurationCountriesDictionary(tmdbHttpClient);
                        Task<ConfigurationLanguagesDictionary?> tmdbLanguagesTask = GetConfigurationLanguagesDictionary(tmdbHttpClient);
                        int? tmdbTvSeriesIdNullable = await GetTmdbId(imdbId, tmdbHttpClient);
                        if (tmdbTvSeriesIdNullable == null)
                        {
                            Log.Debug($"TV series TMDB ID for search '{imdbId}' was null!");
                            return null;
                        }
                        int tmdbTvSeriesId = tmdbTvSeriesIdNullable.GetValueOrDefault();
                        Task<TmdbTvSeriesResponseDataModel?> tmdbTvSeriesTask = GetTmdbTvSeriesResponseDataModel(tmdbTvSeriesId, tmdbHttpClient);
                        Task<TmdbTvSeriesAggregateCreditsResponseDataModel?> tmdbTvSeriesAggregateCreditsTask = GetTmdbTvSeriesAggregateCreditsResponseDataModel(tmdbTvSeriesId, tmdbHttpClient);
                        Task<TmdbWatchProvidersResponseDataModel?> tmdbTvSeriesWatchProvidersTask = GetWatchProvidersResponseDataModel(tmdbTvSeriesId, tmdbHttpClient);

                        // NOTE: Strictly speaking, this is not needed, but it's a good marker for when all tasks have been started
                        await Task.WhenAll(suggestionTask, omdbTvSeriesTask, tmdbCountriesTask, tmdbLanguagesTask, tmdbTvSeriesTask, tmdbTvSeriesAggregateCreditsTask, tmdbTvSeriesWatchProvidersTask);

                        SuggestionViewModel? tvSeriesSuggestionViewModel = await suggestionTask;
                        OmdbResponseDataModel? omdbTvSeriesResponseDataModel = await omdbTvSeriesTask;
                        ConfigurationCountriesDictionary? tmdbConfigurationCountriesDictionary = await tmdbCountriesTask;
                        ConfigurationLanguagesDictionary? tmdbConfigurationLanguagesDictionary = await tmdbLanguagesTask;
                        TmdbTvSeriesResponseDataModel? tmdbTvSeriesResponseDataModel = await tmdbTvSeriesTask;
                        TmdbTvSeriesAggregateCreditsResponseDataModel? tmdbTvSeriesAggregateCreditsResponseDataModel = await tmdbTvSeriesAggregateCreditsTask;
                        TmdbWatchProvidersResponseDataModel? tmdbTvSeriesWatchProvidersResponseDataModel = await tmdbTvSeriesWatchProvidersTask;

                        if (tvSeriesSuggestionViewModel == null)
                        {
                            Log.Debug($"TV series SuggestionViewModel for search '{imdbId}' was null!");
                        }
                        if (omdbTvSeriesResponseDataModel == null)
                        {
                            Log.Debug($"TV series OmdbResponseDataModel for search '{imdbId}' was null!");
                        }
                        if (tmdbConfigurationCountriesDictionary == null)
                        {
                            Log.Debug($"Configuration countries dictionary while searching for '{imdbId}' was null!");
                        }
                        if (tmdbConfigurationLanguagesDictionary == null)
                        {
                            Log.Debug($"Configuration languages dictionary while searching for '{imdbId}' was null!");
                        }
                        if (tmdbTvSeriesResponseDataModel == null)
                        {
                            Log.Debug($"TV series TmdbTvSeriesResponseDataModel for search '{imdbId}' and TMDB ID '{tmdbTvSeriesId}' was null!");
                        }
                        if (tmdbTvSeriesAggregateCreditsResponseDataModel == null)
                        {
                            Log.Debug($"TV series TmdbTvSeriesAggregateCreditsResponseDataModel for search '{imdbId}' and TMDB ID '{tmdbTvSeriesId}' was null!");
                        }
                        if (tmdbTvSeriesWatchProvidersResponseDataModel == null)
                        {
                            Log.Debug($"TV series TmdbWatchProvidersResponseDataModel for search '{imdbId}' and TMDB ID '{tmdbTvSeriesId}' was null!");
                        }

                        TvSeriesViewModel tvSeriesViewModel;
                        // Null checks all happened above, but there's no good way to let the compiler know about that, so recheck here
                        if (tvSeriesSuggestionViewModel != null
                            && tmdbTvSeriesResponseDataModel != null
                            && tmdbTvSeriesAggregateCreditsResponseDataModel != null
                            && tmdbTvSeriesWatchProvidersResponseDataModel != null
                            && tmdbConfigurationCountriesDictionary != null
                            && tmdbConfigurationLanguagesDictionary != null)
                        {
                            if (omdbTvSeriesResponseDataModel == null)  // Still lots of good info if this is null, so use an empty object
                            {
                                omdbTvSeriesResponseDataModel = OmdbResponseDataModel.GetEmptyOmdbResponseDataModel();
                            }
                            
                            tvSeriesViewModel = new TvSeriesViewModel(tvSeriesSuggestionViewModel,
                                                                      omdbTvSeriesResponseDataModel,
                                                                      tmdbTvSeriesResponseDataModel,
                                                                      tmdbTvSeriesAggregateCreditsResponseDataModel,
                                                                      tmdbTvSeriesWatchProvidersResponseDataModel,
                                                                      tmdbConfigurationCountriesDictionary,
                                                                      tmdbConfigurationLanguagesDictionary);
                        }
                        else
                        {
                            return null;  // TODO: return proper HTML error codes
                        }

                        tvSeriesViewModelJson = Results.Json(tvSeriesViewModel);

                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                            .SetSlidingExpiration(TimeSpan.FromHours(1));
                        cache.Set(tvSeriesCacheKey, tvSeriesViewModelJson, cacheEntryOptions);
                    }  // end cache block
                }
                catch (Exception e)
                {
                    string username = user.Identity?.Name ?? "<no username found>";

                    Log.ForContext("Username", username)
                        .Error(e, $"An error occurred while processing the /tvseries request '{imdbId}'.");

                    throw;
                }

                return tvSeriesViewModelJson;
            }
        )
        .WithSummary("TV Series")
        .WithDescription("Searches IMDB, TMDB, and OMDB for detailed information on TV series.")
        .RequireAuthorization(ProgramConstants.LoggedInUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireAuthorization(ProgramConstants.SearchUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireRateLimiting(ProgramConstants.TokenRateLimiterPolicyName);
    }

    public async static Task<SuggestionViewModel?> GetSuggestionViewModel(string imdbId, SuggestionHttpClient suggestionHttpClient, IMemoryCache cache)
    {
        SuggestionViewModel? tvSeriesSuggestionViewModel;
        string tvSeriesSuggestionCacheKey = CachePrefix + imdbId;  // NOTE: We may not hit the cache that often for suggestions, but being a bit paranoid here to minimize impact
        
        if (!cache.TryGetValue(tvSeriesSuggestionCacheKey, out tvSeriesSuggestionViewModel))
        {
            SuggestionsResponseDataModel? suggestionsResponse = await suggestionHttpClient.GetSuggestions(imdbId);

            if (suggestionsResponse == null || suggestionsResponse.Suggestions == null || suggestionsResponse.Suggestions.Length <= 0)
                return null;
            Log.Debug($"Suggestions:\n\n{suggestionsResponse}\n\n");   // NOTE: Not destructuring using @ operator because Serilog doesn't let you configure output easily
                                                                       // (and Seq doesn't support Azure Container Apps, so it's not used in this app)

            tvSeriesSuggestionViewModel = new SuggestionViewModel(suggestionsResponse.Suggestions[0]);
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                .SetSlidingExpiration(TimeSpan.FromHours(1));
            cache.Set(tvSeriesSuggestionCacheKey, tvSeriesSuggestionViewModel, cacheEntryOptions);
        }
        return tvSeriesSuggestionViewModel;
    }

    public async static Task<OmdbResponseDataModel?> GetOmdbResponseDataModel(string imdbId, OmdbHttpClient omdbHttpClient)
    {
        OmdbResponseDataModel? omdbTvSeriesDataModelResponse = await omdbHttpClient.GetMedia(imdbId);
        if (omdbTvSeriesDataModelResponse == null)
            return null;
        
        Log.Debug($"OMDB TV series data response:\n\n{omdbTvSeriesDataModelResponse}\n\n");

        return omdbTvSeriesDataModelResponse;
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

    public async static Task<int?> GetTmdbId(string imdbId, TmdbHttpClient tmdbHttpClient)
    {
        TmdbIdResponseDataModel? tmdbIdResponseDataModel = await tmdbHttpClient.GetFindByImdbIdResults(imdbId);
        if (tmdbIdResponseDataModel == null || tmdbIdResponseDataModel.TvResults == null || tmdbIdResponseDataModel.TvResults.Length <= 0)
            return null;
        
        Log.Debug($"TMDB TV series find by IMDB Id response:\n\n{tmdbIdResponseDataModel}\n\n");

        return tmdbIdResponseDataModel.TvResults[0].TmdbId;
    }

    public async static Task<TmdbTvSeriesResponseDataModel?> GetTmdbTvSeriesResponseDataModel(int tmdbId, TmdbHttpClient tmdbHttpClient)
    {
        TmdbTvSeriesResponseDataModel? tmdbTvSeriesResponseDataModel = await tmdbHttpClient.GetTvSeries(tmdbId);
        if (tmdbTvSeriesResponseDataModel == null)
            return null;
        
        Log.Debug($"TMDB TV series details response:\n\n{tmdbTvSeriesResponseDataModel}\n\n");

        return tmdbTvSeriesResponseDataModel;
    }

    public async static Task<TmdbTvSeriesAggregateCreditsResponseDataModel?> GetTmdbTvSeriesAggregateCreditsResponseDataModel(int tmdbId, TmdbHttpClient tmdbHttpClient)
    {
        TmdbTvSeriesAggregateCreditsResponseDataModel? tmdbTvSeriesAggregateCreditsResponseDataModel = await tmdbHttpClient.GetTvSeriesAggregateCredits(tmdbId);
        if (tmdbTvSeriesAggregateCreditsResponseDataModel == null)
            return null;
        
        Log.Debug($"TMDB TV series aggregate credits response:\n\n{tmdbTvSeriesAggregateCreditsResponseDataModel}\n\n");

        return tmdbTvSeriesAggregateCreditsResponseDataModel;
    }

    public async static Task<TmdbWatchProvidersResponseDataModel?> GetWatchProvidersResponseDataModel(int tmdbId, TmdbHttpClient tmdbHttpClient)
    {
        TmdbWatchProvidersResponseDataModel? tmdbTvSeriesWatchProvidersResponseDataModel = await tmdbHttpClient.GetTvSeriesWatchProviders(tmdbId);
        if (tmdbTvSeriesWatchProvidersResponseDataModel == null)
            return null;
        
        Log.Debug($"TMDB TV series watch providers response:\n\n{tmdbTvSeriesWatchProvidersResponseDataModel}\n\n");

        return tmdbTvSeriesWatchProvidersResponseDataModel;
    }
}