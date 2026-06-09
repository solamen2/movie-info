using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbPersonIdDataModel
{
    [JsonPropertyName("adult")]
    public required bool Adult { get; init; }
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("original_name")]
    public required string OriginalName { get; init; }
    [JsonPropertyName("media_type")]
    public required string MediaType { get; init; }
    [JsonPropertyName("popularity")]
    public required double Popularity { get; init; }
    [JsonPropertyName("gender")]
    public required int Gender { get; init; }
    [JsonPropertyName("known_for_department")]
    public required string KnownForDepartment { get; init; }
    [JsonPropertyName("profile_path")]
    public required string ProfilePath { get; init; }
    [JsonPropertyName("known_for")]
    public TmdbMovieIdDataModel[]? KnownFor { get; init; }

    public override string ToString()
    {
        return $"Adult: {Adult}\nId: {Id}\nName: {Name}\nOriginalName: {OriginalName}\nMediaType: {MediaType}\nPopularity: {Popularity}\nGender: {Gender}\nKnownForDepartment: {KnownForDepartment}\nProfilePath: {ProfilePath}\nKnownFor:\n*****\n{string.Join("\n\n", KnownFor)}";
    }
}