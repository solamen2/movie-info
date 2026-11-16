using Moq;
using Moq.Protected;
using MovieInfoBackend.DataModels;
using System.Net;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.DataModels;

public class WikipediaHttpClientTests
{
    private string movieResponse;
    private string movieBareTitleResponse;
    private string personResponse;
    private string tvSeriesResponse;
    private string emptyResponse;
    private string malformedResponse;
    private string movieExtractResponse;
    private string missingPageExtractResponse;

    public WikipediaHttpClientTests(ITestOutputHelper output)
    {
        // Arrange

        movieResponse = ReadTestData("WikipediaHttpClientMovieResponse.json");
        movieBareTitleResponse = ReadTestData("WikipediaHttpClientMovieBareTitleResponse.json");
        personResponse = ReadTestData("WikipediaHttpClientPersonResponse.json");
        tvSeriesResponse = ReadTestData("WikipediaHttpClientTvSeriesResponse.json");
        emptyResponse = ReadTestData("WikipediaHttpClientEmptyResponse.json");
        malformedResponse = ReadTestData("WikipediaHttpClientMalformedResponse.json");
        movieExtractResponse = ReadTestData("WikipediaHttpClientMovieExtractResponse.json");
        missingPageExtractResponse = ReadTestData("WikipediaHttpClientMissingPageExtractResponse.json");
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

    private static WikipediaHttpClient GetWikipediaHttpClient(HttpStatusCode statusCode, string content, Action<Uri?>? recordRequestUri = null)
    {
        Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => recordRequestUri?.Invoke(request.RequestUri))
            .ReturnsAsync(() => new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });

