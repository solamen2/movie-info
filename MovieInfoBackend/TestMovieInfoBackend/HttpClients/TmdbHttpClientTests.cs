using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using MovieInfoBackend.DataModels;
using System.Net;
using Xunit.Abstractions;
using static MovieInfoBackend.DataModels.TmdbConfigurationCountriesResponseDataModel;
using static MovieInfoBackend.DataModels.TmdbConfigurationLanguagesResponseDataModel;

namespace TestMovieInfoBackend.DataModels;

public class TmdbHttpClientTests
{
    private string malformedResponse;
    private string configCountriesErrorResponse1;
    private string configCountriesErrorResponse2;
    private string configLanguagesErrorResponse1;
    private string configLanguagesErrorResponse2;
    private string genresErrorResponse1;
    private string genresErrorResponse2;
    private string findByImdbIdErrorResponse;
    private string movieCreditsErrorResponse;
    private string personImagesErrorResponse;
    private string personMovieCreditsErrorResponse;
    private string personTvSeriesCreditsErrorResponse;
    private string tvEpisodeCreditsErrorResponse;
    private string tvSeriesAggregateCreditsErrorResponse;
    private string watchProvidersErrorResponse1;
    private string watchProvidersErrorResponse2;
    private string watchProvidersErrorResponse3;

