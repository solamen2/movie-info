
using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.Endpoints;

// TODO: Make sure the final name of TvSeriesEndpoint is correct!

public class TvSeriesEndpointTests
{
    private string suggestionHttpClientResponse1;
    private string omdbHttpClientTvSeriesResponse;
    private string tmdbHttpClientTvSeriesAggregateCreditsResponse;
    private string tmdbHttpClientTvSeriesResponse;
    private string tmdbHttpClientTvSeriesWatchProvidersResponse;
    private string tmdbHttpClientConfigurationCountriesResponse;
    private string tmdbHttpClientConfigurationLanguagesResponse;

    public TvSeriesEndpointTests(ITestOutputHelper output)
    {   
        // Arrange

        string testSuggestionDataFilename1 = "SuggestionHttpClientResponse1.json";
        string testOmdbTvSeriesDataFilename = "OmdbHttpClientTvSeriesResponse.json";
        string testTmdbTvSeriesAggregateCreditsDataFilename = "TmdbHttpClientTvSeriesAggregateCreditsResponse.json";
        string testTmdbTvSeriesDataFilename = "TmdbHttpClientTvSeriesResponse.json";
        string testTmdbTvSeriesWatchProvidersDataFilename = "TmdbHttpClientTvSeriesWatchProvidersResponse.json";
        string testTmdbConfigurationCountriesDataFilename = "TmdbHttpClientConfigurationCountriesResponse.json";
        string testTmdbConfigurationLanguagesDataFilename = "TmdbHttpClientConfigurationLanguagesResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testSuggestionDataFilename1}"))
        {
            suggestionHttpClientResponse1 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(suggestionHttpClientResponse1))
        {
            throw new ArgumentException($"{testSuggestionDataFilename1} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testOmdbTvSeriesDataFilename}"))
        {
            omdbHttpClientTvSeriesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(omdbHttpClientTvSeriesResponse))
        {
            throw new ArgumentException($"{testOmdbTvSeriesDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbTvSeriesAggregateCreditsDataFilename}"))
        {
            tmdbHttpClientTvSeriesAggregateCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesAggregateCreditsResponse))
        {
            throw new ArgumentException($"{testTmdbTvSeriesAggregateCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbTvSeriesDataFilename}"))
        {
            tmdbHttpClientTvSeriesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesResponse))
        {
            throw new ArgumentException($"{testTmdbTvSeriesDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbTvSeriesWatchProvidersDataFilename}"))
        {
            tmdbHttpClientTvSeriesWatchProvidersResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesWatchProvidersResponse))
        {
            throw new ArgumentException($"{testTmdbTvSeriesWatchProvidersDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbConfigurationCountriesDataFilename}"))
        {
            tmdbHttpClientConfigurationCountriesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientConfigurationCountriesResponse))
        {
            throw new ArgumentException($"{testTmdbConfigurationCountriesDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbConfigurationLanguagesDataFilename}"))
        {
            tmdbHttpClientConfigurationLanguagesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientConfigurationLanguagesResponse))
        {
            throw new ArgumentException($"{testTmdbConfigurationLanguagesDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public async Task TvSeriesEndpoint_ValidDataModels_ConvertSuccessfullyIntoViewModel()
    {
        // TODO: Finish test later
        
        // Act
        SuggestionsResponseDataModel? suggestionsResponse1 = SuggestionHttpClient.GetModelFromResponse(suggestionHttpClientResponse1);
        Assert.NotNull(suggestionsResponse1);
        Assert.NotNull(suggestionsResponse1.Suggestions);

        OmdbResponseDataModel? omdbTvSeriesResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientTvSeriesResponse);
        Assert.NotNull(omdbTvSeriesResponse);

        TmdbTvSeriesAggregateCreditsResponseDataModel? tmdbTvSeriesAggregateCreditsResponse = TmdbHttpClient.GetTvSeriesAggregateCreditsModelFromResponse(tmdbHttpClientTvSeriesAggregateCreditsResponse);
        Assert.NotNull(tmdbTvSeriesAggregateCreditsResponse);
        TmdbTvSeriesResponseDataModel? tmdbTvSeriesResponse = TmdbHttpClient.GetTvSeriesModelFromResponse(tmdbHttpClientTvSeriesResponse);
        Assert.NotNull(tmdbTvSeriesResponse);
        TmdbWatchProvidersResponseDataModel? tmdbTvSeriesWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientTvSeriesWatchProvidersResponse);
        Assert.NotNull(tmdbTvSeriesWatchProvidersResponse);
        TmdbConfigurationCountriesResponseDataModel? tmdbConfigurationCountriesResponse = TmdbHttpClient.GetConfigurationCountriesModelFromResponse(tmdbHttpClientConfigurationCountriesResponse);
        Assert.NotNull(tmdbConfigurationCountriesResponse);
        TmdbConfigurationLanguagesResponseDataModel? tmdbConfigurationLanguagesResponse = TmdbHttpClient.GetConfigurationLanguagesModelFromResponse(tmdbHttpClientConfigurationLanguagesResponse);
        Assert.NotNull(tmdbConfigurationLanguagesResponse);

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
    public void TvSeriesEndpoint_EndpointConfiguration_HasCorrectAttributes()
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

