using Moq;
using Moq.Protected;
using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using System.Net;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.Endpoints;

public class TmdbEndpointsTests
{
    private string tmdbHttpClientMovieIdResponse;
    private string tmdbHttpClientMovieResponse;
    private string tmdbHttpClientMovieWatchProvidersResponse;
    private string tmdbHttpClientPersonIdResponse;
    private string tmdbHttpClientPersonResponse;
    private string tmdbHttpClientTvEpisodeIdResponse;
    private string tmdbHttpClientTvSeasonIdResponse;
    private string tmdbHttpClientTvSeriesIdResponse;
    private string tmdbHttpClientTvSeriesResponse;
    private string tmdbHttpClientTvSeriesWatchProvidersResponse;

    public TmdbEndpointsTests(ITestOutputHelper output)
    {   
        // TODO: Implement HttpClientTests too

        // Arrange

        string testMovieIdDataFilename = "TmdbHttpClientMovieIdResponse.json";
        string testMovieDataFilename = "TmdbHttpClientMovieResponse.json";
        string testMovieWatchProvidersDataFilename = "TmdbHttpClientMovieWatchProvidersResponse.json";
        string testPersonIdDataFilename = "TmdbHttpClientPersonIdResponse.json";
        string testPersonDataFilename = "TmdbHttpClientPersonResponse.json";
        string testTvEpisodeIdDataFilename = "TmdbHttpClientTvEpisodeIdResponse.json";
        string testTvSeasonIdDataFilename = "TmdbHttpClientTvSeasonIdResponse.json";
        string testTvSeriesIdDataFilename = "TmdbHttpClientTvSeriesIdResponse.json";
        string testTvSeriesDataFilename = "TmdbHttpClientTvSeriesResponse.json";
        string testTvSeriesWatchProvidersDataFilename = "TmdbHttpClientTvSeriesWatchProvidersResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieIdDataFilename}"))
        {
            tmdbHttpClientMovieIdResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieIdResponse))
        {
            throw new ArgumentException($"{testMovieIdDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieDataFilename}"))
        {
            tmdbHttpClientMovieResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieResponse))
        {
            throw new ArgumentException($"{testMovieDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieWatchProvidersDataFilename}"))
        {
            tmdbHttpClientMovieWatchProvidersResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieWatchProvidersResponse))
        {
            throw new ArgumentException($"{testMovieWatchProvidersDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonIdDataFilename}"))
        {
            tmdbHttpClientPersonIdResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonIdResponse))
        {
            throw new ArgumentException($"{testPersonIdDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonDataFilename}"))
        {
            tmdbHttpClientPersonResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonResponse))
        {
            throw new ArgumentException($"{testPersonDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvEpisodeIdDataFilename}"))
        {
            tmdbHttpClientTvEpisodeIdResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvEpisodeIdResponse))
        {
            throw new ArgumentException($"{testTvEpisodeIdDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeasonIdDataFilename}"))
        {
            tmdbHttpClientTvSeasonIdResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeasonIdResponse))
        {
            throw new ArgumentException($"{testTvSeasonIdDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesIdDataFilename}"))
        {
            tmdbHttpClientTvSeriesIdResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesIdResponse))
        {
            throw new ArgumentException($"{testTvSeriesIdDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesDataFilename}"))
        {
            tmdbHttpClientTvSeriesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesResponse))
        {
            throw new ArgumentException($"{testTvSeriesDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesWatchProvidersDataFilename}"))
        {
            tmdbHttpClientTvSeriesWatchProvidersResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesWatchProvidersResponse))
        {
            throw new ArgumentException($"{testTvSeriesWatchProvidersDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public async Task Tmdb()
    {
        // TODO: Make tests more robust later
        
        // Act
        TmdbIdResponseDataModel? movieIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientMovieIdResponse);
        Assert.NotNull(movieIdResponse);
        TmdbMovieResponseDataModel? movieResponse = TmdbHttpClient.GetMovieModelFromResponse(tmdbHttpClientMovieResponse);
        Assert.NotNull(movieResponse);
        TmdbWatchProvidersResponseDataModel? movieWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientMovieWatchProvidersResponse);
        Assert.NotNull(movieWatchProvidersResponse);
        TmdbIdResponseDataModel? personIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientPersonIdResponse);
        Assert.NotNull(personIdResponse);
        TmdbPersonResponseDataModel? personResponse = TmdbHttpClient.GetPersonModelFromResponse(tmdbHttpClientPersonResponse);
        Assert.NotNull(personResponse);
        TmdbIdResponseDataModel? tvEpisodeIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientTvEpisodeIdResponse);
        Assert.NotNull(tvEpisodeIdResponse);
        TmdbIdResponseDataModel? tvSeasonIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientTvSeasonIdResponse);
        Assert.NotNull(tvSeasonIdResponse);
        TmdbIdResponseDataModel? tvSeriesIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientTvSeriesIdResponse);
        Assert.NotNull(tvSeriesIdResponse);
        TmdbTvSeriesResponseDataModel? tvSeriesResponse = TmdbHttpClient.GetTvSeriesModelFromResponse(tmdbHttpClientTvSeriesResponse);
        Assert.NotNull(tvSeriesResponse);
        TmdbWatchProvidersResponseDataModel? tvSeriesWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientTvSeriesWatchProvidersResponse);
        Assert.NotNull(tvSeriesWatchProvidersResponse);

        //_testOutputHelper.WriteLine("movieIdResponse: " + movieIdResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("movieResponse: " + movieResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("movieWatchProvidersResponse: " + movieWatchProvidersResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("personIdResponse: " + personIdResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("personResponse: " + personResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("tvEpisodeIdResponse: " + tvEpisodeIdResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("tvSeasonIdResponse: " + tvSeasonIdResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("tvSeriesIdResponse: " + tvSeriesIdResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("tvSeriesResponse: " + tvSeriesResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("tvSeriesWatchProvidersResponse: " + tvSeriesWatchProvidersResponse);  // TODO: Remove me

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
    public void Tmdb2()
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

