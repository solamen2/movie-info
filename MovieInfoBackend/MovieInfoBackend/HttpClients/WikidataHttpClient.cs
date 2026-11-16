using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Net.Http.Headers;
using MovieInfoBackend.DataModels;

public partial class WikidataHttpClient
{
    private const string EnglishWikipediaUrl = "https://en.wikipedia.org/";

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

    // The link to the English Wikipedia article about the movie with the given IMDB ID (property P345), e.g.
    // "https://en.wikipedia.org/wiki/Heat_(1995_film)". Unlike a search by title, this cannot land on another film of a
    // similar name, or on the book a film is based on.
    // "" when Wikidata has no such item, or the item has no English Wikipedia article; null when the lookup failed.
    public async Task<string?> GetEnglishWikipediaLink(string imdbId)
    {
        // The ID goes into the query as a string literal, so only ever let a well-formed one in
        if (!ImdbTitleIdRegex().IsMatch(imdbId))
        {
            return null;
        }

        string query = $"SELECT ?article WHERE {{ ?item wdt:P345 \"{imdbId}\" . ?article schema:about ?item ; schema:isPartOf <{EnglishWikipediaUrl}> . }}";

        using HttpResponseMessage response = await _httpClient.GetAsync($"sparql?format=json&query={Uri.EscapeDataString(query)}");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }

        string responseJsonString = await response.Content.ReadAsStringAsync();

        return GetEnglishWikipediaLinkFromResponse(responseJsonString);
    }

    // Reads the article link out of the answer to GetEnglishWikipediaLink's query:
    // {"results": {"bindings": [{"article": {"type": "uri", "value": "https://en.wikipedia.org/wiki/Heat_(1995_film)"}}]}}
    // "" when there are no rows; null when the answer is not of that shape.
    public static string? GetEnglishWikipediaLinkFromResponse(string responseJsonString)
    {
        using JsonDocument response = JsonDocument.Parse(responseJsonString);

        if (response.RootElement.ValueKind != JsonValueKind.Object
            || !response.RootElement.TryGetProperty("results", out JsonElement results)
            || results.ValueKind != JsonValueKind.Object
            || !results.TryGetProperty("bindings", out JsonElement bindings)
            || bindings.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        foreach (JsonElement binding in bindings.EnumerateArray())
        {
            if (binding.ValueKind == JsonValueKind.Object
                && binding.TryGetProperty("article", out JsonElement article)
                && article.ValueKind == JsonValueKind.Object
                && article.TryGetProperty("value", out JsonElement value)
                && value.ValueKind == JsonValueKind.String)
            {
                string link = value.GetString() ?? "";
                // Only ever an English Wikipedia article, whatever the answer holds
                if (link.StartsWith($"{EnglishWikipediaUrl}wiki/", StringComparison.Ordinal))
                {
                    return link;
                }
            }
        }

        return "";
    }

    public static WikidataSparqlResponseDataModel? GetModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<WikidataSparqlResponseDataModel>(responseJsonString);
    }

    [GeneratedRegex(@"^tt\d+\z")]
    private static partial Regex ImdbTitleIdRegex();
}
