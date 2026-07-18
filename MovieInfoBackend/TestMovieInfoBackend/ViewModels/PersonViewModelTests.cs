using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.DataModels;

public class PersonViewModelTests
{
    private string suggestionHttpClientResponse1;
    private string tmdbHttpClientPersonResponse;
    private string tmdbHttpClientPersonMovieCreditsResponse;
    private string tmdbHttpClientPersonTvSeriesCreditsResponse;
    private string tmdbHttpClientPersonImagesResponse;

    ITestOutputHelper _testOutputHelper;

    public PersonViewModelTests(ITestOutputHelper output)
    {
        _testOutputHelper = output;
        
        // Arrange

        string testDataFilename1 = "MovieHttpClientResponse1.json";
        string testPersonDataFilename = "TmdbHttpClientPersonResponse.json";
        string testPersonMovieCreditsDataFilename = "TmdbHttpClientPersonMovieCreditsResponse.json";
        string testPersonTvSeriesCreditsDataFilename = "TmdbHttpClientPersonTvSeriesCreditsResponse.json";
        string testPersonImagesDataFilename = "TmdbHttpClientPersonImagesResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testDataFilename1}"))
        {
            suggestionHttpClientResponse1 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(suggestionHttpClientResponse1))
        {
            throw new ArgumentException($"{testDataFilename1} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonDataFilename}"))
        {
            tmdbHttpClientPersonResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonResponse))
        {
            throw new ArgumentException($"{testPersonDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonMovieCreditsDataFilename}"))
        {
            tmdbHttpClientPersonMovieCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonMovieCreditsResponse))
        {
            throw new ArgumentException($"{testPersonMovieCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonTvSeriesCreditsDataFilename}"))
        {
            tmdbHttpClientPersonTvSeriesCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonTvSeriesCreditsResponse))
        {
            throw new ArgumentException($"{testPersonTvSeriesCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonImagesDataFilename}"))
        {
            tmdbHttpClientPersonImagesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonImagesResponse))
        {
            throw new ArgumentException($"{testPersonImagesDataFilename} is not valid test data.");
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
        SuggestionDataModel? suggestionPersonDataModel = suggestions1?[0];  // Person suggestion = index 0
        Assert.NotNull(suggestionPersonDataModel);
        SuggestionViewModel suggestionPersonViewModel = new(suggestionPersonDataModel);

        TmdbPersonResponseDataModel? personResponse = TmdbHttpClient.GetPersonModelFromResponse(tmdbHttpClientPersonResponse);
        Assert.NotNull(personResponse);

        TmdbPersonMovieCreditsResponseDataModel? personMovieCreditsResponse = TmdbHttpClient.GetPersonMovieCreditsModelFromResponse(tmdbHttpClientPersonMovieCreditsResponse);
        Assert.NotNull(personMovieCreditsResponse);

        TmdbPersonTvSeriesCreditsResponseDataModel? personTvSeriesCreditsResponse = TmdbHttpClient.GetPersonTvSeriesCreditsModelFromResponse(tmdbHttpClientPersonTvSeriesCreditsResponse);
        Assert.NotNull(personTvSeriesCreditsResponse);

        TmdbPersonImagesResponseDataModel? personImagesResponse = TmdbHttpClient.GetPersonImagesModelFromResponse(tmdbHttpClientPersonImagesResponse);
        Assert.NotNull(personImagesResponse);

        PersonViewModel personViewModel = new(suggestionPersonViewModel, 
                                              personResponse,
                                              personMovieCreditsResponse,
                                              personTvSeriesCreditsResponse,
                                              personImagesResponse);
        Assert.NotNull(personViewModel);

        // TODO: Check all fields of view model

        // Act

        _testOutputHelper.WriteLine(personViewModel.ToString());
        Assert.Fail();
    }
}
