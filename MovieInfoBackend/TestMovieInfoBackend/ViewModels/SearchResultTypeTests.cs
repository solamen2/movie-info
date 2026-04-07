public class SearchResultTypeTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void SearchResultType_InvalidSearchValues_ThrowArgumentException(string? invalidId)
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => SearchResultType.GetSearchType(invalidId));
    }
}