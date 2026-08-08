using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbTvEpisodeResponseDataModel
{
    [JsonPropertyName("air_date")]
    public string? AirDate { get; init; }
    [JsonPropertyName("crew")]
    public required TmdbTvEpisodeCrewDataModel[] Crew { get; init; }  // NOTE: Not used by view models currently; using credits version instead
    [JsonPropertyName("episode_number")]
    public required int EpisodeNumber { get; init; }
    [JsonPropertyName("episode_type")]
    public required string EpisodeType { get; init; }
    [JsonPropertyName("guest_stars")]
    public required TmdbTvEpisodeGuestStarDataModel[] GuestStars { get; init; }  // NOTE: Not used by view models currently; using credits version instead
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("overview")]
    public required string Overview { get; init; }
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("production_code")]
    public required string ProductionCode { get; init; }
    [JsonPropertyName("runtime")]
    public required int Runtime { get; init; }
    [JsonPropertyName("season_number")]
    public required int SeasonNumber { get; init; }
    [JsonPropertyName("still_path")]
    public required string StillPath { get; init; }
    [JsonPropertyName("vote_average")]
    public required double VoteAverage { get; init; }
    [JsonPropertyName("vote_count")]
    public required int VoteCount { get; init; }

    public override string ToString()
    {
        return $"AirDate: {AirDate}\nCrew:\n*****\n{string.Join("\n\n", Crew)}\n*****\nEpisodeNumber: {EpisodeNumber}\nEpisodeType: {EpisodeType}\nGuestStars:\n*****\n{string.Join("\n\n", GuestStars)}\n*****\nName: {Name}\nOverview: {Overview}\nId: {Id}\nProductionCode: {ProductionCode}\nRuntime: {Runtime}\nSeasonNumber: {SeasonNumber}\nStillPath: {StillPath}\nVoteAverage: {VoteAverage}\nVoteCount: {VoteCount}";
    }

}
