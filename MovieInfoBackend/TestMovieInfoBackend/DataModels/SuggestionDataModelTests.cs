using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class SuggestionDataModelTests
{
    private string movieHttpClientResponse1;
    private string movieHttpClientResponse2;

    public SuggestionDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testDataFilename1 = "MovieHttpClientResponse1.json";
        string testDataFilename2 = "MovieHttpClientResponse2.json";

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
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {
        // Act

        MovieSuggestionsResponseDataModel? actual1 = MovieHttpClient.GetModelFromResponse(movieHttpClientResponse1);
        Assert.NotNull(actual1);
        MovieSuggestionsResponseDataModel? actual2 = MovieHttpClient.GetModelFromResponse(movieHttpClientResponse2);
        Assert.NotNull(actual2);

        // Assert

        {  // First movie suggestions data file  (blocks help prevent data leakage between tests)
            SuggestionDataModel[]? suggestions1 = actual1.Suggestions;
            Assert.NotNull(suggestions1);
            Assert.NotEmpty(suggestions1);
            Assert.Equal(8, suggestions1.Length);

            {  // First suggestion
                SuggestionDataModel suggestion1 = suggestions1[0];
                Assert.NotNull(suggestion1);
                SuggestionImageDataModel? suggestion1image = suggestion1.Image;
                Assert.NotNull(suggestion1image);
                Assert.True(suggestion1image.Height > 0, "Person image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion1image.ImageURL), "Person ImageURL must not be empty");
                Assert.True(suggestion1image.Width > 0, "Person Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion1.ItemID), "Person ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestion1.Name), "Person Name must not be empty");
                Assert.True(suggestion1.Rank > 0, "Person Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion1.KnownFor), "Person KnownFor must not be empty");
            }

            {  // Second suggestion
                SuggestionDataModel suggestion2 = suggestions1[1];
                Assert.NotNull(suggestion2);
                SuggestionImageDataModel? suggestion2image = suggestion2.Image;
                Assert.NotNull(suggestion2image);
                Assert.True(suggestion2image.Height > 0, "Movie image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion2image.ImageURL), "Movie ImageURL must not be empty");
                Assert.True(suggestion2image.Width > 0, "Movie Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion2.ItemID), "Movie ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestion2.Name), "Movie Name must not be empty");
                MediaResultType? mediaType = MediaResultType.GetMediaType(suggestion2.MediaType);
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.Movie.Value, mediaType.Value);
                Assert.True(suggestion2.Rank > 0, "Movie Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion2.KnownFor), "Movie KnownFor must not be empty");
                Assert.True(suggestion2.Year > 0, "Movie Year must be a positive number");
            }

            {  // Third suggestion
                SuggestionDataModel suggestion3 = suggestions1[2];
                Assert.NotNull(suggestion3);
                SuggestionImageDataModel? suggestion3image = suggestion3.Image;
                Assert.NotNull(suggestion3image);
                Assert.True(suggestion3image.Height > 0, "TV series image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion3image.ImageURL), "TV series ImageURL must not be empty");
                Assert.True(suggestion3image.Width > 0, "TV series Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion3.ItemID), "TV series ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestion3.Name), "TV series Name must not be empty");
                MediaResultType? mediaType = MediaResultType.GetMediaType(suggestion3.MediaType);
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.TVSeries.Value, mediaType.Value);
                Assert.True(suggestion3.Rank > 0, "TV series Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion3.KnownFor), "TV series KnownFor must not be empty");
                Assert.True(suggestion3.Year > 0, "TV series Year must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion3.Years), "TV series Years must not be empty");
            }

            {  // Fourth suggestion
                SuggestionDataModel suggestion4 = suggestions1[3];
                Assert.NotNull(suggestion4);
                SuggestionImageDataModel? suggestion4image = suggestion4.Image;
                Assert.NotNull(suggestion4image);
                Assert.True(suggestion4image.Height > 0, "TV mini series image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion4image.ImageURL), "TV mini series ImageURL must not be empty");
                Assert.True(suggestion4image.Width > 0, "TV mini series Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion4.ItemID), "TV mini series ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestion4.Name), "TV mini series Name must not be empty");
                MediaResultType? mediaType = MediaResultType.GetMediaType(suggestion4.MediaType);
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.TVMiniSeries.Value, mediaType.Value);
                Assert.True(suggestion4.Rank > 0, "TV mini series Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion4.KnownFor), "TV mini series KnownFor must not be empty");
                Assert.True(suggestion4.Year > 0, "TV mini series Year must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion4.Years), "TV mini series Years must not be empty");
            }

            {  // Fifth suggestion
                SuggestionDataModel suggestion5 = suggestions1[4];
                Assert.NotNull(suggestion5);
                SuggestionImageDataModel? suggestion5image = suggestion5.Image;
                Assert.Null(suggestion5image);
                Assert.False(String.IsNullOrWhiteSpace(suggestion5.ItemID), "TV movie ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestion5.Name), "TV movie Name must not be empty");
                MediaResultType? mediaType = MediaResultType.GetMediaType(suggestion5.MediaType);
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.TVMovie.Value, mediaType.Value);
                Assert.True(suggestion5.Rank > 0, "TV movie Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion5.KnownFor), "TV movie KnownFor must not be empty");
                Assert.True(suggestion5.Year > 0, "TV movie Year must be a positive number");
            }

            {  // Sixth suggestion
                SuggestionDataModel suggestion6 = suggestions1[5];
                Assert.NotNull(suggestion6);
                SuggestionImageDataModel? suggestion6image = suggestion6.Image;
                Assert.NotNull(suggestion6image);
                Assert.True(suggestion6image.Height > 0, "TV special image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion6image.ImageURL), "TV special ImageURL must not be empty");
                Assert.True(suggestion6image.Width > 0, "TV special Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion6.ItemID), "TV special ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestion6.Name), "TV special Name must not be empty");
                MediaResultType? mediaType = MediaResultType.GetMediaType(suggestion6.MediaType);
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.TVSpecial.Value, mediaType.Value);
                Assert.True(suggestion6.Rank > 0, "TV special Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion6.KnownFor), "TV special KnownFor must not be empty");
                Assert.True(suggestion6.Year > 0, "TV special Year must be a positive number");
            }

            {  // Seventh suggestion
                SuggestionDataModel suggestion7 = suggestions1[6];
                Assert.NotNull(suggestion7);
                SuggestionImageDataModel? suggestion7image = suggestion7.Image;
                Assert.NotNull(suggestion7image);
                Assert.True(suggestion7image.Height > 0, "TV short image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion7image.ImageURL), "TV short ImageURL must not be empty");
                Assert.True(suggestion7image.Width > 0, "TV short Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion7.ItemID), "TV short ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestion7.Name), "TV short Name must not be empty");
                MediaResultType? mediaType = MediaResultType.GetMediaType(suggestion7.MediaType);
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.TVShort.Value, mediaType.Value);
                Assert.True(suggestion7.Rank > 0, "TV short Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion7.KnownFor), "TV short KnownFor must not be empty");
                Assert.True(suggestion7.Year > 0, "TV short Year must be a positive number");
            }

            {  // Eighth suggestion
                SuggestionDataModel suggestion8 = suggestions1[7];
                Assert.NotNull(suggestion8);
                SuggestionImageDataModel? suggestion8image = suggestion8.Image;
                Assert.NotNull(suggestion8image);
                Assert.True(suggestion8image.Height > 0, "Short image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion8image.ImageURL), "Short ImageURL must not be empty");
                Assert.True(suggestion8image.Width > 0, "Short Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion8.ItemID), "Short ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestion8.Name), "Short Name must not be empty");
                MediaResultType? mediaType = MediaResultType.GetMediaType(suggestion8.MediaType);
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.Short.Value, mediaType.Value);
                Assert.True(suggestion8.Rank > 0, "Short Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion8.KnownFor), "Short KnownFor must not be empty");
                Assert.True(suggestion8.Year > 0, "Short Year must be a positive number");
            }
        }  // End first movie suggestions data file

        {  // Second movie suggestions data file
            SuggestionDataModel[]? suggestions2 = actual2.Suggestions;
            Assert.NotNull(suggestions2);
            Assert.NotEmpty(suggestions2);
            Assert.Equal(6, suggestions2.Length);

            {  // Ninth suggestion
                SuggestionDataModel suggestion9 = suggestions2[0];
                Assert.NotNull(suggestion9);
                SuggestionImageDataModel? suggestion9image = suggestion9.Image;
                Assert.NotNull(suggestion9image);
                Assert.True(suggestion9image.Height > 0, "Video game image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion9image.ImageURL), "Video game ImageURL must not be empty");
                Assert.True(suggestion9image.Width > 0, "Video game Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion9.ItemID), "Video game ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestion9.Name), "Video game Name must not be empty");
                MediaResultType? mediaType = MediaResultType.GetMediaType(suggestion9.MediaType);
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.VideoGame.Value, mediaType.Value);
                Assert.True(suggestion9.Rank > 0, "Video game Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion9.KnownFor), "Video game KnownFor must not be empty");
                Assert.True(suggestion9.Year > 0, "Video game Year must be a positive number");
            }

            {  // Tenth suggestion
                SuggestionDataModel suggestion10 = suggestions2[1];
                Assert.NotNull(suggestion10);
                SuggestionImageDataModel? suggestion10image = suggestion10.Image;
                Assert.NotNull(suggestion10image);
                Assert.True(suggestion10image.Height > 0, "Video 1 image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion10image.ImageURL), "Video 1 ImageURL must not be empty");
                Assert.True(suggestion10image.Width > 0, "Video 1 Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion10.ItemID), "Video 1 ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestion10.Name), "Video 1 Name must not be empty");
                MediaResultType? mediaType = MediaResultType.GetMediaType(suggestion10.MediaType);
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.Video.Value, mediaType.Value);
                Assert.True(suggestion10.Rank > 0, "Video 1 Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion10.KnownFor), "Video 1 KnownFor must not be empty");
                Assert.True(suggestion10.Year > 0, "Video 1 Year must be a positive number");
            }

            {  // Eleventh suggestion
                SuggestionDataModel suggestion11 = suggestions2[2];
                Assert.NotNull(suggestion11);
                SuggestionImageDataModel? suggestion11image = suggestion11.Image;
                Assert.NotNull(suggestion11image);
                Assert.True(suggestion11image.Height > 0, "Music video image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion11image.ImageURL), "Music video ImageURL must not be empty");
                Assert.True(suggestion11image.Width > 0, "Music video Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion11.ItemID), "Music video ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestion11.Name), "Music video Name must not be empty");
                MediaResultType? mediaType = MediaResultType.GetMediaType(suggestion11.MediaType);
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.MusicVideo.Value, mediaType.Value);
                Assert.True(suggestion11.Rank > 0, "Music video Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion11.KnownFor), "Music video KnownFor must not be empty");
                Assert.True(suggestion11.Year > 0, "Music video Year must be a positive number");
            }

            {  // Twelfth suggestion
                SuggestionDataModel suggestion12 = suggestions2[3];
                Assert.NotNull(suggestion12);
                SuggestionImageDataModel? suggestion12image = suggestion12.Image;
                Assert.NotNull(suggestion12image);
                Assert.True(suggestion12image.Height > 0, "Podcast series image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion12image.ImageURL), "Podcast series ImageURL must not be empty");
                Assert.True(suggestion12image.Width > 0, "Podcast series Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion12.ItemID), "Podcast series ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestion12.Name), "Podcast series Name must not be empty");
                MediaResultType? mediaType = MediaResultType.GetMediaType(suggestion12.MediaType);
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.PodcastSeries.Value, mediaType.Value);
                Assert.True(suggestion12.Rank > 0, "Podcast series Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion12.KnownFor), "Podcast series KnownFor must not be empty");
                Assert.True(suggestion12.Year > 0, "Podcast series Year must be a positive number");
            }

            {  // Thirteenth suggestion
                SuggestionDataModel suggestion13 = suggestions2[4];
                Assert.NotNull(suggestion13);
                SuggestionImageDataModel? suggestion13image = suggestion13.Image;
                Assert.NotNull(suggestion13image);
                Assert.True(suggestion13image.Height > 0, "Video 2 image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion13image.ImageURL), "Video 2 ImageURL must not be empty");
                Assert.True(suggestion13image.Width > 0, "Video 2 Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion13.ItemID), "Video 2 ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestion13.Name), "Video 2 Name must not be empty");
                MediaResultType? mediaType = MediaResultType.GetMediaType(suggestion13.MediaType);
                Assert.NotNull(mediaType);
                Assert.Equal(MediaResultType.Video.Value, mediaType.Value);
                Assert.True(suggestion13.Rank > 0, "Video 2 Rank must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion13.KnownFor), "Video 2 KnownFor must not be empty");
                Assert.True(suggestion13.Year > 0, "Video 2 Year must be a positive number");
            }

            {  // Fourteenth suggestion
                SuggestionDataModel suggestion14 = suggestions2[5];
                Assert.NotNull(suggestion14);
                SuggestionImageDataModel? suggestion14image = suggestion14.Image;
                Assert.NotNull(suggestion14image);
                Assert.True(suggestion14image.Height > 0, "Spotlight image Height must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion14image.ImageURL), "Spotlight ImageURL must not be empty");
                Assert.True(suggestion14image.Width > 0, "Spotlight Width must be a positive number");
                Assert.False(String.IsNullOrWhiteSpace(suggestion14.ItemID), "Spotlight ID must not be empty");
                Assert.False(String.IsNullOrWhiteSpace(suggestion14.Name), "Spotlight Name must not be empty");
                MediaResultType? mediaType = MediaResultType.GetMediaType(suggestion14.MediaType);
                Assert.Null(mediaType);
                Assert.False(String.IsNullOrWhiteSpace(suggestion14.KnownFor), "Spotlight KnownFor must not be empty");
            }
        }  // End second movie suggestions data file
    }

    [Fact]
    public void SuggestionImageDataModel_EdgeCaseValues_HandlesCorrectly()
    {
        // Arrange & Act
        var imageDataModel = new SuggestionImageDataModel
        {
            Height = 1,
            Width = 1,
            ImageURL = ""
        };

        // Assert
        Assert.NotNull(imageDataModel);
        Assert.Equal(1, imageDataModel.Height);
        Assert.Equal(1, imageDataModel.Width);
        Assert.Equal("", imageDataModel.ImageURL);
        Assert.Equal("Height: 1\nImageURL: \nWidth: 1", imageDataModel.ToString());
    }

    [Fact]
    public void SuggestionDataModel_ExtremeValues_HandlesCorrectly()
    {
        // Arrange
        var suggestionDataModel = new SuggestionDataModel
        {
            ItemID = "tt" + new string('9', 1000), // Very long ID
            Name = new string('A', 10000), // Very long name
            MediaType = "movie",
            Rank = int.MaxValue,
            KnownFor = new string('B', 5000), // Very long known for
            Year = int.MaxValue,
            Years = new string('C', 100) + "-" + new string('D', 100),
            Image = new SuggestionImageDataModel
            {
                Height = int.MaxValue,
                Width = int.MaxValue,
                ImageURL = "https://" + new string('e', 2000) + ".com/image.jpg"
            }
        };

        // Act & Assert
        Assert.NotNull(suggestionDataModel);
        Assert.Equal(new string('A', 10000), suggestionDataModel.Name);
        Assert.Equal(int.MaxValue, suggestionDataModel.Rank);
        Assert.Equal(int.MaxValue, suggestionDataModel.Year);
        Assert.Equal(int.MaxValue, suggestionDataModel.Image.Height);
        Assert.Equal(int.MaxValue, suggestionDataModel.Image.Width);
    }

    [Fact]
    public void SuggestionDataModel_ValidModelToString_ReturnsCorrectValue()
    {
        // Act

        MovieSuggestionsResponseDataModel? actual1 = MovieHttpClient.GetModelFromResponse(movieHttpClientResponse1);
        Assert.NotNull(actual1);
        SuggestionDataModel[]? suggestions1 = actual1.Suggestions;
        Assert.NotNull(suggestions1);
        SuggestionDataModel suggestion3 = suggestions1[2];

        // Assert
        Assert.Equal(
            "Image:\n*****\nHeight: 400\nImageURL: https://example.com/example3.jpg\nWidth: 313\n*****\nItemID: tt10000002\nName: Example TV Series\nMediaType: tvSeries\nRank: 4444\nKnownFor: John Smith, James Johnson\nYear: 2001\nYears: 2001-2003"
                , suggestion3.ToString());
    }

    [Fact]
    public void MovieSuggestionsResponseDataModel_NullSuggestions_HandlesCorrectly()
    {
        // Arrange & Act
        var responseModel = new MovieSuggestionsResponseDataModel
        {
            Suggestions = null
        };

        // Act & Assert
        Assert.NotNull(responseModel);
        Assert.Null(responseModel.Suggestions);

        string result = responseModel.ToString();
        Assert.NotNull(result);
    }

    [Fact]
    public void MovieSuggestionsResponseDataModel_EmptySuggestions_HandlesCorrectly()
    {
        // Arrange & Act
        var responseModel = new MovieSuggestionsResponseDataModel
        {
            Suggestions = []
        };

        // Assert
        Assert.NotNull(responseModel);
        Assert.NotNull(responseModel.Suggestions);
        Assert.Empty(responseModel.Suggestions);

        string result = responseModel.ToString();
        Assert.NotNull(result);
    }
}