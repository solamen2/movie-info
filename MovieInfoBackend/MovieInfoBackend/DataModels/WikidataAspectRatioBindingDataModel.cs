using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

// One row of the results of WikidataHttpClient's aspect ratio query: a Wikidata item for an aspect ratio, and its English label
// (e.g. "1.85:1", "16:9" or "Academy ratio")
public record WikidataAspectRatioBindingDataModel
{
    [JsonPropertyName("aspectRatio")]
    public required WikidataSparqlValueDataModel AspectRatio { get; init; }

    [JsonPropertyName("aspectRatioLabel")]
    public WikidataSparqlValueDataModel? AspectRatioLabel { get; init; }

    public override string ToString()
    {
        return $"AspectRatio:\n*****\n{AspectRatio}\n*****\nAspectRatioLabel:\n*****\n{AspectRatioLabel}\n*****";
    }
}
