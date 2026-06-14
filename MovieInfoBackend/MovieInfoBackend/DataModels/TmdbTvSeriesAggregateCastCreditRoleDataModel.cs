using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbTvSeriesAggregateCastCreditRoleDataModel
{
    [JsonPropertyName("credit_id")]
    public required string CreditId { get; init; }
    [JsonPropertyName("character")]
    public required string Character { get; init; }
    [JsonPropertyName("episode_count")]
    public required int EpisodeCount { get; init; }

    public override string ToString()
    {
        return $"CreditId: {CreditId}\nCharacter: {Character}\nEpisodeCount: {EpisodeCount}";
    }

}