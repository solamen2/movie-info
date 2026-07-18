using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class TmdbPersonDataModelTests
{
    private string tmdbHttpClientPersonCombinedCreditsResponse;
    private string tmdbHttpClientPersonImagesResponse;
    private string tmdbHttpClientPersonMovieCreditsResponse;
    private string tmdbHttpClientPersonResponse;
    private string tmdbHttpClientPersonTvSeriesCreditsResponse;

    public TmdbPersonDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testPersonCombinedCreditsDataFilename = "TmdbHttpClientPersonCombinedCreditsResponse.json";
        string testPersonImagesDataFilename = "TmdbHttpClientPersonImagesResponse.json";
        string testPersonMovieCreditsDataFilename = "TmdbHttpClientPersonMovieCreditsResponse.json";
        string testPersonDataFilename = "TmdbHttpClientPersonResponse.json";
        string testPersonTvSeriesCreditsDataFilename = "TmdbHttpClientPersonTvSeriesCreditsResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonCombinedCreditsDataFilename}"))
        {
            tmdbHttpClientPersonCombinedCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonCombinedCreditsResponse))
        {
            throw new ArgumentException($"{testPersonCombinedCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonImagesDataFilename}"))
        {
            tmdbHttpClientPersonImagesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonImagesResponse))
        {
            throw new ArgumentException($"{testPersonImagesDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonMovieCreditsDataFilename}"))
        {
            tmdbHttpClientPersonMovieCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonMovieCreditsResponse))
        {
            throw new ArgumentException($"{testPersonMovieCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonDataFilename}"))
        {
            tmdbHttpClientPersonResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonResponse))
        {
            throw new ArgumentException($"{testPersonDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonTvSeriesCreditsDataFilename}"))
        {
            tmdbHttpClientPersonTvSeriesCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonTvSeriesCreditsResponse))
        {
            throw new ArgumentException($"{testPersonTvSeriesCreditsDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {

    }
}