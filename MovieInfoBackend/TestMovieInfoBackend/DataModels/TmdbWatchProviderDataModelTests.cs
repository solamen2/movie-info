using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class TmdbWatchProviderDataModelTests
{
    private string tmdbHttpClientMovieWatchProvidersResponse;
    private string tmdbHttpClientTvSeasonWatchProvidersResponse;
    private string tmdbHttpClientTvSeriesWatchProvidersResponse;

    public TmdbWatchProviderDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testMovieWatchProvidersDataFilename = "TmdbHttpClientMovieWatchProvidersResponse.json";
        string testTvSeasonWatchProvidersDataFilename = "TmdbHttpClientTvSeasonWatchProvidersResponse.json";
        string testTvSeriesWatchProvidersDataFilename = "TmdbHttpClientTvSeriesWatchProvidersResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieWatchProvidersDataFilename}"))
        {
            tmdbHttpClientMovieWatchProvidersResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieWatchProvidersResponse))
        {
            throw new ArgumentException($"{testMovieWatchProvidersDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeasonWatchProvidersDataFilename}"))
        {
            tmdbHttpClientTvSeasonWatchProvidersResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeasonWatchProvidersResponse))
        {
            throw new ArgumentException($"{testTvSeasonWatchProvidersDataFilename} is not valid test data.");
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
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {
        // Assert
        
        TmdbWatchProvidersResponseDataModel? movieWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientMovieWatchProvidersResponse);
        TmdbWatchProvidersResponseDataModel? tvSeasonWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientTvSeasonWatchProvidersResponse);
        TmdbWatchProvidersResponseDataModel? tvSeriesWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientTvSeriesWatchProvidersResponse);

        // Act

        // Movie watch providers
        Assert.NotNull(movieWatchProvidersResponse);
        Assert.True(movieWatchProvidersResponse.Id > 0, "Movie watch providers Id must be a positive number");
        Assert.NotNull(movieWatchProvidersResponse.Results);
        TmdbWatchProviderCountryDataModel? movieUsResults = movieWatchProvidersResponse.Results.US;
        Assert.NotNull(movieUsResults);
        Assert.False(String.IsNullOrWhiteSpace(movieUsResults.Link), "Movie US results Link must not be empty");
        Assert.NotNull(movieUsResults.Flatrate);
        Assert.NotEmpty(movieUsResults.Flatrate);
        TmdbWatchProviderDataModel movieUsResultsFirstFlatRate = movieUsResults.Flatrate[0];
        Assert.True(movieUsResultsFirstFlatRate.ProviderId > 0, "Movie US results first flat rate watch provider ProviderId must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(movieUsResultsFirstFlatRate.ProviderName), "Movie US results first flat rate watch provider ProviderName must not be empty");
        Assert.True(movieUsResultsFirstFlatRate.DisplayPriority >= 0, "Movie US results first flat rate watch provider DisplayPriority must not be a negative number");
        Assert.NotNull(movieUsResults.Buy);
        Assert.NotEmpty(movieUsResults.Buy);
        TmdbWatchProviderDataModel movieUsResultsFirstBuy = movieUsResults.Buy[0];
        Assert.True(movieUsResultsFirstBuy.ProviderId > 0, "Movie US results first buy watch provider ProviderId must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(movieUsResultsFirstBuy.ProviderName), "Movie US results first buy watch provider ProviderName must not be empty");
        Assert.True(movieUsResultsFirstBuy.DisplayPriority >= 0, "Movie US results first buy watch provider DisplayPriority must not be a negative number");
        Assert.NotNull(movieUsResults.Rent);
        Assert.NotEmpty(movieUsResults.Rent);
        TmdbWatchProviderDataModel movieUsResultsFirstRent = movieUsResults.Rent[0];
        Assert.True(movieUsResultsFirstRent.ProviderId > 0, "Movie US results first rent watch provider ProviderId must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(movieUsResultsFirstRent.ProviderName), "Movie US results first rent watch provider ProviderName must not be empty");
        Assert.True(movieUsResultsFirstRent.DisplayPriority >= 0, "Movie US results first rent watch provider DisplayPriority must not be a negative number");

        // TV season watch providers
        Assert.NotNull(tvSeasonWatchProvidersResponse);
        Assert.True(tvSeasonWatchProvidersResponse.Id > 0, "TV season watch providers Id must be a positive number");
        Assert.NotNull(tvSeasonWatchProvidersResponse.Results);
        TmdbWatchProviderCountryDataModel? tvSeasonUsResults = tvSeasonWatchProvidersResponse.Results.US;
        Assert.NotNull(tvSeasonUsResults);
        Assert.False(String.IsNullOrWhiteSpace(tvSeasonUsResults.Link), "TV season US results Link must not be empty");
        Assert.NotNull(tvSeasonUsResults.Flatrate);
        Assert.NotEmpty(tvSeasonUsResults.Flatrate);
        TmdbWatchProviderDataModel tvSeasonUsResultsFirstFlatRate = tvSeasonUsResults.Flatrate[0];
        Assert.True(tvSeasonUsResultsFirstFlatRate.ProviderId > 0, "TV season US results first flat rate watch provider ProviderId must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(tvSeasonUsResultsFirstFlatRate.ProviderName), "TV season US results first flat rate watch provider ProviderName must not be empty");
        Assert.True(tvSeasonUsResultsFirstFlatRate.DisplayPriority >= 0, "TV season US results first flat rate watch provider DisplayPriority must not be a negative number");
        Assert.NotNull(tvSeasonUsResults.Buy);
        Assert.NotEmpty(tvSeasonUsResults.Buy);
        TmdbWatchProviderDataModel tvSeasonUsResultsFirstBuy = tvSeasonUsResults.Buy[0];
        Assert.True(tvSeasonUsResultsFirstBuy.ProviderId > 0, "TV season US results first buy watch provider ProviderId must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(tvSeasonUsResultsFirstBuy.ProviderName), "TV season US results first buy watch provider ProviderName must not be empty");
        Assert.True(tvSeasonUsResultsFirstBuy.DisplayPriority >= 0, "TV season US results first buy watch provider DisplayPriority must not be a negative number");
        // No TV season US rent watch providers, so skipping that

        // TV series watch providers
        Assert.NotNull(tvSeriesWatchProvidersResponse);
        Assert.True(tvSeriesWatchProvidersResponse.Id > 0, "TV series watch providers Id must be a positive number");
        Assert.NotNull(tvSeriesWatchProvidersResponse.Results);
        TmdbWatchProviderCountryDataModel? tvSeriesUsResults = tvSeriesWatchProvidersResponse.Results.US;
        Assert.NotNull(tvSeriesUsResults);
        Assert.False(String.IsNullOrWhiteSpace(tvSeriesUsResults.Link), "TV series US results Link must not be empty");
        Assert.NotNull(tvSeriesUsResults.Flatrate);
        Assert.NotEmpty(tvSeriesUsResults.Flatrate);
        TmdbWatchProviderDataModel tvSeriesUsResultsFirstFlatRate = tvSeriesUsResults.Flatrate[0];
        Assert.True(tvSeriesUsResultsFirstFlatRate.ProviderId > 0, "TV series US results first flat rate watch provider ProviderId must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(tvSeriesUsResultsFirstFlatRate.ProviderName), "TV series US results first flat rate watch provider ProviderName must not be empty");
        Assert.True(tvSeriesUsResultsFirstFlatRate.DisplayPriority >= 0, "TV series US results first flat rate watch provider DisplayPriority must not be a negative number");
        Assert.NotNull(tvSeriesUsResults.Buy);
        Assert.NotEmpty(tvSeriesUsResults.Buy);
        TmdbWatchProviderDataModel tvSeriesUsResultsFirstBuy = tvSeriesUsResults.Buy[0];
        Assert.True(tvSeriesUsResultsFirstBuy.ProviderId > 0, "TV series US results first buy watch provider ProviderId must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(tvSeriesUsResultsFirstBuy.ProviderName), "TV series US results first buy watch provider ProviderName must not be empty");
        Assert.True(tvSeriesUsResultsFirstBuy.DisplayPriority >= 0, "TV series US results first buy watch provider DisplayPriority must not be a negative number");
        // No TV series US rent watch providers, so skipping here
    }

    [Fact]
    public void TmdbWatchProvidersDataModel_ValidModelToString_ReturnsCorrectValue()
    {
        // Assert
        
        TmdbWatchProvidersResponseDataModel? movieWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientMovieWatchProvidersResponse);
        TmdbWatchProvidersResponseDataModel? tvSeasonWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientTvSeasonWatchProvidersResponse);
        TmdbWatchProvidersResponseDataModel? tvSeriesWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientTvSeriesWatchProvidersResponse);

        // Act

        Assert.NotNull(movieWatchProvidersResponse);
        Assert.NotNull(tvSeasonWatchProvidersResponse);
        Assert.NotNull(tvSeriesWatchProvidersResponse);
        Assert.Equal(
            "Id: 278\nResults:\n*****\nUS:\n*****\nLink: https://www.themoviedb.org/movie/278-example-movie/watch?locale=US\nFlatrate:\n*****\nLogoPath: /gAGrSQCTAisxy2CsWbijVvJEnRo.jpg\nProviderId: 635\nProviderName: AMC+ Roku Premium Channel\nDisplayPriority: 28\n\nLogoPath: /x9zOHTUkQzt3PgPVKbMH9CKBwLK.jpg\nProviderId: 2528\nProviderName: YouTube TV\nDisplayPriority: 36\n*****\nBuy:\n*****\nLogoPath: /qR6FKvnPBx2O37FDg8PNM7efwF3.jpg\nProviderId: 10\nProviderName: Amazon Video\nDisplayPriority: 7\n\nLogoPath: /SPnB1qiCkYfirS2it3hZORwGVn.jpg\nProviderId: 2\nProviderName: Apple TV Store\nDisplayPriority: 8\n\nLogoPath: /8z7rC8uIDaTM91X0ZfkRf04ydj2.jpg\nProviderId: 3\nProviderName: Google Play Movies\nDisplayPriority: 17\n\nLogoPath: /pTnn5JwWr4p3pG8H6VrpiQo7Vs0.jpg\nProviderId: 192\nProviderName: YouTube\nDisplayPriority: 18\n\nLogoPath: /19fkcOz0xeUgCVW8tO85uOYnYK9.jpg\nProviderId: 7\nProviderName: Fandango At Home\nDisplayPriority: 35\n*****\nRent:\n*****\nLogoPath: /qR6FKvnPBx2O37FDg8PNM7efwF3.jpg\nProviderId: 10\nProviderName: Amazon Video\nDisplayPriority: 7\n\nLogoPath: /SPnB1qiCkYfirS2it3hZORwGVn.jpg\nProviderId: 2\nProviderName: Apple TV Store\nDisplayPriority: 8\n\nLogoPath: /8z7rC8uIDaTM91X0ZfkRf04ydj2.jpg\nProviderId: 3\nProviderName: Google Play Movies\nDisplayPriority: 17\n\nLogoPath: /pTnn5JwWr4p3pG8H6VrpiQo7Vs0.jpg\nProviderId: 192\nProviderName: YouTube\nDisplayPriority: 18\n\nLogoPath: /19fkcOz0xeUgCVW8tO85uOYnYK9.jpg\nProviderId: 7\nProviderName: Fandango At Home\nDisplayPriority: 35\n\nLogoPath: /aAb9CUHjFe9Y3O57qnrJH0KOF1B.jpg\nProviderId: 486\nProviderName: Spectrum On Demand\nDisplayPriority: 121\n\nLogoPath: /vLZKlXUNDcZR7ilvfY9Wr9k80FZ.jpg\nProviderId: 538\nProviderName: Plex\nDisplayPriority: 126"
                , movieWatchProvidersResponse.ToString());
        Assert.Equal(
            "Id: 59470\nResults:\n*****\nUS:\n*****\nLink: https://www.themoviedb.org/tv/95-example-tv-series/watch?locale=US\nFlatrate:\n*****\nLogoPath: /bxBlRPEPpMVDc4jMhSrTf2339DW.jpg\nProviderId: 15\nProviderName: Hulu\nDisplayPriority: 6\n*****\nBuy:\n*****\nLogoPath: /qR6FKvnPBx2O37FDg8PNM7efwF3.jpg\nProviderId: 10\nProviderName: Amazon Video\nDisplayPriority: 7\n\nLogoPath: /SPnB1qiCkYfirS2it3hZORwGVn.jpg\nProviderId: 2\nProviderName: Apple TV Store\nDisplayPriority: 8\n\nLogoPath: /8z7rC8uIDaTM91X0ZfkRf04ydj2.jpg\nProviderId: 3\nProviderName: Google Play Movies\nDisplayPriority: 17\n\nLogoPath: /19fkcOz0xeUgCVW8tO85uOYnYK9.jpg\nProviderId: 7\nProviderName: Fandango At Home\nDisplayPriority: 35\n*****\nRent: []"
                , tvSeasonWatchProvidersResponse.ToString());
        Assert.Equal(
            "Id: 95\nResults:\n*****\nUS:\n*****\nLink: https://www.themoviedb.org/tv/95-example-tv-series/watch?locale=US\nFlatrate:\n*****\nLogoPath: /bxBlRPEPpMVDc4jMhSrTf2339DW.jpg\nProviderId: 15\nProviderName: Hulu\nDisplayPriority: 6\n*****\nBuy:\n*****\nLogoPath: /qR6FKvnPBx2O37FDg8PNM7efwF3.jpg\nProviderId: 10\nProviderName: Amazon Video\nDisplayPriority: 7\n\nLogoPath: /SPnB1qiCkYfirS2it3hZORwGVn.jpg\nProviderId: 2\nProviderName: Apple TV Store\nDisplayPriority: 8\n\nLogoPath: /8z7rC8uIDaTM91X0ZfkRf04ydj2.jpg\nProviderId: 3\nProviderName: Google Play Movies\nDisplayPriority: 17\n\nLogoPath: /19fkcOz0xeUgCVW8tO85uOYnYK9.jpg\nProviderId: 7\nProviderName: Fandango At Home\nDisplayPriority: 35\n*****\nRent: []"
                , tvSeriesWatchProvidersResponse.ToString());
    }
}