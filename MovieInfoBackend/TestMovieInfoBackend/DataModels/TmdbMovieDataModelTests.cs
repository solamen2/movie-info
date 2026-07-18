using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class TmdbMovieDataModelTests
{
    private string tmdbHttpClientMovieCreditsResponse;
    private string tmdbHttpClientMovieResponse;

    public TmdbMovieDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testMovieCreditsDataFilename = "TmdbHttpClientMovieCreditsResponse.json";
        string testMovieDataFilename = "TmdbHttpClientMovieResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieCreditsDataFilename}"))
        {
            tmdbHttpClientMovieCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieCreditsResponse))
        {
            throw new ArgumentException($"{testMovieCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieDataFilename}"))
        {
            tmdbHttpClientMovieResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieResponse))
        {
            throw new ArgumentException($"{testMovieDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {

    }
}