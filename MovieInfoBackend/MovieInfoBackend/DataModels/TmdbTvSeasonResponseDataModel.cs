using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbTvSeasonResponseDataModel
{
    [JsonPropertyName("_id")]
    public required string Id1 { get; init; }
    [JsonPropertyName("air_date")]
    public required string AirDate { get; init; }
    [JsonPropertyName("episodes")]
    public required TmdbTvSeasonEpisodeDataModel[] Episodes { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("networks")]
    public required TmdbNetworkDataModel[] Networks { get; init; }
    [JsonPropertyName("overview")]
    public required string Overview { get; init; }
    [JsonPropertyName("id")]
    public required int Id2 { get; init; }
    [JsonPropertyName("poster_path")]
    public required string PosterPath { get; init; }
    [JsonPropertyName("season_number")]
    public required int SeasonNumber { get; init; }
    [JsonPropertyName("vote_average")]
    public required double VoteAverage { get; init; }

    

    public override string ToString()
    {
        return $"Id1: {Id1}\nAirDate: {AirDate}\nEpisodes:\n*****\n{string.Join("\n\n", Episodes)}\n*****\nName: {Name}\nNetworks:\n*****\n{string.Join("\n\n", Networks)}\n*****\nOverview: {Overview}\nId2: {Id2}\nPosterPath: {PosterPath}\nSeasonNumber: {SeasonNumber}\nVoteAverage: {VoteAverage}";
    }

}