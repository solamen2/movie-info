using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbNetworkDataModel
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("logo_path")]
    public required string LogoPath { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("origin_country")]
    public required string OriginCountry { get; init; }

    public override string ToString()
    {
        return $"Id: {Id}\nLogoPath: {LogoPath}\nName: {Name}\nOriginCountry: {OriginCountry}";
    }

}