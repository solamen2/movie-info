using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class TmdbWatchProviderDataModelTests
{
    private string tmdbHttpClientMovieWatchProvidersResponse;
    private string tmdbHttpClientTvSeasonWatchProvidersResponse;
    private string tmdbHttpClientTvSeriesWatchProvidersResponse;

    public TmdbWatchProviderDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testMovieWatchProvidersDataFilename = "TmdbHttpClientMovieWatchProvidersResponse.json";
        string testTvSeasonWatchProvidersDataFilename = "TmdbHttpClientTvSeasonWatchProvidersResponse.json";
        string testTvSeriesWatchProvidersDataFilename = "TmdbHttpClientTvSeriesWatchProvidersResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieWatchProvidersDataFilename}"))
        {
            tmdbHttpClientMovieWatchProvidersResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieWatchProvidersResponse))
        {
            throw new ArgumentException($"{testMovieWatchProvidersDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeasonWatchProvidersDataFilename}"))
        {
            tmdbHttpClientTvSeasonWatchProvidersResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeasonWatchProvidersResponse))
        {
            throw new ArgumentException($"{testTvSeasonWatchProvidersDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesWatchProvidersDataFilename}"))
        {
            tmdbHttpClientTvSeriesWatchProvidersResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesWatchProvidersResponse))
        {
            throw new ArgumentException($"{testTvSeriesWatchProvidersDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {

    }
}