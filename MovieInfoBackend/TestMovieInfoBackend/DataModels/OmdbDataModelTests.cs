using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class OmdbDataModelTests
{
    private string omdbHttpClientMovieResponse;
    private string omdbHttpClientTvEpisodeResponse;
    private string omdbHttpClientTvSeriesResponse;

    public OmdbDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testMovieDataFilename = "OmdbHttpClientMovieResponse.json";
        string testTvEpisodeDataFilename = "OmdbHttpClientTvEpisodeResponse.json";
        string testTvSeriesDataFilename = "OmdbHttpClientTvSeriesResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieDataFilename}"))
        {
            omdbHttpClientMovieResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(omdbHttpClientMovieResponse))
        {
            throw new ArgumentException($"{testMovieDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvEpisodeDataFilename}"))
        {
            omdbHttpClientTvEpisodeResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(omdbHttpClientTvEpisodeResponse))
        {
            throw new ArgumentException($"{testTvEpisodeDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesDataFilename}"))
        {
            omdbHttpClientTvSeriesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(omdbHttpClientTvSeriesResponse))
        {
            throw new ArgumentException($"{testTvSeriesDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {
        // Act
        
        OmdbResponseDataModel? omdbMovieResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientMovieResponse);
        OmdbResponseDataModel? omdbTvEpisodeResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientTvEpisodeResponse);
        OmdbResponseDataModel? omdbTvSeriesResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientTvSeriesResponse);
        
        // Assert
        
        // Movie
        Assert.NotNull(omdbMovieResponse);
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Title), "Movie Title must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Year), "Movie Year must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Rated), "Movie Rated must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Released), "Movie Released must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Runtime), "Movie Runtime must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Genre), "Movie Genre must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Director), "Movie Director must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Writer), "Movie Writer must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Actors), "Movie Actors must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Plot), "Movie Plot must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Language), "Movie Language must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Country), "Movie Country must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Awards), "Movie Awards must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Poster), "Movie Poster must not be empty");
        foreach (OmdbRatingDataModel rating in omdbMovieResponse.Ratings)
        {
            Assert.False(String.IsNullOrWhiteSpace(rating.Source), "Movie Rating Source must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(rating.Value), "Movie Rating Value must not be empty");
        }
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Metascore), "Movie Metascore must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.ImdbRating), "Movie ImdbRating must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.ImdbVotes), "Movie ImdbVotes must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.ImdbId), "Movie ImdbId must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Type), "Movie Type must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.BoxOffice), "Movie BoxOffice must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbMovieResponse.Response), "Movie Response must not be empty");

        // TV Episode
        Assert.NotNull(omdbTvEpisodeResponse);
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Title), "TV Episode Title must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Year), "TV Episode Year must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Rated), "TV Episode Rated must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Released), "TV Episode Released must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Season), "TV Episode Season must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Episode), "TV Episode Episode must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Runtime), "TV Episode Runtime must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Genre), "TV Episode Genre must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Director), "TV Episode Director must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Writer), "TV Episode Writer must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Actors), "TV Episode Actors must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Plot), "TV Episode Plot must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Language), "TV Episode Language must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Country), "TV Episode Country must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Awards), "TV Episode Awards must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Poster), "TV Episode Poster must not be empty");
        foreach (OmdbRatingDataModel rating in omdbTvEpisodeResponse.Ratings)
        {
            Assert.False(String.IsNullOrWhiteSpace(rating.Source), "TV Episode Rating Source must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(rating.Value), "TV Episode Rating Value must not be empty");
        }
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Metascore), "TV Episode Metascore must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.ImdbRating), "TV Episode ImdbRating must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.ImdbVotes), "TV Episode ImdbVotes must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.ImdbId), "TV Episode ImdbId must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.SeriesId), "TV Episode SeriesId must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Type), "TV Episode Type must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvEpisodeResponse.Response), "TV Episode Response must not be empty");

        // TV Series
        Assert.NotNull(omdbTvSeriesResponse);
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Title), "TV Series Title must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Year), "TV Series Year must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Rated), "TV Series Rated must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Released), "TV Series Released must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Runtime), "TV Series Runtime must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Genre), "TV Series Genre must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Director), "TV Series Director must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Writer), "TV Series Writer must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Actors), "TV Series Actors must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Plot), "TV Series Plot must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Language), "TV Series Language must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Country), "TV Series Country must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Awards), "TV Series Awards must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Poster), "TV Series Poster must not be empty");
        foreach (OmdbRatingDataModel rating in omdbTvSeriesResponse.Ratings)
        {
            Assert.False(String.IsNullOrWhiteSpace(rating.Source), "TV Series Rating Source must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(rating.Value), "TV Series Rating Value must not be empty");
        }
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Metascore), "TV Series Metascore must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.ImdbRating), "TV Series ImdbRating must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.ImdbVotes), "TV Series ImdbVotes must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.ImdbId), "TV Series ImdbId must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Type), "TV Series Type must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.TotalSeasons), "TV Series TotalSeasons must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(omdbTvSeriesResponse.Response), "TV Series Response must not be empty");
    }

    [Fact]
    public void OmdbResponseDataModel_ValidModelToString_ReturnsCorrectValue()
    {
        // Act

        OmdbResponseDataModel? omdbMovieResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientMovieResponse);
        OmdbResponseDataModel? omdbTvEpisodeResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientTvEpisodeResponse);
        OmdbResponseDataModel? omdbTvSeriesResponse = OmdbHttpClient.GetModelFromResponse(omdbHttpClientTvSeriesResponse);

        // Assert

        Assert.NotNull(omdbMovieResponse);
        Assert.NotNull(omdbTvEpisodeResponse);
        Assert.NotNull(omdbTvSeriesResponse);

        Assert.Equal(
            "Title: Example Movie\nYear: 2016\nRated: R\nReleased: 14 Oct 2016\nSeason: \nEpisode: \nRuntime: 142 min\nGenre: Drama\nDirector: Example Director\nWriter: Example Writer, Example Director\nActors: Example Actress 1, Example Actor 1, Example Actress 2\nPlot: Seriously, some things definitely happen in this movie.\nLanguage: English\nCountry: United States\nAwards: Nominated for 7 Oscars. 21 wins & 42 nominations total\nPoster: https://m.media-amazon.com/images/M/MV5BMDAyY2FhYjctNDc5OS00MDNlLThiMGUtY2UxYWVkNGY2ZjljXkEyXkFqcGc@._V1_QL75_UX380_CR0,4,380,562_.jpg\nRatings:\n*****\nSource: Internet Movie Database\nValue: 9.3/10\n\nSource: Rotten Tomatoes\nValue: 89%\n\nSource: Metacritic\nValue: 82/100\n*****\nMetascore: 82\nImdbRating: 9.3\nImdbVotes: 3,182,645\nImdbId: tt0000001\nSeriesId: \nType: movie\nDVD: N/A\nTotal Seasons: \nBoxOffice: $28,767,189\nProduction: N/A\nWebsite: N/A\nResponse: True"
                , omdbMovieResponse.ToString());
        Assert.Equal(
            "Title: Example TV Episode\nYear: 2001\nRated: 13+\nReleased: 06 Nov 2001\nSeason: 6\nEpisode: 7\nRuntime: 50 min\nGenre: Action, Drama, Fantasy\nDirector: Example Creator Martinez\nWriter: Example Creator Martinez, Rebecca Kirshner, Steven S. DeKnight\nActors: Example Smith, Example Actor 2, Emma Caulfield Ford\nPlot: Tons of interesting stuff happens, including lots of music.\nLanguage: English\nCountry: United States\nAwards: N/A\nPoster: https://m.media-amazon.com/images/M/MV5BZTVkOWEzNzUtMjVkOS00Y2QzLTk2MGQtN2VkOGE3NTBjODI5XkEyXkFqcGdeQXVyMDM2NDM2MQ@@._V1_SX300.jpg\nRatings:\n*****\nSource: Internet Movie Database\nValue: 9.7/10\n*****\nMetascore: N/A\nImdbRating: 9.7\nImdbVotes: 11006\nImdbId: tt0533466\nSeriesId: tt0118276\nType: episode\nDVD: \nTotal Seasons: \nBoxOffice: \nProduction: \nWebsite: \nResponse: True"
                , omdbTvEpisodeResponse.ToString());
        Assert.Equal(
            "Title: Example TV Series\nYear: 1997–2003\nRated: TV-14\nReleased: 10 Mar 1997\nSeason: \nEpisode: \nRuntime: 44 min\nGenre: Action, Adventure, Drama\nDirector: N/A\nWriter: Example Creator Martinez\nActors: Example Smith, Example Actor 2, Example Actress 4\nPlot: This TV show has an awful lot of story in it.\nLanguage: English\nCountry: United States, Japan\nAwards: Won 2 Primetime Emmys. 55 wins & 136 nominations total\nPoster: https://m.media-amazon.com/images/M/MV5BMDk4MGVkNDAtZjQwZi00MDc3LWE4MmEtY2YyODQ2NDQyMjgxXkEyXkFqcGc@._V1_SX300.jpg\nRatings:\n*****\nSource: Internet Movie Database\nValue: 8.3/10\n*****\nMetascore: N/A\nImdbRating: 8.3\nImdbVotes: 172,659\nImdbId: tt0118276\nSeriesId: \nType: series\nDVD: \nTotal Seasons: 7\nBoxOffice: \nProduction: \nWebsite: \nResponse: True"
                , omdbTvSeriesResponse.ToString());
    }
}