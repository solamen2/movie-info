using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class TmdbTvSeriesDataModelTests
{
    private string tmdbHttpClientTvSeriesAggregateCreditsResponse;
    private string tmdbHttpClientTvSeriesResponse;

    public TmdbTvSeriesDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testTvSeriesAggregateCreditsDataFilename = "TmdbHttpClientTvSeriesAggregateCreditsResponse.json";
        string testTvSeriesDataFilename = "TmdbHttpClientTvSeriesResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesAggregateCreditsDataFilename}"))
        {
            tmdbHttpClientTvSeriesAggregateCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesAggregateCreditsResponse))
        {
            throw new ArgumentException($"{testTvSeriesAggregateCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesDataFilename}"))
        {
            tmdbHttpClientTvSeriesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesResponse))
        {
            throw new ArgumentException($"{testTvSeriesDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {

    }
}