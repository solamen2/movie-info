using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Net.Http.Headers;
using MovieInfoBackend.DataModels;

public partial class WikidataHttpClient
{
    private readonly HttpClient _httpClient;

    public WikidataHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _httpClient.BaseAddress = new Uri("https://query.wikidata.org/");
        // The query service is slower and less dependable than the other APIs, and only supplies an extra, so don't let it
        // hold up a whole page (the default timeout is 100 seconds)
        _httpClient.Timeout = TimeSpan.FromSeconds(8);
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/sparql-results+json");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Referer, "https://movieinfo.dev/");
        // Wikimedia asks API clients to identify themselves: https://foundation.wikimedia.org/wiki/Policy:Wikimedia_Foundation_User-Agent_Policy
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "movie-info/1.0 (https://movieinfo.dev/)");
    }

    // The aspect ratios (Wikidata property P2061) of the movie or TV series with the given IMDB ID (property P345).
    // No rows when Wikidata has no such item, or the item has no aspect ratio; null when the lookup failed.
    public async Task<WikidataSparqlResponseDataModel?> GetAspectRatios(string imdbId)
    {
        // The ID goes into the query as a string literal, so only ever let a well-formed one in
        if (!ImdbTitleIdRegex().IsMatch(imdbId))
        {
            return null;
        }

        string query = $"SELECT DISTINCT ?aspectRatio ?aspectRatioLabel WHERE {{ ?item wdt:P345 \"{imdbId}\" ; wdt:P2061 ?aspectRatio . SERVICE wikibase:label {{ bd:serviceParam wikibase:language \"en\" . }} }}";

        using HttpResponseMessage response = await _httpClient.GetAsync($"sparql?format=json&query={Uri.EscapeDataString(query)}");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }

        string responseJsonString = await response.Content.ReadAsStringAsync();

        return GetModelFromResponse(responseJsonString);
    }

    public static WikidataSparqlResponseDataModel? GetModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<WikidataSparqlResponseDataModel>(responseJsonString);
    }

    [GeneratedRegex(@"^tt\d+\z")]
    private static partial Regex ImdbTitleIdRegex();
}
