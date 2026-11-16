using Moq;
using Moq.Protected;
using MovieInfoBackend.DataModels;
using System.Net;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.DataModels;

public class WikidataHttpClientTests
{
    private string movieResponse;
    private string tvSeriesResponse;
    private string emptyResponse;
    private string malformedResponse;
    private string wikipediaLinkResponse;

    public WikidataHttpClientTests(ITestOutputHelper output)
    {
        // Arrange

        movieResponse = ReadTestData("WikidataHttpClientMovieResponse.json");
        tvSeriesResponse = ReadTestData("WikidataHttpClientTvSeriesResponse.json");
        emptyResponse = ReadTestData("WikidataHttpClientEmptyResponse.json");
        malformedResponse = ReadTestData("WikidataHttpClientMalformedResponse.json");
        wikipediaLinkResponse = ReadTestData("WikidataHttpClientWikipediaLinkResponse.json");
    }

    private static string ReadTestData(string filename)
    {
        string testData;
        using (StreamReader sr = File.OpenText($"../../../TestData/{filename}"))
        {
            testData = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(testData))
        {
            throw new ArgumentException($"{filename} is not valid test data.");
        }
        return testData;
    }

    // A client whose requests all get the given response, which also records the URIs of all requests made
    private static WikidataHttpClient GetWikidataHttpClient(HttpStatusCode statusCode, string content, List<Uri?>? requestUris = null)
    {
        Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => requestUris?.Add(request.RequestUri))
            .ReturnsAsync(() => new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });

        HttpClient httpClient = new HttpClient(httpMessageHandlerMock.Object);
        return new WikidataHttpClient(httpClient);
    }

    [Theory]
    [InlineData("invalid json")]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData("")]
    [InlineData("   ")]
    public void GetModelFromResponse_InvalidJsonStrings_ThrowsJsonException(string invalidJson)
    {
        // Act & Assert
        Assert.Throws<System.Text.Json.JsonException>(() => WikidataHttpClient.GetModelFromResponse(invalidJson));
    }

    [Fact]
    public void GetModelFromResponse_MalformedResponse_ThrowsJsonException()
    {
        // Act & Assert (an object without the SPARQL "results")
        Assert.Throws<System.Text.Json.JsonException>(() => WikidataHttpClient.GetModelFromResponse(malformedResponse));
    }

    [Fact]
    public void GetModelFromResponse_ValidResponses_ReturnsValidModels()
    {
        // Act
        WikidataSparqlResponseDataModel? movieModel = WikidataHttpClient.GetModelFromResponse(movieResponse);
        WikidataSparqlResponseDataModel? tvSeriesModel = WikidataHttpClient.GetModelFromResponse(tvSeriesResponse);
        WikidataSparqlResponseDataModel? emptyModel = WikidataHttpClient.GetModelFromResponse(emptyResponse);

        // Assert
        Assert.NotNull(movieModel);
        Assert.Equal(2, movieModel.Results.Bindings.Length);
        Assert.Equal("uri", movieModel.Results.Bindings[0].AspectRatio.Type);
        Assert.Equal("http://www.wikidata.org/entity/Q24668302", movieModel.Results.Bindings[0].AspectRatio.Value);
        Assert.Equal("literal", movieModel.Results.Bindings[0].AspectRatioLabel?.Type);
        Assert.Equal("2.39:1", movieModel.Results.Bindings[0].AspectRatioLabel?.Value);
        Assert.Equal("http://www.wikidata.org/entity/Q28858964", movieModel.Results.Bindings[1].AspectRatio.Value);
        Assert.Equal("2.2:1", movieModel.Results.Bindings[1].AspectRatioLabel?.Value);

        Assert.NotNull(tvSeriesModel);
        Assert.Single(tvSeriesModel.Results.Bindings);
        Assert.Equal("16:9", tvSeriesModel.Results.Bindings[0].AspectRatioLabel?.Value);

        Assert.NotNull(emptyModel);
        Assert.Empty(emptyModel.Results.Bindings);
    }

    [Fact]
    public void ToString_ValidResponse_ListsEveryBinding()
    {
        // Act
        WikidataSparqlResponseDataModel? tvSeriesModel = WikidataHttpClient.GetModelFromResponse(tvSeriesResponse);

        // Assert
        Assert.NotNull(tvSeriesModel);
        Assert.Equal(
            "Results:\n*****\nBindings:\n*****\nAspectRatio:\n*****\nType: uri\nValue: http://www.wikidata.org/entity/Q1383069\n*****\nAspectRatioLabel:\n*****\nType: literal\nValue: 16:9\n*****\n*****\n*****",
            tvSeriesModel.ToString());
    }

    [Fact]
    public async Task GetAspectRatios_Found_ReturnsBindingsAndQueriesByImdbId()
    {
        // Arrange
        List<Uri?> requestUris = new List<Uri?>();
        WikidataHttpClient wikidataHttpClient = GetWikidataHttpClient(HttpStatusCode.OK, movieResponse, requestUris);

        // Act
        WikidataSparqlResponseDataModel? model = await wikidataHttpClient.GetAspectRatios("tt15398776");

        // Assert
        Assert.NotNull(model);
        Assert.Equal(2, model.Results.Bindings.Length);
        Assert.Single(requestUris);
        Assert.NotNull(requestUris[0]);
        Assert.Equal("https://query.wikidata.org/sparql", requestUris[0]!.GetLeftPart(UriPartial.Path));
        string query = Uri.UnescapeDataString(requestUris[0]!.Query);
        Assert.StartsWith("?format=json&query=SELECT DISTINCT ?aspectRatio ?aspectRatioLabel WHERE", query);
        Assert.Contains("?item wdt:P345 \"tt15398776\" ; wdt:P2061 ?aspectRatio .", query);
    }

    [Fact]
    public async Task GetAspectRatios_NothingInWikidata_ReturnsNoBindings()
    {
        // Arrange
        WikidataHttpClient wikidataHttpClient = GetWikidataHttpClient(HttpStatusCode.OK, emptyResponse);

        // Act
        WikidataSparqlResponseDataModel? model = await wikidataHttpClient.GetAspectRatios("tt0468569");

        // Assert
        Assert.NotNull(model);
        Assert.Empty(model.Results.Bindings);
    }

    [Theory]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    [InlineData(HttpStatusCode.NotFound)]
    public async Task GetAspectRatios_HttpError_ReturnsNull(HttpStatusCode statusCode)
    {
        // Arrange
        WikidataHttpClient wikidataHttpClient = GetWikidataHttpClient(statusCode, "error");

        // Act
        WikidataSparqlResponseDataModel? model = await wikidataHttpClient.GetAspectRatios("tt15398776");

        // Assert
        Assert.Null(model);
    }

    // The ID is put into the query text, so anything that isn't an IMDB title ID must never be sent
    [Theory]
    [InlineData("")]
    [InlineData("nm0000001")]
    [InlineData("tt")]
    [InlineData("tt123\" ; wdt:P31 ?x . #")]
    [InlineData("tt123 ")]
    [InlineData("tt123\n")]
    public async Task GetAspectRatios_NotAnImdbTitleId_ReturnsNullWithoutARequest(string imdbId)
    {
        // Arrange
        List<Uri?> requestUris = new List<Uri?>();
        WikidataHttpClient wikidataHttpClient = GetWikidataHttpClient(HttpStatusCode.OK, movieResponse, requestUris);

        // Act
        WikidataSparqlResponseDataModel? model = await wikidataHttpClient.GetAspectRatios(imdbId);

        // Assert
        Assert.Null(model);
        Assert.Empty(requestUris);
    }

    [Fact]
    public async Task GetAspectRatios_MalformedResponse_ThrowsJsonException()
    {
        // Arrange
        WikidataHttpClient wikidataHttpClient = GetWikidataHttpClient(HttpStatusCode.OK, malformedResponse);

        // Act & Assert
        await Assert.ThrowsAsync<System.Text.Json.JsonException>(() => wikidataHttpClient.GetAspectRatios("tt15398776"));
    }

    [Fact]
    public async Task GetEnglishWikipediaLink_Found_ReturnsTheArticleLinkAndQueriesByImdbId()
    {
        // Arrange
        List<Uri?> requestUris = new List<Uri?>();
        WikidataHttpClient wikidataHttpClient = GetWikidataHttpClient(HttpStatusCode.OK, wikipediaLinkResponse, requestUris);

        // Act
        string? link = await wikidataHttpClient.GetEnglishWikipediaLink("tt0211915");

        // Assert
        Assert.Equal("https://en.wikipedia.org/wiki/Am%C3%A9lie", link);
        Assert.Single(requestUris);
        Assert.NotNull(requestUris[0]);
        Assert.Equal("https://query.wikidata.org/sparql", requestUris[0]!.GetLeftPart(UriPartial.Path));
        string query = Uri.UnescapeDataString(requestUris[0]!.Query);
        Assert.Equal("?format=json&query=SELECT ?article WHERE { ?item wdt:P345 \"tt0211915\" . ?article schema:about ?item ; schema:isPartOf <https://en.wikipedia.org/> . }", query);
    }

    [Fact]
    public async Task GetEnglishWikipediaLink_NothingInWikidata_ReturnsEmptyString()
    {
        // Arrange (no item with that IMDB ID, or an item without an English Wikipedia article: no rows either way)
        WikidataHttpClient wikidataHttpClient = GetWikidataHttpClient(HttpStatusCode.OK, emptyResponse);

        // Act
        string? link = await wikidataHttpClient.GetEnglishWikipediaLink("tt0468569");

        // Assert
        Assert.Equal("", link);
    }

    [Theory]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    [InlineData(HttpStatusCode.NotFound)]
    public async Task GetEnglishWikipediaLink_HttpError_ReturnsNull(HttpStatusCode statusCode)
    {
        // Arrange
        WikidataHttpClient wikidataHttpClient = GetWikidataHttpClient(statusCode, "error");

        // Act
        string? link = await wikidataHttpClient.GetEnglishWikipediaLink("tt0211915");

        // Assert
        Assert.Null(link);
    }

    // The ID is put into the query text, so anything that isn't an IMDB title ID must never be sent
    [Theory]
    [InlineData("")]
    [InlineData("nm0000001")]
    [InlineData("tt")]
    [InlineData("tt123\" ; wdt:P31 ?x . #")]
    [InlineData("tt123 ")]
    [InlineData("tt123\n")]
    public async Task GetEnglishWikipediaLink_NotAnImdbTitleId_ReturnsNullWithoutARequest(string imdbId)
    {
        // Arrange
        List<Uri?> requestUris = new List<Uri?>();
        WikidataHttpClient wikidataHttpClient = GetWikidataHttpClient(HttpStatusCode.OK, wikipediaLinkResponse, requestUris);

        // Act
        string? link = await wikidataHttpClient.GetEnglishWikipediaLink(imdbId);

        // Assert
        Assert.Null(link);
        Assert.Empty(requestUris);
    }

    [Theory]
    [InlineData("invalid json")]
    [InlineData("")]
    public async Task GetEnglishWikipediaLink_InvalidJson_ThrowsJsonException(string invalidJson)
    {
        // Arrange
        WikidataHttpClient wikidataHttpClient = GetWikidataHttpClient(HttpStatusCode.OK, invalidJson);

        // Act & Assert
        await Assert.ThrowsAnyAsync<System.Text.Json.JsonException>(() => wikidataHttpClient.GetEnglishWikipediaLink("tt0211915"));
    }

    [Theory]
    [InlineData("null")]
    [InlineData("[]")]
    [InlineData("{}")]
    [InlineData("{\"results\":[]}")]
    [InlineData("{\"results\":{}}")]
    [InlineData("{\"results\":{\"bindings\":{}}}")]
    public void GetEnglishWikipediaLinkFromResponse_NotSparqlResults_ReturnsNull(string response)
    {
        Assert.Null(WikidataHttpClient.GetEnglishWikipediaLinkFromResponse(response));
    }

    [Fact]
    public void GetEnglishWikipediaLinkFromResponse_MalformedResponse_ReturnsNull()
    {
        Assert.Null(WikidataHttpClient.GetEnglishWikipediaLinkFromResponse(malformedResponse));
    }

    [Theory]
    // Rows without a usable article
    [InlineData("{\"results\":{\"bindings\":[{}]}}")]
    [InlineData("{\"results\":{\"bindings\":[\"text\"]}}")]
    [InlineData("{\"results\":{\"bindings\":[{\"article\":\"text\"}]}}")]
    [InlineData("{\"results\":{\"bindings\":[{\"article\":{\"type\":\"uri\"}}]}}")]
    [InlineData("{\"results\":{\"bindings\":[{\"article\":{\"type\":\"uri\",\"value\":7}}]}}")]
    // Only ever a page of English Wikipedia
    [InlineData("{\"results\":{\"bindings\":[{\"article\":{\"type\":\"uri\",\"value\":\"https://fr.wikipedia.org/wiki/Le_Fabuleux_Destin_d%27Am%C3%A9lie_Poulain\"}}]}}")]
    [InlineData("{\"results\":{\"bindings\":[{\"article\":{\"type\":\"uri\",\"value\":\"https://en.wikipedia.org.example.com/wiki/Am%C3%A9lie\"}}]}}")]
    [InlineData("{\"results\":{\"bindings\":[{\"article\":{\"type\":\"uri\",\"value\":\"http://en.wikipedia.org/wiki/Am%C3%A9lie\"}}]}}")]
    [InlineData("{\"results\":{\"bindings\":[{\"article\":{\"type\":\"uri\",\"value\":\"https://en.wikipedia.org/\"}}]}}")]
    public void GetEnglishWikipediaLinkFromResponse_NoEnglishWikipediaArticle_ReturnsEmptyString(string response)
    {
        Assert.Equal("", WikidataHttpClient.GetEnglishWikipediaLinkFromResponse(response));
    }

    [Fact]
    public void GetEnglishWikipediaLinkFromResponse_SeveralRows_ReturnsTheFirstEnglishWikipediaArticle()
    {
        // Arrange (as when Wikidata has two items with the same IMDB ID)
        string response = "{\"results\":{\"bindings\":[{\"article\":{\"type\":\"uri\",\"value\":\"https://fr.wikipedia.org/wiki/Heat_(film,_1995)\"}},{\"article\":{\"type\":\"uri\",\"value\":\"https://en.wikipedia.org/wiki/Heat_(1995_film)\"}},{\"article\":{\"type\":\"uri\",\"value\":\"https://en.wikipedia.org/wiki/Heat_(soundtrack)\"}}]}}";

        // Act & Assert
        Assert.Equal("https://en.wikipedia.org/wiki/Heat_(1995_film)", WikidataHttpClient.GetEnglishWikipediaLinkFromResponse(response));
    }
}
