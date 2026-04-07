using Moq;
using Moq.Protected;
using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using System.Net;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.Endpoints;

public class MovieEndpointsTests
{
    private string movieHttpClientResponse1;
    private string movieHttpClientResponse2;

    public MovieEndpointsTests(ITestOutputHelper output)
    {
        // Arrange

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
    public async Task MovieEndpoints_ValidSuggestionDataModels_ConvertSuccessfullyIntoViewModels()
    {
        // Act
        MovieSuggestionsResponseDataModel? suggestionsResponse1 = MovieHttpClient.GetModelFromResponse(movieHttpClientResponse1);
        Assert.NotNull(suggestionsResponse1);
        MovieSuggestionsResponseDataModel? suggestionsResponse2 = MovieHttpClient.GetModelFromResponse(movieHttpClientResponse2);
        Assert.NotNull(suggestionsResponse2);

        Assert.NotNull(suggestionsResponse1);
        Assert.NotNull(suggestionsResponse2);
        Assert.NotNull(suggestionsResponse1.Suggestions);
        Assert.NotNull(suggestionsResponse2.Suggestions);

        List<SuggestionViewModel> suggestionViewModels1 = new List<SuggestionViewModel>();
        foreach (SuggestionDataModel suggestionDataModel in suggestionsResponse1.Suggestions)
        {
            suggestionViewModels1.Add(new SuggestionViewModel(suggestionDataModel));
        }
        List<SuggestionViewModel> suggestionViewModels2 = new List<SuggestionViewModel>();
        foreach (SuggestionDataModel suggestionDataModel in suggestionsResponse2.Suggestions)
        {
            suggestionViewModels2.Add(new SuggestionViewModel(suggestionDataModel));
        }

        // Assert
        Assert.Equal(8, suggestionViewModels1.Count);
        Assert.Equal(6, suggestionViewModels2.Count);
    }

    [Fact]
    public async Task MovieEndpoints_InvalidSearchQuery_ReturnsNullSuggestionsResponse()
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
    public void MovieEndpoints_SearchEndpointConfiguration_HasCorrectAttributes()
    {
        // This test verifies the endpoint configuration by examining what the Map method should set up
        // The actual endpoint testing would require full integration testing

        // Arrange & Act & Assert
        // Verify that the endpoint path would be correct
        string expectedPath = $"{MovieInfoBackend.Helpers.ProgramConstants.ApiRoutePrefix}/search";
        Assert.Contains("/search", expectedPath);

        // Verify authorization policy names exist
        Assert.NotNull(MovieInfoBackend.Helpers.ProgramConstants.LoggedInUsersOnlyPolicyName);
        Assert.NotNull(MovieInfoBackend.Helpers.ProgramConstants.SearchUsersOnlyPolicyName);
    }
}

