using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class TmdbIdDataModelTests
{
    private string tmdbHttpClientMovieIdResponse;
    private string tmdbHttpClientPersonIdResponse;
    private string tmdbHttpClientTvEpisodeIdResponse;
    private string tmdbHttpClientTvSeasonIdResponse;
    private string tmdbHttpClientTvSeriesIdResponse;

    public TmdbIdDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testMovieIdDataFilename = "TmdbHttpClientMovieIdResponse.json";
        string testPersonIdDataFilename = "TmdbHttpClientPersonIdResponse.json";
        string testTvEpisodeIdDataFilename = "TmdbHttpClientTvEpisodeIdResponse.json";
        string testTvSeasonIdDataFilename = "TmdbHttpClientTvSeasonIdResponse.json";
        string testTvSeriesIdDataFilename = "TmdbHttpClientTvSeriesIdResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieIdDataFilename}"))
        {
            tmdbHttpClientMovieIdResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieIdResponse))
        {
            throw new ArgumentException($"{testMovieIdDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonIdDataFilename}"))
        {
            tmdbHttpClientPersonIdResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonIdResponse))
        {
            throw new ArgumentException($"{testPersonIdDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvEpisodeIdDataFilename}"))
        {
            tmdbHttpClientTvEpisodeIdResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvEpisodeIdResponse))
        {
            throw new ArgumentException($"{testTvEpisodeIdDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeasonIdDataFilename}"))
        {
            tmdbHttpClientTvSeasonIdResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeasonIdResponse))
        {
            throw new ArgumentException($"{testTvSeasonIdDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesIdDataFilename}"))
        {
            tmdbHttpClientTvSeriesIdResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesIdResponse))
        {
            throw new ArgumentException($"{testTvSeriesIdDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {

    }
}