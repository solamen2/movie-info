using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbConfigurationLanguageDataModel
{
    [JsonPropertyName("iso_639_1")]
    public required string Iso6391 { get; init; }
    [JsonPropertyName("english_name")]
    public required string EnglishName { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    public override string ToString()
    {
        return $"Iso6391: {Iso6391}\nEnglishName: {EnglishName}\nName: {Name}";
    }

}
