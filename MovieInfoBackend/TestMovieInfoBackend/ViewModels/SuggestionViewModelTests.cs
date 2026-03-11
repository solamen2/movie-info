using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.DataModels;

public class SuggestionViewModelTests
{
    private string movieHttpClientResponse1;
    private string movieHttpClientResponse2;
    private readonly ITestOutputHelper output;

    public SuggestionViewModelTests(ITestOutputHelper output)
    {
        this.output = output;

        string testDataFilename1 = "MovieHttpClientResponse1.json";
        string testDataFilename2 = "MovieHttpClientResponse2.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testDataFilename1}"))
        {
            movieHttpClientResponse1 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(movieHttpClientResponse1))
        {
            throw new ArgumentException($"{testDataFilename1} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testDataFilename2}"))
        {
            movieHttpClientResponse2 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(movieHttpClientResponse2))
        {
            throw new ArgumentException($"{testDataFilename2} is not valid test data.");
        }
    }

    [Fact]
    public void GetViewModelFromDataModel_ValidDataModel_ReturnsValidViewModel()
    {
        // Arrange

        // Mocked HttpClient is required to make MovieHttpClient, so mock it here
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage());
        var httpClient = new HttpClient(httpMessageHandlerMock.Object);

        MovieHttpClient movieHttpClient = new MovieHttpClient(httpClient);

        // Act

        MovieSuggestionsResponseDataModel? actual1 = movieHttpClient.GetModelFromResponse(movieHttpClientResponse1);
        MovieSuggestionsResponseDataModel? actual2 = movieHttpClient.GetModelFromResponse(movieHttpClientResponse2);

        // Assert
        
        // TODO: Test some SuggestionViewModel / SuggestionImageViewModel stuff here
    }

    // TODO: Do some error tests
}
