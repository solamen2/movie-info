using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class TmdbTvSeasonDataModelTests
{
    private string tmdbHttpClientTvSeasonResponse;

    public TmdbTvSeasonDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testTvSeasonDataFilename = "TmdbHttpClientTvSeasonResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeasonDataFilename}"))
        {
            tmdbHttpClientTvSeasonResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeasonResponse))
        {
            throw new ArgumentException($"{testTvSeasonDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {

    }
}