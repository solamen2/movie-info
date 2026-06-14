using System.Text.Json;
using System.Web;
using Microsoft.Net.Http.Headers;
using MovieInfoBackend.DataModels;

public class TmdbHttpClient
{
    private readonly HttpClient _httpClient;
    public static string CachePrefix = "tmdb-";

    public TmdbHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;

        // TODO Add real headers later
        // All below info copied from https://github.com/pavan412kalyan/imdb-movie-scraper/blob/main/ImdbDataExtraction/search_by_string/search_by_string.py
        //_httpClient.BaseAddress = new Uri("https://v3.sg.media-imdb.com");
        //_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json, text/plain, */*");
        //_httpClient.DefaultRequestHeaders.Add(HeaderNames.AcceptLanguage, "en-US,en;q=0.9");
        //_httpClient.DefaultRequestHeaders.Add(HeaderNames.Origin, "https://m.imdb.com");
        //_httpClient.DefaultRequestHeaders.Add("Priority", "u=1, i");
        //_httpClient.DefaultRequestHeaders.Add(HeaderNames.Referer, "https://m.imdb.com/");
        //_httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, 
        //    "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Mobile Safari/537.36");
    }

    // TODO Implement actual methods later
    //public async Task<MovieSuggestionsResponseDataModel?> GetSuggestions(string searchQuery)
    //{
        //using HttpResponseMessage response = await _httpClient.GetAsync($"suggestion/a/{HttpUtility.HtmlEncode(searchQuery)}.json");
        //if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //{
        //    return null;
        //}
        //
        //string responseJsonString = await response.Content.ReadAsStringAsync();
        //
        //return GetModelFromResponse(responseJsonString);
    //}

    public static TmdbMovieCreditsResponseDataModel? GetMovieCreditsModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbMovieCreditsResponseDataModel>(responseJsonString);
    }
    public static TmdbGenresResponseDataModel? GetGenresModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbGenresResponseDataModel>(responseJsonString);
    }
    public static TmdbIdResponseDataModel? GetIdModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbIdResponseDataModel>(responseJsonString);
    }
    public static TmdbMovieResponseDataModel? GetMovieModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbMovieResponseDataModel>(responseJsonString);
    }
    public static TmdbPersonExternalIdsResponseDataModel? GetPersonExternalIdsModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbPersonExternalIdsResponseDataModel>(responseJsonString);
    }
    public static TmdbPersonImagesResponseDataModel? GetPersonImagesModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbPersonImagesResponseDataModel>(responseJsonString);
    }
    public static TmdbPersonMovieCreditsResponseDataModel? GetPersonMovieCreditsModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbPersonMovieCreditsResponseDataModel>(responseJsonString);
    }
    public static TmdbPersonResponseDataModel? GetPersonModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbPersonResponseDataModel>(responseJsonString);
    }
    public static TmdbPersonTvSeriesCreditsResponseDataModel? GetPersonTvSeriesCreditsModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbPersonTvSeriesCreditsResponseDataModel>(responseJsonString);
    }
    public static TmdbTvEpisodeCreditsResponseDataModel? GetTvEpisodeCreditsModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbTvEpisodeCreditsResponseDataModel>(responseJsonString);
    }
    public static TmdbTvEpisodeExternalIdsResponseDataModel? GetTvEpisodeExternalIdsModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbTvEpisodeExternalIdsResponseDataModel>(responseJsonString);
    }
    public static TmdbTvSeasonResponseDataModel? GetTvSeasonModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbTvSeasonResponseDataModel>(responseJsonString);
    }
    public static TmdbTvSeriesAggregateCreditsResponseDataModel? GetTvSeriesAggregateCreditsModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbTvSeriesAggregateCreditsResponseDataModel>(responseJsonString);
    }
    public static TmdbTvSeriesResponseDataModel? GetTvSeriesModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbTvSeriesResponseDataModel>(responseJsonString);
    }
    public static TmdbWatchProvidersResponseDataModel? GetWatchProvidersModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbWatchProvidersResponseDataModel>(responseJsonString);
    }
}