using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record WikipediaExtractPageDataModel
{
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("missing")]
    public bool Missing { get; init; }

    // The page's text without any markup, its section headings written as in wikitext ("== Plot ==")
    [JsonPropertyName("extract")]
    public string? Extract { get; init; }

    public override string ToString()
    {
        // The extract is a whole article, so only its length is worth logging
        return $"Title: {Title}\nMissing: {Missing}\nExtract: {(Extract == null ? "" : $"{Extract.Length} characters")}";
    }
}
