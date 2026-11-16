using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

// The Wikipedia Action API's answer to a query for one page's plain-text extract (prop=extracts, formatversion=2):
// {"query": {"pages": [{"title": "...", "extract": "..."}]}}
// A page that does not exist comes back as {"title": "...", "missing": true}, without an extract.
public record WikipediaExtractResponseDataModel
{
    [JsonPropertyName("query")]
    public WikipediaExtractQueryDataModel? Query { get; init; }

    public override string ToString()
    {
        return $"Query:\n*****\n{Query}\n*****";
    }
}
