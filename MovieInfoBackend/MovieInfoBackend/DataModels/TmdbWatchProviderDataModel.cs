using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbWatchProviderDataModel
{
    [JsonPropertyName("logo_path")]
    public string? LogoPath { get; init; }
    [JsonPropertyName("provider_id")]
    public required int ProviderId { get; init; }
    [JsonPropertyName("provider_name")]
    public required string ProviderName { get; init; }
    [JsonPropertyName("display_priority")]
    public required int DisplayPriority { get; init; }

    public override string ToString()
    {
        return $"LogoPath: {LogoPath}\nProviderId: {ProviderId}\nProviderName: {ProviderName}\nDisplayPriority: {DisplayPriority}";
    }

}
