using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class WikipediaDataModelTests
{
    private string wikipediaHttpClientMovieResponse;
    private string wikipediaHttpClientPersonResponse;
    private string wikipediaHttpClientTvSeriesResponse;
    private string wikipediaHttpClientEmptyResponse;

    public WikipediaDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        wikipediaHttpClientMovieResponse = ReadTestData("WikipediaHttpClientMovieResponse.json");
        wikipediaHttpClientPersonResponse = ReadTestData("WikipediaHttpClientPersonResponse.json");
        wikipediaHttpClientTvSeriesResponse = ReadTestData("WikipediaHttpClientTvSeriesResponse.json");
        wikipediaHttpClientEmptyResponse = ReadTestData("WikipediaHttpClientEmptyResponse.json");
    }

    private static string ReadTestData(string filename)
    {
        string testData;
        using (StreamReader sr = File.OpenText($"../../../TestData/{filename}"))
        {
            testData = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(testData))
        {
            throw new ArgumentException($"{filename} is not valid test data.");
        }
        return testData;
    }

    [Fact]
    public void GetModelFromResponse_ValidResponses_ReturnsValidModels()
    {
        // Act

        WikipediaOpenSearchResponseDataModel? movieResponse = WikipediaHttpClient.GetModelFromResponse(wikipediaHttpClientMovieResponse);
        WikipediaOpenSearchResponseDataModel? personResponse = WikipediaHttpClient.GetModelFromResponse(wikipediaHttpClientPersonResponse);
        WikipediaOpenSearchResponseDataModel? tvSeriesResponse = WikipediaHttpClient.GetModelFromResponse(wikipediaHttpClientTvSeriesResponse);

        // Assert

        // Movie
        Assert.NotNull(movieResponse);
        Assert.Equal("The Shawshank Redemption (1994 film)", movieResponse.SearchQuery);
        Assert.Equal(["The Shawshank Redemption (1994 film)"], movieResponse.Titles);
        Assert.Equal([""], movieResponse.Descriptions);
        Assert.Equal(["https://en.wikipedia.org/wiki/The_Shawshank_Redemption_(1994_film)"], movieResponse.Links);

        // Person
        Assert.NotNull(personResponse);
        Assert.Equal("Katharine Hepburn", personResponse.SearchQuery);
        Assert.Equal(["Katharine Hepburn"], personResponse.Titles);
        Assert.Equal([""], personResponse.Descriptions);
        Assert.Equal(["https://en.wikipedia.org/wiki/Katharine_Hepburn"], personResponse.Links);

        // TV series
        Assert.NotNull(tvSeriesResponse);
        Assert.Equal("Buffy the Vampire Slayer (TV series)", tvSeriesResponse.SearchQuery);
        Assert.Equal(["Buffy the Vampire Slayer (TV series)"], tvSeriesResponse.Titles);
        Assert.Equal([""], tvSeriesResponse.Descriptions);
        Assert.Equal(["https://en.wikipedia.org/wiki/Buffy_the_Vampire_Slayer_(TV_series)"], tvSeriesResponse.Links);
    }

    [Fact]
    public void GetModelFromResponse_EmptyResponse_ReturnsModelWithEmptyArrays()
    {
        // Act
        WikipediaOpenSearchResponseDataModel? emptyResponse = WikipediaHttpClient.GetModelFromResponse(wikipediaHttpClientEmptyResponse);

        // Assert
        Assert.NotNull(emptyResponse);
        Assert.Equal("Cruel Intentions (1999 film)", emptyResponse.SearchQuery);
        Assert.Empty(emptyResponse.Titles);
        Assert.Empty(emptyResponse.Descriptions);
        Assert.Empty(emptyResponse.Links);
    }

    [Fact]
    public void WikipediaOpenSearchResponseDataModel_ValidModelToString_ReturnsCorrectValue()
    {
        // Arrange
        WikipediaOpenSearchResponseDataModel? movieResponse = WikipediaHttpClient.GetModelFromResponse(wikipediaHttpClientMovieResponse);
        Assert.NotNull(movieResponse);

        // Act & Assert
        Assert.Equal(
            "SearchQuery: The Shawshank Redemption (1994 film)\nTitles: The Shawshank Redemption (1994 film)\nDescriptions: \nLinks: https://en.wikipedia.org/wiki/The_Shawshank_Redemption_(1994_film)",
            movieResponse.ToString());
    }
}
