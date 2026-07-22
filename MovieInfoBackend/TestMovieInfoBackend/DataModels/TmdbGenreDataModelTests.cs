using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class TmdbGenreDataModelTests
{
    private string tmdbHttpClientMovieGenresResponse;
    private string tmdbHttpClientTvSeriesGenresResponse;

    public TmdbGenreDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testMovieGenresDataFilename = "TmdbHttpClientMovieGenresResponse.json";
        string testTvSeriesGenresDataFilename = "TmdbHttpClientTvSeriesGenresResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testMovieGenresDataFilename}"))
        {
            tmdbHttpClientMovieGenresResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientMovieGenresResponse))
        {
            throw new ArgumentException($"{testMovieGenresDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvSeriesGenresDataFilename}"))
        {
            tmdbHttpClientTvSeriesGenresResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvSeriesGenresResponse))
        {
            throw new ArgumentException($"{testTvSeriesGenresDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {
        // Act
        
        TmdbGenresResponseDataModel? movieGenresResponse = TmdbHttpClient.GetGenresModelFromResponse(tmdbHttpClientMovieGenresResponse);
        TmdbGenresResponseDataModel? tvSeriesGenresResponse = TmdbHttpClient.GetGenresModelFromResponse(tmdbHttpClientTvSeriesGenresResponse);
        
        // Assert
        
        // Movie
        Assert.NotNull(movieGenresResponse);
        TmdbGenreDataModel[] movieGenres = movieGenresResponse.Genres;
        Assert.NotEmpty(movieGenres);
        TmdbGenreDataModel firstMovieGenre = movieGenres[0];
        TmdbGenreDataModel lastMovieGenre = movieGenres[movieGenres.Length - 1];
        Assert.True(firstMovieGenre.Id > 0, "First movie genre Id must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(firstMovieGenre.Name), "First movie genre Name must not be empty");
        Assert.True(lastMovieGenre.Id > 0, "Last movie genre Id must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(lastMovieGenre.Name), "Last movie genre Name must not be empty");

        // TV Series
        Assert.NotNull(tvSeriesGenresResponse);
        TmdbGenreDataModel[] tvSeriesGenres = tvSeriesGenresResponse.Genres;
        Assert.NotEmpty(tvSeriesGenres);
        TmdbGenreDataModel firstTvSeriesGenre = tvSeriesGenres[0];
        TmdbGenreDataModel lastTvSeriesGenre = tvSeriesGenres[tvSeriesGenres.Length - 1];
        Assert.True(firstTvSeriesGenre.Id > 0, "First TV series genre Id must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(firstTvSeriesGenre.Name), "First TV series genre Name must not be empty");
        Assert.True(lastTvSeriesGenre.Id > 0, "Last TV series genre Id must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(lastTvSeriesGenre.Name), "Last TV series genre Name must not be empty");
    }

    [Fact]
    public void TmdbGenreDataModel_ValidModelToString_ReturnsCorrectValue()
    {
        // Act
        
        TmdbGenresResponseDataModel? movieGenresResponse = TmdbHttpClient.GetGenresModelFromResponse(tmdbHttpClientMovieGenresResponse);
        TmdbGenresResponseDataModel? tvSeriesGenresResponse = TmdbHttpClient.GetGenresModelFromResponse(tmdbHttpClientTvSeriesGenresResponse);
        
        // Assert
        
        Assert.NotNull(movieGenresResponse);
        Assert.NotNull(tvSeriesGenresResponse);
        Assert.Equal(
            "Genres:\n*****\nId: 28\nName: Action\n\nId: 12\nName: Adventure\n\nId: 16\nName: Animation\n\nId: 35\nName: Comedy\n\nId: 80\nName: Crime\n\nId: 99\nName: Documentary\n\nId: 18\nName: Drama\n\nId: 10751\nName: Family\n\nId: 14\nName: Fantasy\n\nId: 36\nName: History\n\nId: 27\nName: Horror\n\nId: 10402\nName: Music\n\nId: 9648\nName: Mystery\n\nId: 10749\nName: Romance\n\nId: 878\nName: Science Fiction\n\nId: 10770\nName: TV Movie\n\nId: 53\nName: Thriller\n\nId: 10752\nName: War\n\nId: 37\nName: Western"
                , movieGenresResponse.ToString());
        Assert.Equal(
            "Genres:\n*****\nId: 10759\nName: Action & Adventure\n\nId: 16\nName: Animation\n\nId: 35\nName: Comedy\n\nId: 80\nName: Crime\n\nId: 99\nName: Documentary\n\nId: 18\nName: Drama\n\nId: 10751\nName: Family\n\nId: 10762\nName: Kids\n\nId: 9648\nName: Mystery\n\nId: 10763\nName: News\n\nId: 10764\nName: Reality\n\nId: 10765\nName: Sci-Fi & Fantasy\n\nId: 10766\nName: Soap\n\nId: 10767\nName: Talk\n\nId: 10768\nName: War & Politics\n\nId: 37\nName: Western"
                , tvSeriesGenresResponse.ToString());
    }
}