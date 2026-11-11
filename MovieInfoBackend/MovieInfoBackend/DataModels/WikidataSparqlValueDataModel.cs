using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

// One variable's value in a row of SPARQL results
public record WikidataSparqlValueDataModel
{
    // "uri" for a Wikidata item, "literal" for text such as a label, or "bnode" for Wikidata's "unknown value"
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    // An item's value is its URI, e.g. http://www.wikidata.org/entity/Q21037379
    [JsonPropertyName("value")]
    public required string Value { get; init; }

    public override string ToString()
    {
        return $"Type: {Type}\nValue: {Value}";
    }
}
