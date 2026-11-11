using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

// The Wikidata Query Service's answer to a SPARQL SELECT query, in the standard SPARQL JSON results format
// (https://www.w3.org/TR/sparql11-results-json/). Its "head" (the list of variable names) is not needed.
public record WikidataSparqlResponseDataModel
{
    [JsonPropertyName("results")]
    public required WikidataSparqlResultsDataModel Results { get; init; }

    public override string ToString()
    {
        return $"Results:\n*****\n{Results}\n*****";
    }
}
