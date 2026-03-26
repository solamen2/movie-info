using System.Text.Json;
using System.Web;
using Microsoft.Net.Http.Headers;
using MovieInfoBackend.DataModels;

public class MovieHttpClient
{
    private readonly HttpClient _httpClient;
    public static string CachePrefix = "imdb-";

    public MovieHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;

        // All below info copied from https://github.com/pavan412kalyan/imdb-movie-scraper/blob/main/ImdbDataExtraction/search_by_string/search_by_string.py
        _httpClient.BaseAddress = new Uri("https://v3.sg.media-imdb.com");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json, text/plain, */*");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.AcceptLanguage, "en-US,en;q=0.9");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Origin, "https://m.imdb.com");
        _httpClient.DefaultRequestHeaders.Add("Priority", "u=1, i");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Referer, "https://m.imdb.com/");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, 
            "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Mobile Safari/537.36");
    }

    public async Task<MovieSuggestionsResponseDataModel?> GetSuggestions(string searchQuery)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"suggestion/a/{HttpUtility.HtmlEncode(searchQuery)}.json");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }

        string responseJsonString = await response.Content.ReadAsStringAsync();

        return GetModelFromResponse(responseJsonString);
    }

    public static MovieSuggestionsResponseDataModel? GetModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<MovieSuggestionsResponseDataModel>(responseJsonString);
    }
}