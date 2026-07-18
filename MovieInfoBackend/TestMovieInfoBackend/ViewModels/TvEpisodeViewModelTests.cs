using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.DataModels;

public class TvEpisodeViewModelTests
{
    private string suggestionHttpClientResponse1;
    private string omdbHttpClientTvEpisodeResponse;
    private string tmdbHttpClientTvEpisodeResponse;
    private string tmdbHttpClientTvEpisodeCreditsResponse;

    ITestOutputHelper _testOutputHelper;

    public TvEpisodeViewModelTests(ITestOutputHelper output)
    {
        _testOutputHelper = output;
        
        // Arrange

        string testDataFilename1 = "MovieHttpClientResponse1.json";
        string testOmdbTvEpisodeDataFilename = "OmdbHttpClientTvEpisodeResponse.json";
        string testTmdbTvEpisodeDataFilename = "TmdbHttpClientTvEpisodeResponse.json";
        string testTvEpisodeCreditsDataFilename = "TmdbHttpClientTvEpisodeCreditsResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testDataFilename1}"))
        {
            suggestionHttpClientResponse1 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(suggestionHttpClientResponse1))
        {
            throw new ArgumentException($"{testDataFilename1} is not valid test data.");
        }

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

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvEpisodeCreditsDataFilename}"))
        {
            tmdbHttpClientTvEpisodeCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvEpisodeCreditsResponse))
        {
            throw new ArgumentException($"{testTvEpisodeCreditsDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetViewModelFromDataModel_ValidDataModel_ReturnsValidViewModel()
    {
        // Arrange (continued)
        // Get data models
        
        OmdbResponseDataModel? omdbTvEpisodeResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientTvEpisodeResponse);
        Assert.NotNull(omdbTvEpisodeResponse);

        TmdbTvEpisodeResponseDataModel? tmdbTvEpisodeResponse = TmdbHttpClient.GetTvEpisodeModelFromResponse(tmdbHttpClientTvEpisodeResponse);
        Assert.NotNull(tmdbTvEpisodeResponse);

        TmdbTvEpisodeCreditsResponseDataModel? tvEpisodeCreditsResponse = TmdbHttpClient.GetTvEpisodeCreditsModelFromResponse(tmdbHttpClientTvEpisodeCreditsResponse);
        Assert.NotNull(tvEpisodeCreditsResponse);

        TvEpisodeViewModel tvEpisodeViewModel = new(omdbTvEpisodeResponse, 
                                                    tmdbTvEpisodeResponse,
                                                    tvEpisodeCreditsResponse);
        Assert.NotNull(tvEpisodeViewModel);

        // TODO: Check all fields of view model

        // Act

        _testOutputHelper.WriteLine(tvEpisodeViewModel.ToString());
        Assert.Fail();
    }
}
