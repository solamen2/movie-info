using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbTvEpisodeCreditsCastDataModel
{
    [JsonPropertyName("adult")]
    public required bool Adult { get; init; }
    [JsonPropertyName("gender")]
    public required int Gender { get; init; }
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("known_for_department")]
    public required string KnownForDepartment { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("original_name")]
    public required string OriginalName { get; init; }
    [JsonPropertyName("popularity")]
    public required double Popularity { get; init; }
    [JsonPropertyName("profile_path")]
    public string? ProfilePath { get; init; }
    [JsonPropertyName("character")]
    public required string Character { get; init; }
    [JsonPropertyName("credit_id")]
    public required string CreditId { get; init; }
    [JsonPropertyName("order")]
    public required int Order { get; init; }

    public override string ToString()
    {
        return $"Adult: {Adult}\nGender: {Gender}\nId: {Id}\nKnownForDepartment: {KnownForDepartment}\nName: {Name}\nOriginalName: {OriginalName}\nPopularity: {Popularity}\nProfilePath: {ProfilePath}\nCharacter: {Character}\nCreditId: {CreditId}\nOrder: {Order}";
    }
}