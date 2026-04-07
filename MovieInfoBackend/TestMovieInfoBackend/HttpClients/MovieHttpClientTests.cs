using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.DataModels;

public class MovieHttpClientTests
{
    private string malformedResponse;

    public MovieHttpClientTests(ITestOutputHelper output)
    {
        // Arrange

        string malformedDataFilename = "MovieHttpClientMalformedResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{malformedDataFilename}"))
        {
            malformedResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(malformedResponse))
        {
            throw new ArgumentException($"{malformedDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetModelFromResponse_MalformedJson_ReturnsModelWithNullSuggestions()
    {
        // Act
        MovieSuggestionsResponseDataModel? actual = MovieHttpClient.GetModelFromResponse(malformedResponse);

        // Assert
        Assert.NotNull(actual);
        Assert.Null(actual.Suggestions); // Malformed JSON without "d" property results in null Suggestions
    }

    // TODO: Do some error tests here
}
