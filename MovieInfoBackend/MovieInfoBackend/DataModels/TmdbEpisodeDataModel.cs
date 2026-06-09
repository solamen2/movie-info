using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbEpisodeDataModel
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("overview")]
    public required string Overview { get; init; }
    [JsonPropertyName("vote_average")]
    public required double VoteAverage { get; init; }
    [JsonPropertyName("vote_count")]
    public required int VoteCount { get; init; }
    [JsonPropertyName("air_date")]
    public string? AirDate { get; init; }
    [JsonPropertyName("episode_number")]
    public required int EpisodeNumber { get; init; }
    [JsonPropertyName("episode_type")]
    public required string EpisodeType { get; init; }
    [JsonPropertyName("production_code")]
    public required string ProductionCode { get; init; }
    [JsonPropertyName("runtime")]
    public required int Runtime { get; init; }
    [JsonPropertyName("season_number")]
    public required int SeasonNumber { get; init; }
    [JsonPropertyName("show_id")]
    public required int ShowId { get; init; }
    [JsonPropertyName("still_path")]
    public required string StillPath { get; init; }

    public override string ToString()
    {
        return $"Id: {Id}\nName: {Name}\nOverview: {Overview}\nVoteAverage: {VoteAverage}\nVoteCount: {VoteCount}\nAirDate: {AirDate}\nEpisodeNumber: {EpisodeNumber}\nEpisodeType: {EpisodeType}\nProductionCode: {ProductionCode}\nRuntime: {Runtime}\nSeasonNumber: {SeasonNumber}\nShowId: {ShowId}\nStillPath: {StillPath}";
    }

}
