using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.DataModels;

public class MovieViewModelTests
{
    private string suggestionHttpClientResponse1;
    private string omdbHttpClientMovieResponse;
    private string tmdbHttpClientMovieResponse;
    private string tmdbHttpClientMovieCreditsResponse;
    private string tmdbHttpClientMovieWatchProvidersResponse;
    private string tmdbHttpClientConfigurationCountriesResponse;
    private string tmdbHttpClientConfigurationLanguagesResponse;

    public MovieViewModelTests(ITestOutputHelper output)
    {   
        // Arrange

        string testSuggestionDataFilename1 = "MovieHttpClientResponse1.json";
        string testOmdbMovieDataFilename = "OmdbHttpClientMovieResponse.json";
        string testTmdbMovieDataFilename = "TmdbHttpClientMovieResponse.json";
        string testMovieCreditsDataFilename = "TmdbHttpClientMovieCreditsResponse.json";
        string testMovieWatchProvidersDataFilename = "TmdbHttpClientMovieWatchProvidersResponse.json";
        string testCountriesDataFilename = "TmdbHttpClientConfigurationCountriesResponse.json";
        string testLanguagesDataFilename = "TmdbHttpClientConfigurationLanguagesResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testSuggestionDataFilename1}"))
        {
            suggestionHttpClientResponse1 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(suggestionHttpClientResponse1))
        {
            throw new ArgumentException($"{testSuggestionDataFilename1} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testOmdbMovieDataFilename}"))
        {
            omdbHttpClientMovieResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(omdbHttpClientMovieResponse))
        {
            throw new ArgumentException($"{testOmdbMovieDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbMovieDataFilename}"))
        {
            tmdbHttpClientMovieResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieResponse))
        {
            throw new ArgumentException($"{testTmdbMovieDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieCreditsDataFilename}"))
        {
            tmdbHttpClientMovieCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieCreditsResponse))
        {
            throw new ArgumentException($"{testMovieCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieWatchProvidersDataFilename}"))
        {
            tmdbHttpClientMovieWatchProvidersResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieWatchProvidersResponse))
        {
            throw new ArgumentException($"{testMovieWatchProvidersDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testCountriesDataFilename}"))
        {
            tmdbHttpClientConfigurationCountriesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientConfigurationCountriesResponse))
        {
            throw new ArgumentException($"{testCountriesDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testLanguagesDataFilename}"))
        {
            tmdbHttpClientConfigurationLanguagesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientConfigurationLanguagesResponse))
        {
            throw new ArgumentException($"{testLanguagesDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetViewModelFromDataModel_ValidDataModel_ReturnsValidViewModel()
    {
        // Arrange (continued)
        
        MovieSuggestionsResponseDataModel? actual1 = MovieHttpClient.GetModelFromResponse(suggestionHttpClientResponse1);
        SuggestionDataModel[]? suggestions1 = actual1?.Suggestions;
        Assert.Equal(8, suggestions1?.Length);
        SuggestionDataModel? suggestionMovieDataModel = suggestions1?[1];  // Movie suggestion = index 1
        Assert.NotNull(suggestionMovieDataModel);
        SuggestionViewModel suggestionMovieViewModel = new(suggestionMovieDataModel);

        OmdbResponseDataModel? omdbMovieResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientMovieResponse);
        Assert.NotNull(omdbMovieResponse);

        TmdbMovieResponseDataModel? tmdbMovieResponse = TmdbHttpClient.GetMovieModelFromResponse(tmdbHttpClientMovieResponse);
        Assert.NotNull(tmdbMovieResponse);

        TmdbMovieCreditsResponseDataModel? movieCreditsResponse = TmdbHttpClient.GetMovieCreditsModelFromResponse(tmdbHttpClientMovieCreditsResponse);
        Assert.NotNull(movieCreditsResponse);

        TmdbWatchProvidersResponseDataModel? movieWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientMovieWatchProvidersResponse);
        Assert.NotNull(movieWatchProvidersResponse);

        TmdbConfigurationCountriesResponseDataModel? tvConfigurationCountriesResponse = TmdbHttpClient.GetConfigurationCountriesModelFromResponse(tmdbHttpClientConfigurationCountriesResponse);
        Assert.NotNull(tvConfigurationCountriesResponse);
        TmdbConfigurationCountriesResponseDataModel.ConfigurationCountriesDictionary? tvConfigurationCountriesDictionary = tvConfigurationCountriesResponse.GetConfigurationCountriesDictionary();
        Assert.NotNull(tvConfigurationCountriesDictionary);

        TmdbConfigurationLanguagesResponseDataModel? tvConfigurationLanguagesResponse = TmdbHttpClient.GetConfigurationLanguagesModelFromResponse(tmdbHttpClientConfigurationLanguagesResponse);
        Assert.NotNull(tvConfigurationLanguagesResponse);
        TmdbConfigurationLanguagesResponseDataModel.ConfigurationLanguagesDictionary? tvConfigurationLanguagesDictionary = tvConfigurationLanguagesResponse.GetConfigurationLanguagesDictionary();
        Assert.NotNull(tvConfigurationLanguagesDictionary);

        // Act

        MovieViewModel movieViewModel = new(suggestionMovieViewModel, 
                                            omdbMovieResponse,
                                            tmdbMovieResponse,
                                            movieCreditsResponse,
                                            movieWatchProvidersResponse,
                                            tvConfigurationCountriesDictionary,
                                            tvConfigurationLanguagesDictionary);
        
        // Assert
        
        Assert.NotNull(movieViewModel);

        // NOTE: Only checking fields which are transformed by the view model. Other fields have been covered by data model tests or ToString() test below

        Assert.Equal(28767189, movieViewModel.BoxOfficeNumber);
        Assert.Equal("Drama, Crime", movieViewModel.TmdbGenres);
        Assert.Equal("United States of America, Japan", movieViewModel.OriginCountries);
        Assert.Equal("English", movieViewModel.OriginLanguage);
        Assert.Equal("Castle Rock Entertainment; Nippon Herald Films", movieViewModel.ProductionCompanies);
        Assert.Equal("United States of America, Japan", movieViewModel.ProductionCountries);
        Assert.Equal("English, Japanese", movieViewModel.SpokenLanguages);
        Assert.Equal("Example Director", movieViewModel.Directors[0].Name);
        Assert.Equal("Example Director", movieViewModel.Writers[0].Name);
        Assert.Equal("Example Writer", movieViewModel.Writers[1].Name);
        Assert.Equal("Niki Marvin", movieViewModel.Producers[0].Name);
        Assert.Equal("David V. Lester", movieViewModel.Producers[1].Name);
        Assert.Equal("Liz Glotzer", movieViewModel.Producers[2].Name);
        Assert.Equal("Melissa Taylor", movieViewModel.Producers[3].Name);
        Assert.Equal("Dan Goldwasser", movieViewModel.Producers[4].Name);
    }

    [Fact]
    public void MovieViewModel_EmptyDataFields_ReturnEmptyValues()
    {
        // Arrange (continued)

        MovieSuggestionsResponseDataModel? actual1 = MovieHttpClient.GetModelFromResponse(suggestionHttpClientResponse1);
        SuggestionDataModel[]? suggestions1 = actual1?.Suggestions;
        Assert.Equal(8, suggestions1?.Length);
        SuggestionDataModel? suggestionMovieDataModel = suggestions1?[1];  // Movie suggestion = index 1
        Assert.NotNull(suggestionMovieDataModel);
        SuggestionViewModel suggestionMovieViewModel = new(suggestionMovieDataModel);

        TmdbMovieResponseDataModel? tmdbMovieResponse = TmdbHttpClient.GetMovieModelFromResponse(tmdbHttpClientMovieResponse);
        Assert.NotNull(tmdbMovieResponse);

        TmdbMovieCreditsResponseDataModel? movieCreditsResponse = TmdbHttpClient.GetMovieCreditsModelFromResponse(tmdbHttpClientMovieCreditsResponse);
        Assert.NotNull(movieCreditsResponse);

        TmdbWatchProvidersResponseDataModel? movieWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientMovieWatchProvidersResponse);
        Assert.NotNull(movieWatchProvidersResponse);

        TmdbConfigurationCountriesResponseDataModel? tvConfigurationCountriesResponse = TmdbHttpClient.GetConfigurationCountriesModelFromResponse(tmdbHttpClientConfigurationCountriesResponse);
        Assert.NotNull(tvConfigurationCountriesResponse);
        TmdbConfigurationCountriesResponseDataModel.ConfigurationCountriesDictionary? tvConfigurationCountriesDictionary = tvConfigurationCountriesResponse.GetConfigurationCountriesDictionary();
        Assert.NotNull(tvConfigurationCountriesDictionary);

        TmdbConfigurationLanguagesResponseDataModel? tvConfigurationLanguagesResponse = TmdbHttpClient.GetConfigurationLanguagesModelFromResponse(tmdbHttpClientConfigurationLanguagesResponse);
        Assert.NotNull(tvConfigurationLanguagesResponse);
        TmdbConfigurationLanguagesResponseDataModel.ConfigurationLanguagesDictionary? tvConfigurationLanguagesDictionary = tvConfigurationLanguagesResponse.GetConfigurationLanguagesDictionary();
        Assert.NotNull(tvConfigurationLanguagesDictionary);

        var omdbMovieResponseDataModel = new OmdbResponseDataModel
        {
            Title = "Example Movie 2",
            Year = "2013",
            Rated = "Not Rated",
            Released = "01 Mar 2013",
            Runtime = "90 min",
            Genre = "Family",
            Director = "David DeCoteau",
            Writer = "Sebastian Dinwiddie",
            Actors = "Alison Sieke, August Roads, Chris Petrovski",
            Plot = "This is a very silly movie.",
            Language = "English",
            Country = "United States",
            Awards = "N/A",
            Poster = "https://m.media-amazon.com/images/M/MV5BMTQ3MzY4OTE5N15BMl5BanBnXkFtZTcwMDg0OTgxOQ@@._V1_SX300.jpg",
            Ratings = new OmdbRatingDataModel[]
            {
                new OmdbRatingDataModel
                {
                    Source = "Internet Movie Database",
                    Value = "2.4/10"
                }
            },
            Metascore = "N/A",
            ImdbRating = "2.4",
            ImdbVotes = "211",
            ImdbId = "tt0000002",
            Type = "movie",
            DVD = "N/A",
            BoxOffice = "N/A",  // Testing this value
            Production = "N/A",
            Website = "N/A",
            Response = "True"
        };

        var omdbMovieResponseDataModel2 = new OmdbResponseDataModel
        {
            Title = "Example Movie 3",
            Year = "2013",
            Rated = "Not Rated",
            Released = "01 Mar 2013",
            Runtime = "90 min",
            Genre = "Family",
            Director = "David DeCoteau",
            Writer = "Sebastian Dinwiddie",
            Actors = "Alison Sieke, August Roads, Chris Petrovski",
            Plot = "This is another very silly movie.",
            Language = "English",
            Country = "United States",
            Awards = "N/A",
            Poster = "https://m.media-amazon.com/images/M/MV5BMTQ3MzY4OTE5N15BMl5BanBnXkFtZTcwMDg0OTgxOQ@@._V1_SX300.jpg",
            Ratings = new OmdbRatingDataModel[]
            {
                new OmdbRatingDataModel
                {
                    Source = "Internet Movie Database",
                    Value = "2.4/10"
                }
            },
            Metascore = "N/A",
            ImdbRating = "2.4",
            ImdbVotes = "211",
            ImdbId = "tt0000003",
            Type = "movie",
            DVD = "N/A",
            BoxOffice = "",  // Testing this value
            Production = "N/A",
            Website = "N/A",
            Response = "True"
        };

        // Act

        MovieViewModel movieViewModel = new(suggestionMovieViewModel, 
                                            omdbMovieResponseDataModel,
                                            tmdbMovieResponse,
                                            movieCreditsResponse,
                                            movieWatchProvidersResponse,
                                            tvConfigurationCountriesDictionary,
                                            tvConfigurationLanguagesDictionary);

        Assert.Equal(0, movieViewModel.BoxOfficeNumber);

        MovieViewModel movieViewModel2 = new(suggestionMovieViewModel, 
                                            omdbMovieResponseDataModel2,
                                            tmdbMovieResponse,
                                            movieCreditsResponse,
                                            movieWatchProvidersResponse,
                                            tvConfigurationCountriesDictionary,
                                            tvConfigurationLanguagesDictionary);

        Assert.Equal(0, movieViewModel2.BoxOfficeNumber);
    }

    [Fact]
    public void MovieViewModel_ValidModelToString_ReturnsCorrectValue()
    {
        // Arrange (continued)
        
        MovieSuggestionsResponseDataModel? actual1 = MovieHttpClient.GetModelFromResponse(suggestionHttpClientResponse1);
        SuggestionDataModel[]? suggestions1 = actual1?.Suggestions;
        Assert.Equal(8, suggestions1?.Length);
        SuggestionDataModel? suggestionMovieDataModel = suggestions1?[1];  // Movie suggestion = index 1
        Assert.NotNull(suggestionMovieDataModel);
        SuggestionViewModel suggestionMovieViewModel = new(suggestionMovieDataModel, default(Guid));

        OmdbResponseDataModel? omdbMovieResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientMovieResponse);
        Assert.NotNull(omdbMovieResponse);

        TmdbMovieResponseDataModel? tmdbMovieResponse = TmdbHttpClient.GetMovieModelFromResponse(tmdbHttpClientMovieResponse);
        Assert.NotNull(tmdbMovieResponse);

        TmdbMovieCreditsResponseDataModel? movieCreditsResponse = TmdbHttpClient.GetMovieCreditsModelFromResponse(tmdbHttpClientMovieCreditsResponse);
        Assert.NotNull(movieCreditsResponse);

        TmdbWatchProvidersResponseDataModel? movieWatchProvidersResponse = TmdbHttpClient.GetWatchProvidersModelFromResponse(tmdbHttpClientMovieWatchProvidersResponse);
        Assert.NotNull(movieWatchProvidersResponse);

        TmdbConfigurationCountriesResponseDataModel? tvConfigurationCountriesResponse = TmdbHttpClient.GetConfigurationCountriesModelFromResponse(tmdbHttpClientConfigurationCountriesResponse);
        Assert.NotNull(tvConfigurationCountriesResponse);
        TmdbConfigurationCountriesResponseDataModel.ConfigurationCountriesDictionary? tvConfigurationCountriesDictionary = tvConfigurationCountriesResponse.GetConfigurationCountriesDictionary();
        Assert.NotNull(tvConfigurationCountriesDictionary);

        TmdbConfigurationLanguagesResponseDataModel? tvConfigurationLanguagesResponse = TmdbHttpClient.GetConfigurationLanguagesModelFromResponse(tmdbHttpClientConfigurationLanguagesResponse);
        Assert.NotNull(tvConfigurationLanguagesResponse);
        TmdbConfigurationLanguagesResponseDataModel.ConfigurationLanguagesDictionary? tvConfigurationLanguagesDictionary = tvConfigurationLanguagesResponse.GetConfigurationLanguagesDictionary();
        Assert.NotNull(tvConfigurationLanguagesDictionary);

        // Act

        MovieViewModel movieViewModel = new(suggestionMovieViewModel, 
                                            omdbMovieResponse,
                                            tmdbMovieResponse,
                                            movieCreditsResponse,
                                            movieWatchProvidersResponse,
                                            tvConfigurationCountriesDictionary,
                                            tvConfigurationLanguagesDictionary,
                                            default(Guid));
        
        // Assert
        
        Assert.NotNull(movieViewModel);
        
        Assert.Equal(
            "ID: 00000000-0000-0000-0000-000000000000\nImage:\n*****\nID: 00000000-0000-0000-0000-000000000000\nHeight: 3000\nImageURL: https://example.com/example2.jpg\nWidth: 2031\n*****\nImdbId: tt0000001\nTitle: Example Movie\nImdbRank: 4444\nKnownForActors: Example Jones, Example Brown\nYear: 2016\nRated: R\nOmdbGenres: Drama\nOmdbPlot: Seriously, some things definitely happen in this movie.\nAwards: Nominated for 7 Oscars. 21 wins & 42 nominations total\nImdbRating: 9.3\nImdbVotes: 3,182,645\nBoxOfficeString: $28,767,189\nBoxOfficeNumber: 28767189\nBudget: 25000000\nTmdbGenres: Drama, Crime\nHomepage: \nTmdbId: 278\nOriginCountries: United States of America, Japan\nOriginLanguage: English\nOriginalTitle: Example Movie\nTmdbPlot: Things happen in this movie.\nProductionCompanies: Castle Rock Entertainment; Nippon Herald Films\nProductionCountries: United States of America, Japan\nReleaseDate: 2016-09-23\nRevenue: 28341469\nRuntime: 142\nSpokenLanguages: English, Japanese\nStatus: Released\nTagline: Catchy sentence here.\nCast:\n*****\nID: 00000000-0000-0000-0000-000000000000\nGender: Female\nTmdbId: 11863\nName: Example Smith\nOriginalName: Example Smith\nPopularity: 2.8389\nProfilePath: /3FfJMIVwXgsIXbAT8ECBSZJAncR.jpg\nCharacter: Example Character 1\nBilledOrder: 0\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 192\nName: Example Actor\nOriginalName: Example Actor\nPopularity: 6.849\nProfilePath: /1ahENoyEgKSbRUd0IsydIff7rt1.jpg\nCharacter: Example Character 2\nBilledOrder: 1\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 4029\nName: Bob Gunton\nOriginalName: Bob Gunton\nPopularity: 2.0569\nProfilePath: /ulbVvuBToBN3aCGcV028hwO0MOP.jpg\nCharacter: Warden Norton\nBilledOrder: 2\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 6573\nName: William Sadler\nOriginalName: William Sadler\nPopularity: 2.9418\nProfilePath: /rWeb2kjYCA7V9MC9kRwRpm57YoY.jpg\nCharacter: Heywood\nBilledOrder: 3\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 6574\nName: Clancy Brown\nOriginalName: Clancy Brown\nPopularity: 3.9758\nProfilePath: /1JeBRNG7VS7r64V9lOvej9bZXW5.jpg\nCharacter: Captain Byron T. Hadley\nBilledOrder: 4\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 6575\nName: Gil Bellows\nOriginalName: Gil Bellows\nPopularity: 1.4592\nProfilePath: /eCOIv2nSGnWTHdn88NoMyNOKWyR.jpg\nCharacter: Tommy\nBilledOrder: 5\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 6577\nName: James Whitmore\nOriginalName: James Whitmore\nPopularity: 1.5482\nProfilePath: /nYMAbkfwFIgKK84vnLoQctI6vHg.jpg\nCharacter: Brooks Hatlen\nBilledOrder: 6\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 6576\nName: Mark Rolston\nOriginalName: Mark Rolston\nPopularity: 1.8183\nProfilePath: /hcrNRIptYMRXgkJ9k76BlQu6DQp.jpg\nCharacter: Bogs Diamond\nBilledOrder: 7\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 12645\nName: Jeffrey DeMunn\nOriginalName: Jeffrey DeMunn\nPopularity: 1.6632\nProfilePath: /70bkLdlkBB7x2NztuJAh4pjdyxy.jpg\nCharacter: 1946 D.A.\nBilledOrder: 8\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 92119\nName: Larry Brandenburg\nOriginalName: Larry Brandenburg\nPopularity: 0.5669\nProfilePath: /y13c1a4keaLnoTbi3dERwolQXWP.jpg\nCharacter: Skeet\nBilledOrder: 9\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 168323\nName: Neil Giuntoli\nOriginalName: Neil Giuntoli\nPopularity: 0.3932\nProfilePath: /dRFzPIAinOjXbYOqYEax0sGrfTl.jpg\nCharacter: Jigger\nBilledOrder: 10\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 6580\nName: Brian Libby\nOriginalName: Brian Libby\nPopularity: 1.0208\nProfilePath: /sumWxPgIbCpnp1v1exfJWIt43i3.jpg\nCharacter: Floyd\nBilledOrder: 11\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 2555\nName: David Proval\nOriginalName: David Proval\nPopularity: 0.863\nProfilePath: /ku3LWQXiJ80nWgs21fKvTMaR6Ui.jpg\nCharacter: Snooze\nBilledOrder: 12\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 5063\nName: Joseph Ragno\nOriginalName: Joseph Ragno\nPopularity: 0.2836\nProfilePath: /fF7M5JyfA2kUeh8YItkiCd1of1b.jpg\nCharacter: Ernie\nBilledOrder: 13\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 8693\nName: Jude Ciccolella\nOriginalName: Jude Ciccolella\nPopularity: 0.8988\nProfilePath: /sS506N50Brzyu0wgq18mk3pj4dH.jpg\nCharacter: Guard Mert\nBilledOrder: 14\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 32393\nName: Paul McCrane\nOriginalName: Paul McCrane\nPopularity: 1.6533\nProfilePath: /nzFUnpptZ5hhiivD1lsUcPDB8V0.jpg\nCharacter: Guard Trout\nBilledOrder: 15\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Female\nTmdbId: 6578\nName: Renee Blaine\nOriginalName: Renee Blaine\nPopularity: 0.3418\nProfilePath: \nCharacter: Andy Dufresne's Wife\nBilledOrder: 16\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 1624179\nName: Scott Mann\nOriginalName: Scott Mann\nPopularity: 0.0645\nProfilePath: /sOqi4IDMyOo96eZhk15gqsxDitc.jpg\nCharacter: Glenn Quentin\nBilledOrder: 17\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 163979\nName: John Horton\nOriginalName: John Horton\nPopularity: 0.4272\nProfilePath: /6c1x9prTCTcG3DyZ8l6pqvTEobk.jpg\nCharacter: 1946 Judge\nBilledOrder: 18\n\nID: 00000000-0000-0000-0000-000000000000\nGender: NotSetNotSpecified\nTmdbId: 194459\nName: Gordon Greene\nOriginalName: Gordon Greene\nPopularity: 0.0896\nProfilePath: /uJF1H9opCmHJjWIOIUvez1pGhPW.jpg\nCharacter: 1947 Parole Hearings Man\nBilledOrder: 19\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 52603\nName: Alfonso Freeman\nOriginalName: Alfonso Freeman\nPopularity: 0.6771\nProfilePath: /9H4wX5clEpFfD3SG2JPoGH3LIgV.jpg\nCharacter: Fresh Fish Con\nBilledOrder: 20\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 122596\nName: Vincent Foster\nOriginalName: Vincent Foster\nPopularity: 0.2136\nProfilePath: /4H11cOROVjOyVNOoaIDdYYOkNNr.jpg\nCharacter: Hungry Fish Con\nBilledOrder: 21\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 2772979\nName: John E. Summers\nOriginalName: John E. Summers\nPopularity: 0.0621\nProfilePath: \nCharacter: New Fish Guard\nBilledOrder: 22\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 1216752\nName: Frank Medrano\nOriginalName: Frank Medrano\nPopularity: 0.5438\nProfilePath: /ps427z5PMzVcwxipZptxLxx2Hbe.jpg\nCharacter: Fat Ass\nBilledOrder: 23\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 139992\nName: Mack Miles\nOriginalName: Mack Miles\nPopularity: 0.1209\nProfilePath: /z4WVgPpOvUFkS5VCn8rOeuCMh5O.jpg\nCharacter: Tyrell\nBilledOrder: 24\n\nID: 00000000-0000-0000-0000-000000000000\nGender: NotSetNotSpecified\nTmdbId: 2772983\nName: Alan R. Kessler\nOriginalName: Alan R. Kessler\nPopularity: 0.1288\nProfilePath: /cSK84R5XIYyq2XTjvv6dhlzhJHE.jpg\nCharacter: Laundry Bob\nBilledOrder: 25\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 1853997\nName: Morgan Lund\nOriginalName: Morgan Lund\nPopularity: 0.3449\nProfilePath: /80p9IRMll98CxH8w1gpPwieFV83.jpg\nCharacter: Laundry Truck Driver\nBilledOrder: 26\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 2772985\nName: Cornell Wallace\nOriginalName: Cornell Wallace\nPopularity: 0.2835\nProfilePath: /xsleUHzEWMcBZq6sLMJLBws5wds.jpg\nCharacter: Laundry Leonard\nBilledOrder: 27\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 32656\nName: Gary Lee Davis\nOriginalName: Gary Lee Davis\nPopularity: 0.3112\nProfilePath: /a0UgFGytMZGJrDH6T0VBcsGt1qd.jpg\nCharacter: Rooster\nBilledOrder: 28\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 179188\nName: Neil Summers\nOriginalName: Neil Summers\nPopularity: 0.5552\nProfilePath: /2ESnlgBQzWM17KvSao5GrJInd0u.jpg\nCharacter: Pete\nBilledOrder: 29\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 2141\nName: Ned Bellamy\nOriginalName: Ned Bellamy\nPopularity: 0.6152\nProfilePath: /9qj6thu0g6iUc2hvfoEYlO4GSbY.jpg\nCharacter: Guard Youngblood\nBilledOrder: 30\n\nID: 00000000-0000-0000-0000-000000000000\nGender: NotSetNotSpecified\nTmdbId: 2772989\nName: Joe Pecoraro\nOriginalName: Joe Pecoraro\nPopularity: 0.0561\nProfilePath: \nCharacter: Projectionist\nBilledOrder: 31\n\nID: 00000000-0000-0000-0000-000000000000\nGender: NotSetNotSpecified\nTmdbId: 2772990\nName: Harold E. Cope Jr.\nOriginalName: Harold E. Cope Jr.\nPopularity: 0.0547\nProfilePath: \nCharacter: Hole Guard\nBilledOrder: 32\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 6579\nName: Brian Delate\nOriginalName: Brian Delate\nPopularity: 0.604\nProfilePath: /rydDYe4VHjZsJZPWOus4KGkhroG.jpg\nCharacter: Guard Dekins\nBilledOrder: 33\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 79025\nName: Don McManus\nOriginalName: Don McManus\nPopularity: 0.9341\nProfilePath: /rGdpdQ0DDVDX4FlQzrdFGcOlRGB.jpg\nCharacter: Guard Wiley\nBilledOrder: 34\n\nID: 00000000-0000-0000-0000-000000000000\nGender: NotSetNotSpecified\nTmdbId: 2574896\nName: Donald Zinn\nOriginalName: Donald Zinn\nPopularity: 0.0638\nProfilePath: \nCharacter: Moresby Batter\nBilledOrder: 35\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Female\nTmdbId: 1422439\nName: Dorothy Silver\nOriginalName: Dorothy Silver\nPopularity: 0.2426\nProfilePath: /nZNrUhLbEGHQfot0rPcep8wzzpf.jpg\nCharacter: 1954 Landlady\nBilledOrder: 36\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 123302\nName: Robert Haley\nOriginalName: Robert Haley\nPopularity: 0.1113\nProfilePath: /cf7UhLqiRXB2efOyPHh34dqJhP4.jpg\nCharacter: 1954 Food-Way Manager\nBilledOrder: 37\n\nID: 00000000-0000-0000-0000-000000000000\nGender: NotSetNotSpecified\nTmdbId: 2772991\nName: Dana Snyder\nOriginalName: Dana Snyder\nPopularity: 0.0892\nProfilePath: \nCharacter: 1954 Food-Way Woman\nBilledOrder: 38\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 2294531\nName: John D. Craig\nOriginalName: John D. Craig\nPopularity: 0.1038\nProfilePath: \nCharacter: 1957 Parole Hearings Man\nBilledOrder: 39\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 1537819\nName: Ken Magee\nOriginalName: Ken Magee\nPopularity: 0.5791\nProfilePath: /uAdTFwnXcTjrODawHbSVelZrBKN.jpg\nCharacter: Ned Grimes\nBilledOrder: 40\n\nID: 00000000-0000-0000-0000-000000000000\nGender: NotSetNotSpecified\nTmdbId: 2772995\nName: Eugene C. DePasquale\nOriginalName: Eugene C. DePasquale\nPopularity: 0.0862\nProfilePath: /43zitHs11uwJUfrq43PlPtsYbiZ.jpg\nCharacter: Mail Caller\nBilledOrder: 41\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 105649\nName: Bill Bolender\nOriginalName: Bill Bolender\nPopularity: 0.568\nProfilePath: \nCharacter: Elmo Blatch\nBilledOrder: 42\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 2779347\nName: Ron Newell\nOriginalName: Ron Newell\nPopularity: 0.1349\nProfilePath: /81Pex6ci4TGDEJvcMXtAxD1UzvM.jpg\nCharacter: Elderly Hole Guard\nBilledOrder: 43\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 1123794\nName: John R. Woodward\nOriginalName: John R. Woodward\nPopularity: 0.5673\nProfilePath: /9vsgOeMtjnVCZYrzxAfT7ADjoeX.jpg\nCharacter: Bullhorn Tower Guard\nBilledOrder: 44\n\nID: 00000000-0000-0000-0000-000000000000\nGender: NotSetNotSpecified\nTmdbId: 2774986\nName: Chuck Brauchler\nOriginalName: Chuck Brauchler\nPopularity: 0.0559\nProfilePath: \nCharacter: Man Missing Guard\nBilledOrder: 45\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 91420\nName: Dion Anderson\nOriginalName: Dion Anderson\nPopularity: 0.7221\nProfilePath: /d59y9z1vIkLAqtK0Ykx3StHim18.jpg\nCharacter: Head Bull Haig\nBilledOrder: 46\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Female\nTmdbId: 2231406\nName: Claire Slemmer\nOriginalName: Claire Slemmer\nPopularity: 0.2246\nProfilePath: /16y5wtGEpk7e6Bj9PEfhWpAc3d6.jpg\nCharacter: Bank Teller\nBilledOrder: 47\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 1634770\nName: James Kisicki\nOriginalName: James Kisicki\nPopularity: 0.1089\nProfilePath: /kOtD6LFfePlOHIlBbQzLG811u5l.jpg\nCharacter: Bank Manager\nBilledOrder: 48\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 27690\nName: Rohn Thomas\nOriginalName: Rohn Thomas\nPopularity: 0.2364\nProfilePath: /iDZbEoYDdZQbmFppaNVJG36Scms.jpg\nCharacter: Bugle Editor\nBilledOrder: 49\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 2774993\nName: Charlie Kearns\nOriginalName: Charlie Kearns\nPopularity: 0.061\nProfilePath: /q91O9tQQp4hwpmyynPIbz9q0MCI.jpg\nCharacter: 1966 D.A.\nBilledOrder: 50\n\nID: 00000000-0000-0000-0000-000000000000\nGender: NotSetNotSpecified\nTmdbId: 2774995\nName: Rob Reider\nOriginalName: Rob Reider\nPopularity: 0.0543\nProfilePath: /3LLS44SL0B3W5W12rNl8ijTPohr.jpg\nCharacter: Duty Guard\nBilledOrder: 51\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 48587\nName: Brian Brophy\nOriginalName: Brian Brophy\nPopularity: 0.3609\nProfilePath: /cvC2xxeBgeGSQVBvqJBy7tVgiIw.jpg\nCharacter: 1967 Parole Hearings Man\nBilledOrder: 52\n\nID: 00000000-0000-0000-0000-000000000000\nGender: NotSetNotSpecified\nTmdbId: 2775011\nName: Paul Kennedy\nOriginalName: Paul Kennedy\nPopularity: 0.0819\nProfilePath: /tmJC7194JYk9Rp42lr1rZBtwpmo.jpg\nCharacter: 1967 Food-Way Manager\nBilledOrder: 53\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 25659\nName: James Babson\nOriginalName: James Babson\nPopularity: 0.4218\nProfilePath: /8O5HEPymkRNiOFS2kr23JHXt5w4.jpg\nCharacter: Con (uncredited)\nBilledOrder: 54\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 1771867\nName: Dennis Baker\nOriginalName: Dennis Baker\nPopularity: 0.1079\nProfilePath: \nCharacter: Old Man on Bus (uncredited)\nBilledOrder: 55\n\nID: 00000000-0000-0000-0000-000000000000\nGender: NotSetNotSpecified\nTmdbId: 1185588\nName: Fred Culbertson\nOriginalName: Fred Culbertson\nPopularity: 0.1101\nProfilePath: \nCharacter: Police Officer (uncredited)\nBilledOrder: 56\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 92647\nName: Alonzo F. Jones\nOriginalName: Alonzo F. Jones\nPopularity: 0.0878\nProfilePath: /uf29tyRXttZBS5xSxBU2tuOEu2K.jpg\nCharacter: Inmate (uncredited)\nBilledOrder: 57\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 1337623\nName: Sergio Kato\nOriginalName: Sergio Kato\nPopularity: 0.3403\nProfilePath: \nCharacter: Inmate II (uncredited)\nBilledOrder: 58\n*****\nDirectors:\n*****\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 4027\nKnownForDepartment: Directing\nName: Example Director\nOriginalName: Example Director\nPopularity: 2.1385\nProfilePath: /vZ50guP86otYTiBSGfi35GNHWVf.jpg\nDepartment: Directing\nJob: Director\n*****\nWriters:\n*****\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 4027\nKnownForDepartment: Directing\nName: Example Director\nOriginalName: Example Director\nPopularity: 2.1385\nProfilePath: /vZ50guP86otYTiBSGfi35GNHWVf.jpg\nDepartment: Writing\nJob: Screenplay\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 3027\nKnownForDepartment: Writing\nName: Example Writer\nOriginalName: Example Writer\nPopularity: 2.2231\nProfilePath: /7r5nEzNanuEhmxtpsKE1uCBU5Jd.jpg\nDepartment: Writing\nJob: Novel\n*****\nProducers:\n*****\nID: 00000000-0000-0000-0000-000000000000\nGender: Female\nTmdbId: 4028\nKnownForDepartment: Production\nName: Niki Marvin\nOriginalName: Niki Marvin\nPopularity: 0.0978\nProfilePath: \nDepartment: Production\nJob: Producer\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 4054\nKnownForDepartment: Production\nName: David V. Lester\nOriginalName: David V. Lester\nPopularity: 0.3448\nProfilePath: \nDepartment: Production\nJob: Executive Producer\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Female\nTmdbId: 46347\nKnownForDepartment: Production\nName: Liz Glotzer\nOriginalName: Liz Glotzer\nPopularity: 0.2349\nProfilePath: /lLUAf2vn1n3AlYl7kk5V1eB9Ikr.jpg\nDepartment: Production\nJob: Executive Producer\n\nID: 00000000-0000-0000-0000-000000000000\nGender: NotSetNotSpecified\nTmdbId: 1400821\nKnownForDepartment: Visual Effects\nName: Melissa Taylor\nOriginalName: Melissa Taylor\nPopularity: 0.1094\nProfilePath: \nDepartment: Visual Effects\nJob: Visual Effects Producer\n\nID: 00000000-0000-0000-0000-000000000000\nGender: NotSetNotSpecified\nTmdbId: 1771841\nKnownForDepartment: Crew\nName: Dan Goldwasser\nOriginalName: Dan Goldwasser\nPopularity: 0.0588\nProfilePath: \nDepartment: Crew\nJob: Executive Music Producer\n*****\nWatchProvidersBuy:\n*****\nID: 00000000-0000-0000-0000-000000000000\nLogoPath: /qR6FKvnPBx2O37FDg8PNM7efwF3.jpg\nProviderName: Amazon Video\nDisplayPriority: 7\n\nID: 00000000-0000-0000-0000-000000000000\nLogoPath: /SPnB1qiCkYfirS2it3hZORwGVn.jpg\nProviderName: Apple TV Store\nDisplayPriority: 8\n\nID: 00000000-0000-0000-0000-000000000000\nLogoPath: /8z7rC8uIDaTM91X0ZfkRf04ydj2.jpg\nProviderName: Google Play Movies\nDisplayPriority: 17\n\nID: 00000000-0000-0000-0000-000000000000\nLogoPath: /pTnn5JwWr4p3pG8H6VrpiQo7Vs0.jpg\nProviderName: YouTube\nDisplayPriority: 18\n\nID: 00000000-0000-0000-0000-000000000000\nLogoPath: /19fkcOz0xeUgCVW8tO85uOYnYK9.jpg\nProviderName: Fandango At Home\nDisplayPriority: 35\n*****\nWatchProvidersFlatrate:\n*****\nID: 00000000-0000-0000-0000-000000000000\nLogoPath: /gAGrSQCTAisxy2CsWbijVvJEnRo.jpg\nProviderName: AMC+ Roku Premium Channel\nDisplayPriority: 28\n\nID: 00000000-0000-0000-0000-000000000000\nLogoPath: /x9zOHTUkQzt3PgPVKbMH9CKBwLK.jpg\nProviderName: YouTube TV\nDisplayPriority: 36\n*****\nWatchProvidersRent:\n*****\nID: 00000000-0000-0000-0000-000000000000\nLogoPath: /qR6FKvnPBx2O37FDg8PNM7efwF3.jpg\nProviderName: Amazon Video\nDisplayPriority: 7\n\nID: 00000000-0000-0000-0000-000000000000\nLogoPath: /SPnB1qiCkYfirS2it3hZORwGVn.jpg\nProviderName: Apple TV Store\nDisplayPriority: 8\n\nID: 00000000-0000-0000-0000-000000000000\nLogoPath: /8z7rC8uIDaTM91X0ZfkRf04ydj2.jpg\nProviderName: Google Play Movies\nDisplayPriority: 17\n\nID: 00000000-0000-0000-0000-000000000000\nLogoPath: /pTnn5JwWr4p3pG8H6VrpiQo7Vs0.jpg\nProviderName: YouTube\nDisplayPriority: 18\n\nID: 00000000-0000-0000-0000-000000000000\nLogoPath: /19fkcOz0xeUgCVW8tO85uOYnYK9.jpg\nProviderName: Fandango At Home\nDisplayPriority: 35\n\nID: 00000000-0000-0000-0000-000000000000\nLogoPath: /aAb9CUHjFe9Y3O57qnrJH0KOF1B.jpg\nProviderName: Spectrum On Demand\nDisplayPriority: 121\n\nID: 00000000-0000-0000-0000-000000000000\nLogoPath: /vLZKlXUNDcZR7ilvfY9Wr9k80FZ.jpg\nProviderName: Plex\nDisplayPriority: 126"
                , movieViewModel.ToString());
    }
}
