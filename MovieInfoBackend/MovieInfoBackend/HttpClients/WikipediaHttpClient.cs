using System.Text.Json;
using Microsoft.Net.Http.Headers;
using MovieInfoBackend.DataModels;

public class WikipediaHttpClient
{
    private readonly HttpClient _httpClient;

    public WikipediaHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _httpClient.BaseAddress = new Uri("https://en.wikipedia.org/w/");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Referer, "https://movieinfo.dev/");
        // Wikimedia asks API clients to identify themselves: https://foundation.wikimedia.org/wiki/Policy:Wikimedia_Foundation_User-Agent_Policy
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "movie-info/1.0 (https://movieinfo.dev/)");
    }

    public async Task<string?> GetMovieLink(string title, int? year)
    {
        if (year != null)
        {
            string? link = await GetLink($"{title} ({year} film)");
            if (link != "")  // Found, or the call failed (null)
            {
                return link;
            }
        }
        return await GetLink(title);
    }

    public async Task<string?> GetPersonLink(string name)
    {
        return await GetLink(name);
    }

    public async Task<string?> GetTvSeriesLink(string name)
    {
        return await GetLink($"{name} (TV series)");
    }

    private async Task<string?> GetLink(string searchQuery)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"api.php?action=opensearch&search={Uri.EscapeDataString(searchQuery)}&limit=1&namespace=0&format=json");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }

        string responseJsonString = await response.Content.ReadAsStringAsync();

        WikipediaOpenSearchResponseDataModel? openSearchResponseDataModel = GetModelFromResponse(responseJsonString);
        if (openSearchResponseDataModel == null)
        {
            return null;
        }

        return openSearchResponseDataModel.Links.Length > 0 ? openSearchResponseDataModel.Links[0] : "";
    }

    // The whole text of the English Wikipedia page with the given title, as plain text: no markup, references or tables,
    // and its section headings written as in wikitext ("== Plot =="). Redirects are followed.
    // "" when there is no such page, null when the call failed.
    public async Task<string?> GetPageText(string title)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"api.php?action=query&prop=extracts&explaintext=1&exsectionformat=wiki&redirects=1&titles={Uri.EscapeDataString(title)}&format=json&formatversion=2");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }

        string responseJsonString = await response.Content.ReadAsStringAsync();

        WikipediaExtractResponseDataModel? extractResponseDataModel = GetExtractModelFromResponse(responseJsonString);
        WikipediaExtractPageDataModel[]? pages = extractResponseDataModel?.Query?.Pages;
        if (pages == null || pages.Length == 0)
        {
            return null;
        }

        return pages[0].Missing ? "" : pages[0].Extract;
    }

    public static WikipediaExtractResponseDataModel? GetExtractModelFromResponse(string responseJsonString)
    {
        return JsonSerializer.Deserialize<WikipediaExtractResponseDataModel>(responseJsonString);
    }

    public static WikipediaOpenSearchResponseDataModel? GetModelFromResponse(string responseJsonString)
    {
        // See WikipediaOpenSearchResponseDataModel for the shape of the response
        JsonElement[]? responseElements = JsonSerializer.Deserialize<JsonElement[]>(responseJsonString);
        if (responseElements == null || responseElements.Length < 4)
        {
            return null;
        }

        return new WikipediaOpenSearchResponseDataModel
        {
            SearchQuery = responseElements[0].GetString() ?? "",
            Titles = responseElements[1].Deserialize<string[]>() ?? [],
            Descriptions = responseElements[2].Deserialize<string[]>() ?? [],
            Links = responseElements[3].Deserialize<string[]>() ?? [],
        };
    }
}
