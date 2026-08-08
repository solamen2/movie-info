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
        // Act
        
        TmdbMovieExternalIdsResponseDataModel? movieExternalIdsResponse = TmdbHttpClient.GetMovieExternalIdsModelFromResponse(tmdbHttpClientMovieExternalIdsResponse);
        TmdbPersonExternalIdsResponseDataModel? personExternalIdsResponse = TmdbHttpClient.GetPersonExternalIdsModelFromResponse(tmdbHttpClientPersonExternalIdsResponse);
        TmdbTvEpisodeExternalIdsResponseDataModel? tvEpisodeExternalIdsResponse = TmdbHttpClient.GetTvEpisodeExternalIdsModelFromResponse(tmdbHttpClientTvEpisodeExternalIdsResponse);
        TmdbTvSeriesExternalIdsResponseDataModel? tvSeriesExternalIdsResponse = TmdbHttpClient.GetTvSeriesExternalIdsModelFromResponse(tmdbHttpClientTvSeriesExternalIdsResponse);

        // Assert

        // Movie
        Assert.NotNull(movieExternalIdsResponse);
        Assert.True(movieExternalIdsResponse.TmdbId > 0, "Movie TmdbId must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(movieExternalIdsResponse.ImdbId), "Movie ImdbId must not be empty");

        // Person
        Assert.NotNull(personExternalIdsResponse);
        Assert.True(personExternalIdsResponse.TmdbId > 0, "Person TmdbId must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(personExternalIdsResponse.FreebaseMid), "Person FreebaseMid must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(personExternalIdsResponse.FreebaseId), "Person FreebaseId must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(personExternalIdsResponse.ImdbId), "Person ImdbId must not be empty");

        // TV Episode        
        Assert.NotNull(tvEpisodeExternalIdsResponse);
        Assert.True(tvEpisodeExternalIdsResponse.TmdbId > 0, "TV Episode TmdbId must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(tvEpisodeExternalIdsResponse.ImdbId), "TV Episode ImdbId must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(tvEpisodeExternalIdsResponse.FreebaseMid), "TV Episode FreebaseMid must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(tvEpisodeExternalIdsResponse.FreebaseId), "TV Episode FreebaseId must not be empty");
        
        // TV Series
        Assert.NotNull(tvSeriesExternalIdsResponse);
        Assert.True(tvSeriesExternalIdsResponse.TmdbId > 0, "TV Series TmdbId must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(tvSeriesExternalIdsResponse.ImdbId), "TV Series ImdbId must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(tvSeriesExternalIdsResponse.FreebaseMid), "TV Series FreebaseMid must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(tvSeriesExternalIdsResponse.FreebaseId), "TV Series FreebaseId must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(tvSeriesExternalIdsResponse.WikidataId), "TV Series WikidataId must not be empty");
    }

    [Fact]
    public void TmdbExternalIdDataModel_ValidModelToString_ReturnsCorrectValue()
    {
        // Act
        
        TmdbMovieExternalIdsResponseDataModel? movieExternalIdsResponse = TmdbHttpClient.GetMovieExternalIdsModelFromResponse(tmdbHttpClientMovieExternalIdsResponse);
        TmdbPersonExternalIdsResponseDataModel? personExternalIdsResponse = TmdbHttpClient.GetPersonExternalIdsModelFromResponse(tmdbHttpClientPersonExternalIdsResponse);
        TmdbTvEpisodeExternalIdsResponseDataModel? tvEpisodeExternalIdsResponse = TmdbHttpClient.GetTvEpisodeExternalIdsModelFromResponse(tmdbHttpClientTvEpisodeExternalIdsResponse);
        TmdbTvSeriesExternalIdsResponseDataModel? tvSeriesExternalIdsResponse = TmdbHttpClient.GetTvSeriesExternalIdsModelFromResponse(tmdbHttpClientTvSeriesExternalIdsResponse);

        // Assert

        Assert.NotNull(movieExternalIdsResponse);
        Assert.NotNull(personExternalIdsResponse);      
        Assert.NotNull(tvEpisodeExternalIdsResponse);
        Assert.NotNull(tvSeriesExternalIdsResponse);

        Assert.Equal(
            "TmdbId: 278\nImdbId: tt0111161\nWikidataId: Q172241\nFacebookId: \nInstagramId: \nTwitterId: "
                , movieExternalIdsResponse.ToString());
        Assert.Equal(
            "TmdbId: 11863\nFreebaseMid: /m/06w6_\nFreebaseId: /en/example_smith\nImdbId: nm9000000\nTvRageId: 237017\nWikidataId: Q180665\nFacebookId: examplesmith\nInstagramId: examplesmith\nTiktokId: \nTwitterId: ExampleSmith\nYoutubeId: "
                , personExternalIdsResponse.ToString());
        Assert.Equal(
            "TmdbId: 949534\nImdbId: tt0533466\nFreebaseMid: /m/01lch4\nFreebaseId: /en/example_tv_episode\nTvdbId: 108\nTvRageId: \nWikidataId: Q1501524"
                , tvEpisodeExternalIdsResponse.ToString());
        Assert.Equal(
            "TmdbId: 95\nImdbId: tt0118276\nFreebaseMid: /m/0cskb\nFreebaseId: /en/example_tv_series\nTvdbId: 70327\nTvRageId: \nWikidataId: Q183513\nFacebookId: \nInstagramId: \nTwitterId: "
                , tvSeriesExternalIdsResponse.ToString());
    }
}