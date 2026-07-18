using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class OmdbDataModelTests
{
    private string omdbHttpClientMovieResponse;
    private string omdbHttpClientTvEpisodeResponse;
    private string omdbHttpClientTvSeriesResponse;

    public OmdbDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testMovieDataFilename = "OmdbHttpClientMovieResponse.json";
        string testTvEpisodeDataFilename = "OmdbHttpClientTvEpisodeResponse.json";
        string testTvSeriesDataFilename = "OmdbHttpClientTvSeriesResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieDataFilename}"))
        {
            omdbHttpClientMovieResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(omdbHttpClientMovieResponse))
        {
            throw new ArgumentException($"{testMovieDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvEpisodeDataFilename}"))
        {
            omdbHttpClientTvEpisodeResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(omdbHttpClientTvEpisodeResponse))
        {
            throw new ArgumentException($"{testTvEpisodeDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesDataFilename}"))
        {
            omdbHttpClientTvSeriesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(omdbHttpClientTvSeriesResponse))
        {
            throw new ArgumentException($"{testTvSeriesDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {

    }
}