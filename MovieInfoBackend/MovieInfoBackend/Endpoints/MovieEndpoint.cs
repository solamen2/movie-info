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
                [FromServices] WikidataHttpClient wikidataHttpClient,
                [FromServices] OscarsHttpClient oscarsHttpClient,
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
                        Task<OscarsViewModel?> oscarsTask = GetOscarsViewModel(oscarsHttpClient);
                        Task<List<AspectRatioViewModel>?> aspectRatiosTask = GetAspectRatios(imdbId, wikidataHttpClient);
                        // Wikidata knows the movie's Wikipedia page by its IMDB ID, which is exact (a search by title can land on another film, or on the book)
                        Task<string?> wikidataWikipediaLinkTask = GetWikidataWikipediaLink(imdbId, wikidataHttpClient);
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
                        // Without a page from Wikidata, Wikipedia is searched by the title and year from the suggestion (see WikipediaHttpClient.GetMovieLink), so that lookup can only start once the suggestion is in
                        SuggestionViewModel? movieSuggestionViewModel = await suggestionTask;
                        Task<string?> wikipediaLinkTask = GetWikipediaLink(movieSuggestionViewModel, wikidataWikipediaLinkTask, wikipediaHttpClient);
                        // The page's text (for its plot and Rotten Tomatoes critics consensus) is only read from the page Wikidata names: a page found by title is good enough to link to, but not certain enough to quote
                        Task<string?> wikipediaPageTextTask = GetWikipediaPageText(wikidataWikipediaLinkTask, wikipediaHttpClient);

                        // NOTE: Strictly speaking, this is not needed, but it's a good marker for when all tasks have been started
                        await Task.WhenAll(omdbMovieTask, tmdbCountriesTask, tmdbLanguagesTask, oscarsTask, aspectRatiosTask, tmdbMovieTask, tmdbMovieCreditsTask, tmdbMovieWatchProvidersTask, wikidataWikipediaLinkTask, wikipediaLinkTask, wikipediaPageTextTask);

                        OmdbResponseDataModel? omdbMovieResponseDataModel = await omdbMovieTask;
                        ConfigurationCountriesDictionary? tmdbConfigurationCountriesDictionary = await tmdbCountriesTask;
                        ConfigurationLanguagesDictionary? tmdbConfigurationLanguagesDictionary = await tmdbLanguagesTask;
                        TmdbMovieResponseDataModel? tmdbMovieResponseDataModel = await tmdbMovieTask;
                        TmdbMovieCreditsResponseDataModel? tmdbMovieCreditsResponseDataModel = await tmdbMovieCreditsTask;
                        TmdbWatchProvidersResponseDataModel? tmdbMovieWatchProvidersResponseDataModel = await tmdbMovieWatchProvidersTask;
                        string? wikipediaLink = await wikipediaLinkTask;
                        string? wikipediaPageText = await wikipediaPageTextTask;
                        OscarsViewModel? oscarsViewModel = await oscarsTask;
                        List<AspectRatioViewModel>? aspectRatios = await aspectRatiosTask;

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
                        if (wikipediaPageText == null)
                        {
                            Log.Debug($"Movie Wikipedia page text for search '{imdbId}' was null!");
                        }
                        if (aspectRatios == null)
                        {
                            Log.Debug($"Movie aspect ratios for search '{imdbId}' were null!");
                        }
                        if (oscarsViewModel == null)
                        {
                            Log.Warning($"Oscars data unavailable for the /movie request '{imdbId}'.");
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
                            // Null only when the Oscars data is unavailable; a movie without awards gets an empty list
                            List<OscarAwardViewModel>? movieOscars =
                                oscarsViewModel == null ? null : OscarAwardsHelper.ForMovie(oscarsViewModel, movieSuggestionViewModel.ItemID, movieSuggestionViewModel.Name);

                            movieViewModel = new MovieViewModel(movieSuggestionViewModel,
                                                                omdbMovieResponseDataModel,
                                                                tmdbMovieResponseDataModel,
                                                                tmdbMovieCreditsResponseDataModel,
                                                                tmdbMovieWatchProvidersResponseDataModel,
                                                                tmdbConfigurationCountriesDictionary,
                                                                tmdbConfigurationLanguagesDictionary,
                                                                wikipediaLink,
                                                                WikipediaMovieHelper.GetPlot(wikipediaPageText),
                                                                WikipediaMovieHelper.GetRtCriticsConsensus(wikipediaPageText),
                                                                movieOscars,
                                                                aspectRatios);
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
        .WithDescription("Searches IMDB, TMDB, OMDB, Wikipedia, Wikidata, and the Academy Awards for detailed information on movies, looked up by IMDB ID or by TMDB ID.")
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

    // Null when the dataset couldn't be downloaded or parsed, which the callers treat as the Oscars being unavailable
    // rather than as an error
    public async static Task<OscarsViewModel?> GetOscarsViewModel(OscarsHttpClient oscarsHttpClient)
    {
        OscarsViewModel? oscarsViewModel;
        try
        {
            oscarsViewModel = await oscarsHttpClient.GetOscars();
        }
        catch (Exception e) when (e is HttpRequestException || e is TaskCanceledException || e is Polly.CircuitBreaker.BrokenCircuitException)
        {
            Log.Warning(e, "Downloading the Academy Awards data failed.");
            return null;
        }

        if (oscarsViewModel == null)
        {
            Log.Warning("Oscars data unavailable: the Academy Awards dataset could not be downloaded or parsed.");
        }
        else
        {
            Log.Debug($"Oscars ceremonies: {oscarsViewModel.Years.Count}");
        }

        return oscarsViewModel;
    }

    // The title's aspect ratios from Wikidata: empty when it lists none, null when the lookup failed. They are an extra,
    // so a failed lookup is never an error for the page as a whole. (Also used for TV series.)
    public async static Task<List<AspectRatioViewModel>?> GetAspectRatios(string imdbId, WikidataHttpClient wikidataHttpClient)
    {
        WikidataSparqlResponseDataModel? wikidataResponseDataModel;
        try
        {
            wikidataResponseDataModel = await wikidataHttpClient.GetAspectRatios(imdbId);
        }
        catch (Exception e) when (e is HttpRequestException || e is TaskCanceledException || e is Polly.CircuitBreaker.BrokenCircuitException || e is System.Text.Json.JsonException)
        {
            Log.Warning(e, $"Looking up the Wikidata aspect ratios for '{imdbId}' failed.");
            return null;
        }
        if (wikidataResponseDataModel == null)
            return null;

        Log.Debug($"Wikidata aspect ratios response:\n\n{wikidataResponseDataModel}\n\n");

        return AspectRatiosHelper.FromWikidata(wikidataResponseDataModel);
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

    // The link to the movie's English Wikipedia page according to Wikidata: "" when Wikidata knows of none, null when the
    // lookup failed. An extra, so a failed lookup is never an error for the page as a whole.
    public async static Task<string?> GetWikidataWikipediaLink(string imdbId, WikidataHttpClient wikidataHttpClient)
    {
        string? wikipediaLink;
        try
        {
            wikipediaLink = await wikidataHttpClient.GetEnglishWikipediaLink(imdbId);
        }
        catch (Exception e) when (e is HttpRequestException || e is TaskCanceledException || e is Polly.CircuitBreaker.BrokenCircuitException || e is System.Text.Json.JsonException)
        {
            Log.Warning(e, $"Looking up the Wikidata link to the Wikipedia page for '{imdbId}' failed.");
            return null;
        }
        if (wikipediaLink == null)
            return null;

        Log.Debug($"Wikidata link to the Wikipedia page for '{imdbId}': '{wikipediaLink}'");

        return wikipediaLink;
    }

    // Returns the movie's Wikipedia page link, "" when Wikipedia has no page for it, or null when the lookup failed (or had nothing to search for).
    // The page Wikidata names for the movie when there is one; otherwise (Wikidata knows of none, or could not be asked) the page a search by title and year finds.
    //
    // NOTE ON THE FALLBACK (the search by title, WikipediaHttpClient.GetMovieLink): it is only good enough to link to, which is why the plot and the Rotten Tomatoes
    // critics consensus are never read from the page it finds (see GetWikipediaPageText). It is rarely needed: Wikidata names no page when it is down, when the movie
    // has no English Wikipedia page (the usual reason), or when its Wikidata item lacks the IMDB ID (about 2.7% of some 2,000 film pages checked in November 2026).
    // But when it is needed it is often wrong. Tried then on 1,858 films with a page and 195 without one (searching by their Wikidata titles and years), it:
    //   - linked the wrong page for 19% of the films that have one: another film of a similar name ("Amélie" finds "Amelia (2009 film)"), the book a film is based
    //     on ("Battlefield Earth"), or what a film is named after ("Deepwater Horizon");
    //   - linked some page for 54% of the films that have none.
    //
    // A better fallback was built and then taken out again, as a lot of code and tests for a rare case. Should it be wanted after all, this is what it did, and on the
    // same films it found the right page for 99.5% of those that have one (wrong for 0.2%, none for 0.4%) and linked a page for 4% of those that have none:
    //   1. Ask Wikipedia which articles link to the movie's IMDB page: action=query&list=exturlusage&euquery=www.imdb.com/title/<IMDB ID>&eunamespace=0&euprop=title|url
    //      (it matches by the start of the address, so drop links whose ID only starts with this one). On its own this found 96% of the pages, with one mistake.
    //   2. Get what is needed to judge those pages, all in one call: action=query&titles=<A|B|C>&redirects=1&prop=extracts|templates|info&exintro=1&explaintext=1
    //      &exsentences=2&exlimit=20&tltemplates=Template:Infobox film&tllimit=500&inprop=url. A page is an article about a film if it has "templates" (the film
    //      infobox); "extract" is its opening sentences; "fullurl" is its link.
    //   3. Of the pages from step 1, take the only article about a film. If there are several: the only one with the movie's title; failing that, the only one of
    //      exactly the movie's year. (Other pages link to a film's IMDB page too: its soundtrack's, a sequel's, an article citing it.)
    //   4. Failing that, search (action=opensearch, limit=5) for "Title (YEAR film)", then "Title (film)", then "Title", judging each search's results as in step 2,
    //      and take the first page that is all of: an article about a film; of the movie's year, give or take one, going by the years in its title and the first 400
    //      characters of its opening (anything goes when the year is unknown); and titled as the movie is, once what is in brackets at the end of the page title is
    //      dropped and case, accents and punctuation are ignored (the title the page was found under, which can be a redirect, counts too).
    //   5. Otherwise there is no page ("") - which is the point: a search always finds something, and most films that get this far have no page. A failed call is
    //      null, never "".
    // Its remaining mistakes were films of the same title released within a year of each other, films whose year was not known, and film series pages. With a page
    // that well checked, the plot and the consensus could be read from it too.
    public async static Task<string?> GetWikipediaLink(SuggestionViewModel? movieSuggestionViewModel, Task<string?> wikidataWikipediaLinkTask, WikipediaHttpClient wikipediaHttpClient)
    {
        string? wikidataWikipediaLink = await wikidataWikipediaLinkTask;
        if (!string.IsNullOrEmpty(wikidataWikipediaLink))
            return wikidataWikipediaLink;

        if (movieSuggestionViewModel == null)
            return null;

        string? wikipediaLink = await wikipediaHttpClient.GetMovieLink(movieSuggestionViewModel.Name, movieSuggestionViewModel.Year);
        if (wikipediaLink == null)
            return null;
        
        Log.Debug($"Wikipedia movie link response:\n\n{wikipediaLink}\n\n");

        return wikipediaLink;
    }

    // The plain text of the movie's English Wikipedia page (the one Wikidata names for it), which its plot and Rotten
    // Tomatoes critics consensus are read from (see WikipediaMovieHelper): "" when there is no such page, null when a
    // lookup failed. Both are extras, so a failed lookup is never an error for the page as a whole.
    public async static Task<string?> GetWikipediaPageText(Task<string?> wikidataWikipediaLinkTask, WikipediaHttpClient wikipediaHttpClient)
    {
        string? wikipediaLink = await wikidataWikipediaLinkTask;
        if (wikipediaLink == null)
            return null;
        if (wikipediaLink == "")
            return "";

        string? pageTitle = WikipediaMovieHelper.GetPageTitle(wikipediaLink);
        if (pageTitle == null)
        {
            Log.Warning($"Wikipedia link '{wikipediaLink}' is not an English Wikipedia page link.");
            return null;
        }

        string? pageText;
        try
        {
            pageText = await wikipediaHttpClient.GetPageText(pageTitle);
        }
        catch (Exception e) when (e is HttpRequestException || e is TaskCanceledException || e is Polly.CircuitBreaker.BrokenCircuitException || e is System.Text.Json.JsonException)
        {
            Log.Warning(e, $"Getting the text of the Wikipedia page '{pageTitle}' failed.");
            return null;
        }
        if (pageText == null)
            return null;

        Log.Debug($"Wikipedia page text for '{pageTitle}': {pageText.Length} characters");

        return pageText;
    }
}