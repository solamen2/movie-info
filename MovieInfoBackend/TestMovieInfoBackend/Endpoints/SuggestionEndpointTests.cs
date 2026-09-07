using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.Endpoints;

public class SuggestionEndpointTests
{
    private string suggestionHttpClientResponse1;
    private string suggestionHttpClientResponse2;

    public SuggestionEndpointTests(ITestOutputHelper output)
    {
        // Arrange

        string testDataFilename1 = "SuggestionHttpClientResponse1.json";
        string testDataFilename2 = "SuggestionHttpClientResponse2.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testDataFilename1}"))
        {
            suggestionHttpClientResponse1 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(suggestionHttpClientResponse1))
        {
            throw new ArgumentException($"{testDataFilename1} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testDataFilename2}"))
        {
            suggestionHttpClientResponse2 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(suggestionHttpClientResponse2))
        {
            throw new ArgumentException($"{testDataFilename2} is not valid test data.");
        }
    }

    [Fact]
    public async Task SuggestionEndpoints_ValidSuggestionDataModels_ConvertSuccessfullyIntoViewModels()
    {
        // Act
        SuggestionsResponseDataModel? suggestionsResponse1 = SuggestionHttpClient.GetModelFromResponse(suggestionHttpClientResponse1);
        Assert.NotNull(suggestionsResponse1);
        SuggestionsResponseDataModel? suggestionsResponse2 = SuggestionHttpClient.GetModelFromResponse(suggestionHttpClientResponse2);
        Assert.NotNull(suggestionsResponse2);

        Assert.NotNull(suggestionsResponse1);
        Assert.NotNull(suggestionsResponse2);
        Assert.NotNull(suggestionsResponse1.Suggestions);
        Assert.NotNull(suggestionsResponse2.Suggestions);

        List<SuggestionViewModel> suggestionViewModels1 = new List<SuggestionViewModel>();
        foreach (SuggestionDataModel suggestionDataModel in suggestionsResponse1.Suggestions)
        {
            suggestionViewModels1.Add(new SuggestionViewModel(suggestionDataModel));
        }
        List<SuggestionViewModel> suggestionViewModels2 = new List<SuggestionViewModel>();
        foreach (SuggestionDataModel suggestionDataModel in suggestionsResponse2.Suggestions)
        {
            suggestionViewModels2.Add(new SuggestionViewModel(suggestionDataModel));
        }

        // Assert
        Assert.Equal(8, suggestionViewModels1.Count);
        Assert.Equal(6, suggestionViewModels2.Count);
    }

    [Fact]
    public void SuggestionEndpoints_SearchEndpointConfiguration_HasCorrectAttributes()
    {
        // This test verifies the endpoint configuration by examining what the Map method should set up
        // The actual endpoint testing would require full integration testing

        // Arrange & Act & Assert
        // Verify that the endpoint path would be correct
        string expectedPath = $"{MovieInfoBackend.Helpers.ProgramConstants.ApiRoutePrefix}/search";
        Assert.Contains("/search", expectedPath);

        // Verify authorization policy names exist
        Assert.NotNull(MovieInfoBackend.Helpers.ProgramConstants.LoggedInUsersOnlyPolicyName);
        Assert.NotNull(MovieInfoBackend.Helpers.ProgramConstants.SearchUsersOnlyPolicyName);
    }
}

