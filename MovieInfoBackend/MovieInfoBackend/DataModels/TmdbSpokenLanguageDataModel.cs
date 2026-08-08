using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbSpokenLanguageDataModel
{
    [JsonPropertyName("english_name")]
    public required string EnglishName { get; init; }
    [JsonPropertyName("iso_639_1")]
    public required string Iso6391 { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    public override string ToString()
    {
        return $"EnglishName: {EnglishName}\nIso6391: {Iso6391}\nName: {Name}";
    }

}
