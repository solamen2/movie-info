public class MediaResultTypeTests
{
    [Fact]
    public void MediaResultType_InvalidMediaTypeStrings_ReturnNullMediaTypeValues()
    {
        // Arrange, Act & Assert
        Assert.Null(MediaResultType.GetMediaType("invalidType"));
        Assert.Null(MediaResultType.GetMediaType(""));
        Assert.Null(MediaResultType.GetMediaType(null));
        Assert.Null(MediaResultType.GetMediaType("MOVIE")); // Case sensitive
    }
}