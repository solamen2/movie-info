using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.DataModels;

public class TvSeriesViewModelTests
{
    private string suggestionHttpClientResponse1;
    private string omdbHttpClientTvSeriesResponse;
    private string tmdbHttpClientTvSeriesResponse;
    private string tmdbHttpClientTvSeriesAggregateCreditsResponse;
    private string tmdbHttpClientTvSeriesWatchProvidersResponse;
    private string tmdbHttpClientConfigurationCountriesResponse;
    private string tmdbHttpClientConfigurationLanguagesResponse;

    ITestOutputHelper _testOutputHelper;

    public TvSeriesViewModelTests(ITestOutputHelper output)
    {
        _testOutputHelper = output;
        
        // Arrange

        string testDataFilename1 = "MovieHttpClientResponse1.json";
        string testOmdbTvSeriesDataFilename = "OmdbHttpClientTvSeriesResponse.json";
        string testTvSeriesDataFilename = "TmdbHttpClientTvSeriesResponse.json";
        string testTvSeriesAggregateCreditsDataFilename = "TmdbHttpClientTvSeriesAggregateCreditsResponse.json";
        string testTvSeriesWatchProvidersDataFilename = "TmdbHttpClientTvSeriesWatchProvidersResponse.json";
        string testCountriesDataFilename = "TmdbHttpClientConfigurationCountriesResponse.json";
        string testLanguagesDataFilename = "TmdbHttpClientConfigurationLanguagesResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testDataFilename1}"))
        {
            suggestionHttpClientResponse1 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(suggestionHttpClientResponse1))
        {
            throw new ArgumentException($"{testDataFilename1} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testOmdbTvSeriesDataFilename}"))
        {
            omdbHttpClientTvSeriesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(omdbHttpClientTvSeriesResponse))
        {
            throw new ArgumentException($"{testOmdbTvSeriesDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesDataFilename}"))
        {
            tmdbHttpClientTvSeriesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesResponse))
        {
            throw new ArgumentException($"{testTvSeriesDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesAggregateCreditsDataFilename}"))
        {
            tmdbHttpClientTvSeriesAggregateCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesAggregateCreditsResponse))
        {
            throw new ArgumentException($"{testTvSeriesAggregateCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesWatchProvidersDataFilename}"))
        {
            tmdbHttpClientTvSeriesWatchProvidersResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesWatchProvidersResponse))
        {
            throw new ArgumentException($"{testTvSeriesWatchProvidersDataFilename} is not valid test data.");
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
        SuggestionDataModel? suggestionTvSeriesDataModel = suggestions1?[2];  // TV series suggestion = index 2
        Assert.NotNull(suggestionTvSeriesDataModel);
        SuggestionViewModel suggestionTvSeriesViewModel = new(suggestionTvSeriesDataModel);

        OmdbResponseDataModel? omdbTvSeriesResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientTvSeriesResponse);
        Assert.NotNull(omdbTvSeriesResponse);

        TmdbTvSeriesResponseDataModel? tmdbTvSeriesResponse = TmdbHttpClient.GetTvSeriesModelFromResponse(tmdbHttpClientTvSeriesResponse);
        Assert.NotNull(tmdbTvSeriesResponse);

        TmdbTvSeriesAggregateCreditsResponseDataModel? tvSeriesAggregateCreditsResponse = TmdbHttpClient.GetTvSeriesAggregateCreditsModelFromResponse(tmdbHttpClientTvSeriesAggregateCreditsResponse);
        Assert.NotNull(tvSeriesAggregateCreditsResponse);

        TmdbWatchProvidersResponseDataModel? tvSeriesWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientTvSeriesWatchProvidersResponse);
        Assert.NotNull(tvSeriesWatchProvidersResponse);

        TmdbConfigurationCountriesResponseDataModel? tvConfigurationCountriesResponse = TmdbHttpClient.GetConfigurationCountriesModelFromResponse(tmdbHttpClientConfigurationCountriesResponse);
        Assert.NotNull(tvConfigurationCountriesResponse);
        TmdbConfigurationCountriesResponseDataModel.ConfigurationCountriesDictionary? tvConfigurationCountriesDictionary = tvConfigurationCountriesResponse.GetConfigurationCountriesDictionary();
        Assert.NotNull(tvConfigurationCountriesDictionary);

        TmdbConfigurationLanguagesResponseDataModel? tvConfigurationLanguagesResponse = TmdbHttpClient.GetConfigurationLanguagesModelFromResponse(tmdbHttpClientConfigurationLanguagesResponse);
        Assert.NotNull(tvConfigurationLanguagesResponse);
        TmdbConfigurationLanguagesResponseDataModel.ConfigurationLanguagesDictionary? tvConfigurationLanguagesDictionary = tvConfigurationLanguagesResponse.GetConfigurationLanguagesDictionary();
        Assert.NotNull(tvConfigurationLanguagesDictionary);

        // Act

        TvSeriesViewModel tvSeriesViewModel = new(suggestionTvSeriesViewModel, 
                                                  omdbTvSeriesResponse,
                                                  tmdbTvSeriesResponse,
                                                  tvSeriesAggregateCreditsResponse,
                                                  tvSeriesWatchProvidersResponse,
                                                  tvConfigurationCountriesDictionary,
                                                  tvConfigurationLanguagesDictionary);
        
        // Assert
        
        Assert.NotNull(tvSeriesViewModel);

        // TODO: Check all fields of view model

        _testOutputHelper.WriteLine(tvSeriesViewModel.ToString());
        Assert.Fail();
    }
}
