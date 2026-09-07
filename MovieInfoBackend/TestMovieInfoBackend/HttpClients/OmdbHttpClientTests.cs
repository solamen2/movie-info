using Moq;
using Moq.Protected;
using MovieInfoBackend.DataModels;
using System.Net;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.DataModels;

public class OmdbHttpClientTests
{
    private string malformedResponse;
    private string errorResponse1;

    public OmdbHttpClientTests(ITestOutputHelper output)
    {
        // Arrange

        string malformedDataFilename = "OmdbHttpClientMalformedResponse.json";
        string errorFilename1 = "OmdbHttpClientErrorResponse1.json";

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
        Assert.Throws<System.Text.Json.JsonException>(() => OmdbHttpClient.GetModelFromResponse(invalidJson));
    }

    [Fact]
    public async Task GetMedia_InvalidImdbId_ReturnsNullMediaResponse()
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
        OmdbHttpClient omdbHttpClient = new OmdbHttpClient(httpClient);

        // Act
        OmdbResponseDataModel? mediaResponse = await omdbHttpClient.GetMedia("");

        // Assert
        Assert.Null(mediaResponse);
    }

    [Fact]
    public async Task GetMedia_HttpClientInternalServerError_ReturnsNull()
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
        OmdbHttpClient omdbHttpClient = new OmdbHttpClient(httpClient);

        // Act
        OmdbResponseDataModel? result = await omdbHttpClient.GetMedia("test query");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetMedia_HttpClientTimeout_ThrowsException()
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
        OmdbHttpClient omdbHttpClient = new OmdbHttpClient(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(
            () => omdbHttpClient.GetMedia("test query"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\n\t")]
    [InlineData("very long query that might cause issues with the API endpoint and should still be handled properly")]
    public async Task GetMedia_EdgeCaseImdbIds_ReturnEmptyResults(string query)
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
                Content = new StringContent(errorResponse1) // Error response (no usable fields)
            });

        HttpClient httpClient = new HttpClient(httpMessageHandlerMock.Object);
        OmdbHttpClient omdbHttpClient = new OmdbHttpClient(httpClient);

        // Act
        OmdbResponseDataModel? result = await omdbHttpClient.GetMedia(query);

        // Assert
        Assert.Null(result);
    }
}
