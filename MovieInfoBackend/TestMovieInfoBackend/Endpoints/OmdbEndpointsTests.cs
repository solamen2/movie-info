using Moq;
using Moq.Protected;
using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using System.Diagnostics;
using System.Net;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestMovieInfoBackend.Endpoints;

public class OmdbEndpointsTests
{
    private string omdbHttpClientMovieResponse;
    private string omdbHttpClientTvSeriesResponse;
    private string omdbHttpClientTvEpisodeResponse;

    public OmdbEndpointsTests(ITestOutputHelper output)
    {        
        // TODO: Implement HttpClientTests too

        // Arrange

        string testMovieDataFilename = "OmdbHttpClientMovieResponse.json";
        string testTvSeriesDataFilename = "OmdbHttpClientTvSeriesResponse.json";
        string testTvEpisodeDataFilename = "OmdbHttpClientTvEpisodeResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieDataFilename}"))
        {
            omdbHttpClientMovieResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(omdbHttpClientMovieResponse))
        {
            throw new ArgumentException($"{testMovieDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesDataFilename}"))
        {
            omdbHttpClientTvSeriesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(omdbHttpClientTvSeriesResponse))
        {
            throw new ArgumentException($"{testTvSeriesDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvEpisodeDataFilename}"))
        {
            omdbHttpClientTvEpisodeResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(omdbHttpClientTvEpisodeResponse))
        {
            throw new ArgumentException($"{testTvEpisodeDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public async Task Omdb()
    {
        // TODO: Make tests more robust later
        
        // Act
        OmdbResponseDataModel? movieResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientMovieResponse);
        Assert.NotNull(movieResponse);
        OmdbResponseDataModel? tvSeriesResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientTvSeriesResponse);
        Assert.NotNull(tvSeriesResponse);
        OmdbResponseDataModel? tvEpisodeResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientTvEpisodeResponse);
        Assert.NotNull(tvEpisodeResponse);

        //_testOutputHelper.WriteLine("movieResponse:" + movieResponse.ToString());  // TODO: Remove me
        //_testOutputHelper.WriteLine("tvSeriesResponse: " + tvSeriesResponse.ToString());  // TODO: Remove me
        //_testOutputHelper.WriteLine("tvEpisodeResponse: " + tvEpisodeResponse.ToString());  // TODO: Remove me

        /*Assert.NotNull(suggestionsResponse1);
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
        Assert.Equal(6, suggestionViewModels2.Count); */
    }

    [Fact]
    public void Omdb2()
    {
        Assert.True(true);
        
        // TODO: Implement me later
        
        // This test verifies the endpoint configuration by examining what the Map method should set up
        // The actual endpoint testing would require full integration testing

        // Arrange & Act & Assert
        // Verify that the endpoint path would be correct
        //string expectedPath = $"{MovieInfoBackend.Helpers.ProgramConstants.ApiRoutePrefix}/search";
        //Assert.Contains("/search", expectedPath);

        // Verify authorization policy names exist
        //Assert.NotNull(MovieInfoBackend.Helpers.ProgramConstants.LoggedInUsersOnlyPolicyName);
        //Assert.NotNull(MovieInfoBackend.Helpers.ProgramConstants.SearchUsersOnlyPolicyName);
    }
}

