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
public class PersonEndpoint
{
    public static string CachePrefix = "person-";

    public static void Map(WebApplication app)
    {
        app.MapGet($"{ApiRoutePrefix}/person", [Authorize]
            async (
                string imdbId,
                ClaimsPrincipal user,
                [FromServices] SuggestionHttpClient suggestionHttpClient,
                [FromServices] TmdbHttpClient tmdbHttpClient,
                [FromServices] IMemoryCache cache) =>
            {
                IResult? personViewModelJson;
                
                try
                {
                    string personCacheKey = CachePrefix + imdbId;  // NOTE: We may not hit the cache that often for suggestions, but being a bit paranoid here to minimize impact

                    if (!cache.TryGetValue(personCacheKey, out personViewModelJson))
                    {
                        string username = user?.Identity?.Name ?? "<no username found>";
                        Log.Debug($"Username: {username}");

                        Task<SuggestionViewModel?> suggestionTask = GetSuggestionViewModel(imdbId, suggestionHttpClient, cache);
                        int? tmdbPersonIdNullable = await GetTmdbId(imdbId, tmdbHttpClient);
                        if (tmdbPersonIdNullable == null)
                        {
                            Log.Debug($"Person TMDB ID for search '{imdbId}' was null!");
                            return null;
                        }
                        int tmdbPersonId = tmdbPersonIdNullable.GetValueOrDefault();
                        Task<TmdbPersonResponseDataModel?> tmdbPersonTask = GetTmdbPersonResponseDataModel(tmdbPersonId, tmdbHttpClient);
                        Task<TmdbPersonMovieCreditsResponseDataModel?> tmdbPersonMovieCreditsTask = GetTmdbPersonMovieCreditsResponseDataModel(tmdbPersonId, tmdbHttpClient);
                        Task<TmdbPersonTvSeriesCreditsResponseDataModel?> tmdbPersonTvSeriesCreditsTask = GetTmdbPersonTvSeriesCreditsResponseDataModel(tmdbPersonId, tmdbHttpClient);
                        Task<TmdbPersonImagesResponseDataModel?> tmdbPersonImagesTask = GetTmdbPersonImagesResponseDataModel(tmdbPersonId, tmdbHttpClient);

                        // NOTE: Strictly speaking, this is not needed, but it's a good marker for when all tasks have been started
                        await Task.WhenAll(suggestionTask, tmdbPersonTask, tmdbPersonMovieCreditsTask, tmdbPersonTvSeriesCreditsTask, tmdbPersonImagesTask);

                        SuggestionViewModel? personSuggestionViewModel = await suggestionTask;
                        TmdbPersonResponseDataModel? tmdbPersonResponseDataModel = await tmdbPersonTask;
                        TmdbPersonMovieCreditsResponseDataModel? tmdbPersonMovieCreditsResponseDataModel = await tmdbPersonMovieCreditsTask;
                        TmdbPersonTvSeriesCreditsResponseDataModel? tmdbPersonTvSeriesCreditsResponseDataModel = await tmdbPersonTvSeriesCreditsTask;
                        TmdbPersonImagesResponseDataModel? tmdbPersonImagesResponseDataModel = await tmdbPersonImagesTask;

                        if (personSuggestionViewModel == null)
                        {
                            Log.Debug($"Person SuggestionViewModel for search '{imdbId}' was null!");
                        }
                        if (tmdbPersonResponseDataModel == null)
                        {
                            Log.Debug($"Person TmdbPersonMovieResponseDataModel for search '{imdbId}' and TMDB ID '{tmdbPersonId}' was null!");
                        }
                        if (tmdbPersonMovieCreditsResponseDataModel == null)
                        {
                            Log.Debug($"Person TmdbPersonMovieCreditsResponseDataModel for search '{imdbId}' and TMDB ID '{tmdbPersonId}' was null!");
                        }
                        if (tmdbPersonTvSeriesCreditsResponseDataModel == null)
                        {
                            Log.Debug($"Person TmdbPersonTvSeriesCreditsResponseDataModel for search '{imdbId}' and TMDB ID '{tmdbPersonId}' was null!");
                        }
                        if (tmdbPersonImagesResponseDataModel == null)
                        {
                            Log.Debug($"Person TmdbPersonImagesResponseDataModel for search '{imdbId}' and TMDB ID '{tmdbPersonId}' was null!");
                        }

                        PersonViewModel personViewModel;
                        // Null checks all happened above, but there's no good way to let the compiler know about that, so recheck here
                        if (personSuggestionViewModel != null
                            && tmdbPersonResponseDataModel != null
                            && tmdbPersonMovieCreditsResponseDataModel != null
                            && tmdbPersonTvSeriesCreditsResponseDataModel != null
                            && tmdbPersonImagesResponseDataModel != null)
                        {
                            personViewModel = new PersonViewModel(personSuggestionViewModel,
                                                                  tmdbPersonResponseDataModel,
                                                                  tmdbPersonMovieCreditsResponseDataModel,
                                                                  tmdbPersonTvSeriesCreditsResponseDataModel,
                                                                  tmdbPersonImagesResponseDataModel);
                        }
                        else
                        {
                            return null;  // TODO: return proper HTML error codes
                        }

                        personViewModelJson = Results.Json(personViewModel);

                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                            .SetSlidingExpiration(TimeSpan.FromHours(1));
                        cache.Set(personCacheKey, personViewModelJson, cacheEntryOptions);
                    }  // end cache block
                }
                catch (Exception e)
                {
                    string username = user.Identity?.Name ?? "<no username found>";

                    Log.ForContext("Username", username)
                        .Error(e, $"An error occurred while processing the /person request '{imdbId}'.");

                    throw;
                }

                return personViewModelJson;
            }
        )
        .WithSummary("Person")
        .WithDescription("Searches IMDB and TMDB for detailed information on people.")
        .RequireAuthorization(ProgramConstants.LoggedInUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireAuthorization(ProgramConstants.SearchUsersOnlyPolicyName)  // TODO: Check that this returns appropriate error on frontend
        .RequireRateLimiting(ProgramConstants.TokenRateLimiterPolicyName);
    }

    public async static Task<SuggestionViewModel?> GetSuggestionViewModel(string imdbId, SuggestionHttpClient suggestionHttpClient, IMemoryCache cache)
    {
        SuggestionViewModel? personSuggestionViewModel;
        string personSuggestionCacheKey = CachePrefix + imdbId;  // NOTE: We may not hit the cache that often for suggestions, but being a bit paranoid here to minimize impact
        
        if (!cache.TryGetValue(personSuggestionCacheKey, out personSuggestionViewModel))
        {
            SuggestionsResponseDataModel? suggestionsResponse = await suggestionHttpClient.GetSuggestions(imdbId);

            if (suggestionsResponse == null || suggestionsResponse.Suggestions == null || suggestionsResponse.Suggestions.Length <= 0)
                return null;
            Log.Debug($"Suggestions:\n\n{suggestionsResponse}\n\n");   // NOTE: Not destructuring using @ operator because Serilog doesn't let you configure output easily
                                                                       // (and Seq doesn't support Azure Container Apps, so it's not used in this app)

            personSuggestionViewModel = new SuggestionViewModel(suggestionsResponse.Suggestions[0]);
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                .SetSlidingExpiration(TimeSpan.FromHours(1));
            cache.Set(personSuggestionCacheKey, personSuggestionViewModel, cacheEntryOptions);
        }
        return personSuggestionViewModel;
    }

    public async static Task<int?> GetTmdbId(string imdbId, TmdbHttpClient tmdbHttpClient)
    {
        TmdbIdResponseDataModel? tmdbIdResponseDataModel = await tmdbHttpClient.GetFindByImdbIdResults(imdbId);
        if (tmdbIdResponseDataModel == null || tmdbIdResponseDataModel.PersonResults == null || tmdbIdResponseDataModel.PersonResults.Length <= 0)
            return null;
        
        Log.Debug($"TMDB person find by IMDB Id response:\n\n{tmdbIdResponseDataModel}\n\n");

        return tmdbIdResponseDataModel.PersonResults[0].TmdbId;
    }

    public async static Task<TmdbPersonResponseDataModel?> GetTmdbPersonResponseDataModel(int tmdbId, TmdbHttpClient tmdbHttpClient)
    {
        TmdbPersonResponseDataModel? tmdbPersonResponseDataModel = await tmdbHttpClient.GetPerson(tmdbId);
        if (tmdbPersonResponseDataModel == null)
            return null;
        
        Log.Debug($"TMDB person details response:\n\n{tmdbPersonResponseDataModel}\n\n");

        return tmdbPersonResponseDataModel;
    }

    public async static Task<TmdbPersonMovieCreditsResponseDataModel?> GetTmdbPersonMovieCreditsResponseDataModel(int tmdbId, TmdbHttpClient tmdbHttpClient)
    {
        TmdbPersonMovieCreditsResponseDataModel? tmdbPersonMovieCreditsResponseDataModel = await tmdbHttpClient.GetPersonMovieCredits(tmdbId);
        if (tmdbPersonMovieCreditsResponseDataModel == null)
            return null;
        
        Log.Debug($"TMDB person movie credits response:\n\n{tmdbPersonMovieCreditsResponseDataModel}\n\n");

        return tmdbPersonMovieCreditsResponseDataModel;
    }

    public async static Task<TmdbPersonTvSeriesCreditsResponseDataModel?> GetTmdbPersonTvSeriesCreditsResponseDataModel(int tmdbId, TmdbHttpClient tmdbHttpClient)
    {
        TmdbPersonTvSeriesCreditsResponseDataModel? tmdbPersonTvSeriesCreditsResponseDataModel = await tmdbHttpClient.GetPersonTvSeriesCredits(tmdbId);
        if (tmdbPersonTvSeriesCreditsResponseDataModel == null)
            return null;
        
        Log.Debug($"TMDB person TV series credits response:\n\n{tmdbPersonTvSeriesCreditsResponseDataModel}\n\n");

        return tmdbPersonTvSeriesCreditsResponseDataModel;
    }

    public async static Task<TmdbPersonImagesResponseDataModel?> GetTmdbPersonImagesResponseDataModel(int tmdbId, TmdbHttpClient tmdbHttpClient)
    {
        TmdbPersonImagesResponseDataModel? tmdbPersonImagesResponseDataModel = await tmdbHttpClient.GetPersonImages(tmdbId);
        if (tmdbPersonImagesResponseDataModel == null)
            return null;
        
        Log.Debug($"TMDB person images response:\n\n{tmdbPersonImagesResponseDataModel}\n\n");

        return tmdbPersonImagesResponseDataModel;
    }
}