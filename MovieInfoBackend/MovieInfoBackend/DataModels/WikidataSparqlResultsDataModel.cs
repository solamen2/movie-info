using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record WikidataSparqlResultsDataModel
{
    // One per row of results; empty when nothing matched the query
    [JsonPropertyName("bindings")]
    public required WikidataAspectRatioBindingDataModel[] Bindings { get; init; }

    public override string ToString()
    {
        return $"Bindings:\n*****\n{string.Join("\n\n", (IEnumerable<WikidataAspectRatioBindingDataModel>)Bindings)}\n*****";
    }
}
