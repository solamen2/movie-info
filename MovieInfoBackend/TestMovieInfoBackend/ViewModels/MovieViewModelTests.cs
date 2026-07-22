using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.DataModels;

public class MovieViewModelTests
{
    private string suggestionHttpClientResponse1;
    private string omdbHttpClientMovieResponse;
    private string tmdbHttpClientMovieResponse;
    private string tmdbHttpClientMovieCreditsResponse;
    private string tmdbHttpClientMovieWatchProvidersResponse;
    private string tmdbHttpClientConfigurationCountriesResponse;
    private string tmdbHttpClientConfigurationLanguagesResponse;

    ITestOutputHelper _testOutputHelper;

    public MovieViewModelTests(ITestOutputHelper output)
    {
        _testOutputHelper = output;
        
        // Arrange

        string testSuggestionDataFilename1 = "MovieHttpClientResponse1.json";
        string testOmdbMovieDataFilename = "OmdbHttpClientMovieResponse.json";
        string testTmdbMovieDataFilename = "TmdbHttpClientMovieResponse.json";
        string testMovieCreditsDataFilename = "TmdbHttpClientMovieCreditsResponse.json";
        string testMovieWatchProvidersDataFilename = "TmdbHttpClientMovieWatchProvidersResponse.json";
        string testCountriesDataFilename = "TmdbHttpClientConfigurationCountriesResponse.json";
        string testLanguagesDataFilename = "TmdbHttpClientConfigurationLanguagesResponse.json";

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

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbMovieDataFilename}"))
        {
            tmdbHttpClientMovieResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieResponse))
        {
            throw new ArgumentException($"{testTmdbMovieDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieCreditsDataFilename}"))
        {
            tmdbHttpClientMovieCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieCreditsResponse))
        {
            throw new ArgumentException($"{testMovieCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieWatchProvidersDataFilename}"))
        {
            tmdbHttpClientMovieWatchProvidersResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieWatchProvidersResponse))
        {
            throw new ArgumentException($"{testMovieWatchProvidersDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testCountriesDataFilename}"))
        {
            tmdbHttpClientConfigurationCountriesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientConfigurationCountriesResponse))
        {
            throw new ArgumentException($"{testCountriesDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testLanguagesDataFilename}"))
        {
            tmdbHttpClientConfigurationLanguagesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientConfigurationLanguagesResponse))
        {
            throw new ArgumentException($"{testLanguagesDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetViewModelFromDataModel_ValidDataModel_ReturnsValidViewModel()
    {
        // Arrange (continued)
        // Get data models
        
        MovieSuggestionsResponseDataModel? actual1 = MovieHttpClient.GetModelFromResponse(suggestionHttpClientResponse1);
        SuggestionDataModel[]? suggestions1 = actual1?.Suggestions;
        Assert.Equal(8, suggestions1?.Length);
        SuggestionDataModel? suggestionMovieDataModel = suggestions1?[1];  // Movie suggestion = index 1
        Assert.NotNull(suggestionMovieDataModel);
        SuggestionViewModel suggestionMovieViewModel = new(suggestionMovieDataModel);

        OmdbResponseDataModel? omdbMovieResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientMovieResponse);
        Assert.NotNull(omdbMovieResponse);

        TmdbMovieResponseDataModel? tmdbMovieResponse = TmdbHttpClient.GetMovieModelFromResponse(tmdbHttpClientMovieResponse);
        Assert.NotNull(tmdbMovieResponse);

        TmdbMovieCreditsResponseDataModel? movieCreditsResponse = TmdbHttpClient.GetMovieCreditsModelFromResponse(tmdbHttpClientMovieCreditsResponse);
        Assert.NotNull(movieCreditsResponse);

        TmdbWatchProvidersResponseDataModel? movieWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientMovieWatchProvidersResponse);
        Assert.NotNull(movieWatchProvidersResponse);

        TmdbConfigurationCountriesResponseDataModel? tvConfigurationCountriesResponse = TmdbHttpClient.GetConfigurationCountriesModelFromResponse(tmdbHttpClientConfigurationCountriesResponse);
        Assert.NotNull(tvConfigurationCountriesResponse);
        TmdbConfigurationCountriesResponseDataModel.ConfigurationCountriesDictionary? tvConfigurationCountriesDictionary = tvConfigurationCountriesResponse.GetConfigurationCountriesDictionary();
        Assert.NotNull(tvConfigurationCountriesDictionary);

        TmdbConfigurationLanguagesResponseDataModel? tvConfigurationLanguagesResponse = TmdbHttpClient.GetConfigurationLanguagesModelFromResponse(tmdbHttpClientConfigurationLanguagesResponse);
        Assert.NotNull(tvConfigurationLanguagesResponse);
        TmdbConfigurationLanguagesResponseDataModel.ConfigurationLanguagesDictionary? tvConfigurationLanguagesDictionary = tvConfigurationLanguagesResponse.GetConfigurationLanguagesDictionary();
        Assert.NotNull(tvConfigurationLanguagesDictionary);

        // Act

        MovieViewModel movieViewModel = new(suggestionMovieViewModel, 
                                            omdbMovieResponse,
                                            tmdbMovieResponse,
                                            movieCreditsResponse,
                                            movieWatchProvidersResponse,
                                            tvConfigurationCountriesDictionary,
                                            tvConfigurationLanguagesDictionary);
        
        // Assert
        
        Assert.NotNull(movieViewModel);

        // TODO: Check all fields of view model

        _testOutputHelper.WriteLine(movieViewModel.ToString());
        Assert.Fail();
    }
}
