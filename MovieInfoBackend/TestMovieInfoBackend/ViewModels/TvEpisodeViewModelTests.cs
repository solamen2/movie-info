using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.DataModels;

public class TvEpisodeViewModelTests
{
    private string suggestionHttpClientResponse1;
    private string omdbHttpClientTvEpisodeResponse;
    private string tmdbHttpClientTvEpisodeResponse;
    private string tmdbHttpClientTvEpisodeCreditsResponse;

    public TvEpisodeViewModelTests(ITestOutputHelper output)
    {        
        // Arrange

        string testDataFilename1 = "MovieHttpClientResponse1.json";
        string testOmdbTvEpisodeDataFilename = "OmdbHttpClientTvEpisodeResponse.json";
        string testTmdbTvEpisodeDataFilename = "TmdbHttpClientTvEpisodeResponse.json";
        string testTvEpisodeCreditsDataFilename = "TmdbHttpClientTvEpisodeCreditsResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testDataFilename1}"))
        {
            suggestionHttpClientResponse1 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(suggestionHttpClientResponse1))
        {
            throw new ArgumentException($"{testDataFilename1} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testOmdbTvEpisodeDataFilename}"))
        {
            omdbHttpClientTvEpisodeResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(omdbHttpClientTvEpisodeResponse))
        {
            throw new ArgumentException($"{testOmdbTvEpisodeDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTmdbTvEpisodeDataFilename}"))
        {
            tmdbHttpClientTvEpisodeResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvEpisodeResponse))
        {
            throw new ArgumentException($"{testTmdbTvEpisodeDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvEpisodeCreditsDataFilename}"))
        {
            tmdbHttpClientTvEpisodeCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvEpisodeCreditsResponse))
        {
            throw new ArgumentException($"{testTvEpisodeCreditsDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetViewModelFromDataModel_ValidDataModel_ReturnsValidViewModel()
    {
        // Arrange (continued)
        
        OmdbResponseDataModel? omdbTvEpisodeResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientTvEpisodeResponse);
        Assert.NotNull(omdbTvEpisodeResponse);

        TmdbTvEpisodeResponseDataModel? tmdbTvEpisodeResponse = TmdbHttpClient.GetTvEpisodeModelFromResponse(tmdbHttpClientTvEpisodeResponse);
        Assert.NotNull(tmdbTvEpisodeResponse);

        TmdbTvEpisodeCreditsResponseDataModel? tvEpisodeCreditsResponse = TmdbHttpClient.GetTvEpisodeCreditsModelFromResponse(tmdbHttpClientTvEpisodeCreditsResponse);
        Assert.NotNull(tvEpisodeCreditsResponse);

        // Act

        TvEpisodeViewModel tvEpisodeViewModel = new(omdbTvEpisodeResponse, 
                                                    tmdbTvEpisodeResponse,
                                                    tvEpisodeCreditsResponse);
        
        // Assert
        
        Assert.NotNull(tvEpisodeViewModel);

        // NOTE: Only checking fields which are transformed by the view model. Other fields have been covered by data model tests or ToString() test below
        Assert.Equal(2001, tvEpisodeViewModel.Year);
        Assert.Equal(50, tvEpisodeViewModel.Runtime);
        Assert.Equal("11/6/2001", tvEpisodeViewModel.AirDate.ToString());
        Assert.Equal("Example Creator Martinez", tvEpisodeViewModel.Directors[0].Name);
        Assert.Equal("Example Creator Martinez", tvEpisodeViewModel.Writers[0].Name);
        Assert.Equal("Douglas Petrie", tvEpisodeViewModel.Producers[0].Name);
        Assert.Equal("Brian Wankum", tvEpisodeViewModel.Producers[1].Name);
    }

    [Fact]
    public void MovieViewModel_EmptyDataFields_ReturnEmptyValues()
    {
        // Arrange (continued)

        TmdbTvEpisodeCreditsResponseDataModel? tvEpisodeCreditsResponse = TmdbHttpClient.GetTvEpisodeCreditsModelFromResponse(tmdbHttpClientTvEpisodeCreditsResponse);
        Assert.NotNull(tvEpisodeCreditsResponse);

        var omdbTvEpisodeResponseDataModel = new OmdbResponseDataModel
        {
            Title = "Example TV Episode 2",
            Year = "",  // Testing this value
            Rated = "13+",
            Released = "N/A",
            Season = "6",
            Episode = "30",
            Runtime = "",  // Testing this value
            Genre = "Action, Drama, Fantasy",
            Director = "Example Creator Martinez",
            Writer = "Example Creator Martinez, Rebecca Kirshner, Steven S. DeKnight",
            Actors = "Example Smith, Example Actor 2, Emma Caulfield Ford",
            Plot = "Tons less interesting stuff probably happens.",
            Language = "English",
            Country = "United States",
            Awards = "N/A",
            Poster = "https://m.media-amazon.com/images/M/MV5BZTVkOWEzNzUtMjVkOS00Y2QzLTk2MGQtN2VkOGE3NTBjODI5XkEyXkFqcGdeQXVyMDM2NDM2MQ@@._V1_SX300.jpg",
            Ratings = new OmdbRatingDataModel[]
            {
                new OmdbRatingDataModel
                {
                    Source = "Internet Movie Database",
                    Value = "9.6/10"
                }
            },
            Metascore = "N/A",
            ImdbRating = "9.6",
            ImdbVotes = "11006",
            ImdbId = "tt0533467",
            SeriesId = "tt0118276",
            Type = "episode",
            Response = "True"
        };
        
        var omdbTvEpisodeResponseDataModel2 = new OmdbResponseDataModel
        {
            Title = "Example TV Episode 3",
            Year = "N/A",  // Testing this value
            Rated = "13+",
            Released = "N/A",
            Season = "6",
            Episode = "31",
            Runtime = "N/A",  // Testing this value
            Genre = "Action, Drama, Fantasy",
            Director = "Example Creator Martinez",
            Writer = "Example Creator Martinez, Rebecca Kirshner, Steven S. DeKnight",
            Actors = "Example Smith, Example Actor 2, Emma Caulfield Ford",
            Plot = "Tons of more interesting stuff probably happens.",
            Language = "English",
            Country = "United States",
            Awards = "N/A",
            Poster = "https://m.media-amazon.com/images/M/MV5BZTVkOWEzNzUtMjVkOS00Y2QzLTk2MGQtN2VkOGE3NTBjODI5XkEyXkFqcGdeQXVyMDM2NDM2MQ@@._V1_SX300.jpg",
            Ratings = new OmdbRatingDataModel[]
            {
                new OmdbRatingDataModel
                {
                    Source = "Internet Movie Database",
                    Value = "9.5/10"
                }
            },
            Metascore = "N/A",
            ImdbRating = "9.5",
            ImdbVotes = "11006",
            ImdbId = "tt0533468",
            SeriesId = "tt0118276",
            Type = "episode",
            Response = "True"
        };

        var tmdbTvEpisodeResponseDataModel = new TmdbTvEpisodeResponseDataModel
        {
            AirDate = "",  // Testing this value
            Crew = new TmdbTvEpisodeCrewDataModel[]
            {
                new TmdbTvEpisodeCrewDataModel {
                    Department = "Writing",
                    Job = "Writer",
                    CreditId = "5253387019c2957940053fa5",
                    Adult = false,
                    Gender = 2,
                    Id = 12891,
                    KnownForDepartment = "Writing",
                    Name = "Example Creator Martinez",
                    OriginalName = "Example Creator Martinez",
                    Popularity = 1.5599,
                    ProfilePath = "/6PJwHV17KTuTRQaqrXBtVCwchcU.jpg"
                }
            },
            EpisodeNumber = 30,
            EpisodeType = "standard",
            GuestStars = [],
            Name = "Example TV Episode 2",
            Overview = "This episode is not very interesting.",
            Id = 949534,
            ProductionCode = "6ABB08",
            Runtime = 0,
            SeasonNumber = 6,
            StillPath = "/m6DAoR7I3UAeyjGA5ekLf5KQDfS.jpg",
            VoteAverage = 8.878,
            VoteCount = 44
        };

        var tmdbTvEpisodeResponseDataModel2 = new TmdbTvEpisodeResponseDataModel
        {
            AirDate = "N/A",  // Testing this value
            Crew = new TmdbTvEpisodeCrewDataModel[]
            {
                new TmdbTvEpisodeCrewDataModel {
                    Department = "Writing",
                    Job = "Writer",
                    CreditId = "5253387019c2957940053fa5",
                    Adult = false,
                    Gender = 2,
                    Id = 12891,
                    KnownForDepartment = "Writing",
                    Name = "Example Creator Martinez",
                    OriginalName = "Example Creator Martinez",
                    Popularity = 1.5599,
                    ProfilePath = "/6PJwHV17KTuTRQaqrXBtVCwchcU.jpg"
                }
            },
            EpisodeNumber = 31,
            EpisodeType = "standard",
            GuestStars = [],
            Name = "Example TV Episode 3",
            Overview = "This episode is really very interesting.",
            Id = 949534,
            ProductionCode = "6ABB09",
            Runtime = 0,
            SeasonNumber = 6,
            StillPath = "/m6DAoR7I3UAeyjGA5ekLf5KQDfS.jpg",
            VoteAverage = 8.978,
            VoteCount = 43
        };

        // Act

        TvEpisodeViewModel tvEpisodeViewModel = new(omdbTvEpisodeResponseDataModel, 
                                                    tmdbTvEpisodeResponseDataModel,
                                                    tvEpisodeCreditsResponse);

        TvEpisodeViewModel tvEpisodeViewModel2 = new(omdbTvEpisodeResponseDataModel2, 
                                                    tmdbTvEpisodeResponseDataModel2,
                                                    tvEpisodeCreditsResponse);

        // Assert

        Assert.NotNull(tvEpisodeViewModel);

        Assert.Equal(0, tvEpisodeViewModel.Year);
        Assert.Equal(0, tvEpisodeViewModel.Runtime);
        Assert.Null(tvEpisodeViewModel.AirDate);

        Assert.NotNull(tvEpisodeViewModel2);

        Assert.Equal(0, tvEpisodeViewModel2.Year);
        Assert.Equal(0, tvEpisodeViewModel2.Runtime);
        Assert.Null(tvEpisodeViewModel2.AirDate);
    }

    [Fact]
    public void TvEpisodeViewModel_ValidModelToString_ReturnsCorrectValue()
    {
        // Arrange (continued)
        
        OmdbResponseDataModel? omdbTvEpisodeResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientTvEpisodeResponse);
        Assert.NotNull(omdbTvEpisodeResponse);

        TmdbTvEpisodeResponseDataModel? tmdbTvEpisodeResponse = TmdbHttpClient.GetTvEpisodeModelFromResponse(tmdbHttpClientTvEpisodeResponse);
        Assert.NotNull(tmdbTvEpisodeResponse);

        TmdbTvEpisodeCreditsResponseDataModel? tvEpisodeCreditsResponse = TmdbHttpClient.GetTvEpisodeCreditsModelFromResponse(tmdbHttpClientTvEpisodeCreditsResponse);
        Assert.NotNull(tvEpisodeCreditsResponse);

        // Act

        TvEpisodeViewModel tvEpisodeViewModel = new(omdbTvEpisodeResponse, 
                                                    tmdbTvEpisodeResponse,
                                                    tvEpisodeCreditsResponse,
                                                    default(Guid));
        
        // Assert
        
        Assert.NotNull(tvEpisodeViewModel);

        Assert.Equal(
            "ID: 00000000-0000-0000-0000-000000000000\nYear: 2001\nRated: 13+\nOmdbAverageEpisodeRuntimeString: 50 min\nOmdbAverageEpisodeRuntimeNumber: 50\nOmdbGenres: Action, Drama, Fantasy\nKnownForActors: Example Smith, Example Actor 2, Emma Caulfield Ford\nOmdbOverview: Tons of interesting stuff happens, including lots of music.\nAwards: N/A\nImdbRating: 9.7\nImdbVotes: 11006\nImdbId: tt0533466\nAirDateString: 2001-11-06\nAirDate: 11/6/2001\nEpisodeNumber: 7\nEpisodeType: standard\nTitle: Example TV Episode\nTmdbOverview: This episode has a lot of interesting things in it, especially music.\nTmdbId: 949534\nRuntime: 50\nSeasonNumber: 6\nStillPath: /m6DAoR7I3UAeyjGA5ekLf5KQDfS.jpg\nCast:\n*****\nID: 00000000-0000-0000-0000-000000000000\nGender: Female\nTmdbId: 11863\nName: Example Smith\nOriginalName: Example Smith\nPopularity: 2.7348\nProfilePath: /xKe52w4tpv61ohz9iz75wNdzcwZ.jpg\nCharacter: Example Character 1\nBilledOrder: 0\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 71675\nName: Example Actor 2\nOriginalName: Example Actor 2\nPopularity: 1.0828\nProfilePath: /6O5sg97Fv07EzdWjpoq3lb8KnTM.jpg\nCharacter: Example Character 4\nBilledOrder: 1\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Female\nTmdbId: 21595\nName: Example Actress 3\nOriginalName: Example Actress 3\nPopularity: 2.4503\nProfilePath: /bO16z8rAzZWdjCga8dcbJ2AFAh2.jpg\nCharacter: Example Character 5\nBilledOrder: 2\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Female\nTmdbId: 66745\nName: Emma Caulfield\nOriginalName: Emma Caulfield\nPopularity: 0.9354\nProfilePath: /tYQURD5pl4iGHjyA2yVwmmhaoPt.jpg\nCharacter: Anya\nBilledOrder: 3\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Female\nTmdbId: 49961\nName: Michelle Trachtenberg\nOriginalName: Michelle Trachtenberg\nPopularity: 2.7178\nProfilePath: /8eb8ts6E7gM5SgEFn8VcXfGc39r.jpg\nCharacter: Dawn Summers\nBilledOrder: 6\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 47297\nName: Example Actor 4\nOriginalName: Example Actor 4\nPopularity: 1.1273\nProfilePath: /oJnBB3g2IINnsLSr8B79bJ9ykkx.jpg\nCharacter: Example Character 4\nBilledOrder: 10\n*****\nDirectors:\n*****\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 12891\nKnownForDepartment: Writing\nName: Example Creator Martinez\nOriginalName: Example Creator Martinez\nPopularity: 1.4737\nProfilePath: /6PJwHV17KTuTRQaqrXBtVCwchcU.jpg\nDepartment: Directing\nJob: Director\n*****\nWriters:\n*****\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 12891\nKnownForDepartment: Writing\nName: Example Creator Martinez\nOriginalName: Example Creator Martinez\nPopularity: 1.4737\nProfilePath: /6PJwHV17KTuTRQaqrXBtVCwchcU.jpg\nDepartment: Writing\nJob: Writer\n*****\nProducers:\n*****\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 1213071\nKnownForDepartment: Writing\nName: Douglas Petrie\nOriginalName: Douglas Petrie\nPopularity: 0.4306\nProfilePath: /jt79Llnx8MdrgfBV7Tjoy27t3Gj.jpg\nDepartment: Production\nJob: Producer\n\nID: 00000000-0000-0000-0000-000000000000\nGender: Male\nTmdbId: 1699138\nKnownForDepartment: Production\nName: Brian Wankum\nOriginalName: Brian Wankum\nPopularity: 0.0933\nProfilePath: \nDepartment: Production\nJob: Associate Producer\n*****\nGuestStars:\n*****\nID: 00000000-0000-0000-0000-000000000000\nCharacter: Example Character 6\nBilledOrder: 5\nGender: Male\nTmdbId: 34257\nName: Example Actor 3\nOriginalName: Example Actor 3\nPopularity: 5.9016\nProfilePath: /eRfRWnoipu1Tx84fcuOEdfR87qb.jpg\n\nID: 00000000-0000-0000-0000-000000000000\nCharacter: Tara Maclay\nBilledOrder: 14\nGender: Female\nTmdbId: 35468\nName: Amber Benson\nOriginalName: Amber Benson\nPopularity: 0.8414\nProfilePath: /vEBdN1BhSOG2pCZPpCb6dgj6Wer.jpg\n\nID: 00000000-0000-0000-0000-000000000000\nCharacter: Mustard Man\nBilledOrder: 522\nGender: Male\nTmdbId: 149520\nName: David Fury\nOriginalName: David Fury\nPopularity: 0.4174\nProfilePath: /4V371yAWCJ1s2cVCamX5eYjMNVc.jpg\n\nID: 00000000-0000-0000-0000-000000000000\nCharacter: Sweet\nBilledOrder: 625\nGender: Male\nTmdbId: 15567\nName: Hinton Battle\nOriginalName: Hinton Battle\nPopularity: 0.1936\nProfilePath: /zdAzNmXQUXhc9cDTjGBQ0HEJK2V.jpg\n\nID: 00000000-0000-0000-0000-000000000000\nCharacter: Parking Ticket Woman\nBilledOrder: 694\nGender: Female\nTmdbId: 149495\nName: Marti Noxon\nOriginalName: Marti Noxon\nPopularity: 0.6933\nProfilePath: /rrt5WkIi31DKr30vJWEPlRpkgHL.jpg\n\nID: 00000000-0000-0000-0000-000000000000\nCharacter: Henchman / Tap Dancing Victim\nBilledOrder: 1123\nGender: Male\nTmdbId: 149740\nName: Scot Zeller\nOriginalName: Scot Zeller\nPopularity: 0.2752\nProfilePath: /vYb6it59p5TADS6oapywXLRaEGl.jpg\n\nID: 00000000-0000-0000-0000-000000000000\nCharacter: Demon / Henchman\nBilledOrder: 1124\nGender: Male\nTmdbId: 29216\nName: Zachary Woodlee\nOriginalName: Zachary Woodlee\nPopularity: 0.2066\nProfilePath: /1fmzKHfS928pYm4sqE4o3CNFw.jpg\n\nID: 00000000-0000-0000-0000-000000000000\nCharacter: Henchman\nBilledOrder: 1125\nGender: NotSetNotSpecified\nTmdbId: 1773681\nName: Timothy Anderson\nOriginalName: Timothy Anderson\nPopularity: 0.169\nProfilePath: \n\nID: 00000000-0000-0000-0000-000000000000\nCharacter: Henchman\nBilledOrder: 1126\nGender: Male\nTmdbId: 1282303\nName: Alejandro Estornel\nOriginalName: Alejandro Estornel\nPopularity: 0.0387\nProfilePath: /hNEicTJNU3iCGQaULDDlCkqdD9I.jpg\n\nID: 00000000-0000-0000-0000-000000000000\nCharacter: Young Man\nBilledOrder: 1127\nGender: Male\nTmdbId: 2737347\nName: Daniel Weaver\nOriginalName: Daniel Weaver\nPopularity: 0.0537\nProfilePath: \n\nID: 00000000-0000-0000-0000-000000000000\nCharacter: College Guy\nBilledOrder: 1128\nGender: NotSetNotSpecified\nTmdbId: 202663\nName: Hunter Cochran\nOriginalName: Hunter Cochran\nPopularity: 0.095\nProfilePath: \n\nID: 00000000-0000-0000-0000-000000000000\nCharacter: College Guy\nBilledOrder: 1301\nGender: NotSetNotSpecified\nTmdbId: 4463868\nName: Matt Sims\nOriginalName: Matt Sims\nPopularity: 0.0143\nProfilePath: "
                , tvEpisodeViewModel.ToString());
    }
}
