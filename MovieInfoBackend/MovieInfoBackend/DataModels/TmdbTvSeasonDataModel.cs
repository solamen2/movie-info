using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbTvSeasonDataModel
{
    [JsonPropertyName("air_date")]
    public string? AirDate { get; init; }
    [JsonPropertyName("episode_count")]
    public required int EpisodeCount { get; init; }
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("overview")]
    public required string Overview { get; init; }
    [JsonPropertyName("poster_path")]
    public required string PosterPath { get; init; }
    [JsonPropertyName("season_number")]
    public required int SeasonNumber { get; init; }
    [JsonPropertyName("vote_average")]
    public required double VoteAverage { get; init; }

    public override string ToString()
    {
        return $"AirDate: {AirDate}\nEpisodeCount: {EpisodeCount}\nId: {Id}\nName: {Name}\nOverview: {Overview}\nPosterPath: {PosterPath}\nSeasonNumber: {SeasonNumber}\nVoteAverage: {VoteAverage}";
    }

}