    public TmdbHttpClientTests(ITestOutputHelper output)
    {
        // Arrange

        string malformedDataFilename = "TmdbHttpClientMalformedResponse.json";
        string configCountriesErrorFilename1 = "TmdbHttpClientConfigurationCountriesErrorResponse1.json";
        string configCountriesErrorFilename2 = "TmdbHttpClientConfigurationCountriesErrorResponse2.json";
        string configLanguagesErrorFilename1 = "TmdbHttpClientConfigurationLanguagesErrorResponse1.json";
        string configLanguagesErrorFilename2 = "TmdbHttpClientConfigurationLanguagesErrorResponse2.json";
        string genresErrorFilename1 = "TmdbHttpClientGenresErrorResponse1.json";
        string genresErrorFilename2 = "TmdbHttpClientGenresErrorResponse2.json";
        string findByImdbIdErrorFilename = "TmdbHttpClientFindByImdbIdErrorResponse1.json";
        string movieCreditsErrorFilename = "TmdbHttpClientMovieCreditsErrorResponse1.json";
        string personImagesErrorFilename = "TmdbHttpClientPersonImagesErrorResponse1.json";
        string personMovieCreditsErrorFilename = "TmdbHttpClientPersonMovieCreditsErrorResponse1.json";
        string personTvSeriesCreditsErrorFilename = "TmdbHttpClientPersonTvSeriesCreditsErrorResponse1.json";
        string tvEpisodeCreditsErrorFilename = "TmdbHttpClientTvEpisodeCreditsErrorResponse1.json";
        string tvSeriesAggregateCreditsErrorFilename = "TmdbHttpClientTvSeriesAggregateCreditsErrorResponse1.json";
        string watchProvidersErrorFilename1 = "TmdbHttpClientWatchProvidersErrorResponse1.json";
        string watchProvidersErrorFilename2 = "TmdbHttpClientWatchProvidersErrorResponse1.json";
        string watchProvidersErrorFilename3 = "TmdbHttpClientWatchProvidersErrorResponse1.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{malformedDataFilename}"))
        {
            malformedResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(malformedResponse))
        {
            throw new ArgumentException($"{malformedDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{configCountriesErrorFilename1}"))
        {
            configCountriesErrorResponse1 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(configCountriesErrorResponse1))
        {
            throw new ArgumentException($"{configCountriesErrorFilename1} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{configCountriesErrorFilename2}"))
        {
            configCountriesErrorResponse2 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(configCountriesErrorResponse2))
        {
            throw new ArgumentException($"{configCountriesErrorFilename2} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{configLanguagesErrorFilename1}"))
        {
            configLanguagesErrorResponse1 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(configLanguagesErrorResponse1))
        {
            throw new ArgumentException($"{configLanguagesErrorFilename1} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{configLanguagesErrorFilename2}"))
        {
            configLanguagesErrorResponse2 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(configLanguagesErrorResponse2))
        {
            throw new ArgumentException($"{configLanguagesErrorFilename2} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{genresErrorFilename1}"))
        {
            genresErrorResponse1 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(genresErrorResponse1))
        {
            throw new ArgumentException($"{genresErrorFilename1} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{genresErrorFilename2}"))
        {
            genresErrorResponse2 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(genresErrorResponse2))
        {
            throw new ArgumentException($"{genresErrorFilename2} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{findByImdbIdErrorFilename}"))
        {
            findByImdbIdErrorResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(findByImdbIdErrorResponse))
        {
            throw new ArgumentException($"{findByImdbIdErrorFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{movieCreditsErrorFilename}"))
        {
            movieCreditsErrorResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(movieCreditsErrorResponse))
        {
            throw new ArgumentException($"{movieCreditsErrorFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{personImagesErrorFilename}"))
        {
            personImagesErrorResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(personImagesErrorResponse))
        {
            throw new ArgumentException($"{personImagesErrorFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{personMovieCreditsErrorFilename}"))
        {
            personMovieCreditsErrorResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(personMovieCreditsErrorResponse))
        {
            throw new ArgumentException($"{personMovieCreditsErrorFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{personTvSeriesCreditsErrorFilename}"))
        {
            personTvSeriesCreditsErrorResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(personTvSeriesCreditsErrorResponse))
        {
            throw new ArgumentException($"{personTvSeriesCreditsErrorFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{tvEpisodeCreditsErrorFilename}"))
        {
            tvEpisodeCreditsErrorResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tvEpisodeCreditsErrorResponse))
        {
            throw new ArgumentException($"{tvEpisodeCreditsErrorFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{tvSeriesAggregateCreditsErrorFilename}"))
        {
            tvSeriesAggregateCreditsErrorResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tvSeriesAggregateCreditsErrorResponse))
        {
            throw new ArgumentException($"{tvSeriesAggregateCreditsErrorFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{watchProvidersErrorFilename1}"))
        {
            watchProvidersErrorResponse1 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(watchProvidersErrorResponse1))
        {
            throw new ArgumentException($"{watchProvidersErrorFilename1} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{watchProvidersErrorFilename2}"))
        {
            watchProvidersErrorResponse2 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(watchProvidersErrorResponse2))
        {
            throw new ArgumentException($"{watchProvidersErrorFilename2} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{watchProvidersErrorFilename3}"))
        {
            watchProvidersErrorResponse3 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(watchProvidersErrorResponse3))
        {
            throw new ArgumentException($"{watchProvidersErrorFilename3} is not valid test data.");
        }
    }

    [Theory]
    [InlineData("invalid json")]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData("")]
    [InlineData("   ")]
    public void GetModelFromResponse_InvalidJsonStrings_ThrowsJsonException(string invalidJson)
    {
        // Act & Assert
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetConfigurationCountriesModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetConfigurationLanguagesModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetGenresModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetIdModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetMovieModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetMovieCreditsModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetMovieExternalIdsModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetPersonModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetPersonExternalIdsModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetPersonImagesModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetPersonMovieCreditsModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetPersonTvSeriesCreditsModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetTvEpisodeModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetTvEpisodeCreditsModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetTvEpisodeExternalIdsModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetTvSeasonModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetTvSeriesModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetTvSeriesAggregateCreditsModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetTvSeriesExternalIdsModelFromResponse(invalidJson));
        Assert.Throws<System.Text.Json.JsonException>(() => TmdbHttpClient.GetWatchProvidersModelFromResponse(invalidJson));
    }

    [Fact]
    public async Task GetModels_InvalidIds_ReturnsNullResponses()
    {
        // Arrange
        Mock<HttpMessageHandler> httpMessageHandlerMock;
        HttpClient httpClient;
        IMemoryCache? memoryCache = GetMemoryCache();
        Assert.NotNull(memoryCache);
        TmdbHttpClient tmdbHttpClient;

        // Arrange & Act
        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbIdResponseDataModel? idResponse = await tmdbHttpClient.GetFindByImdbIdResults("");

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbMovieResponseDataModel? movieResponse = await tmdbHttpClient.GetMovie(-1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbMovieCreditsResponseDataModel? movieCreditsResponse = await tmdbHttpClient.GetMovieCredits(-1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbMovieExternalIdsResponseDataModel? movieExternalIdsResponse = await tmdbHttpClient.GetMovieExternalIds(-1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbPersonResponseDataModel? personResponse = await tmdbHttpClient.GetPerson(-1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbPersonExternalIdsResponseDataModel? personExternalIdsResponse = await tmdbHttpClient.GetPersonExternalIds(-1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbPersonImagesResponseDataModel? personImagesResponse = await tmdbHttpClient.GetPersonImages(-1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbPersonMovieCreditsResponseDataModel? personMovieCreditsResponse = await tmdbHttpClient.GetPersonMovieCredits(-1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbPersonTvSeriesCreditsResponseDataModel? personTvSeriesCreditsResponse = await tmdbHttpClient.GetPersonTvSeriesCredits(-1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeResponseDataModel? tvEpisodeResponse1 = await tmdbHttpClient.GetTvEpisode(-1, 1, 1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeResponseDataModel? tvEpisodeResponse2 = await tmdbHttpClient.GetTvEpisode(1, -1, 1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeResponseDataModel? tvEpisodeResponse3 = await tmdbHttpClient.GetTvEpisode(1, 1, -1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeResponseDataModel? tvEpisodeResponse4 = await tmdbHttpClient.GetTvEpisode(-1, -1, 1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeResponseDataModel? tvEpisodeResponse5 = await tmdbHttpClient.GetTvEpisode(-1, 1, -1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeResponseDataModel? tvEpisodeResponse6 = await tmdbHttpClient.GetTvEpisode(1, -1, -1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeResponseDataModel? tvEpisodeResponse7 = await tmdbHttpClient.GetTvEpisode(-1, -1, -1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeCreditsResponseDataModel? tvEpisodeCreditsResponse1 = await tmdbHttpClient.GetTvEpisodeCredits(-1, 1, 1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeCreditsResponseDataModel? tvEpisodeCreditsResponse2 = await tmdbHttpClient.GetTvEpisodeCredits(1, -1, 1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeCreditsResponseDataModel? tvEpisodeCreditsResponse3 = await tmdbHttpClient.GetTvEpisodeCredits(1, 1, -1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeCreditsResponseDataModel? tvEpisodeCreditsResponse4 = await tmdbHttpClient.GetTvEpisodeCredits(-1, -1, 1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeCreditsResponseDataModel? tvEpisodeCreditsResponse5 = await tmdbHttpClient.GetTvEpisodeCredits(-1, 1, -1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeCreditsResponseDataModel? tvEpisodeCreditsResponse6 = await tmdbHttpClient.GetTvEpisodeCredits(1, -1, -1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeCreditsResponseDataModel? tvEpisodeCreditsResponse7 = await tmdbHttpClient.GetTvEpisodeCredits(-1, -1, -1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeExternalIdsResponseDataModel? tvEpisodeExternalIdsResponse1 = await tmdbHttpClient.GetTvEpisodeExternalIds(-1, 1 ,1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeExternalIdsResponseDataModel? tvEpisodeExternalIdsResponse2 = await tmdbHttpClient.GetTvEpisodeExternalIds(1, -1 ,1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeExternalIdsResponseDataModel? tvEpisodeExternalIdsResponse3 = await tmdbHttpClient.GetTvEpisodeExternalIds(1, 1 ,-1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeExternalIdsResponseDataModel? tvEpisodeExternalIdsResponse4 = await tmdbHttpClient.GetTvEpisodeExternalIds(-1, -1 ,1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeExternalIdsResponseDataModel? tvEpisodeExternalIdsResponse5 = await tmdbHttpClient.GetTvEpisodeExternalIds(-1, 1 ,-1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeExternalIdsResponseDataModel? tvEpisodeExternalIdsResponse6 = await tmdbHttpClient.GetTvEpisodeExternalIds(1, -1 ,-1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeExternalIdsResponseDataModel? tvEpisodeExternalIdsResponse7 = await tmdbHttpClient.GetTvEpisodeExternalIds(-1, -1 ,-1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvSeasonResponseDataModel? tvSeasonResponse1 = await tmdbHttpClient.GetTvSeason(-1, 1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvSeasonResponseDataModel? tvSeasonResponse2 = await tmdbHttpClient.GetTvSeason(1, -1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvSeasonResponseDataModel? tvSeasonResponse3 = await tmdbHttpClient.GetTvSeason(-1, -1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvSeriesResponseDataModel? tvSeriesResponse = await tmdbHttpClient.GetTvSeries(-1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvSeriesAggregateCreditsResponseDataModel? tvSeriesAggregateCreditsResponse = await tmdbHttpClient.GetTvSeriesAggregateCredits(-1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvSeriesExternalIdsResponseDataModel? tvSeriesExternalIdsResponse = await tmdbHttpClient.GetTvSeriesExternalIds(-1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbWatchProvidersResponseDataModel? movieWatchProvidersResponse = await tmdbHttpClient.GetMovieWatchProviders(-1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbWatchProvidersResponseDataModel? tvSeasonWatchProvidersResponse1 = await tmdbHttpClient.GetTvSeasonWatchProviders(-1, 1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbWatchProvidersResponseDataModel? tvSeasonWatchProvidersResponse2 = await tmdbHttpClient.GetTvSeasonWatchProviders(1, -1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbWatchProvidersResponseDataModel? tvSeasonWatchProvidersResponse3 = await tmdbHttpClient.GetTvSeasonWatchProviders(-1, -1);

        httpMessageHandlerMock = GetMockHttpMessageHandlerBadRequest();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbWatchProvidersResponseDataModel? tvSeriesWatchProvidersResponse = await tmdbHttpClient.GetTvSeriesWatchProviders(-1);

        // Assert
        Assert.Null(idResponse);
        Assert.Null(movieResponse);
        Assert.Null(movieCreditsResponse);
        Assert.Null(movieExternalIdsResponse);
        Assert.Null(personResponse);
        Assert.Null(personExternalIdsResponse);
        Assert.Null(personImagesResponse);
        Assert.Null(personMovieCreditsResponse);
        Assert.Null(personTvSeriesCreditsResponse);
        Assert.Null(tvEpisodeResponse1);
        Assert.Null(tvEpisodeResponse2);
        Assert.Null(tvEpisodeResponse3);
        Assert.Null(tvEpisodeResponse4);
        Assert.Null(tvEpisodeResponse5);
        Assert.Null(tvEpisodeResponse6);
        Assert.Null(tvEpisodeResponse7);
        Assert.Null(tvEpisodeCreditsResponse1);
        Assert.Null(tvEpisodeCreditsResponse2);
        Assert.Null(tvEpisodeCreditsResponse3);
        Assert.Null(tvEpisodeCreditsResponse4);
        Assert.Null(tvEpisodeCreditsResponse5);
        Assert.Null(tvEpisodeCreditsResponse6);
        Assert.Null(tvEpisodeCreditsResponse7);
        Assert.Null(tvEpisodeExternalIdsResponse1);
        Assert.Null(tvEpisodeExternalIdsResponse2);
        Assert.Null(tvEpisodeExternalIdsResponse3);
        Assert.Null(tvEpisodeExternalIdsResponse4);
        Assert.Null(tvEpisodeExternalIdsResponse5);
        Assert.Null(tvEpisodeExternalIdsResponse6);
        Assert.Null(tvEpisodeExternalIdsResponse7);
        Assert.Null(tvSeasonResponse1);
        Assert.Null(tvSeasonResponse2);
        Assert.Null(tvSeasonResponse3);
        Assert.Null(tvSeriesResponse);
        Assert.Null(tvSeriesAggregateCreditsResponse);
        Assert.Null(tvSeriesExternalIdsResponse);
        Assert.Null(movieWatchProvidersResponse);
        Assert.Null(tvSeasonWatchProvidersResponse1);
        Assert.Null(tvSeasonWatchProvidersResponse2);
        Assert.Null(tvSeasonWatchProvidersResponse3);
        Assert.Null(tvSeriesWatchProvidersResponse);
    }

    [Fact]
    public async Task GetMedia_HttpClientInternalServerError_ReturnsNull()
    {        
        // Arrange
        Mock<HttpMessageHandler> httpMessageHandlerMock;
        HttpClient httpClient;
        IMemoryCache? memoryCache = GetMemoryCache();
        Assert.NotNull(memoryCache);
        TmdbHttpClient tmdbHttpClient;

        // Arrange & Act
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        ConfigurationCountriesDictionary? configurationCountriesDictionary = await tmdbHttpClient.GetCountries();

        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        ConfigurationLanguagesDictionary? configurationLanguagesDictionary = await tmdbHttpClient.GetLanguages();
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbGenresResponseDataModel? movieGenresResponse = await tmdbHttpClient.GetMovieGenres();
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbGenresResponseDataModel? tvGenresResponse = await tmdbHttpClient.GetTvSeriesGenres();
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbIdResponseDataModel? idResponse = await tmdbHttpClient.GetFindByImdbIdResults("test");
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbMovieResponseDataModel? movieResponse = await tmdbHttpClient.GetMovie(1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbMovieCreditsResponseDataModel? movieCreditsResponse = await tmdbHttpClient.GetMovieCredits(1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbMovieExternalIdsResponseDataModel? movieExternalIdsResponse = await tmdbHttpClient.GetMovieExternalIds(1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbPersonResponseDataModel? personResponse = await tmdbHttpClient.GetPerson(1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbPersonExternalIdsResponseDataModel? personExternalIdsResponse = await tmdbHttpClient.GetPersonExternalIds(1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbPersonImagesResponseDataModel? personImagesResponse = await tmdbHttpClient.GetPersonImages(1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbPersonMovieCreditsResponseDataModel? personMovieCreditsResponse = await tmdbHttpClient.GetPersonMovieCredits(1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbPersonTvSeriesCreditsResponseDataModel? personTvSeriesCreditsResponse = await tmdbHttpClient.GetPersonTvSeriesCredits(1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeResponseDataModel? tvEpisodeResponse = await tmdbHttpClient.GetTvEpisode(1, 1, 1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeCreditsResponseDataModel? tvEpisodeCreditsResponse = await tmdbHttpClient.GetTvEpisodeCredits(1, 1, 1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvEpisodeExternalIdsResponseDataModel? tvEpisodeExternalIdsResponse = await tmdbHttpClient.GetTvEpisodeExternalIds(1, 1, 1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvSeasonResponseDataModel? tvSeasonResponse = await tmdbHttpClient.GetTvSeason(1, 1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvSeriesResponseDataModel? tvSeriesResponse = await tmdbHttpClient.GetTvSeries(1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvSeriesAggregateCreditsResponseDataModel? tvSeriesAggregateCreditsResponse = await tmdbHttpClient.GetTvSeriesAggregateCredits(1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbTvSeriesExternalIdsResponseDataModel? tvSeriesExternalIdsResponse = await tmdbHttpClient.GetTvSeriesExternalIds(1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbWatchProvidersResponseDataModel? movieWatchProvidersResponse = await tmdbHttpClient.GetMovieWatchProviders(1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbWatchProvidersResponseDataModel? tvSeasonWatchProvidersResponse = await tmdbHttpClient.GetTvSeasonWatchProviders(1, 1);
        
        httpMessageHandlerMock = GetMockHttpMessageHandlerInternalServerError();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);
        TmdbWatchProvidersResponseDataModel? tvSeriesWatchProvidersResponse = await tmdbHttpClient.GetTvSeriesWatchProviders(1);

        // Assert
        Assert.Null(configurationCountriesDictionary);
        Assert.Null(configurationLanguagesDictionary);
        Assert.Null(movieGenresResponse);
        Assert.Null(tvGenresResponse);
        Assert.Null(idResponse);
        Assert.Null(movieResponse);
        Assert.Null(movieCreditsResponse);
        Assert.Null(movieExternalIdsResponse);
        Assert.Null(personResponse);
        Assert.Null(personExternalIdsResponse);
        Assert.Null(personImagesResponse);
        Assert.Null(personMovieCreditsResponse);
        Assert.Null(personTvSeriesCreditsResponse);
        Assert.Null(tvEpisodeResponse);
        Assert.Null(tvEpisodeCreditsResponse);
        Assert.Null(tvEpisodeExternalIdsResponse);
        Assert.Null(tvSeasonResponse);
        Assert.Null(tvSeriesResponse);
        Assert.Null(tvSeriesAggregateCreditsResponse);
        Assert.Null(tvSeriesExternalIdsResponse);
        Assert.Null(movieWatchProvidersResponse);
        Assert.Null(tvSeasonWatchProvidersResponse);
        Assert.Null(tvSeriesWatchProvidersResponse);
    }

    [Fact]
    public async Task GetMedia_HttpClientTimeout_ThrowsException()
    {
        // Arrange
        Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("Request timed out"));

        HttpClient httpClient = new HttpClient(httpMessageHandlerMock.Object);
        IMemoryCache? memoryCache = GetMemoryCache();
        Assert.NotNull(memoryCache);
        TmdbHttpClient tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);

        // Act & Assert

        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetCountries());
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetLanguages());
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetMovieGenres());
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetTvSeriesGenres());
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetFindByImdbIdResults("test"));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetMovie(1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetMovieCredits(1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetMovieExternalIds(1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetPerson(1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetPersonExternalIds(1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetPersonImages(1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetPersonMovieCredits(1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetPersonTvSeriesCredits(1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetTvEpisode(1, 1, 1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetTvEpisodeCredits(1, 1, 1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetTvEpisodeExternalIds(1, 1, 1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetTvSeason(1, 1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetTvSeries(1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetTvSeriesAggregateCredits(1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetTvSeriesExternalIds(1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetMovieWatchProviders(1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetTvSeasonWatchProviders(1, 1));
        await Assert.ThrowsAsync<TaskCanceledException>(() => tmdbHttpClient.GetTvSeriesWatchProviders(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\n\t")]
    [InlineData("very long query that might cause issues with the API endpoint and should still be handled properly")]
    public async Task GetMedia_EdgeCaseImdbIds_ReturnEmptyResults(string query)
    {
        // Arrange
        Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(findByImdbIdErrorResponse) // Empty response
            });

        HttpClient httpClient = new HttpClient(httpMessageHandlerMock.Object);
        IMemoryCache? memoryCache = GetMemoryCache();
        Assert.NotNull(memoryCache);
        TmdbHttpClient tmdbHttpClient = new TmdbHttpClient(httpClient, memoryCache);

        // Act
        TmdbIdResponseDataModel? result = await tmdbHttpClient.GetFindByImdbIdResults(query);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.MovieResults);
        Assert.Empty(result.MovieResults);
        Assert.NotNull(result.PersonResults);
        Assert.Empty(result.PersonResults);
        Assert.NotNull(result.TvResults);
        Assert.Empty(result.TvResults);
        Assert.NotNull(result.TvEpisodeResults);
        Assert.Empty(result.TvEpisodeResults);
        Assert.NotNull(result.TvSeasonResults);
        Assert.Empty(result.TvSeasonResults);
    }

    private Mock<HttpMessageHandler> GetMockHttpMessageHandlerInternalServerError()
    {
        Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Internal Server Error")
            });
        
        return httpMessageHandlerMock;
    }

    private Mock<HttpMessageHandler> GetMockHttpMessageHandlerBadRequest()
    {
        Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Invalid response")
            });
        
        return httpMessageHandlerMock;
    }

    private IMemoryCache? GetMemoryCache()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddMemoryCache();
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetService<IMemoryCache>();
    }
}
