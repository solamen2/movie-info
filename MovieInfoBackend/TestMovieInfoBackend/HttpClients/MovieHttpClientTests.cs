using Moq;
using Moq.Protected;
using MovieInfoBackend.DataModels;
using System.Net;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.DataModels;

public class MovieHttpClientTests
{
    private string malformedResponse;
    private string errorResponse1;

    public MovieHttpClientTests(ITestOutputHelper output)
    {
        // Arrange

        string malformedDataFilename = "MovieHttpClientMalformedResponse.json";
        string errorFilename1 = "MovieHttpClientErrorResponse1.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{malformedDataFilename}"))
        {
            malformedResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(malformedResponse))
        {
            throw new ArgumentException($"{malformedDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{errorFilename1}"))
        {
            errorResponse1 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(errorResponse1))
        {
            throw new ArgumentException($"{errorFilename1} is not valid test data.");
        }
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
        Assert.Throws<System.Text.Json.JsonException>(() => MovieHttpClient.GetModelFromResponse(invalidJson));
    }

    [Fact]
    public async Task GetSuggestions_InvalidSearchQuery_ReturnsNullSuggestionsResponse()
    {
        // Arrange
        Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Invalid response")
            });

        HttpClient httpClient = new HttpClient(httpMessageHandlerMock.Object);
        MovieHttpClient movieHttpClient = new MovieHttpClient(httpClient);

        // Act
        MovieSuggestionsResponseDataModel? suggestionsResponse = await movieHttpClient.GetSuggestions("");

        // Assert
        Assert.Null(suggestionsResponse);
    }

    [Fact]
    public async Task GetSuggestions_HttpClientInternalServerError_ReturnsNull()
    {
        // Arrange
        Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Internal Server Error")
            });

        HttpClient httpClient = new HttpClient(httpMessageHandlerMock.Object);
        MovieHttpClient movieHttpClient = new MovieHttpClient(httpClient);

        // Act
        MovieSuggestionsResponseDataModel? result = await movieHttpClient.GetSuggestions("test query");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetSuggestions_HttpClientTimeout_ThrowsException()
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
        MovieHttpClient movieHttpClient = new MovieHttpClient(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(
            () => movieHttpClient.GetSuggestions("test query"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\n\t")]
    [InlineData("very long query that might cause issues with the API endpoint and should still be handled properly")]
    public async Task GetSuggestions_EdgeCaseSearchQueries_ReturnEmptyResults(string query)
    {
        // Arrange
        Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(errorResponse1) // Empty response
            });

        HttpClient httpClient = new HttpClient(httpMessageHandlerMock.Object);
        MovieHttpClient movieHttpClient = new MovieHttpClient(httpClient);

        // Act
        MovieSuggestionsResponseDataModel? result = await movieHttpClient.GetSuggestions(query);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Suggestions);
        Assert.Empty(result.Suggestions);
    }

    [Fact]
    public void MovieHttpClient_ToString_HandlesNullSuggestions()
    {
        // Arrange
        var model = new MovieSuggestionsResponseDataModel { Suggestions = null };

        // Act & Assert - Should not throw an exception
        string result = model.ToString();
        Assert.NotNull(result);
    }
}
