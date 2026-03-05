using Serilog;

public enum SearchResultType
{
    Person,
    Media
}

public static class SearchResultTypeExtensions
{
    extension(SearchResultType searchResultType)
    {
        public static SearchResultType? GetSearchType(string? itemId)
        {
            if (String.IsNullOrWhiteSpace(itemId))
                throw new ArgumentException("Item ID is null, empty, or whitespace!");

            // From IMDB 
            if (itemId.StartsWith("nm"))
            {
                return SearchResultType.Person;
            }
            else if (itemId.StartsWith("tt"))
            {
                return SearchResultType.Media;
            }
            else
            {
                Log.Warning("Item id has invalid prefix: '" + itemId + "'.");
                return null;
            }
        }
    }
}