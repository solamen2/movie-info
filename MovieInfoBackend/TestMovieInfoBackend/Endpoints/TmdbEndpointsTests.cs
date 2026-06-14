using Moq;
using Moq.Protected;
using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using System.Net;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestMovieInfoBackend.Endpoints;

public class TmdbEndpointsTests
{
    private string tmdbHttpClientMovieCreditsResponse;
    private string tmdbHttpClientMovieGenresResponse;
    private string tmdbHttpClientMovieIdResponse;
    private string tmdbHttpClientMovieResponse;
    private string tmdbHttpClientMovieWatchProvidersResponse;
    private string tmdbHttpClientPersonExternalIdsResponse;
    private string tmdbHttpClientPersonIdResponse;
    private string tmdbHttpClientPersonImagesResponse;
    private string tmdbHttpClientPersonMovieCreditsResponse;
    private string tmdbHttpClientPersonResponse;
    private string tmdbHttpClientPersonTvSeriesCreditsResponse;
    private string tmdbHttpClientTvEpisodeCreditsResponse;
    private string tmdbHttpClientTvEpisodeExternalIdsResponse;
    private string tmdbHttpClientTvEpisodeIdResponse;
    private string tmdbHttpClientTvSeasonResponse;
    private string tmdbHttpClientTvSeasonIdResponse;
    private string tmdbHttpClientTvSeasonWatchProvidersResponse;
    private string tmdbHttpClientTvSeriesAggregateCreditsResponse;
    private string tmdbHttpClientTvSeriesGenresResponse;
    private string tmdbHttpClientTvSeriesIdResponse;
    private string tmdbHttpClientTvSeriesResponse;
    private string tmdbHttpClientTvSeriesWatchProvidersResponse;

