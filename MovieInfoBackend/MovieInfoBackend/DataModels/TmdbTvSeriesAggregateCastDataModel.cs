using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbTvSeriesAggregateCastDataModel
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
    public required string ProfilePath { get; init; }
    [JsonPropertyName("roles")]
    public required TmdbTvSeriesAggregateCastRoleDataModel[] Roles { get; init; }
    [JsonPropertyName("total_episode_count")]
    public required int TotalEpisodeCount { get; init; }
    [JsonPropertyName("order")]
    public required int Order { get; init; }

    public override string ToString()
    {
        return $"Adult: {Adult}\nGender: {Gender}\nId: {Id}\nKnownForDepartment: {KnownForDepartment}\nName: {Name}\nOriginalName: {OriginalName}\nPopularity: {Popularity}\nProfilePath: {ProfilePath}\nRoles:\n*****\n{string.Join("\n\n", Roles)}\n*****\nTotalEpisodeCount: {TotalEpisodeCount}\nOrder: {Order}";
    }
}