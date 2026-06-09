using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbGenreDataModel
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    public override string ToString()
    {
        return $"Id: {Id}\nName: {Name}";
    }

}