using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbPersonImagesResponseDataModel
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("profiles")]
    public required TmdbProfileDataModel[] Profiles { get; init; }

    public override string ToString()
    {
        return $"Id: {Id}\nProfiles:\n*****\n{string.Join("\n\n", Profiles)}";
    }
}