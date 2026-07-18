using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class TmdbExternalIdDataModelTests
{
    private string tmdbHttpClientMovieExternalIdsResponse;
    private string tmdbHttpClientPersonExternalIdsResponse;
    private string tmdbHttpClientTvEpisodeExternalIdsResponse;
    private string tmdbHttpClientTvSeriesExternalIdsResponse;

    public TmdbExternalIdDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testMovieExternalIdsDataFilename = "TmdbHttpClientMovieExternalIdsResponse.json";
        string testPersonExternalIdsDataFilename = "TmdbHttpClientPersonExternalIdsResponse.json";
        string testTvEpisodeExternalIdsDataFilename = "TmdbHttpClientTvEpisodeExternalIdsResponse.json";
        string testTvSeriesExternalIdsDataFilename = "TmdbHttpClientTvSeriesExternalIdsResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieExternalIdsDataFilename}"))
        {
            tmdbHttpClientMovieExternalIdsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieExternalIdsResponse))
        {
            throw new ArgumentException($"{testMovieExternalIdsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonExternalIdsDataFilename}"))
        {
            tmdbHttpClientPersonExternalIdsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonExternalIdsResponse))
        {
            throw new ArgumentException($"{testPersonExternalIdsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvEpisodeExternalIdsDataFilename}"))
        {
            tmdbHttpClientTvEpisodeExternalIdsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvEpisodeExternalIdsResponse))
        {
            throw new ArgumentException($"{testTvEpisodeExternalIdsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesExternalIdsDataFilename}"))
        {
            tmdbHttpClientTvSeriesExternalIdsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesExternalIdsResponse))
        {
            throw new ArgumentException($"{testTvSeriesExternalIdsDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {

    }
}