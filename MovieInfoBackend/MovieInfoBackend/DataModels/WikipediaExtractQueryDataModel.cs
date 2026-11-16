using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record WikipediaExtractQueryDataModel
{
    [JsonPropertyName("pages")]
    public WikipediaExtractPageDataModel[]? Pages { get; init; }

    public override string ToString()
    {
        return $"Pages:\n*****\n{(Pages == null ? "" : string.Join("\n\n", (IEnumerable<WikipediaExtractPageDataModel>)Pages))}\n*****";
    }
}
