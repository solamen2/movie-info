using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class TmdbGenreDataModelTests
{
    private string tmdbHttpClientMovieGenresResponse;
    private string tmdbHttpClientTvSeriesGenresResponse;

    public TmdbGenreDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testMovieGenresDataFilename = "TmdbHttpClientMovieGenresResponse.json";
        string testTvSeriesGenresDataFilename = "TmdbHttpClientTvSeriesGenresResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieGenresDataFilename}"))
        {
            tmdbHttpClientMovieGenresResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieGenresResponse))
        {
            throw new ArgumentException($"{testMovieGenresDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesGenresDataFilename}"))
        {
            tmdbHttpClientTvSeriesGenresResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesGenresResponse))
        {
            throw new ArgumentException($"{testTvSeriesGenresDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {

    }
}