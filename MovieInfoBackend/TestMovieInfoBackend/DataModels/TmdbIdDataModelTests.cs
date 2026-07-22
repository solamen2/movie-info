using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class TmdbIdDataModelTests
{
    private string tmdbHttpClientMovieIdResponse;
    private string tmdbHttpClientPersonIdResponse;
    private string tmdbHttpClientTvSeriesIdResponse;
    private string tmdbHttpClientTvEpisodeIdResponse;
    private string tmdbHttpClientTvSeasonIdResponse;

    public TmdbIdDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testMovieIdDataFilename = "TmdbHttpClientMovieIdResponse.json";
        string testPersonIdDataFilename = "TmdbHttpClientPersonIdResponse.json";
        string testTvSeriesIdDataFilename = "TmdbHttpClientTvSeriesIdResponse.json";
        string testTvEpisodeIdDataFilename = "TmdbHttpClientTvEpisodeIdResponse.json";
        string testTvSeasonIdDataFilename = "TmdbHttpClientTvSeasonIdResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieIdDataFilename}"))
        {
            tmdbHttpClientMovieIdResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieIdResponse))
        {
            throw new ArgumentException($"{testMovieIdDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonIdDataFilename}"))
        {
            tmdbHttpClientPersonIdResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonIdResponse))
        {
            throw new ArgumentException($"{testPersonIdDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesIdDataFilename}"))
        {
            tmdbHttpClientTvSeriesIdResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesIdResponse))
        {
            throw new ArgumentException($"{testTvSeriesIdDataFilename} is not valid test data.");
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
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {
        // Act / Assert

        {  // Movie  (blocks help prevent data leakage between tests)
            TmdbIdResponseDataModel? movieIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientMovieIdResponse);
            Assert.NotNull(movieIdResponse);
            // ----- Actual movie part -----
            Assert.NotNull(movieIdResponse.MovieResults);
            TmdbMovieIdDataModel[] movieIdDataModels = movieIdResponse.MovieResults;
            Assert.NotNull(movieIdDataModels);
            Assert.Single(movieIdDataModels);
            TmdbMovieIdDataModel movieIdDataModel = movieIdDataModels[0];
            Assert.False(String.IsNullOrWhiteSpace(movieIdDataModel.BackdropPath), "Movie id data model BackdropPath must not be empty");
            Assert.True(movieIdDataModel.TmdbId > 0, "Movie id data model TmdbId must be a positive number");
            Assert.False(String.IsNullOrWhiteSpace(movieIdDataModel.Title), "Movie id data model Title must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(movieIdDataModel.OriginalTitle), "Movie id data model OriginalTitle must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(movieIdDataModel.Overview), "Movie id data model Overview must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(movieIdDataModel.PosterPath), "Movie id data model PosterPath must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(movieIdDataModel.MediaType), "Movie id data model MediaType must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(movieIdDataModel.OriginalLanguage), "Movie id data model OriginalLanguage must not be empty");
            Assert.NotEmpty(movieIdDataModel.GenreIds);
            Assert.True(movieIdDataModel.Popularity >= 0.0, "Movie id data model Popularity must not be a negative decimal number");
            Assert.True(movieIdDataModel.VoteAverage >= 0.0, "Movie id data model VoteAverage must not be a negative decimal number");
            Assert.True(movieIdDataModel.VoteCount >= 0, "Movie id data model VoteCount must not be a negative number");
            // ----- End actual movie part -----
            Assert.NotNull(movieIdResponse.PersonResults);
            Assert.Empty(movieIdResponse.PersonResults);
            Assert.NotNull(movieIdResponse.TvResults);
            Assert.Empty(movieIdResponse.TvResults);
            Assert.NotNull(movieIdResponse.TvEpisodeResults);
            Assert.Empty(movieIdResponse.TvEpisodeResults);
            Assert.NotNull(movieIdResponse.TvSeasonResults);
            Assert.Empty(movieIdResponse.TvSeasonResults);
        }

        {  // Person
            TmdbIdResponseDataModel? personIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientPersonIdResponse);
            Assert.NotNull(personIdResponse);
            Assert.NotNull(personIdResponse.MovieResults);
            Assert.Empty(personIdResponse.MovieResults);
            // ----- Actual person part -----
            Assert.NotNull(personIdResponse.PersonResults);
            TmdbPersonIdDataModel[] personIdDataModels = personIdResponse.PersonResults;
            Assert.NotNull(personIdDataModels);
            Assert.Single(personIdDataModels);
            TmdbPersonIdDataModel personIdDataModel = personIdDataModels[0];
            Assert.True(personIdDataModel.TmdbId > 0, "Person id data model TmdbId must be a positive number");
            Assert.False(String.IsNullOrWhiteSpace(personIdDataModel.Name), "Person id data model Name must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(personIdDataModel.OriginalName), "Person id data model OriginalName must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(personIdDataModel.MediaType), "Person id data model MediaType must not be empty");
            Assert.True(personIdDataModel.Popularity >= 0.0, "Person id data model Popularity must not be a negative decimal number");
            Assert.True(personIdDataModel.Gender >= 0, "Person id data model Gender must not be a negative number");
            Assert.False(String.IsNullOrWhiteSpace(personIdDataModel.KnownForDepartment), "Person id data model KnownForDepartment must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(personIdDataModel.ProfilePath), "Person id data model ProfilePath must not be empty");
            Assert.NotNull(personIdDataModel.KnownFor);
            Assert.NotEmpty(personIdDataModel.KnownFor);
            // ----- End actual person part -----
            Assert.NotNull(personIdResponse.TvResults);
            Assert.Empty(personIdResponse.TvResults);
            Assert.NotNull(personIdResponse.TvEpisodeResults);
            Assert.Empty(personIdResponse.TvEpisodeResults);
            Assert.NotNull(personIdResponse.TvSeasonResults);
            Assert.Empty(personIdResponse.TvSeasonResults);
        }

        {  // TV Series
            TmdbIdResponseDataModel? tvSeriesIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientTvSeriesIdResponse);
            Assert.NotNull(tvSeriesIdResponse);
            Assert.NotNull(tvSeriesIdResponse.MovieResults);
            Assert.Empty(tvSeriesIdResponse.MovieResults);
            Assert.NotNull(tvSeriesIdResponse.PersonResults);
            Assert.Empty(tvSeriesIdResponse.PersonResults);
            // ----- Actual TV series part -----
            Assert.NotNull(tvSeriesIdResponse.TvResults);
            TmdbTvSeriesIdDataModel[] tvSeriesIdDataModels = tvSeriesIdResponse.TvResults;
            Assert.NotNull(tvSeriesIdDataModels);
            Assert.Single(tvSeriesIdDataModels);
            TmdbTvSeriesIdDataModel tvSeriesIdDataModel = tvSeriesIdDataModels[0];
            Assert.False(String.IsNullOrWhiteSpace(tvSeriesIdDataModel.BackdropPath), "TV series id data model BackdropPath must not be empty");
            Assert.True(tvSeriesIdDataModel.TmdbId > 0, "TV series id data model TmdbId must be a positive number");
            Assert.False(String.IsNullOrWhiteSpace(tvSeriesIdDataModel.Name), "TV series id data model Name must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(tvSeriesIdDataModel.OriginalName), "TV series id data model OriginalName must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(tvSeriesIdDataModel.Overview), "TV series id data model Overview must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(tvSeriesIdDataModel.PosterPath), "TV series id data model PosterPath must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(tvSeriesIdDataModel.MediaType), "TV series id data model MediaType must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(tvSeriesIdDataModel.OriginalLanguage), "TV series id data model OriginalLanguage must not be empty");
            Assert.NotEmpty(tvSeriesIdDataModel.GenreIds);
            Assert.True(tvSeriesIdDataModel.Popularity >= 0.0, "TV series id data model Popularity must not be a negative decimal number");
            Assert.True(tvSeriesIdDataModel.VoteAverage >= 0.0, "TV series id data model VoteAverage must not be a negative decimal number");
            Assert.True(tvSeriesIdDataModel.VoteCount >= 0, "TV series id data model VoteCount must not be a negative number");
            Assert.NotEmpty(tvSeriesIdDataModel.OriginCountry);
            // ----- End actual TV series part -----
            Assert.NotNull(tvSeriesIdResponse.TvEpisodeResults);
            Assert.Empty(tvSeriesIdResponse.TvEpisodeResults);
            Assert.NotNull(tvSeriesIdResponse.TvSeasonResults);
            Assert.Empty(tvSeriesIdResponse.TvSeasonResults);
        }

        {  // TV Episode
            TmdbIdResponseDataModel? tvEpisodeIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientTvEpisodeIdResponse);
            Assert.NotNull(tvEpisodeIdResponse);
            Assert.NotNull(tvEpisodeIdResponse.MovieResults);
            Assert.Empty(tvEpisodeIdResponse.MovieResults);
            Assert.NotNull(tvEpisodeIdResponse.PersonResults);
            Assert.Empty(tvEpisodeIdResponse.PersonResults);
            Assert.NotNull(tvEpisodeIdResponse.TvResults);
            Assert.Empty(tvEpisodeIdResponse.TvResults);
            // ----- Actual TV episode part -----
            Assert.NotNull(tvEpisodeIdResponse.TvEpisodeResults);
            TmdbIndividualTvEpisodeIdDataModel[] tvEpisodeIdDataModels = tvEpisodeIdResponse.TvEpisodeResults;
            Assert.NotNull(tvEpisodeIdDataModels);
            Assert.Single(tvEpisodeIdDataModels);
            TmdbIndividualTvEpisodeIdDataModel tvEpisodeIdDataModel = tvEpisodeIdDataModels[0];
            Assert.True(tvEpisodeIdDataModel.TmdbId > 0, "TV episode id data model TmdbId must be a positive number");
            Assert.False(String.IsNullOrWhiteSpace(tvEpisodeIdDataModel.Name), "TV episode id data model Name must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(tvEpisodeIdDataModel.Overview), "TV episode id data model Overview must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(tvEpisodeIdDataModel.MediaType), "TV episode id data model MediaType must not be empty");
            Assert.True(tvEpisodeIdDataModel.VoteAverage >= 0.0, "TV episode id data model VoteAverage must not be a negative decimal number");
            Assert.True(tvEpisodeIdDataModel.VoteCount >= 0, "TV episode id data model VoteCount must not be a negative number");
            Assert.True(tvEpisodeIdDataModel.EpisodeNumber > 0, "TV episode id data model EpisodeNumber must be a positive number");
            Assert.False(String.IsNullOrWhiteSpace(tvEpisodeIdDataModel.EpisodeType), "TV episode id data model EpisodeType must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(tvEpisodeIdDataModel.ProductionCode), "TV episode id data model ProductionCode must not be empty");
            Assert.True(tvEpisodeIdDataModel.Runtime > 0, "TV episode id data model Runtime must be a positive number");
            Assert.True(tvEpisodeIdDataModel.SeasonNumber > 0, "TV episode id data model SeasonNumber must be a positive number");
            Assert.True(tvEpisodeIdDataModel.ShowId > 0, "TV episode id data model ShowId must be a positive number");
            Assert.False(String.IsNullOrWhiteSpace(tvEpisodeIdDataModel.StillPath), "TV episode id data model StillPath must not be empty");
            // ----- End actual TV episode part -----
            Assert.NotNull(tvEpisodeIdResponse.TvSeasonResults);
            Assert.Empty(tvEpisodeIdResponse.TvSeasonResults);
        }

        {  // TV Season
            TmdbIdResponseDataModel? tvSeasonIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientTvSeasonIdResponse);
            Assert.NotNull(tvSeasonIdResponse);
            Assert.NotNull(tvSeasonIdResponse);
            Assert.NotNull(tvSeasonIdResponse.MovieResults);
            Assert.Empty(tvSeasonIdResponse.MovieResults);
            Assert.NotNull(tvSeasonIdResponse.PersonResults);
            Assert.Empty(tvSeasonIdResponse.PersonResults);
            Assert.NotNull(tvSeasonIdResponse.TvResults);
            Assert.Empty(tvSeasonIdResponse.TvResults);
            Assert.NotNull(tvSeasonIdResponse.TvEpisodeResults);
            Assert.Empty(tvSeasonIdResponse.TvEpisodeResults);
            // ----- Actual TV season part -----
            Assert.NotNull(tvSeasonIdResponse.TvSeasonResults);
            TmdbTvSeasonIdDataModel[] tvSeasonIdDataModels = tvSeasonIdResponse.TvSeasonResults;
            Assert.NotNull(tvSeasonIdDataModels);
            Assert.Single(tvSeasonIdDataModels);
            TmdbTvSeasonIdDataModel tvSeasonIdDataModel = tvSeasonIdDataModels[0];
            Assert.True(tvSeasonIdDataModel.TmdbId > 0, "TV season id data model TmdbId must be a positive number");
            Assert.False(String.IsNullOrWhiteSpace(tvSeasonIdDataModel.Name), "TV season id data model Name must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(tvSeasonIdDataModel.Overview), "TV season id data model Overview must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(tvSeasonIdDataModel.PosterPath), "TV season id data model PosterPath must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(tvSeasonIdDataModel.MediaType), "TV season id data model MediaType must not be empty");
            Assert.True(tvSeasonIdDataModel.VoteAverage >= 0.0, "TV season id data model VoteAverage must not be a negative decimal number");
            Assert.True(tvSeasonIdDataModel.SeasonNumber > 0, "TV season id data model SeasonNumber must be a positive number");
            Assert.True(tvSeasonIdDataModel.ShowId > 0, "TV season id data model ShowId must be a positive number");
            Assert.True(tvSeasonIdDataModel.EpisodeCount > 0, "TV season id data model EpisodeCount must be a positive number");
            // ----- End actual TV season part -----
        }
    }

    [Fact]
    public void TmdbIdDataModel_ValidModelToString_ReturnsCorrectValue()
    {
        // Act
        
        TmdbIdResponseDataModel? movieIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientMovieIdResponse);
        TmdbIdResponseDataModel? personIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientPersonIdResponse);
        TmdbIdResponseDataModel? tvSeriesIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientTvSeriesIdResponse);
        TmdbIdResponseDataModel? tvEpisodeIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientTvEpisodeIdResponse);
        TmdbIdResponseDataModel? tvSeasonIdResponse = TmdbHttpClient.GetIdModelFromResponse(tmdbHttpClientTvSeasonIdResponse);

        // Assert

        Assert.NotNull(movieIdResponse);
        Assert.NotNull(personIdResponse);
        Assert.NotNull(tvSeriesIdResponse);
        Assert.NotNull(tvEpisodeIdResponse);
        Assert.NotNull(tvSeasonIdResponse);

        Assert.Equal(
            "MovieResults:\n*****\nAdult: False\nBackdropPath: /zfbjgQE1uSd9wiPTX4VzsLi0rGG.jpg\nTmdbd: 278\nTitle: Example Movie\nOriginalTitle: Example Movie\nOverview: Things happen in this movie.\nPosterPath: /9cqNxx0GxF0bflZmeSMuL5tnGzr.jpg\nMediaType: movie\nOriginalLanguage: en\nGenreIds: 18, 80\nPopularity: 67.3993\nReleaseDate: 2016-09-23\nSoftcore: False\nVideo: False\nVoteAverage: 8.722\nVoteCount: 30473\n*****\nPersonResults:\n*****\n\n*****\nTvResults:\n*****\n\n*****\nTvEpisodeResults:\n*****\n\n*****\nTvSeasonResults:\n*****\n"
                , movieIdResponse.ToString());
        Assert.Equal(
            "MovieResults:\n*****\n\n*****\nPersonResults:\n*****\nAdult: False\nTmdbd: 11863\nName: Example Smith\nOriginalName: Example Smith\nMediaType: person\nPopularity: 2.564\nGender: 1\nKnownForDepartment: Acting\nProfilePath: /xKe52w4tpv61ohz9iz75wNdzcwZ.jpg\nKnownFor:\n*****\nAdult: False\nBackdropPath: /m2QvGozzKz5ux74gKocn4kFTGz1.jpg\nTmdbd: 796\nTitle: Movie Number 1\nOriginalTitle: Movie Number 1\nOverview: Many things happen in this first movie.\nPosterPath: /76cCsRtQ5MJBAqoigojXsLXLJwh.jpg\nMediaType: movie\nOriginalLanguage: en\nGenreIds: 18, 10749\nPopularity: 29.7672\nReleaseDate: 1999-03-05\nSoftcore: False\nVideo: False\nVoteAverage: 6.816\nVoteCount: 3578\n\nAdult: False\nBackdropPath: /yefSD6lp3jOBYkBeY5QxRRKTJME.jpg\nTmdbd: 1970\nTitle: Movie Number 2\nOriginalTitle: Movie Number 2\nOverview: Many other things happen in this second movie.\nPosterPath: /7vPAVPKYexQVmvC578wPLn2CGCL.jpg\nMediaType: movie\nOriginalLanguage: en\nGenreIds: 27, 9648, 53\nPopularity: 4.6788\nReleaseDate: 2004-10-22\nSoftcore: False\nVideo: False\nVoteAverage: 5.931\nVoteCount: 3188\n\nAdult: False\nBackdropPath: /1RAxtBxslR4OZCZC1vxIRUxjR7a.jpg\nTmdbd: 9637\nTitle: Movie Number 3\nOriginalTitle: Movie Number 3\nOverview: Yet more things happen in this third movie.\nPosterPath: /mTAiBJGg8mqEfnYHHbi37ZoRSZm.jpg\nMediaType: movie\nOriginalLanguage: en\nGenreIds: 9648, 12, 35\nPopularity: 6.6028\nReleaseDate: 2002-06-14\nSoftcore: False\nVideo: False\nVoteAverage: 6.106\nVoteCount: 4782\n*****\nTvResults:\n*****\n\n*****\nTvEpisodeResults:\n*****\n\n*****\nTvSeasonResults:\n*****\n"
                , personIdResponse.ToString());
        Assert.Equal(
            "MovieResults:\n*****\n\n*****\nPersonResults:\n*****\n\n*****\nTvResults:\n*****\nAdult: False\nBackdropPath: /lBmlLro9ZfY815ZXE5NKhYNxPRQ.jpg\nTmdbd: 95\nName: Example TV Series\nOriginalName: Example TV Series\nOverview: This series went on for a while and many exciting events happened in it.\nPosterPath: /y7fVZkyheCEQHDUEHwNmYENGfT2.jpg\nMediaType: tv\nOriginalLanguage: en\nGenreIds: 35, 18, 10765\nPopularity: 54.4575\nFirstAirDate: 1997-03-10\nSoftcore: False\nVoteAverage: 8.1\nVoteCount: 1990\nOriginCountry: US\n*****\nTvEpisodeResults:\n*****\n\n*****\nTvSeasonResults:\n*****\n"
                , tvSeriesIdResponse.ToString());
        Assert.Equal(
            "MovieResults:\n*****\n\n*****\nPersonResults:\n*****\n\n*****\nTvResults:\n*****\n\n*****\nTvEpisodeResults:\n*****\nTmdbd: 949534\nName: Example TV Episode\nOverview: This episode has a lot of interesting things in it, especially music.\nMediaType: tv_episode\nVoteAverage: 8.8\nVoteCount: 44\nAirDate: 2001-11-06\nEpisodeNumber: 7\nEpisodeType: standard\nProductionCode: 6ABB07\nRuntime: 50\nSeasonNumber: 6\nShowId: 95\nStillPath: /m6DAoR7I3UAeyjGA5ekLf5KQDfS.jpg\n*****\nTvSeasonResults:\n*****\n"
                , tvEpisodeIdResponse.ToString());
        Assert.Equal(
            "MovieResults:\n*****\n\n*****\nPersonResults:\n*****\n\n*****\nTvResults:\n*****\n\n*****\nTvEpisodeResults:\n*****\n\n*****\nTvSeasonResults:\n*****\nTmdbd: 59470\nName: Season 6\nOverview: This was the sixth season of the show. Flo Welch is a big fan of this one.\nPosterPath: /aAJLnT7nSD5JaZcyUElyp4dYpmq.jpg\nMediaType: tv_season\nVoteAverage: 7.7\nAirDate: 2001-10-02\nSeasonNumber: 6\nShowId: 95\nEpisodeCount: 22"
                , tvSeasonIdResponse.ToString());
    }
}