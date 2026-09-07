
using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.Endpoints;

// TODO: Make sure the final name of TvEpisodeEndpoint is correct!

public class TvEpisodeEndpointTests
{
    private string omdbHttpClientTvEpisodeResponse;
    private string tmdbHttpClientTvEpisodeResponse;
    private string tmdbHttpClientTvEpisodeCreditsResponse;

    public TvEpisodeEndpointTests(ITestOutputHelper output)
    {   
        // Arrange

        string testOmdbTvEpisodeDataFilename = "OmdbHttpClientTvEpisodeResponse.json";
        string testTmdbTvEpisodeDataFilename = "TmdbHttpClientTvEpisodeResponse.json";
        string testTmdbTvEpisodeCreditsDataFilename = "TmdbHttpClientTvEpisodeCreditsResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testOmdbTvEpisodeDataFilename}"))
        {
            omdbHttpClientTvEpisodeResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(omdbHttpClientTvEpisodeResponse))
        {
            throw new ArgumentException($"{testOmdbTvEpisodeDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbTvEpisodeDataFilename}"))
        {
            tmdbHttpClientTvEpisodeResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvEpisodeResponse))
        {
            throw new ArgumentException($"{testTmdbTvEpisodeDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbTvEpisodeCreditsDataFilename}"))
        {
            tmdbHttpClientTvEpisodeCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvEpisodeCreditsResponse))
        {
            throw new ArgumentException($"{testTmdbTvEpisodeCreditsDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public async Task TvEpisodeEndpoint_ValidDataModels_ConvertSuccessfullyIntoViewModel()
    {
        // TODO: Finish test later
        
        // Act
        OmdbResponseDataModel? omdbTvEpisodeResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientTvEpisodeResponse);
        Assert.NotNull(omdbTvEpisodeResponse);

        TmdbTvEpisodeResponseDataModel? tmdbTvEpisodeResponse = TmdbHttpClient.GetTvEpisodeModelFromResponse(tmdbHttpClientTvEpisodeResponse);
        Assert.NotNull(tmdbTvEpisodeResponse);
        TmdbTvEpisodeCreditsResponseDataModel? tmdbTvEpisodeCreditsResponse = TmdbHttpClient.GetTvEpisodeCreditsModelFromResponse(tmdbHttpClientTvEpisodeCreditsResponse);
        Assert.NotNull(tmdbTvEpisodeCreditsResponse);

        /*

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
    public void TvEpisodeEndpoint_EndpointConfiguration_HasCorrectAttributes()
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

