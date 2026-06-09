using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbProductionCountryDataModel
{
    [JsonPropertyName("iso_3166_1")]
    public required string Iso31661 { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    public override string ToString()
    {
        return $"Iso31661: {Iso31661}\nName: {Name}\n";
    }

}
