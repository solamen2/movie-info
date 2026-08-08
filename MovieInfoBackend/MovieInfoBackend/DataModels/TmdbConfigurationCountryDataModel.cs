using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbConfigurationCountryDataModel
{
    [JsonPropertyName("iso_3166_1")]
    public required string Iso31661 { get; init; }
    [JsonPropertyName("english_name")]
    public required string EnglishName { get; init; }
    [JsonPropertyName("native_name")]
    public required string NativeName { get; init; }

    public override string ToString()
    {
        return $"Iso31661: {Iso31661}\nEnglishName: {EnglishName}\nNativeName: {NativeName}";
    }

}
