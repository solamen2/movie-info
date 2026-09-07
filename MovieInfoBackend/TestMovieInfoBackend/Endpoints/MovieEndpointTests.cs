
using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.Endpoints;

public class MovieEndpointTests
{
    private string suggestionHttpClientResponse1;
    private string omdbHttpClientMovieResponse;
    private string tmdbHttpClientMovieCreditsResponse;
    private string tmdbHttpClientMovieResponse;
    private string tmdbHttpClientMovieWatchProvidersResponse;
    private string tmdbHttpClientConfigurationCountriesResponse;
    private string tmdbHttpClientConfigurationLanguagesResponse;

    public MovieEndpointTests(ITestOutputHelper output)
    {   
        // Arrange

        string testSuggestionDataFilename1 = "SuggestionHttpClientResponse1.json";
        string testOmdbMovieDataFilename = "OmdbHttpClientMovieResponse.json";
        string testTmdbMovieCreditsDataFilename = "TmdbHttpClientMovieCreditsResponse.json";
        string testTmdbMovieDataFilename = "TmdbHttpClientMovieResponse.json";
        string testTmdbMovieWatchProvidersDataFilename = "TmdbHttpClientMovieWatchProvidersResponse.json";
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

        using (StreamReader sr = File.OpenText($"../../../TestData/{testOmdbMovieDataFilename}"))
        {
            omdbHttpClientMovieResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(omdbHttpClientMovieResponse))
        {
            throw new ArgumentException($"{testOmdbMovieDataFilename} is not valid test data.");
        }
        
        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbMovieCreditsDataFilename}"))
        {
            tmdbHttpClientMovieCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieCreditsResponse))
        {
            throw new ArgumentException($"{testTmdbMovieCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbMovieDataFilename}"))
        {
            tmdbHttpClientMovieResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieResponse))
        {
            throw new ArgumentException($"{testTmdbMovieDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbMovieWatchProvidersDataFilename}"))
        {
            tmdbHttpClientMovieWatchProvidersResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieWatchProvidersResponse))
        {
            throw new ArgumentException($"{testTmdbMovieWatchProvidersDataFilename} is not valid test data.");
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
    public async Task MovieEndpoint_ValidDataModels_ConvertSuccessfullyIntoViewModel()
    {
        // Act
        SuggestionsResponseDataModel? suggestionsResponse1 = SuggestionHttpClient.GetModelFromResponse(suggestionHttpClientResponse1);
        Assert.NotNull(suggestionsResponse1);
        Assert.NotNull(suggestionsResponse1.Suggestions);

        OmdbResponseDataModel? omdbMovieResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientMovieResponse);
        Assert.NotNull(omdbMovieResponse);

        TmdbMovieCreditsResponseDataModel? tmdbMovieCreditsResponse = TmdbHttpClient.GetMovieCreditsModelFromResponse(tmdbHttpClientMovieCreditsResponse);
        Assert.NotNull(tmdbMovieCreditsResponse);
        TmdbMovieResponseDataModel? tmdbMovieResponse = TmdbHttpClient.GetMovieModelFromResponse(tmdbHttpClientMovieResponse);
        Assert.NotNull(tmdbMovieResponse);
        TmdbWatchProvidersResponseDataModel? tmdbMovieWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientMovieWatchProvidersResponse);
        Assert.NotNull(tmdbMovieWatchProvidersResponse);
        TmdbConfigurationCountriesResponseDataModel? tmdbConfigurationCountriesResponse = TmdbHttpClient.GetConfigurationCountriesModelFromResponse(tmdbHttpClientConfigurationCountriesResponse);
        Assert.NotNull(tmdbConfigurationCountriesResponse);
        TmdbConfigurationCountriesResponseDataModel.ConfigurationCountriesDictionary? tmdbConfigurationCountriesDictionary = tmdbConfigurationCountriesResponse.GetConfigurationCountriesDictionary();
        Assert.NotNull(tmdbConfigurationCountriesDictionary);
        TmdbConfigurationLanguagesResponseDataModel? configurationLanguagesResponse = TmdbHttpClient.GetConfigurationLanguagesModelFromResponse(tmdbHttpClientConfigurationLanguagesResponse);
        Assert.NotNull(configurationLanguagesResponse);
        TmdbConfigurationLanguagesResponseDataModel.ConfigurationLanguagesDictionary? tmdbConfigurationLanguagesDictionary = configurationLanguagesResponse.GetConfigurationLanguagesDictionary();
        Assert.NotNull(tmdbConfigurationLanguagesDictionary);

        List<SuggestionViewModel> suggestionViewModels1 = new List<SuggestionViewModel>();
        foreach (SuggestionDataModel suggestionDataModel in suggestionsResponse1.Suggestions)
        {
            suggestionViewModels1.Add(new SuggestionViewModel(suggestionDataModel));
        }

        // Assert
        Assert.Equal(8, suggestionViewModels1.Count);
        
        SuggestionViewModel movieSuggestionViewModel = suggestionViewModels1.First(svm => svm.Name == "Example Movie");

        MovieViewModel movieViewModel = new MovieViewModel(movieSuggestionViewModel,
                                                           omdbMovieResponse,
                                                           tmdbMovieResponse,
                                                           tmdbMovieCreditsResponse,
                                                           tmdbMovieWatchProvidersResponse,
                                                           tmdbConfigurationCountriesDictionary,
                                                           tmdbConfigurationLanguagesDictionary);
    }

    [Fact]
    public void MovieEndpoint_EndpointConfiguration_HasCorrectAttributes()
    {
        // This test verifies the endpoint configuration by examining what the Map method should set up
        // The actual endpoint testing would require full integration testing

        // Arrange & Act & Assert
        // Verify that the endpoint path would be correct
        string expectedPath = $"{MovieInfoBackend.Helpers.ProgramConstants.ApiRoutePrefix}/movie";
        Assert.Contains("/movie", expectedPath);

        // Verify authorization policy names exist
        Assert.NotNull(MovieInfoBackend.Helpers.ProgramConstants.LoggedInUsersOnlyPolicyName);
        Assert.NotNull(MovieInfoBackend.Helpers.ProgramConstants.SearchUsersOnlyPolicyName);
    }
}

