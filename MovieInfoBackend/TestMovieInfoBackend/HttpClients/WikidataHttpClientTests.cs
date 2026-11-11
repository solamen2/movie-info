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

    public WikidataHttpClientTests(ITestOutputHelper output)
    {
        // Arrange

        movieResponse = ReadTestData("WikidataHttpClientMovieResponse.json");
        tvSeriesResponse = ReadTestData("WikidataHttpClientTvSeriesResponse.json");
        emptyResponse = ReadTestData("WikidataHttpClientEmptyResponse.json");
        malformedResponse = ReadTestData("WikidataHttpClientMalformedResponse.json");
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
}