    public TmdbEndpointsTests(ITestOutputHelper output)
    {   
        // TODO: Implement HttpClientTests too

        // Arrange

        string testMovieCreditsDataFilename = "TmdbHttpClientMovieCreditsResponse.json";
        string testMovieGenresDataFilename = "TmdbHttpClientMovieGenresResponse.json";
        string testMovieIdDataFilename = "TmdbHttpClientMovieIdResponse.json";
        string testMovieDataFilename = "TmdbHttpClientMovieResponse.json";
        string testMovieWatchProvidersDataFilename = "TmdbHttpClientMovieWatchProvidersResponse.json";
        string testPersonExternalIdsDataFilename= "TmdbHttpClientPersonExternalIdsResponse.json";
        string testPersonIdDataFilename = "TmdbHttpClientPersonIdResponse.json";
        string testPersonImagesDataFilename = "TmdbHttpClientPersonImagesResponse.json";
        string testPersonMovieCreditsDataFilename = "TmdbHttpClientPersonMovieCreditsResponse.json";
        string testPersonDataFilename = "TmdbHttpClientPersonResponse.json";
        string testPersonTvSeriesCreditsDataFilename = "TmdbHttpClientPersonTvSeriesCreditsResponse.json";
        string testTvEpisodeCreditsDataFilename = "TmdbHttpClientTvEpisodeCreditsResponse.json";
        string testTvEpisodeExternalIdsDataFilename = "TmdbHttpClientTvEpisodeExternalIdsResponse.json";
        string testTvEpisodeIdDataFilename = "TmdbHttpClientTvEpisodeIdResponse.json";
        string testTvSeasonDataFilename = "TmdbHttpClientTvSeasonResponse.json";
        string testTvSeasonIdDataFilename = "TmdbHttpClientTvSeasonIdResponse.json";
        string testTvSeasonWatchProvidersDataFilename = "TmdbHttpClientTvSeasonWatchProvidersResponse.json";
        string testTvSeriesAggregateCreditsDataFilename = "TmdbHttpClientTvSeriesAggregateCreditsResponse.json";
        string testTvSeriesGenresDataFilename = "TmdbHttpClientTvSeriesGenresResponse.json";
        string testTvSeriesIdDataFilename = "TmdbHttpClientTvSeriesIdResponse.json";
        string testTvSeriesDataFilename = "TmdbHttpClientTvSeriesResponse.json";
        string testTvSeriesWatchProvidersDataFilename = "TmdbHttpClientTvSeriesWatchProvidersResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieCreditsDataFilename}"))
        {
            tmdbHttpClientMovieCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieCreditsResponse))
        {
            throw new ArgumentException($"{testMovieCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieGenresDataFilename}"))
        {
            tmdbHttpClientMovieGenresResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieGenresResponse))
        {
            throw new ArgumentException($"{testMovieGenresDataFilename} is not valid test data.");
        }

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

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonExternalIdsDataFilename}"))
        {
            tmdbHttpClientPersonExternalIdsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonExternalIdsResponse))
        {
            throw new ArgumentException($"{testPersonExternalIdsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonIdDataFilename}"))
        {
            tmdbHttpClientPersonIdResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonIdResponse))
        {
            throw new ArgumentException($"{testPersonIdDataFilename} is not valid test data.");
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

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvEpisodeCreditsDataFilename}"))
        {
            tmdbHttpClientTvEpisodeCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvEpisodeCreditsResponse))
        {
            throw new ArgumentException($"{testTvEpisodeCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvEpisodeExternalIdsDataFilename}"))
        {
            tmdbHttpClientTvEpisodeExternalIdsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvEpisodeExternalIdsResponse))
        {
            throw new ArgumentException($"{testTvEpisodeExternalIdsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvEpisodeIdDataFilename}"))
        {
            tmdbHttpClientTvEpisodeIdResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvEpisodeIdResponse))
        {
            throw new ArgumentException($"{testTvEpisodeIdDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeasonDataFilename}"))
        {
            tmdbHttpClientTvSeasonResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeasonResponse))
        {
            throw new ArgumentException($"{testTvSeasonDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeasonIdDataFilename}"))
        {
            tmdbHttpClientTvSeasonIdResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeasonIdResponse))
        {
            throw new ArgumentException($"{testTvSeasonIdDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeasonWatchProvidersDataFilename}"))
        {
            tmdbHttpClientTvSeasonWatchProvidersResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeasonWatchProvidersResponse))
        {
            throw new ArgumentException($"{testTvSeasonWatchProvidersDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesAggregateCreditsDataFilename}"))
        {
            tmdbHttpClientTvSeriesAggregateCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesAggregateCreditsResponse))
        {
            throw new ArgumentException($"{testTvSeriesAggregateCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesGenresDataFilename}"))
        {
            tmdbHttpClientTvSeriesGenresResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesGenresResponse))
        {
            throw new ArgumentException($"{testTvSeriesGenresDataFilename} is not valid test data.");
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
        TmdbMovieCreditsResponseDataModel? movieCreditsResponse = TmdbHttpClient.GetMovieCreditsModelFromResponse(tmdbHttpClientMovieCreditsResponse);
        Assert.NotNull(movieCreditsResponse);
        TmdbGenresResponseDataModel? movieGenresResponse = TmdbHttpClient.GetGenresModelFromResponse(tmdbHttpClientMovieGenresResponse);
        Assert.NotNull(movieGenresResponse);
        TmdbIdResponseDataModel? movieIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientMovieIdResponse);
        Assert.NotNull(movieIdResponse);
        TmdbMovieResponseDataModel? movieResponse = TmdbHttpClient.GetMovieModelFromResponse(tmdbHttpClientMovieResponse);
        Assert.NotNull(movieResponse);
        TmdbWatchProvidersResponseDataModel? movieWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientMovieWatchProvidersResponse);
        Assert.NotNull(movieWatchProvidersResponse);
        TmdbPersonExternalIdsResponseDataModel? personExternalIdsResponse = TmdbHttpClient.GetPersonExternalIdsModelFromResponse(tmdbHttpClientPersonExternalIdsResponse);
        Assert.NotNull(personExternalIdsResponse);
        TmdbIdResponseDataModel? personIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientPersonIdResponse);
        Assert.NotNull(personIdResponse);
        TmdbPersonImagesResponseDataModel? personImagesResponse = TmdbHttpClient.GetPersonImagesModelFromResponse(tmdbHttpClientPersonImagesResponse);
        Assert.NotNull(personImagesResponse);
        TmdbPersonMovieCreditsResponseDataModel? personMovieCreditsResponse = TmdbHttpClient.GetPersonMovieCreditsModelFromResponse(tmdbHttpClientPersonMovieCreditsResponse);
        Assert.NotNull(personMovieCreditsResponse);
        TmdbPersonResponseDataModel? personResponse = TmdbHttpClient.GetPersonModelFromResponse(tmdbHttpClientPersonResponse);
        Assert.NotNull(personResponse);
        TmdbPersonTvSeriesCreditsResponseDataModel? personTvSeriesCreditsResponse = TmdbHttpClient.GetPersonTvSeriesCreditsModelFromResponse(tmdbHttpClientPersonTvSeriesCreditsResponse);
        Assert.NotNull(personTvSeriesCreditsResponse);
        TmdbTvEpisodeCreditsResponseDataModel? tvEpisodeCreditsResponse = TmdbHttpClient.GetTvEpisodeCreditsModelFromResponse(tmdbHttpClientTvEpisodeCreditsResponse);
        Assert.NotNull(tvEpisodeCreditsResponse);
        TmdbTvEpisodeExternalIdsResponseDataModel? tvEpisodeExternalIdsResponse = TmdbHttpClient.GetTvEpisodeExternalIdsModelFromResponse(tmdbHttpClientTvEpisodeExternalIdsResponse);
        Assert.NotNull(tvEpisodeExternalIdsResponse);
        TmdbIdResponseDataModel? tvEpisodeIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientTvEpisodeIdResponse);
        Assert.NotNull(tvEpisodeIdResponse);
        TmdbTvSeasonResponseDataModel? tvSeasonResponse = TmdbHttpClient.GetTvSeasonModelFromResponse(tmdbHttpClientTvSeasonResponse);
        Assert.NotNull(tvSeasonResponse);
        TmdbWatchProvidersResponseDataModel? tvSeasonWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientTvSeasonWatchProvidersResponse);
        Assert.NotNull(tvSeasonWatchProvidersResponse);
        TmdbIdResponseDataModel? tvSeasonIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientTvSeasonIdResponse);
        Assert.NotNull(tvSeasonIdResponse);
        TmdbTvSeriesAggregateCreditsResponseDataModel? tvSeriesAggregateCreditsResponse = TmdbHttpClient.GetTvSeriesAggregateCreditsModelFromResponse(tmdbHttpClientTvSeriesAggregateCreditsResponse);
        Assert.NotNull(tvSeriesAggregateCreditsResponse);
        TmdbGenresResponseDataModel? tvSeriesGenresResponse = TmdbHttpClient.GetGenresModelFromResponse(tmdbHttpClientTvSeriesGenresResponse);
        Assert.NotNull(tvSeriesGenresResponse);
        TmdbIdResponseDataModel? tvSeriesIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientTvSeriesIdResponse);
        Assert.NotNull(tvSeriesIdResponse);
        TmdbTvSeriesResponseDataModel? tvSeriesResponse = TmdbHttpClient.GetTvSeriesModelFromResponse(tmdbHttpClientTvSeriesResponse);
        Assert.NotNull(tvSeriesResponse);
        TmdbWatchProvidersResponseDataModel? tvSeriesWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientTvSeriesWatchProvidersResponse);
        Assert.NotNull(tvSeriesWatchProvidersResponse);

        //_testOutputHelper.WriteLine("movieCreditsResponse: " + movieCreditsResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("movieGenresResponse: " + movieGenresResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("movieIdResponse: " + movieIdResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("movieResponse: " + movieResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("movieWatchProvidersResponse: " + movieWatchProvidersResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("personExternalIdsResponse: " + personExternalIdsResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("personIdResponse: " + personIdResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("personImagesResponse: " + personImagesResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("personMovieCreditsResponse: " + personMovieCreditsResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("personResponse: " + personResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("personTvSeriesCreditsResponse: " + personTvSeriesCreditsResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("tvEpisodeCreditsResponse: " + tvEpisodeCreditsResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("tvEpisodeExternalIdsResponse: " + tvEpisodeExternalIdsResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("tvEpisodeIdResponse: " + tvEpisodeIdResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("tvSeasonResponse: " + tvSeasonResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("tvSeasonWatchProvidersResponse: " + tvSeasonWatchProvidersResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("tvSeasonIdResponse: " + tvSeasonIdResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("tvSeriesAggregateCreditsResponse: " + tvSeriesAggregateCreditsResponse);  // TODO: Remove me
        //_testOutputHelper.WriteLine("tvSeriesGenresResponse: " + tvSeriesGenresResponse);  // TODO: Remove me
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

