using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record OmdbRatingDataModel
{
    [JsonPropertyName("Source")]
    public required string Source { get; init;}

    [JsonPropertyName("Value")]
    public required string Value { get; init; }

    public override string ToString()
    {
        return $"Source: {Source}\nValue: {Value}";
    }
}