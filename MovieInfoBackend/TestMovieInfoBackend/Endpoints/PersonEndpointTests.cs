
using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.Endpoints;

// TODO: Make sure the final name of PersonEndpoint is correct!

public class PersonEndpointTests
{
    private string suggestionHttpClientResponse1;
    private string tmdbHttpClientPersonImagesResponse;
    private string tmdbHttpClientPersonMovieCreditsResponse;
    private string tmdbHttpClientPersonResponse;
    private string tmdbHttpClientPersonTvSeriesCreditsResponse;

    public PersonEndpointTests(ITestOutputHelper output)
    {   
        // Arrange

        string testSuggestionDataFilename1 = "SuggestionHttpClientResponse1.json";
        string testTmdbPersonImagesDataFilename = "TmdbHttpClientPersonImagesResponse.json";
        string testTmdbPersonMovieCreditsDataFilename = "TmdbHttpClientPersonMovieCreditsResponse.json";
        string testTmdbPersonDataFilename = "TmdbHttpClientPersonResponse.json";
        string testTmdbPersonTvSeriesCreditsDataFilename = "TmdbHttpClientPersonTvSeriesCreditsResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testSuggestionDataFilename1}"))
        {
            suggestionHttpClientResponse1 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(suggestionHttpClientResponse1))
        {
            throw new ArgumentException($"{testSuggestionDataFilename1} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbPersonImagesDataFilename}"))
        {
            tmdbHttpClientPersonImagesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonImagesResponse))
        {
            throw new ArgumentException($"{testTmdbPersonImagesDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbPersonMovieCreditsDataFilename}"))
        {
            tmdbHttpClientPersonMovieCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonMovieCreditsResponse))
        {
            throw new ArgumentException($"{testTmdbPersonMovieCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbPersonDataFilename}"))
        {
            tmdbHttpClientPersonResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonResponse))
        {
            throw new ArgumentException($"{testTmdbPersonDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbPersonTvSeriesCreditsDataFilename}"))
        {
            tmdbHttpClientPersonTvSeriesCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonTvSeriesCreditsResponse))
        {
            throw new ArgumentException($"{testTmdbPersonTvSeriesCreditsDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public async Task PersonEndpoint_ValidDataModels_ConvertSuccessfullyIntoViewModel()
    {
        // TODO: Finish test later
        
        // Act
        SuggestionsResponseDataModel? suggestionsResponse1 = SuggestionHttpClient.GetModelFromResponse(suggestionHttpClientResponse1);
        Assert.NotNull(suggestionsResponse1);
        Assert.NotNull(suggestionsResponse1.Suggestions);

        TmdbPersonImagesResponseDataModel? tmdbPersonImagesResponse = TmdbHttpClient.GetPersonImagesModelFromResponse(tmdbHttpClientPersonImagesResponse);
        Assert.NotNull(tmdbPersonImagesResponse);
        TmdbPersonMovieCreditsResponseDataModel? tmdbPersonMovieCreditsResponse = TmdbHttpClient.GetPersonMovieCreditsModelFromResponse(tmdbHttpClientPersonMovieCreditsResponse);
        Assert.NotNull(tmdbPersonMovieCreditsResponse);
        TmdbPersonResponseDataModel? tmdbPersonResponse = TmdbHttpClient.GetPersonModelFromResponse(tmdbHttpClientPersonResponse);
        Assert.NotNull(tmdbPersonResponse);
        TmdbPersonTvSeriesCreditsResponseDataModel? tmdbPersonTvSeriesCreditsResponse = TmdbHttpClient.GetPersonTvSeriesCreditsModelFromResponse(tmdbHttpClientPersonTvSeriesCreditsResponse);
        Assert.NotNull(tmdbPersonTvSeriesCreditsResponse);

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
    public void PersonEndpoint_EndpointConfiguration_HasCorrectAttributes()
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

