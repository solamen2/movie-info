using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbTvSeriesAggregateCrewCreditDataModel
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
    [JsonPropertyName("jobs")]
    public required TmdbTvSeriesAggregateCrewCreditJobDataModel[] Jobs { get; init; }
    [JsonPropertyName("department")]
    public required string Department { get; init; }
    [JsonPropertyName("total_episode_count")]
    public required int TotalEpisodeCount { get; init; }

    public override string ToString()
    {
        return $"Adult: {Adult}\nGender: {Gender}\nId: {Id}\nKnownForDepartment: {KnownForDepartment}\nName: {Name}\nOriginalName: {OriginalName}\nPopularity: {Popularity}\nProfilePath: {ProfilePath}\nJobs:\n*****\n{string.Join("\n\n", Jobs)}\n*****\nDepartment: {Department}\nTotalEpisodeCount: {TotalEpisodeCount}";
    }
}