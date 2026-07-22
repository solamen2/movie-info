using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbTvSeasonIdDataModel
{
    [JsonPropertyName("id")]
    public required int TmdbId { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("overview")]
    public required string Overview { get; init; }
    [JsonPropertyName("poster_path")]
    public required string PosterPath { get; init; }
    [JsonPropertyName("media_type")]
    public required string MediaType { get; init; }
    [JsonPropertyName("vote_average")]
    public required double VoteAverage { get; init; }
    [JsonPropertyName("air_date")]
    public string? AirDate { get; init; }
    [JsonPropertyName("season_number")]
    public required int SeasonNumber { get; init; }
    [JsonPropertyName("show_id")]
    public required int ShowId { get; init; }
    [JsonPropertyName("episode_count")]
    public required int EpisodeCount { get; init; }

    public override string ToString()
    {
        return $"Tmdbd: {TmdbId}\nName: {Name}\nOverview: {Overview}\nPosterPath: {PosterPath}\nMediaType: {MediaType}\nVoteAverage: {VoteAverage}\nAirDate: {AirDate}\nSeasonNumber: {SeasonNumber}\nShowId: {ShowId}\nEpisodeCount: {EpisodeCount}";
    }
}