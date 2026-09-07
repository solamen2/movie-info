using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;
using MovieInfoBackend.DataModels;
using static MovieInfoBackend.DataModels.TmdbConfigurationCountriesResponseDataModel;
using static MovieInfoBackend.DataModels.TmdbConfigurationLanguagesResponseDataModel;

public class TmdbHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly string _readAccessToken;
    public static string CachePrefix = "tmdb-";
    public readonly TimeSpan AbsoluteExpirationTime = TimeSpan.FromDays(1);
    public readonly TimeSpan SlidingExpirationTime = TimeSpan.FromHours(1); 

    public TmdbHttpClient(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;

        _readAccessToken = Environment.GetEnvironmentVariable("TMDB_API_READ_ACCESS_TOKEN") ?? "No TMDB Read Access Token Found";

        _httpClient.BaseAddress = new Uri("https://api.themoviedb.org/3/");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Referer, "https://movieinfo.dev/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _readAccessToken);
    }

    public async Task<ConfigurationCountriesDictionary?> GetCountries()
    {
        string countriesCacheKey = CachePrefix + "countries";
        ConfigurationCountriesDictionary? countriesDictionary;

        if (!_cache.TryGetValue(countriesCacheKey, out countriesDictionary))
        {
            using HttpResponseMessage response = await _httpClient.GetAsync($"configuration/countries");
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }
            string responseJsonString = await response.Content.ReadAsStringAsync();
        
            TmdbConfigurationCountriesResponseDataModel? countriesResponseDataModel = GetConfigurationCountriesModelFromResponse(responseJsonString);
            if (countriesResponseDataModel == null)
            {
                return null;
            }
            countriesDictionary = countriesResponseDataModel.GetConfigurationCountriesDictionary();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(AbsoluteExpirationTime)
                .SetSlidingExpiration(SlidingExpirationTime);
            _cache.Set(countriesCacheKey, countriesDictionary, cacheEntryOptions);
        }
        
        return countriesDictionary;
    }
    
    public async Task<ConfigurationLanguagesDictionary?> GetLanguages()
    {
        string languagesCacheKey = CachePrefix + "languages";
        ConfigurationLanguagesDictionary? languagesDictionary;

        if (!_cache.TryGetValue(languagesCacheKey, out languagesDictionary))
        {
            using HttpResponseMessage response = await _httpClient.GetAsync($"configuration/languages");
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }
            string responseJsonString = await response.Content.ReadAsStringAsync();

            TmdbConfigurationLanguagesResponseDataModel? languagesResponseDataModel = GetConfigurationLanguagesModelFromResponse(responseJsonString);
            if (languagesResponseDataModel == null)
            {
                return null;
            }
            languagesDictionary = languagesResponseDataModel.GetConfigurationLanguagesDictionary();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(AbsoluteExpirationTime)
                .SetSlidingExpiration(SlidingExpirationTime);
            _cache.Set(languagesCacheKey, languagesDictionary, cacheEntryOptions);
        }
        
        return languagesDictionary;
    }

    public async Task<TmdbGenresResponseDataModel?> GetMovieGenres()
    {
        string movieGenresCacheKey = CachePrefix + "movie_genres";
        TmdbGenresResponseDataModel? movieGenresResponseDataModel;
        
        if (!_cache.TryGetValue(movieGenresCacheKey, out movieGenresResponseDataModel))
        {
            using HttpResponseMessage response = await _httpClient.GetAsync($"genre/movie/list");
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }
            string responseJsonString = await response.Content.ReadAsStringAsync();

            movieGenresResponseDataModel = GetGenresModelFromResponse(responseJsonString);
            if (movieGenresResponseDataModel == null)
            {
                return null;
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(AbsoluteExpirationTime)
                .SetSlidingExpiration(SlidingExpirationTime);
            _cache.Set(movieGenresCacheKey, movieGenresResponseDataModel, cacheEntryOptions);
        }
        
        return movieGenresResponseDataModel;
    }

    public async Task<TmdbGenresResponseDataModel?> GetTvSeriesGenres()
    {
        string tvGenresCacheKey = CachePrefix + "tv_genres";
        TmdbGenresResponseDataModel? tvGenresResponseDataModel;
        
        if (!_cache.TryGetValue(tvGenresCacheKey, out tvGenresResponseDataModel))
        {
            using HttpResponseMessage response = await _httpClient.GetAsync($"genre/tv/list");
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }
            string responseJsonString = await response.Content.ReadAsStringAsync();

            tvGenresResponseDataModel = GetGenresModelFromResponse(responseJsonString);
            if (tvGenresResponseDataModel == null)
            {
                return null;
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(AbsoluteExpirationTime)
                .SetSlidingExpiration(SlidingExpirationTime);
            _cache.Set(tvGenresCacheKey, tvGenresResponseDataModel, cacheEntryOptions);
        }
        
        return tvGenresResponseDataModel;
    }

    public async Task<TmdbIdResponseDataModel?> GetFindByImdbIdResults(string imdbId)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"find/{HttpUtility.HtmlEncode(imdbId)}?external_source=imdb_id");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetIdModelFromResponse(responseJsonString);
    }

    public async Task<TmdbMovieResponseDataModel?> GetMovie(int movieTmdbId)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"movie/{movieTmdbId}");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetMovieModelFromResponse(responseJsonString);
    }

    public async Task<TmdbMovieCreditsResponseDataModel?> GetMovieCredits(int movieTmdbId)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"movie/{movieTmdbId}/credits");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetMovieCreditsModelFromResponse(responseJsonString);
    }

    public async Task<TmdbMovieExternalIdsResponseDataModel?> GetMovieExternalIds(int movieTmdbId)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"movie/{movieTmdbId}/external_ids");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetMovieExternalIdsModelFromResponse(responseJsonString);
    }

    public async Task<TmdbPersonResponseDataModel?> GetPerson(int personTmdbId)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"person/{personTmdbId}");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetPersonModelFromResponse(responseJsonString);
    }

    public async Task<TmdbPersonExternalIdsResponseDataModel?> GetPersonExternalIds(int personTmdbId)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"person/{personTmdbId}/external_ids");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetPersonExternalIdsModelFromResponse(responseJsonString);
    }

    public async Task<TmdbPersonImagesResponseDataModel?> GetPersonImages(int personTmdbId)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"person/{personTmdbId}/images");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetPersonImagesModelFromResponse(responseJsonString);
    }

    public async Task<TmdbPersonMovieCreditsResponseDataModel?> GetPersonMovieCredits(int personTmdbId)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"person/{personTmdbId}/movie_credits");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetPersonMovieCreditsModelFromResponse(responseJsonString);
    }

    public async Task<TmdbPersonTvSeriesCreditsResponseDataModel?> GetPersonTvSeriesCredits(int personTmdbId)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"person/{personTmdbId}/tv_credits");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetPersonTvSeriesCreditsModelFromResponse(responseJsonString);
    }

    public async Task<TmdbTvEpisodeResponseDataModel?> GetTvEpisode(int tvSeriesTmdbId, int seasonNumber, int episodeNumber)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"tv/{tvSeriesTmdbId}/season/{seasonNumber}/episode/{episodeNumber}");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetTvEpisodeModelFromResponse(responseJsonString);
    }

    public async Task<TmdbTvEpisodeCreditsResponseDataModel?> GetTvEpisodeCredits(int tvSeriesTmdbId, int seasonNumber, int episodeNumber)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"tv/{tvSeriesTmdbId}/season/{seasonNumber}/episode/{episodeNumber}/credits");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetTvEpisodeCreditsModelFromResponse(responseJsonString);
    }

    public async Task<TmdbTvEpisodeExternalIdsResponseDataModel?> GetTvEpisodeExternalIds(int tvSeriesTmdbId, int seasonNumber, int episodeNumber)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"tv/{tvSeriesTmdbId}/season/{seasonNumber}/episode/{episodeNumber}/external_ids");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetTvEpisodeExternalIdsModelFromResponse(responseJsonString);
    }

    public async Task<TmdbTvSeasonResponseDataModel?> GetTvSeason(int tvSeriesTmdbId, int seasonNumber)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"tv/{tvSeriesTmdbId}/season/{seasonNumber}");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetTvSeasonModelFromResponse(responseJsonString);
    }

    public async Task<TmdbTvSeriesResponseDataModel?> GetTvSeries(int tvSeriesTmdbId)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"tv/{tvSeriesTmdbId}");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetTvSeriesModelFromResponse(responseJsonString);
    }

    public async Task<TmdbTvSeriesAggregateCreditsResponseDataModel?> GetTvSeriesAggregateCredits(int tvSeriesTmdbId)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"tv/{tvSeriesTmdbId}/aggregate_credits");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetTvSeriesAggregateCreditsModelFromResponse(responseJsonString);
    }

    public async Task<TmdbTvSeriesExternalIdsResponseDataModel?> GetTvSeriesExternalIds(int tvSeriesTmdbId)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"tv/{tvSeriesTmdbId}/external_ids");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetTvSeriesExternalIdsModelFromResponse(responseJsonString);
    }

    public async Task<TmdbWatchProvidersResponseDataModel?> GetMovieWatchProviders(int movieTmdbId)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"movie/{movieTmdbId}/watch/providers");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetWatchProvidersModelFromResponse(responseJsonString);
    }

    public async Task<TmdbWatchProvidersResponseDataModel?> GetTvSeasonWatchProviders(int tvSeriesTmdbId, int seasonNumber)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"tv/{tvSeriesTmdbId}/season/{seasonNumber}/watch/providers");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetWatchProvidersModelFromResponse(responseJsonString);
    }

    public async Task<TmdbWatchProvidersResponseDataModel?> GetTvSeriesWatchProviders(int tvSeriesTmdbId)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"tv/{tvSeriesTmdbId}/watch/providers");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetWatchProvidersModelFromResponse(responseJsonString);
    }

    public static TmdbConfigurationCountriesResponseDataModel? GetConfigurationCountriesModelFromResponse(string responseJsonString)
    {
        TmdbConfigurationCountryDataModel[]? configurationCountries = JsonSerializer.Deserialize<TmdbConfigurationCountryDataModel[]>(responseJsonString);
        if (configurationCountries == null)
        {
            return null;
        }
        TmdbConfigurationCountriesResponseDataModel countriesResponse = new TmdbConfigurationCountriesResponseDataModel { Countries = configurationCountries };
        
        return countriesResponse;
    }
    public static TmdbConfigurationLanguagesResponseDataModel? GetConfigurationLanguagesModelFromResponse(string responseJsonString)
    {
        TmdbConfigurationLanguageDataModel[]? configurationLanguages = JsonSerializer.Deserialize<TmdbConfigurationLanguageDataModel[]>(responseJsonString);
        if (configurationLanguages == null)
        {
            return null;
        }
        TmdbConfigurationLanguagesResponseDataModel languagesResponse = new TmdbConfigurationLanguagesResponseDataModel { Languages = configurationLanguages };

        return languagesResponse;
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
    public static TmdbMovieCreditsResponseDataModel? GetMovieCreditsModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbMovieCreditsResponseDataModel>(responseJsonString);
    }
    public static TmdbMovieExternalIdsResponseDataModel? GetMovieExternalIdsModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbMovieExternalIdsResponseDataModel>(responseJsonString);
    }
    public static TmdbPersonResponseDataModel? GetPersonModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbPersonResponseDataModel>(responseJsonString);
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
    public static TmdbPersonTvSeriesCreditsResponseDataModel? GetPersonTvSeriesCreditsModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbPersonTvSeriesCreditsResponseDataModel>(responseJsonString);
    }
    public static TmdbTvEpisodeResponseDataModel? GetTvEpisodeModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbTvEpisodeResponseDataModel>(responseJsonString);
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
    public static TmdbTvSeriesResponseDataModel? GetTvSeriesModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbTvSeriesResponseDataModel>(responseJsonString);
    }
    public static TmdbTvSeriesAggregateCreditsResponseDataModel? GetTvSeriesAggregateCreditsModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbTvSeriesAggregateCreditsResponseDataModel>(responseJsonString);
    }
    public static TmdbTvSeriesExternalIdsResponseDataModel? GetTvSeriesExternalIdsModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbTvSeriesExternalIdsResponseDataModel>(responseJsonString);
    }
    public static TmdbWatchProvidersResponseDataModel? GetWatchProvidersModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<TmdbWatchProvidersResponseDataModel>(responseJsonString);
    }
}