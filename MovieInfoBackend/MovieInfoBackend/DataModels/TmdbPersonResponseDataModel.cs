using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbPersonResponseDataModel
{
    [JsonPropertyName("adult")]
    public required bool Adult { get; init; }
    [JsonPropertyName("also_known_as")]
    public required string[] AlsoKnownAs { get; init; }  // NOTE: Can be empty array, but leaving as required to avoid null warning in ToString()
    [JsonPropertyName("biography")]
    public string? Biography { get; init; }
    [JsonPropertyName("birthday")]
    public required string Birthday { get; init; }
    [JsonPropertyName("deathday")]
    public string? Deathday { get; init; }
    [JsonPropertyName("gender")]
    public required int Gender { get; init; }
    [JsonPropertyName("homepage")]
    public string? Homepage { get; init; }
    [JsonPropertyName("id")]
    public required int TmdbId { get; init; }
    [JsonPropertyName("imdb_id")]
    public required string ImdbId { get; init; }
    [JsonPropertyName("known_for_department")]
    public required string KnownForDepartment { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("place_of_birth")]
    public required string PlaceOfBirth { get; init; }
    [JsonPropertyName("popularity")]
    public required double Popularity { get; init; }
    [JsonPropertyName("profile_path")]
    public required string ProfilePath { get; init; }

    public override string ToString()
    {
        return $"Adult: {Adult}\nAlsoKnownAs: {string.Join(", ", AlsoKnownAs)}\nBiography: {Biography}\nBirthday: {Birthday}\nDeathday: {Deathday}\nGender: {Gender}\nHomepage: {Homepage}\nTmdbId: {TmdbId}\nImdbId: {ImdbId}\nKnownForDepartment: {KnownForDepartment}\nName: {Name}\nPlaceOfBirth: {PlaceOfBirth}\nPopularity: {Popularity}\nProfilePath: {ProfilePath}";
    }

}
