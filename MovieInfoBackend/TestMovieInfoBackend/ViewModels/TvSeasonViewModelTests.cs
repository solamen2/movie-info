using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.DataModels;

public class TvSeasonViewModelTests
{
    private string tmdbHttpClientTvSeasonResponse;
    private string tmdbHttpClientTvSeasonWatchProvidersResponse;

    ITestOutputHelper _testOutputHelper;

    public TvSeasonViewModelTests(ITestOutputHelper output)
    {
        _testOutputHelper = output;
        
        // Arrange

        string testTvSeasonDataFilename = "TmdbHttpClientTvSeasonResponse.json";
        string testTvSeasonWatchProvidersDataFilename = "TmdbHttpClientTvSeasonWatchProvidersResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeasonDataFilename}"))
        {
            tmdbHttpClientTvSeasonResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeasonResponse))
        {
            throw new ArgumentException($"{testTvSeasonDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeasonWatchProvidersDataFilename}"))
        {
            tmdbHttpClientTvSeasonWatchProvidersResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeasonWatchProvidersResponse))
        {
            throw new ArgumentException($"{testTvSeasonWatchProvidersDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetViewModelFromDataModel_ValidDataModel_ReturnsValidViewModel()
    {
        // Arrange (continued)
        // Get data models

        TmdbTvSeasonResponseDataModel? tvSeasonResponse = TmdbHttpClient.GetTvSeasonModelFromResponse(tmdbHttpClientTvSeasonResponse);
        Assert.NotNull(tvSeasonResponse);

        TmdbWatchProvidersResponseDataModel? tvSeasonWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientTvSeasonWatchProvidersResponse);
        Assert.NotNull(tvSeasonWatchProvidersResponse);

        TvSeasonViewModel tvSeasonViewModel = new(tvSeasonResponse, 
                                                  tvSeasonWatchProvidersResponse);
        Assert.NotNull(tvSeasonViewModel);

        // TODO: Check all fields of view model

        // Act

        _testOutputHelper.WriteLine(tvSeasonViewModel.ToString());
        Assert.Fail();
    }
}
