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
public class TvSeasonEndpoint
{
    public static string CachePrefix = "tv_season-";

    public static void Map(WebApplication app)
    {
        app.MapGet($"{ApiRoutePrefix}/tvseason", [Authorize]
            async (
                int tmdbTvSeriesId,
                int seasonNumber,
                ClaimsPrincipal user,
                [FromServices] TmdbHttpClient tmdbHttpClient,
                [FromServices] IMemoryCache cache) =>
            {
                IResult? tvSeasonViewModelJson;
                
                try
                {
                    // NOTE: We may not hit the cache that often, but being a bit paranoid here to minimize impact
                    string tvSeasonCacheKey = $"{CachePrefix}-{tmdbTvSeriesId}-{seasonNumber}";

                    if (!cache.TryGetValue(tvSeasonCacheKey, out tvSeasonViewModelJson))
                    {
                        string username = user?.Identity?.Name ?? "<no username found>";
                        Log.Debug($"Username: {username}");

                        Task<TmdbTvSeasonResponseDataModel?> tmdbTvSeasonTask = GetTmdbTvSeasonResponseDataModel(tmdbTvSeriesId, seasonNumber, tmdbHttpClient);
                        Task<TmdbWatchProvidersResponseDataModel?> tmdbTvSeasonWatchProvidersTask = GetWatchProvidersResponseDataModel(tmdbTvSeriesId, seasonNumber, tmdbHttpClient);

                        // NOTE: Strictly speaking, this is not needed, but it's a good marker for when all tasks have been started
                        await Task.WhenAll(tmdbTvSeasonTask, tmdbTvSeasonWatchProvidersTask);

                        TmdbTvSeasonResponseDataModel? tmdbTvSeasonResponseDataModel = await tmdbTvSeasonTask;
                        TmdbWatchProvidersResponseDataModel? tmdbTvSeasonWatchProvidersResponseDataModel = await tmdbTvSeasonWatchProvidersTask;

                        if (tmdbTvSeasonResponseDataModel == null)
                        {
                            Log.Debug($"TV season TmdbTvSeasonResponseDataModel for search with TMDB TV series ID '{tmdbTvSeriesId}' and seasonNumber '{seasonNumber}' was null!");
                        }
                        if (tmdbTvSeasonWatchProvidersResponseDataModel == null)
                        {
                            Log.Debug($"TV season TmdbWatchProvidersResponseDataModel for search with TMDB TV series ID '{tmdbTvSeriesId}' and seasonNumber '{seasonNumber}' was null!");
                        }

                        TvSeasonViewModel tvSeasonViewModel;
                        // Null checks all happened above, but there's no good way to let the compiler know about that, so recheck here
                        if (tmdbTvSeasonResponseDataModel != null
                            && tmdbTvSeasonWatchProvidersResponseDataModel != null)
                        {
                            tvSeasonViewModel = new TvSeasonViewModel(tmdbTvSeasonResponseDataModel,
                                                                      tmdbTvSeasonWatchProvidersResponseDataModel);
                        }
                        else
                        {
                            return null;  // TODO: return proper HTML error codes
                        }

                        tvSeasonViewModelJson = Results.Json(tvSeasonViewModel);

                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                            .SetSlidingExpiration(TimeSpan.FromHours(1));
                        cache.Set(tvSeasonCacheKey, tvSeasonViewModelJson, cacheEntryOptions);
                    }  // end cache block
                }
                catch (Exception e)
                {
                    string username = user.Identity?.Name ?? "<no username found>";

                    Log.ForContext("Username", username)
                        .Error(e, $"An error occurred while processing the /tvseason request with TMDB TV series ID '{tmdbTvSeriesId}' and seasonNumber '{seasonNumber}'.");

                    throw;
                }

                return tvSeasonViewModelJson;
            }
        )
        .WithSummary("TV Season")
        .WithDescription("Searches TMDB for detailed information on TV seasons.")
        .RequireAuthorization(ProgramConstants.LoggedInUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireAuthorization(ProgramConstants.SearchUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireRateLimiting(ProgramConstants.TokenRateLimiterPolicyName);
    }

    public async static Task<TmdbTvSeasonResponseDataModel?> GetTmdbTvSeasonResponseDataModel(int tmdbTvSeriesId, int seasonNumber, TmdbHttpClient tmdbHttpClient)
    {
        TmdbTvSeasonResponseDataModel? tmdbTvSeasonResponseDataModel = await tmdbHttpClient.GetTvSeason(tmdbTvSeriesId, seasonNumber);
        if (tmdbTvSeasonResponseDataModel == null)
            return null;
        
        Log.Debug($"TMDB TV season details response:\n\n{tmdbTvSeasonResponseDataModel}\n\n");

        return tmdbTvSeasonResponseDataModel;
    }

    public async static Task<TmdbWatchProvidersResponseDataModel?> GetWatchProvidersResponseDataModel(int tmdbTvSeriesId, int seasonNumber, TmdbHttpClient tmdbHttpClient)
    {
        TmdbWatchProvidersResponseDataModel? tmdbTvSeasonWatchProvidersResponseDataModel = await tmdbHttpClient.GetTvSeasonWatchProviders(tmdbTvSeriesId, seasonNumber);
        if (tmdbTvSeasonWatchProvidersResponseDataModel == null)
            return null;
        
        Log.Debug($"TMDB TV season watch providers response:\n\n{tmdbTvSeasonWatchProvidersResponseDataModel}\n\n");

        return tmdbTvSeasonWatchProvidersResponseDataModel;
    }
}