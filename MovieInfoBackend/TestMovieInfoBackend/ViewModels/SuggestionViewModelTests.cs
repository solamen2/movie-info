using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.DataModels;

public class SuggestionViewModelTests
{
    private string movieHttpClientResponse1;
    private string movieHttpClientResponse2;
    private string noSuggestionsErrorResponse;
    private string badDataErrorResponse;

    public SuggestionViewModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testDataFilename1 = "MovieHttpClientResponse1.json";
        string testDataFilename2 = "MovieHttpClientResponse2.json";
        string noSuggestionsDataFilename = "MovieHttpClientErrorResponse1.json";
        string badDataFilename = "MovieHttpClientErrorResponse2.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testDataFilename1}"))
        {
            movieHttpClientResponse1 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(movieHttpClientResponse1))
        {
            throw new ArgumentException($"{testDataFilename1} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testDataFilename2}"))
        {
            movieHttpClientResponse2 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(movieHttpClientResponse2))
        {
            throw new ArgumentException($"{testDataFilename2} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{noSuggestionsDataFilename}"))
        {
            noSuggestionsErrorResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(noSuggestionsErrorResponse))
        {
            throw new ArgumentException($"{noSuggestionsErrorResponse} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{badDataFilename}"))
        {
            badDataErrorResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(badDataErrorResponse))
        {
            throw new ArgumentException($"{badDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetViewModelFromDataModel_ValidDataModel_ReturnsValidViewModel()
    {
        // Act

        MovieSuggestionsResponseDataModel? actual1 = MovieHttpClient.GetModelFromResponse(movieHttpClientResponse1);
        MovieSuggestionsResponseDataModel? actual2 = MovieHttpClient.GetModelFromResponse(movieHttpClientResponse2);

        // Assert

        // Test SuggestionViewModel creation and properties
        {  // First movie suggestions data file  (blocks help prevent data leakage between tests)
            SuggestionDataModel[]? suggestions1 = actual1?.Suggestions;
            Assert.Equal(8, suggestions1?.Length);

            {  // First suggestion - Person
                SuggestionDataModel? suggestionDataModel1 = suggestions1?[0];
                Assert.NotNull(suggestionDataModel1);
                SuggestionViewModel suggestionViewModel1 = new(suggestionDataModel1);

                Assert.NotEqual(Guid.Empty, suggestionViewModel1.ID);
                Assert.NotNull(suggestionViewModel1.Image);
                Assert.True(suggestionViewModel1.Image.Height > 0, "Person image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel1.Image.ImageURL), "Person ImageURL must not be empty");
                Assert.True(suggestionViewModel1.Image.Width > 0, "Person Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel1.ItemID), "Person ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel1.Name), "Person Name must not be empty");
                Assert.Equal(SearchResultType.Person, suggestionViewModel1.SearchType);
                Assert.Null(suggestionViewModel1.MediaType);
                Assert.True(suggestionViewModel1.Rank > 0, "Person Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel1.KnownFor), "Person KnownFor must not be empty");
            }

            {  // Second suggestion - Movie
                SuggestionDataModel? suggestionDataModel2 = suggestions1?[1];
                Assert.NotNull(suggestionDataModel2);
                SuggestionViewModel suggestionViewModel2 = new(suggestionDataModel2);

                Assert.NotEqual(Guid.Empty, suggestionViewModel2.ID);
                Assert.NotNull(suggestionViewModel2.Image);
                Assert.True(suggestionViewModel2.Image.Height > 0, "Movie image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel2.Image.ImageURL), "Movie ImageURL must not be empty");
                Assert.True(suggestionViewModel2.Image.Width > 0, "Movie Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel2.ItemID), "Movie ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel2.Name), "Movie Name must not be empty");
                Assert.Equal(SearchResultType.Media, suggestionViewModel2.SearchType);
                MediaResultType? mediaType = suggestionViewModel2.MediaType;
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.Movie.Value, mediaType.Value);
                Assert.True(suggestionViewModel2.Rank > 0, "Movie Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel2.KnownFor), "Movie KnownFor must not be empty");
                Assert.True(suggestionViewModel2.Year > 0, "Movie Year must be a positive number");
            }

            {  // Third suggestion - TV Series
                SuggestionDataModel? suggestionDataModel3 = suggestions1?[2];
                Assert.NotNull(suggestionDataModel3);
                SuggestionViewModel suggestionViewModel3 = new(suggestionDataModel3);

                Assert.NotEqual(Guid.Empty, suggestionViewModel3.ID);
                Assert.NotNull(suggestionViewModel3.Image);
                Assert.True(suggestionViewModel3.Image.Height > 0, "TV series image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel3.Image.ImageURL), "TV series ImageURL must not be empty");
                Assert.True(suggestionViewModel3.Image.Width > 0, "TV series Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel3.ItemID), "TV series ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel3.Name), "TV series Name must not be empty");
                Assert.Equal(SearchResultType.Media, suggestionViewModel3.SearchType);
                MediaResultType? mediaType = suggestionViewModel3.MediaType;
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.TVSeries.Value, mediaType.Value);
                Assert.True(suggestionViewModel3.Rank > 0, "TV series Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel3.KnownFor), "TV series KnownFor must not be empty");
                Assert.True(suggestionViewModel3.Year > 0, "TV series Year must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel3.Years), "TV series Years must not be empty");
            }

            {  // Fourth suggestion - TV Mini Series
                SuggestionDataModel? suggestionDataModel4 = suggestions1?[3];
                Assert.NotNull(suggestionDataModel4);
                SuggestionViewModel suggestionViewModel4 = new(suggestionDataModel4);

                Assert.NotEqual(Guid.Empty, suggestionViewModel4.ID);
                Assert.NotNull(suggestionViewModel4.Image);
                Assert.True(suggestionViewModel4.Image.Height > 0, "TV mini series image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel4.Image.ImageURL), "TV mini series ImageURL must not be empty");
                Assert.True(suggestionViewModel4.Image.Width > 0, "TV mini series Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel4.ItemID), "TV mini series ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel4.Name), "TV mini series Name must not be empty");
                Assert.Equal(SearchResultType.Media, suggestionViewModel4.SearchType);
                MediaResultType? mediaType = suggestionViewModel4.MediaType;
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.TVMiniSeries.Value, mediaType.Value);
                Assert.True(suggestionViewModel4.Rank > 0, "TV mini series Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel4.KnownFor), "TV mini series KnownFor must not be empty");
                Assert.True(suggestionViewModel4.Year > 0, "TV mini series Year must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel4.Years), "TV mini series Years must not be empty");
            }

            {  // Fifth suggestion - TV Movie (no image)
                SuggestionDataModel? suggestionDataModel5 = suggestions1?[4];
                Assert.NotNull(suggestionDataModel5);
                SuggestionViewModel suggestionViewModel5 = new(suggestionDataModel5);

                Assert.NotEqual(Guid.Empty, suggestionViewModel5.ID);
                Assert.Null(suggestionViewModel5.Image);
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel5.ItemID), "TV movie ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel5.Name), "TV movie Name must not be empty");
                Assert.Equal(SearchResultType.Media, suggestionViewModel5.SearchType);
                MediaResultType? mediaType = suggestionViewModel5.MediaType;
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.TVMovie.Value, mediaType.Value);
                Assert.True(suggestionViewModel5.Rank > 0, "TV movie Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel5.KnownFor), "TV movie KnownFor must not be empty");
                Assert.True(suggestionViewModel5.Year > 0, "TV movie Year must be a positive number");
            }

            {  // Sixth suggestion - TV Special
                SuggestionDataModel? suggestionDataModel6 = suggestions1?[5];
                Assert.NotNull(suggestionDataModel6);
                SuggestionViewModel suggestionViewModel6 = new(suggestionDataModel6);

                Assert.NotEqual(Guid.Empty, suggestionViewModel6.ID);
                Assert.NotNull(suggestionViewModel6.Image);
                Assert.True(suggestionViewModel6.Image.Height > 0, "TV special image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel6.Image.ImageURL), "TV special ImageURL must not be empty");
                Assert.True(suggestionViewModel6.Image.Width > 0, "TV special Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel6.ItemID), "TV special ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel6.Name), "TV special Name must not be empty");
                Assert.Equal(SearchResultType.Media, suggestionViewModel6.SearchType);
                MediaResultType? mediaType = suggestionViewModel6.MediaType;
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.TVSpecial.Value, mediaType.Value);
                Assert.True(suggestionViewModel6.Rank > 0, "TV special Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel6.KnownFor), "TV special KnownFor must not be empty");
                Assert.True(suggestionViewModel6.Year > 0, "TV special Year must be a positive number");
            }

            {  // Seventh suggestion - TV Short
                SuggestionDataModel? suggestionDataModel7 = suggestions1?[6];
                Assert.NotNull(suggestionDataModel7);
                SuggestionViewModel suggestionViewModel7 = new(suggestionDataModel7);

                Assert.NotEqual(Guid.Empty, suggestionViewModel7.ID);
                Assert.NotNull(suggestionViewModel7.Image);
                Assert.True(suggestionViewModel7.Image.Height > 0, "TV short image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel7.Image.ImageURL), "TV short ImageURL must not be empty");
                Assert.True(suggestionViewModel7.Image.Width > 0, "TV short Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel7.ItemID), "TV short ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel7.Name), "TV short Name must not be empty");
                Assert.Equal(SearchResultType.Media, suggestionViewModel7.SearchType);
                MediaResultType? mediaType = suggestionViewModel7.MediaType;
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.TVShort.Value, mediaType.Value);
                Assert.True(suggestionViewModel7.Rank > 0, "TV short Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel7.KnownFor), "TV short KnownFor must not be empty");
                Assert.True(suggestionViewModel7.Year > 0, "TV short Year must be a positive number");
            }

            {  // Eighth suggestion - Short
                SuggestionDataModel? suggestionDataModel8 = suggestions1?[7];
                Assert.NotNull(suggestionDataModel8);
                SuggestionViewModel suggestionViewModel8 = new(suggestionDataModel8);

                Assert.NotEqual(Guid.Empty, suggestionViewModel8.ID);
                Assert.NotNull(suggestionViewModel8.Image);
                Assert.True(suggestionViewModel8.Image.Height > 0, "Short image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel8.Image.ImageURL), "Short ImageURL must not be empty");
                Assert.True(suggestionViewModel8.Image.Width > 0, "Short Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel8.ItemID), "Short ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel8.Name), "Short Name must not be empty");
                Assert.Equal(SearchResultType.Media, suggestionViewModel8.SearchType);
                MediaResultType? mediaType = suggestionViewModel8.MediaType;
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.Short.Value, mediaType.Value);
                Assert.True(suggestionViewModel8.Rank > 0, "Short Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel8.KnownFor), "Short KnownFor must not be empty");
                Assert.True(suggestionViewModel8.Year > 0, "Short Year must be a positive number");
            }
        }  // End first movie suggestions data file

        {  // Second movie suggestions data file
            SuggestionDataModel[]? suggestions2 = actual2?.Suggestions;
            Assert.Equal(6, suggestions2?.Length);

            {  // Ninth suggestion - Video Game
                SuggestionDataModel? suggestionDataModel9 = suggestions2?[0];
                Assert.NotNull(suggestionDataModel9);
                SuggestionViewModel suggestionViewModel9 = new(suggestionDataModel9);

                Assert.NotEqual(Guid.Empty, suggestionViewModel9.ID);
                Assert.NotNull(suggestionViewModel9.Image);
                Assert.True(suggestionViewModel9.Image.Height > 0, "Video game image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel9.Image.ImageURL), "Video game ImageURL must not be empty");
                Assert.True(suggestionViewModel9.Image.Width > 0, "Video game Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel9.ItemID), "Video game ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel9.Name), "Video game Name must not be empty");
                Assert.Equal(SearchResultType.Media, suggestionViewModel9.SearchType);
                MediaResultType? mediaType = suggestionViewModel9.MediaType;
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.VideoGame.Value, mediaType.Value);
                Assert.True(suggestionViewModel9.Rank > 0, "Video game Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel9.KnownFor), "Video game KnownFor must not be empty");
                Assert.True(suggestionViewModel9.Year > 0, "Video game Year must be a positive number");
            }

            {  // Tenth suggestion - Video
                SuggestionDataModel? suggestionDataModel10 = suggestions2?[1];
                Assert.NotNull(suggestionDataModel10);
                SuggestionViewModel suggestionViewModel10 = new(suggestionDataModel10);

                Assert.NotEqual(Guid.Empty, suggestionViewModel10.ID);
                Assert.NotNull(suggestionViewModel10.Image);
                Assert.True(suggestionViewModel10.Image.Height > 0, "Video 1 image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel10.Image.ImageURL), "Video 1 ImageURL must not be empty");
                Assert.True(suggestionViewModel10.Image.Width > 0, "Video 1 Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel10.ItemID), "Video 1 ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel10.Name), "Video 1 Name must not be empty");
                Assert.Equal(SearchResultType.Media, suggestionViewModel10.SearchType);
                MediaResultType? mediaType = suggestionViewModel10.MediaType;
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.Video.Value, mediaType.Value);
                Assert.True(suggestionViewModel10.Rank > 0, "Video 1 Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel10.KnownFor), "Video 1 KnownFor must not be empty");
                Assert.True(suggestionViewModel10.Year > 0, "Video 1 Year must be a positive number");
            }

            {  // Eleventh suggestion - Music Video
                SuggestionDataModel? suggestionDataModel11 = suggestions2?[2];
                Assert.NotNull(suggestionDataModel11);
                SuggestionViewModel suggestionViewModel11 = new(suggestionDataModel11);

                Assert.NotEqual(Guid.Empty, suggestionViewModel11.ID);
                Assert.NotNull(suggestionViewModel11.Image);
                Assert.True(suggestionViewModel11.Image.Height > 0, "Music video image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel11.Image.ImageURL), "Music video ImageURL must not be empty");
                Assert.True(suggestionViewModel11.Image.Width > 0, "Music video Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel11.ItemID), "Music video ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel11.Name), "Music video Name must not be empty");
                Assert.Equal(SearchResultType.Media, suggestionViewModel11.SearchType);
                MediaResultType? mediaType = suggestionViewModel11.MediaType;
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.MusicVideo.Value, mediaType.Value);
                Assert.True(suggestionViewModel11.Rank > 0, "Music video Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel11.KnownFor), "Music video KnownFor must not be empty");
                Assert.True(suggestionViewModel11.Year > 0, "Music video Year must be a positive number");
            }

            {  // Twelfth suggestion - Podcast Series
                SuggestionDataModel? suggestionDataModel12 = suggestions2?[3];
                Assert.NotNull(suggestionDataModel12);
                SuggestionViewModel suggestionViewModel12 = new(suggestionDataModel12);

                Assert.NotEqual(Guid.Empty, suggestionViewModel12.ID);
                Assert.NotNull(suggestionViewModel12.Image);
                Assert.True(suggestionViewModel12.Image.Height > 0, "Podcast series image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel12.Image.ImageURL), "Podcast series ImageURL must not be empty");
                Assert.True(suggestionViewModel12.Image.Width > 0, "Podcast series Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel12.ItemID), "Podcast series ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel12.Name), "Podcast series Name must not be empty");
                Assert.Equal(SearchResultType.Media, suggestionViewModel12.SearchType);
                MediaResultType? mediaType = suggestionViewModel12.MediaType;
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.PodcastSeries.Value, mediaType.Value);
                Assert.True(suggestionViewModel12.Rank > 0, "Podcast series Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel12.KnownFor), "Podcast series KnownFor must not be empty");
                Assert.True(suggestionViewModel12.Year > 0, "Podcast series Year must be a positive number");
            }

            {  // Thirteenth suggestion - Video (second)
                SuggestionDataModel? suggestionDataModel13 = suggestions2?[4];
                Assert.NotNull(suggestionDataModel13);
                SuggestionViewModel suggestionViewModel13 = new(suggestionDataModel13);

                Assert.NotEqual(Guid.Empty, suggestionViewModel13.ID);
                Assert.NotNull(suggestionViewModel13.Image);
                Assert.True(suggestionViewModel13.Image.Height > 0, "Video 2 image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel13.Image.ImageURL), "Video 2 ImageURL must not be empty");
                Assert.True(suggestionViewModel13.Image.Width > 0, "Video 2 Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel13.ItemID), "Video 2 ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel13.Name), "Video 2 Name must not be empty");
                Assert.Equal(SearchResultType.Media, suggestionViewModel13.SearchType);
                MediaResultType? mediaType = suggestionViewModel13.MediaType;
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.Video.Value, mediaType.Value);
                Assert.True(suggestionViewModel13.Rank > 0, "Video 2 Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel13.KnownFor), "Video 2 KnownFor must not be empty");
                Assert.True(suggestionViewModel13.Year > 0, "Video 2 Year must be a positive number");
            }

            {  // Fourteenth suggestion - Unknown item type with invalid ID prefix
                SuggestionDataModel? suggestionDataModel14 = suggestions2?[5];
                Assert.NotNull(suggestionDataModel14);
                SuggestionViewModel suggestionViewModel14 = new(suggestionDataModel14);

                Assert.NotEqual(Guid.Empty, suggestionViewModel14.ID);
                Assert.NotNull(suggestionViewModel14.Image);
                Assert.True(suggestionViewModel14.Image.Height > 0, "Spotlight image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel14.Image.ImageURL), "Spotlight ImageURL must not be empty");
                Assert.True(suggestionViewModel14.Image.Width > 0, "Spotlight Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel14.ItemID), "Spotlight ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel14.Name), "Spotlight Name must not be empty");
                Assert.Null(suggestionViewModel14.SearchType); // Should be null for "/spotlight/" prefix
                Assert.Null(suggestionViewModel14.MediaType);
                Assert.Null(suggestionViewModel14.Rank); // Spotlight items may not have rank
                Assert.False(String.IsNullOrWhiteSpace(suggestionViewModel14.KnownFor), "Spotlight KnownFor must not be empty");
                Assert.Null(suggestionViewModel14.Year); // Spotlight items may not have year
                Assert.Null(suggestionViewModel14.Years);
            }
        }  // End second movie suggestions data file
    }

    [Fact]
    public void SuggestionViewModel_ValidViewModelToString_ReturnsFormattedString()
    {
        // Act
        MovieSuggestionsResponseDataModel? actual1 = MovieHttpClient.GetModelFromResponse(movieHttpClientResponse1);
        SuggestionDataModel[]? suggestions1 = actual1?.Suggestions;
        Assert.NotNull(suggestions1);
        Assert.NotEmpty(suggestions1);
        SuggestionViewModel tvMiniSeriesViewModel = new(suggestions1[3], default(Guid));
        string tvMiniSeriesViewModelString = tvMiniSeriesViewModel.ToString();

        // Assert

        Assert.Contains("ID: 00000000-0000-0000-0000-000000000000\nImage:\n*****\nID: 00000000-0000-0000-0000-000000000000\nHeight: 300\nImageURL: https://example.com/example4.jpg\nWidth: 209\n*****\nItemID: tt0100005\nName: Example TV Mini Series\nSearchType: Media\nMediaType: TV Mini Series\nRank: 4444\nKnownFor: Maria Garcia, James Smith\nYear: 1982\nYears: 1982-1982"
            , tvMiniSeriesViewModelString);
    }

    [Fact]
    public void SuggestionViewModel_ValidViewModelCreation_GeneratesUniqueGuids()
    {
        // Arrange
        
        MovieSuggestionsResponseDataModel? actual1 = MovieHttpClient.GetModelFromResponse(movieHttpClientResponse1);
        SuggestionDataModel[]? suggestions1 = actual1?.Suggestions;
        Assert.NotNull(suggestions1);
        Assert.NotEmpty(suggestions1);

        // Act

        SuggestionViewModel movieViewModel1 = new(suggestions1[1]);
        SuggestionViewModel movieViewModel2 = new(suggestions1[1]);

        // Assert

        Assert.NotEqual(Guid.Empty, movieViewModel1.ID);
        Assert.NotEqual(Guid.Empty, movieViewModel2.ID);
        Assert.NotEqual(movieViewModel1.ID, movieViewModel2.ID);
    }

    [Fact]
    public void GetModelFromResponse_EmptyResponse_ReturnsEmptyModel()
    {
        // Act
        MovieSuggestionsResponseDataModel? actual = MovieHttpClient.GetModelFromResponse(noSuggestionsErrorResponse);

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual.Suggestions);
        Assert.Empty(actual.Suggestions);
    }

    [Fact]
    public void GetModelFromResponse_ErrorResponse_GeneratesNullFieldsAndThrowsExceptions()
    {
        // Act
        MovieSuggestionsResponseDataModel? actual = MovieHttpClient.GetModelFromResponse(badDataErrorResponse);

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual.Suggestions);
        Assert.Equal(2, actual.Suggestions.Length);

        SuggestionDataModel? suggestion1 = actual.Suggestions[0];
        Assert.NotNull(suggestion1);
        SuggestionViewModel viewModel1 = new(suggestion1);
        Assert.Null(viewModel1.SearchType); // Should be null for invalid prefix "xx"
        Assert.Null(viewModel1.MediaType); // Should be null for invalid MediaType

        SuggestionDataModel? suggestion2 = actual.Suggestions[1];
        Assert.NotNull(suggestion2);
        Assert.Throws<ArgumentException>(() => new SuggestionViewModel(suggestion2)); // Should throw for empty ItemID
    }

    [Fact]
    public void SuggestionImageViewModel_NegativeImageValues_ThrowsException()
    {
        // Arrange
        SuggestionImageDataModel badHeightImageDataModel = new SuggestionImageDataModel
        {
            Height = -1,
            Width = 1,
            ImageURL = ""
        };
        SuggestionImageDataModel badWidthImageDataModel = new SuggestionImageDataModel
        {
            Height = 0,
            Width = -1,
            ImageURL = ""
        };

        // Act / Assert
        Assert.Throws<ArgumentException>(() => new SuggestionImageViewModel(badHeightImageDataModel)); // Should throw for negative height
        Assert.Throws<ArgumentException>(() => new SuggestionImageViewModel(badWidthImageDataModel)); // Should throw for negative width
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SuggestionViewModel_NullOrEmptyItemId_ThrowsArgumentException(string? invalidItemId)
    {
        // Arrange
        #pragma warning disable CS8601 // Possible null reference assignment.
        var suggestionDataModel = new SuggestionDataModel
        {
            ItemID = invalidItemId,
            Name = "Test",
            MediaType = "movie",
            Rank = 1,
            KnownFor = "Test",
            Year = 2023,
            Years = null,
            Image = null
        };
        #pragma warning restore CS8601 // Possible null reference assignment.

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new SuggestionViewModel(suggestionDataModel));
    }

    [Fact]
    public void SuggestionViewModel_EdgeCaseValuesForViewModel_GeneratesValidModelValues()
    {
        // Arrange
        var suggestionDataModel = new SuggestionDataModel
        {
            ItemID = "tt0000001",
            Name = "Edge Case Movie",
            MediaType = "movie",
            Rank = 0, // Edge case: zero rank
            KnownFor = "Test",
            Year = 0, // Edge case: zero year
            Years = "", // Edge case: empty string
            Image = null
        };

        // Act
        var viewModel = new SuggestionViewModel(suggestionDataModel);

        // Assert
        
        Assert.Equal(SearchResultType.Media, viewModel.SearchType);
        Assert.Equal(MediaResultType.Movie.Value, viewModel.MediaType?.Value);
        Assert.Equal(0, viewModel.Rank);
        Assert.Equal(0, viewModel.Year);
        Assert.Equal("", viewModel.Years);
    }

    [Fact]
    public void SuggestionViewModel_ValidModelToString_ReturnsCorrectValue()
    {
        // Arrange

        MovieSuggestionsResponseDataModel? actual1 = MovieHttpClient.GetModelFromResponse(movieHttpClientResponse1);

        SuggestionDataModel[]? suggestions1 = actual1?.Suggestions;
        Assert.Equal(8, suggestions1?.Length);
        
        SuggestionDataModel? suggestionTvSeriesDataModel = suggestions1?[2];
        Assert.NotNull(suggestionTvSeriesDataModel);

        // Act
        SuggestionViewModel suggestionTvSeriesViewModel = new(suggestionTvSeriesDataModel, default(Guid));

        // Assert
        Assert.Equal(
            "ID: 00000000-0000-0000-0000-000000000000\nImage:\n*****\nID: 00000000-0000-0000-0000-000000000000\nHeight: 400\nImageURL: https://example.com/example3.jpg\nWidth: 313\n*****\nItemID: tt10000002\nName: Example TV Series\nSearchType: Media\nMediaType: TV Series\nRank: 4444\nKnownFor: John Smith, James Johnson\nYear: 2001\nYears: 2001-2003"
                , suggestionTvSeriesViewModel.ToString());
    }
}
