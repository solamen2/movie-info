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
public class TvEpisodeEndpoint
{
    public static string CachePrefix = "tv_episode-";

    public static void Map(WebApplication app)
    {
        app.MapGet($"{ApiRoutePrefix}/tvepisode", [Authorize]
            async (
                int tmdbTvSeriesId,
                int seasonNumber,
                int episodeNumber,
                ClaimsPrincipal user,
                [FromServices] OmdbHttpClient omdbHttpClient,
                [FromServices] TmdbHttpClient tmdbHttpClient,
                [FromServices] IMemoryCache cache) =>
            {
                IResult? tvEpisodeViewModelJson;
                
                try
                {
                    // NOTE: We may not hit the cache that often, but being a bit paranoid here to minimize impact
                    string tvEpisodeCacheKey = $"{CachePrefix}-{tmdbTvSeriesId}-{seasonNumber}-{episodeNumber}";

                    if (!cache.TryGetValue(tvEpisodeCacheKey, out tvEpisodeViewModelJson))
                    {
                        string username = user?.Identity?.Name ?? "<no username found>";
                        Log.Debug($"Username: {username}");

                        Task<TmdbTvEpisodeResponseDataModel?> tmdbTvEpisodeTask = GetTmdbTvEpisodeResponseDataModel(tmdbTvSeriesId, seasonNumber, episodeNumber, tmdbHttpClient);
                        Task<TmdbTvEpisodeCreditsResponseDataModel?> tmdbTvEpisodeCreditsTask = GetTmdbTvEpisodeCreditsResponseDataModel(tmdbTvSeriesId, seasonNumber, episodeNumber, tmdbHttpClient);
                        string? imdbTvEpisodeIdNullable = await GetTmdbExternalImdbId(tmdbTvSeriesId, seasonNumber, episodeNumber, tmdbHttpClient);
                        if (imdbTvEpisodeIdNullable == null)
                        {
                            Log.Debug($"TV episode external IMDB ID for search with TMDB TV series ID '{tmdbTvSeriesId}', seasonNumber '{seasonNumber}', and episode number '{episodeNumber}' was null!");
                            return null;
                        }
                        string imdbTvEpisodeId = imdbTvEpisodeIdNullable;
                        Task<OmdbResponseDataModel?> omdbTvEpisodeTask = GetOmdbResponseDataModel(imdbTvEpisodeId, omdbHttpClient);

                        // NOTE: Strictly speaking, this is not needed, but it's a good marker for when all tasks have been started
                        await Task.WhenAll(tmdbTvEpisodeTask, tmdbTvEpisodeCreditsTask, omdbTvEpisodeTask);

                        OmdbResponseDataModel? omdbTvEpisodeResponseDataModel = await omdbTvEpisodeTask;
                        TmdbTvEpisodeResponseDataModel? tmdbTvEpisodeResponseDataModel = await tmdbTvEpisodeTask;
                        TmdbTvEpisodeCreditsResponseDataModel? tmdbTvEpisodeCreditsResponseDataModel = await tmdbTvEpisodeCreditsTask;

                        if (omdbTvEpisodeResponseDataModel == null)
                        {
                            Log.Debug($"TV episode OmdbResponseDataModel for search with TMDB TV series ID '{tmdbTvSeriesId}', seasonNumber '{seasonNumber}', and episode number '{episodeNumber}' was null!");
                        }
                        if (tmdbTvEpisodeResponseDataModel == null)
                        {
                            Log.Debug($"TV episode TmdbTvEpisodeResponseDataModel for search with TMDB TV series ID '{tmdbTvSeriesId}', seasonNumber '{seasonNumber}', and episode number '{episodeNumber}' was null!");
                        }
                        if (tmdbTvEpisodeCreditsResponseDataModel == null)
                        {
                            Log.Debug($"TV episode TmdbTvEpisodeCreditsResponseDataModel for search with TMDB TV series ID '{tmdbTvSeriesId}', seasonNumber '{seasonNumber}', and episode number '{episodeNumber}' was null!");
                        }

                        TvEpisodeViewModel tvEpisodeViewModel;
                        // Null checks all happened above, but there's no good way to let the compiler know about that, so recheck here
                        if (omdbTvEpisodeResponseDataModel != null
                            && tmdbTvEpisodeResponseDataModel != null
                            && tmdbTvEpisodeCreditsResponseDataModel != null)
                        {
                            tvEpisodeViewModel = new TvEpisodeViewModel(omdbTvEpisodeResponseDataModel,
                                                                        tmdbTvEpisodeResponseDataModel,
                                                                        tmdbTvEpisodeCreditsResponseDataModel);
                        }
                        else
                        {
                            return null;  // TODO: return proper HTML error codes
                        }

                        tvEpisodeViewModelJson = Results.Json(tvEpisodeViewModel);

                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                            .SetSlidingExpiration(TimeSpan.FromHours(1));
                        cache.Set(tvEpisodeCacheKey, tvEpisodeViewModelJson, cacheEntryOptions);
                    }  // end cache block
                }
                catch (Exception e)
                {
                    string username = user.Identity?.Name ?? "<no username found>";

                    Log.ForContext("Username", username)
                        .Error(e, $"An error occurred while processing the /tvepisode request with TMDB TV series ID '{tmdbTvSeriesId}', seasonNumber '{seasonNumber}', and episode number '{episodeNumber}'.");

                    throw;
                }

                return tvEpisodeViewModelJson;
            }
        )
        .WithSummary("TV Episode")
        .WithDescription("Searches TMDB and OMDB for detailed information on TV episodes.")
        .RequireAuthorization(ProgramConstants.LoggedInUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireAuthorization(ProgramConstants.SearchUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireRateLimiting(ProgramConstants.TokenRateLimiterPolicyName);
    }

    public async static Task<string?> GetTmdbExternalImdbId(int tmdbTvSeriesId, int seasonNumber, int episodeNumber, TmdbHttpClient tmdbHttpClient)
    {
        TmdbTvEpisodeExternalIdsResponseDataModel? tmdbTvEpisodeExternalIdsResponseDataModel = await tmdbHttpClient.GetTvEpisodeExternalIds(tmdbTvSeriesId, seasonNumber, episodeNumber);
        if (tmdbTvEpisodeExternalIdsResponseDataModel == null || tmdbTvEpisodeExternalIdsResponseDataModel.ImdbId == null)
            return null;
        
        Log.Debug($"TMDB TV episode external IDs response:\n\n{tmdbTvEpisodeExternalIdsResponseDataModel}\n\n");

        return tmdbTvEpisodeExternalIdsResponseDataModel.ImdbId;
    }

    public async static Task<OmdbResponseDataModel?> GetOmdbResponseDataModel(string imdbId, OmdbHttpClient omdbHttpClient)
    {   
        OmdbResponseDataModel? omdbTvEpisodeDataModelResponse = await omdbHttpClient.GetMedia(imdbId);
        if (omdbTvEpisodeDataModelResponse == null)
            return null;
        
        Log.Debug($"OMDB TV episode data response:\n\n{omdbTvEpisodeDataModelResponse}\n\n");

        return omdbTvEpisodeDataModelResponse;
    }

    public async static Task<TmdbTvEpisodeResponseDataModel?> GetTmdbTvEpisodeResponseDataModel(int tmdbTvSeriesId, int seasonNumber, int episodeNumber, TmdbHttpClient tmdbHttpClient)
    {
        TmdbTvEpisodeResponseDataModel? tmdbTvEpisodeResponseDataModel = await tmdbHttpClient.GetTvEpisode(tmdbTvSeriesId, seasonNumber, episodeNumber);
        if (tmdbTvEpisodeResponseDataModel == null)
            return null;
        
        Log.Debug($"TMDB TV episode details response:\n\n{tmdbTvEpisodeResponseDataModel}\n\n");

        return tmdbTvEpisodeResponseDataModel;
    }

    public async static Task<TmdbTvEpisodeCreditsResponseDataModel?> GetTmdbTvEpisodeCreditsResponseDataModel(int tmdbTvSeriesId, int seasonNumber, int episodeNumber, TmdbHttpClient tmdbHttpClient)
    {
        TmdbTvEpisodeCreditsResponseDataModel? tmdbTvEpisodeCreditsResponseDataModel = await tmdbHttpClient.GetTvEpisodeCredits(tmdbTvSeriesId, seasonNumber, episodeNumber);
        if (tmdbTvEpisodeCreditsResponseDataModel == null)
            return null;
        
        Log.Debug($"TMDB TV episode credits response:\n\n{tmdbTvEpisodeCreditsResponseDataModel}\n\n");

        return tmdbTvEpisodeCreditsResponseDataModel;
    }
}