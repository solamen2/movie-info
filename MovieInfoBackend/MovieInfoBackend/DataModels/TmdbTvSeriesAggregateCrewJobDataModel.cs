using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbTvSeriesAggregateCrewJobDataModel
{
    [JsonPropertyName("credit_id")]
    public required string CreditId { get; init; }
    [JsonPropertyName("job")]
    public required string Job { get; init; }
    [JsonPropertyName("episode_count")]
    public required int EpisodeCount { get; init; }

    public override string ToString()
    {
        return $"CreditId: {CreditId}\nJob: {Job}\nEpisodeCount: {EpisodeCount}";
    }

}