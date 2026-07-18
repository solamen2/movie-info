using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbPersonTvSeriesCrewDataModel
{
    [JsonPropertyName("adult")]
    public required bool Adult { get; init; }
    [JsonPropertyName("backdrop_path")]
    public required string BackdropPath { get; init; }
    [JsonPropertyName("genre_ids")]
    public required int[] GenreIds { get; init; }
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("origin_country")]
    public required string[] OriginCountry { get; init; }
    [JsonPropertyName("original_language")]
    public required string OriginalLanguage { get; init; }
    [JsonPropertyName("original_name")]
    public required string OriginalName { get; init; }
    [JsonPropertyName("overview")]
    public required string Overview { get; init; }
    [JsonPropertyName("popularity")]
    public required double Popularity { get; init; }
    [JsonPropertyName("poster_path")]
    public required string PosterPath { get; init; }
    [JsonPropertyName("first_air_date")]
    public string? FirstAirDate { get; init; }
    [JsonPropertyName("softcore")]
    public required bool Softcore { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("vote_average")]
    public required double VoteAverage { get; init; }
    [JsonPropertyName("vote_count")]
    public required int VoteCount { get; init; }
    [JsonPropertyName("credit_id")]
    public required string CreditId { get; init; }
    [JsonPropertyName("department")]
    public required string Department { get; init; }
    [JsonPropertyName("episode_count")]
    public required int EpisodeCount { get; init; }
    [JsonPropertyName("first_credit_air_date")]
    public required string FirstCreditAirDate { get; init; }
    [JsonPropertyName("job")]
    public required string Job { get; init; }


    public override string ToString()
    {
        return $"Adult: {Adult}\nBackdropPath: {BackdropPath}\nGenreIds: {string.Join(", ", GenreIds)}\nId: {Id}\nOriginCountry: {string.Join(", ", OriginCountry)}\nOriginalLanguage: {OriginalLanguage}\nOriginalName: {OriginalName}\nOverview: {Overview}\nPopularity: {Popularity}\nPosterPath: {PosterPath}\nFirstAirDate: {FirstAirDate}\nSoftcore: {Softcore}\nName: {Name}\nVoteAverage: {VoteAverage}\nVoteCount: {VoteCount}\nCreditId: {CreditId}\nDepartment: {Department}\nEpisodeCount: {EpisodeCount}\nFirstCreditAirDate: {FirstCreditAirDate}\nJob: {Job}";
    }
}