        HttpClient httpClient = new HttpClient(httpMessageHandlerMock.Object);
        return new WikipediaHttpClient(httpClient);
    }

    // A client whose requests get the given responses in turn (status code and content), which also records the URIs of all requests made
    private static WikipediaHttpClient GetWikipediaHttpClient(IEnumerable<(HttpStatusCode statusCode, string content)> responses, List<Uri?> requestUris)
    {
        Queue<(HttpStatusCode statusCode, string content)> responseQueue = new Queue<(HttpStatusCode, string)>(responses);
        Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => requestUris.Add(request.RequestUri))
            .ReturnsAsync(() =>
            {
                (HttpStatusCode statusCode, string content) = responseQueue.Dequeue();
                return new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content)
                };
            });

        HttpClient httpClient = new HttpClient(httpMessageHandlerMock.Object);
        return new WikipediaHttpClient(httpClient);
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
        Assert.Throws<System.Text.Json.JsonException>(() => WikipediaHttpClient.GetModelFromResponse(invalidJson));
    }

    [Fact]
    public void GetModelFromResponse_MalformedResponse_ThrowsJsonException()
    {
        // Act & Assert (an object where the OpenSearch array is expected)
        Assert.Throws<System.Text.Json.JsonException>(() => WikipediaHttpClient.GetModelFromResponse(malformedResponse));
    }

    [Theory]
    [InlineData("null")]
    [InlineData("[]")]
    [InlineData("[\"only the query\"]")]
    [InlineData("[\"query\", [\"title\"], [\"\"]]")]
    public void GetModelFromResponse_TooFewArrayElements_ReturnsNull(string shortJson)
    {
        // Act
        WikipediaOpenSearchResponseDataModel? model = WikipediaHttpClient.GetModelFromResponse(shortJson);

        // Assert
        Assert.Null(model);
    }

    [Fact]
    public async Task GetMovieLink_MovieFound_ReturnsLink()
    {
        // Arrange
        Uri? requestUri = null;
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient(HttpStatusCode.OK, movieResponse, uri => requestUri = uri);

        // Act
        string? link = await wikipediaHttpClient.GetMovieLink("The Shawshank Redemption", 1994);

        // Assert
        Assert.Equal("https://en.wikipedia.org/wiki/The_Shawshank_Redemption_(1994_film)", link);
        Assert.NotNull(requestUri);
        Assert.Equal("https://en.wikipedia.org/w/api.php?action=opensearch&search=The%20Shawshank%20Redemption%20%281994%20film%29&limit=1&namespace=0&format=json",
                     requestUri.AbsoluteUri);
    }

    [Fact]
    public async Task GetMovieLink_MovieFound_DoesNotSearchAgain()
    {
        // Arrange
        List<Uri?> requestUris = new List<Uri?>();
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient([(HttpStatusCode.OK, movieResponse)], requestUris);

        // Act
        string? link = await wikipediaHttpClient.GetMovieLink("The Shawshank Redemption", 1994);

        // Assert
        Assert.Equal("https://en.wikipedia.org/wiki/The_Shawshank_Redemption_(1994_film)", link);
        Assert.Single(requestUris);
    }

    [Fact]
    public async Task GetMovieLink_NoYear_SearchesByBareTitleOnly()
    {
        // Arrange
        List<Uri?> requestUris = new List<Uri?>();
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient([(HttpStatusCode.OK, movieBareTitleResponse)], requestUris);

        // Act
        string? link = await wikipediaHttpClient.GetMovieLink("Cruel Intentions", null);

        // Assert
        Assert.Equal("https://en.wikipedia.org/wiki/Cruel_Intentions", link);
        Assert.Single(requestUris);
        Assert.Equal("https://en.wikipedia.org/w/api.php?action=opensearch&search=Cruel%20Intentions&limit=1&namespace=0&format=json",
                     requestUris[0]?.AbsoluteUri);
    }

    [Fact]
    public async Task GetMovieLink_NotFoundWithYear_FallsBackToBareTitle()
    {
        // Arrange
        List<Uri?> requestUris = new List<Uri?>();
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient([(HttpStatusCode.OK, emptyResponse), (HttpStatusCode.OK, movieBareTitleResponse)], requestUris);

        // Act
        string? link = await wikipediaHttpClient.GetMovieLink("Cruel Intentions", 1999);

        // Assert
        Assert.Equal("https://en.wikipedia.org/wiki/Cruel_Intentions", link);
        Assert.Equal(2, requestUris.Count);
        Assert.Equal("https://en.wikipedia.org/w/api.php?action=opensearch&search=Cruel%20Intentions%20%281999%20film%29&limit=1&namespace=0&format=json",
                     requestUris[0]?.AbsoluteUri);
        Assert.Equal("https://en.wikipedia.org/w/api.php?action=opensearch&search=Cruel%20Intentions&limit=1&namespace=0&format=json",
                     requestUris[1]?.AbsoluteUri);
    }

    [Fact]
    public async Task GetMovieLink_NotFoundWithYearOrBareTitle_ReturnsEmptyString()
    {
        // Arrange
        List<Uri?> requestUris = new List<Uri?>();
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient([(HttpStatusCode.OK, emptyResponse), (HttpStatusCode.OK, emptyResponse)], requestUris);

        // Act
        string? link = await wikipediaHttpClient.GetMovieLink("Cruel Intentions", 1999);

        // Assert
        Assert.Equal("", link);
        Assert.Equal(2, requestUris.Count);
    }

    [Fact]
    public async Task GetMovieLink_FallbackSearchFails_ReturnsNull()
    {
        // Arrange
        List<Uri?> requestUris = new List<Uri?>();
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient([(HttpStatusCode.OK, emptyResponse), (HttpStatusCode.InternalServerError, "Internal Server Error")], requestUris);

        // Act
        string? link = await wikipediaHttpClient.GetMovieLink("Cruel Intentions", 1999);

        // Assert
        Assert.Null(link);
        Assert.Equal(2, requestUris.Count);
    }

    [Fact]
    public async Task GetMovieLink_FirstSearchFails_ReturnsNullWithoutFallingBack()
    {
        // Arrange
        List<Uri?> requestUris = new List<Uri?>();
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient([(HttpStatusCode.InternalServerError, "Internal Server Error")], requestUris);

        // Act
        string? link = await wikipediaHttpClient.GetMovieLink("Cruel Intentions", 1999);

        // Assert
        Assert.Null(link);
        Assert.Single(requestUris);
    }

    [Fact]
    public async Task GetPersonLink_PersonFound_ReturnsLink()
    {
        // Arrange
        Uri? requestUri = null;
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient(HttpStatusCode.OK, personResponse, uri => requestUri = uri);

        // Act
        string? link = await wikipediaHttpClient.GetPersonLink("Katharine Hepburn");

        // Assert
        Assert.Equal("https://en.wikipedia.org/wiki/Katharine_Hepburn", link);
        Assert.NotNull(requestUri);
        Assert.Equal("https://en.wikipedia.org/w/api.php?action=opensearch&search=Katharine%20Hepburn&limit=1&namespace=0&format=json",
                     requestUri.AbsoluteUri);
    }

    [Fact]
    public async Task GetTvSeriesLink_TvSeriesFound_ReturnsLink()
    {
        // Arrange
        Uri? requestUri = null;
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient(HttpStatusCode.OK, tvSeriesResponse, uri => requestUri = uri);

        // Act
        string? link = await wikipediaHttpClient.GetTvSeriesLink("Buffy the Vampire Slayer");

        // Assert
        Assert.Equal("https://en.wikipedia.org/wiki/Buffy_the_Vampire_Slayer_(TV_series)", link);
        Assert.NotNull(requestUri);
        Assert.Equal("https://en.wikipedia.org/w/api.php?action=opensearch&search=Buffy%20the%20Vampire%20Slayer%20%28TV%20series%29&limit=1&namespace=0&format=json",
                     requestUri.AbsoluteUri);
    }

    [Fact]
    public async Task GetLinks_NothingFound_ReturnEmptyStrings()
    {
        // Arrange
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient(HttpStatusCode.OK, emptyResponse);

        // Act
        string? movieLink = await wikipediaHttpClient.GetMovieLink("Cruel Intentions", 1999);
        string? personLink = await wikipediaHttpClient.GetPersonLink("Nobody Anybody Knows");
        string? tvSeriesLink = await wikipediaHttpClient.GetTvSeriesLink("No Such Show");

        // Assert
        Assert.Equal("", movieLink);
        Assert.Equal("", personLink);
        Assert.Equal("", tvSeriesLink);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, "Invalid response")]
    [InlineData(HttpStatusCode.InternalServerError, "Internal Server Error")]
    [InlineData(HttpStatusCode.TooManyRequests, "Rate limited")]
    public async Task GetLinks_HttpClientErrors_ReturnNull(HttpStatusCode statusCode, string content)
    {
        // Arrange
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient(statusCode, content);

        // Act
        string? movieLink = await wikipediaHttpClient.GetMovieLink("The Shawshank Redemption", 1994);
        string? personLink = await wikipediaHttpClient.GetPersonLink("Katharine Hepburn");
        string? tvSeriesLink = await wikipediaHttpClient.GetTvSeriesLink("Buffy the Vampire Slayer");

        // Assert
        Assert.Null(movieLink);
        Assert.Null(personLink);
        Assert.Null(tvSeriesLink);
    }

    [Fact]
    public async Task GetLinks_TooFewArrayElements_ReturnNull()
    {
        // Arrange
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient(HttpStatusCode.OK, "[]");

        // Act
        string? movieLink = await wikipediaHttpClient.GetMovieLink("The Shawshank Redemption", 1994);
        string? personLink = await wikipediaHttpClient.GetPersonLink("Katharine Hepburn");
        string? tvSeriesLink = await wikipediaHttpClient.GetTvSeriesLink("Buffy the Vampire Slayer");

        // Assert
        Assert.Null(movieLink);
        Assert.Null(personLink);
        Assert.Null(tvSeriesLink);
    }

    [Fact]
    public async Task GetLinks_MalformedResponse_ThrowsJsonException()
    {
        // Arrange
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient(HttpStatusCode.OK, malformedResponse);

        // Act & Assert
        await Assert.ThrowsAsync<System.Text.Json.JsonException>(() => wikipediaHttpClient.GetMovieLink("The Shawshank Redemption", 1994));
        await Assert.ThrowsAsync<System.Text.Json.JsonException>(() => wikipediaHttpClient.GetPersonLink("Katharine Hepburn"));
        await Assert.ThrowsAsync<System.Text.Json.JsonException>(() => wikipediaHttpClient.GetTvSeriesLink("Buffy the Vampire Slayer"));
    }

    [Fact]
    public async Task GetLinks_HttpClientTimeout_ThrowsException()
    {
        // Arrange
        Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("Request timed out"));

        HttpClient httpClient = new HttpClient(httpMessageHandlerMock.Object);
        WikipediaHttpClient wikipediaHttpClient = new WikipediaHttpClient(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => wikipediaHttpClient.GetMovieLink("The Shawshank Redemption", 1994));
        await Assert.ThrowsAsync<TaskCanceledException>(() => wikipediaHttpClient.GetPersonLink("Katharine Hepburn"));
        await Assert.ThrowsAsync<TaskCanceledException>(() => wikipediaHttpClient.GetTvSeriesLink("Buffy the Vampire Slayer"));
    }

    [Theory]
    [InlineData("Schitt's Creek", "Schitt%27s%20Creek%20%28TV%20series%29")]
    [InlineData("Amélie", "Am%C3%A9lie%20%28TV%20series%29")]
    [InlineData("What/If?", "What%2FIf%3F%20%28TV%20series%29")]
    [InlineData("50% off & more #1", "50%25%20off%20%26%20more%20%231%20%28TV%20series%29")]
    [InlineData("\"quoted\" <tag>", "%22quoted%22%20%3Ctag%3E%20%28TV%20series%29")]
    public async Task GetTvSeriesLink_SpecialCharactersInName_PercentEncodesTheSearchQuery(string name, string expectedEncodedSearch)
    {
        // Arrange
        Uri? requestUri = null;
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient(HttpStatusCode.OK, emptyResponse, uri => requestUri = uri);

        // Act
        string? link = await wikipediaHttpClient.GetTvSeriesLink(name);

        // Assert
        Assert.Equal("", link);
        Assert.NotNull(requestUri);
        Assert.Equal($"https://en.wikipedia.org/w/api.php?action=opensearch&search={expectedEncodedSearch}&limit=1&namespace=0&format=json",
                     requestUri.AbsoluteUri);
    }

    [Fact]
    public async Task GetPageText_PageFound_ReturnsItsPlainText()
    {
        // Arrange
        Uri? requestUri = null;
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient(HttpStatusCode.OK, movieExtractResponse, uri => requestUri = uri);

        // Act
        string? pageText = await wikipediaHttpClient.GetPageText("Heat (1995 film)");

        // Assert
        Assert.NotNull(pageText);
        Assert.StartsWith("Heat is a 1995 American epic crime drama film", pageText);
        Assert.Contains("\n== Plot ==\nNeil McCauley, a Los Angeles professional thief,", pageText);
        Assert.Contains("\n== Reception ==\nOn Rotten Tomatoes, Heat holds an approval rating of", pageText);
        Assert.NotNull(requestUri);
        Assert.Equal("https://en.wikipedia.org/w/api.php?action=query&prop=extracts&explaintext=1&exsectionformat=wiki&redirects=1&titles=Heat%20%281995%20film%29&format=json&formatversion=2",
                     requestUri.AbsoluteUri);
    }

    [Fact]
    public async Task GetPageText_NoSuchPage_ReturnsEmptyString()
    {
        // Arrange
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient(HttpStatusCode.OK, missingPageExtractResponse);

        // Act
        string? pageText = await wikipediaHttpClient.GetPageText("No Such Movie Page Xyzzy (1995 film)");

        // Assert
        Assert.Equal("", pageText);
    }

    [Theory]
    [InlineData("What/If? & more #1", "What%2FIf%3F%20%26%20more%20%231")]
    [InlineData("Amélie", "Am%C3%A9lie")]
    public async Task GetPageText_SpecialCharactersInTitle_PercentEncodesTheTitle(string title, string expectedEncodedTitle)
    {
        // Arrange
        Uri? requestUri = null;
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient(HttpStatusCode.OK, missingPageExtractResponse, uri => requestUri = uri);

        // Act
        await wikipediaHttpClient.GetPageText(title);

        // Assert
        Assert.NotNull(requestUri);
        Assert.Contains($"&titles={expectedEncodedTitle}&", requestUri.AbsoluteUri);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, "Invalid response")]
    [InlineData(HttpStatusCode.InternalServerError, "Internal Server Error")]
    [InlineData(HttpStatusCode.TooManyRequests, "Rate limited")]
    public async Task GetPageText_HttpClientErrors_ReturnNull(HttpStatusCode statusCode, string content)
    {
        // Arrange
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient(statusCode, content);

        // Act
        string? pageText = await wikipediaHttpClient.GetPageText("Heat (1995 film)");

        // Assert
        Assert.Null(pageText);
    }

    [Theory]
    [InlineData("null")]
    [InlineData("{}")]
    [InlineData("{\"batchcomplete\":true}")]
    [InlineData("{\"query\":{}}")]
    [InlineData("{\"query\":{\"pages\":[]}}")]
    // An invalid title is neither a page with text nor a missing page
    [InlineData("{\"query\":{\"pages\":[{\"title\":\"[\",\"invalidreason\":\"The requested page title contains invalid characters.\",\"invalid\":true}]}}")]
    public async Task GetPageText_ResponseWithoutAPage_ReturnsNull(string responseWithoutAPage)
    {
        // Arrange
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient(HttpStatusCode.OK, responseWithoutAPage);

        // Act
        string? pageText = await wikipediaHttpClient.GetPageText("Heat (1995 film)");

        // Assert
        Assert.Null(pageText);
    }

    [Fact]
    public async Task GetPageText_UnexpectedJsonObject_ReturnsNull()
    {
        // Arrange (an object, but not the query API's)
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient(HttpStatusCode.OK, malformedResponse);

        // Act
        string? pageText = await wikipediaHttpClient.GetPageText("Heat (1995 film)");

        // Assert
        Assert.Null(pageText);
    }

    [Theory]
    [InlineData("invalid json")]
    [InlineData("[\"an array\"]")]
    [InlineData("")]
    public async Task GetPageText_InvalidJson_ThrowsJsonException(string invalidJson)
    {
        // Arrange
        WikipediaHttpClient wikipediaHttpClient = GetWikipediaHttpClient(HttpStatusCode.OK, invalidJson);

        // Act & Assert
        await Assert.ThrowsAsync<System.Text.Json.JsonException>(() => wikipediaHttpClient.GetPageText("Heat (1995 film)"));
    }

    [Fact]
    public async Task GetPageText_HttpClientTimeout_ThrowsException()
    {
        // Arrange
        Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("Request timed out"));

        HttpClient httpClient = new HttpClient(httpMessageHandlerMock.Object);
        WikipediaHttpClient wikipediaHttpClient = new WikipediaHttpClient(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => wikipediaHttpClient.GetPageText("Heat (1995 film)"));
    }

    [Fact]
    public void ExtractDataModels_ToString_SummarizeTheExtract()
    {
        // Act
        WikipediaExtractResponseDataModel? model = WikipediaHttpClient.GetExtractModelFromResponse(missingPageExtractResponse);
        WikipediaExtractResponseDataModel? movieModel = WikipediaHttpClient.GetExtractModelFromResponse("{\"query\":{\"pages\":[{\"title\":\"Example Movie\",\"extract\":\"Twelve chars\"}]}}");

        // Assert
        Assert.NotNull(model);
        Assert.Equal("Query:\n*****\nPages:\n*****\nTitle: No Such Movie Page Xyzzy (1995 film)\nMissing: True\nExtract: \n*****\n*****", model.ToString());
        Assert.NotNull(movieModel);
        Assert.Equal("Query:\n*****\nPages:\n*****\nTitle: Example Movie\nMissing: False\nExtract: 12 characters\n*****\n*****", movieModel.ToString());
    }
}
