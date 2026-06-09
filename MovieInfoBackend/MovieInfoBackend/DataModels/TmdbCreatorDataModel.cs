using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbCreatorDataModel
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("credit_id")]
    public required string CreditId { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("original_name")]
    public required string OriginalName { get; init; }
    [JsonPropertyName("gender")]
    public required int Gender { get; init; }
    [JsonPropertyName("profile_path")]
    public required string ProfilePath { get; init; }

    public override string ToString()
    {
        return $"Id: {Id}\nCreditId: {CreditId}\nName: {Name}\nOriginalName: {OriginalName}\nGender: {Gender}\nProfilePath: {ProfilePath}";
    }

}