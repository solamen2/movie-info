namespace MovieInfoBackend.DataModels;

// The Wikipedia OpenSearch API answers with a JSON array rather than an object:
// [search query, [page titles], [page descriptions (always empty nowadays)], [page links]]
// so this model is filled in by WikipediaHttpClient.GetModelFromResponse rather than deserialized directly.
public record WikipediaOpenSearchResponseDataModel
{
    public required string SearchQuery { get; init; }
    public required string[] Titles { get; init; }
    public required string[] Descriptions { get; init; }
    public required string[] Links { get; init; }

    public override string ToString()
    {
        return $"SearchQuery: {SearchQuery}\nTitles: {string.Join(", ", Titles)}\nDescriptions: {string.Join(", ", Descriptions)}\nLinks: {string.Join(", ", Links)}";
    }
}
