using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class TmdbTvEpisodeDataModelTests
{
    private string tmdbHttpClientTvEpisodeCreditsResponse;
    private string tmdbHttpClientTvEpisodeResponse;

    public TmdbTvEpisodeDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testTvEpisodeCreditsDataFilename = "TmdbHttpClientTvEpisodeCreditsResponse.json";
        string testTvEpisodeDataFilename = "TmdbHttpClientTvEpisodeResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvEpisodeCreditsDataFilename}"))
        {
            tmdbHttpClientTvEpisodeCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvEpisodeCreditsResponse))
        {
            throw new ArgumentException($"{testTvEpisodeCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvEpisodeDataFilename}"))
        {
            tmdbHttpClientTvEpisodeResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvEpisodeResponse))
        {
            throw new ArgumentException($"{testTvEpisodeDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {

    }
}