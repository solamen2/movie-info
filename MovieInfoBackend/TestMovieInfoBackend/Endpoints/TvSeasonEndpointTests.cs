
using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.Endpoints;

// TODO: Make sure the final name of TvSeasonEndpoint is correct!

public class TvSeasonEndpointTests
{
    private string tmdbHttpClientTvSeasonResponse;
    private string tmdbHttpClientTvSeasonWatchProvidersResponse;

    public TvSeasonEndpointTests(ITestOutputHelper output)
    {   
        // Arrange

        string testTmdbTvSeasonDataFilename = "TmdbHttpClientTvSeasonResponse.json";
        string testTmdbTvSeasonWatchProvidersDataFilename = "TmdbHttpClientTvSeasonWatchProvidersResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbTvSeasonDataFilename}"))
        {
            tmdbHttpClientTvSeasonResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeasonResponse))
        {
            throw new ArgumentException($"{testTmdbTvSeasonDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbTvSeasonWatchProvidersDataFilename}"))
        {
            tmdbHttpClientTvSeasonWatchProvidersResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeasonWatchProvidersResponse))
        {
            throw new ArgumentException($"{testTmdbTvSeasonWatchProvidersDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public async Task TvSeasonEndpoint_ValidDataModels_ConvertSuccessfullyIntoViewModel()
    {
        // TODO: Finish test later

        // Act
        
        TmdbTvSeasonResponseDataModel? tmdbTvSeasonResponse = TmdbHttpClient.GetTvSeasonModelFromResponse(tmdbHttpClientTvSeasonResponse);
        Assert.NotNull(tmdbTvSeasonResponse);
        TmdbWatchProvidersResponseDataModel? tmdbTvSeasonWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientTvSeasonWatchProvidersResponse);
        Assert.NotNull(tmdbTvSeasonWatchProvidersResponse);

        /*Assert.NotNull(suggestionsResponse1);
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
        Assert.Equal(6, suggestionViewModels2.Count); */
    }

    [Fact]
    public void TvSeasonEndpoint_EndpointConfiguration_HasCorrectAttributes()
    {
        Assert.True(true);
        
        // TODO: Implement me later
        
        // This test verifies the endpoint configuration by examining what the Map method should set up
        // The actual endpoint testing would require full integration testing

        // Arrange & Act & Assert
        // Verify that the endpoint path would be correct
        //string expectedPath = $"{MovieInfoBackend.Helpers.ProgramConstants.ApiRoutePrefix}/search";
        //Assert.Contains("/search", expectedPath);

        // Verify authorization policy names exist
        //Assert.NotNull(MovieInfoBackend.Helpers.ProgramConstants.LoggedInUsersOnlyPolicyName);
        //Assert.NotNull(MovieInfoBackend.Helpers.ProgramConstants.SearchUsersOnlyPolicyName);
    }
}

