using System.Text.Json;
using System.Web;
using Microsoft.Net.Http.Headers;
using MovieInfoBackend.DataModels;

public class OmdbHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OmdbHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("OMDB_API_KEY") ?? "No OMDB API Key Found";

        _httpClient.BaseAddress = new Uri("https://www.omdbapi.com");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Referer, "https://movieinfo.dev/");
    }

    public async Task<OmdbResponseDataModel?> GetMedia(string imdbId)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"?i={HttpUtility.HtmlEncode(imdbId)}&apikey={_apiKey}");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }
        
        string responseJsonString = await response.Content.ReadAsStringAsync();
        
        return GetModelFromResponse(responseJsonString);
    }

    public static OmdbResponseDataModel? GetModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<OmdbResponseDataModel>(responseJsonString);
    }
}