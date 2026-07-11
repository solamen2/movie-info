using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbPersonMovieCastDataModel
{
    [JsonPropertyName("adult")]
    public required bool Adult { get; init; }
    [JsonPropertyName("backdrop_path")]
    public required string BackdropPath { get; init; }
    [JsonPropertyName("genre_ids")]
    public required int[] GenreIds { get; init; }
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("title")]
    public required string Title { get; init; }
    [JsonPropertyName("original_language")]
    public required string OriginalLanguage { get; init; }
    [JsonPropertyName("original_title")]
    public required string OriginalTitle { get; init; }
    [JsonPropertyName("overview")]
    public required string Overview { get; init; }
    [JsonPropertyName("popularity")]
    public required double Popularity { get; init; }
    [JsonPropertyName("poster_path")]
    public required string PosterPath { get; init; }
    [JsonPropertyName("release_date")]
    public required string ReleaseDate { get; init; }
    [JsonPropertyName("softcore")]
    public required bool Softcore { get; init; }
    [JsonPropertyName("video")]
    public required bool Video { get; init; }
    [JsonPropertyName("vote_average")]
    public required double VoteAverage { get; init; }
    [JsonPropertyName("vote_count")]
    public required int VoteCount { get; init; }
    [JsonPropertyName("character")]
    public required string Character { get; init; }
    [JsonPropertyName("credit_id")]
    public required string CreditId { get; init; }
    [JsonPropertyName("order")]
    public required int Order { get; init; }

    public override string ToString()
    {
        return $"Adult: {Adult}\nBackdropPath: {BackdropPath}\nGenreIds: {string.Join(",", GenreIds)}\nId: {Id}\nTitle: {Title}\nOriginalLanguage: {OriginalLanguage}\nOriginalTitle: {OriginalTitle}\nOverview: {Overview}\nPopularity: {Popularity}\nPosterPath: {PosterPath}\nReleaseDate: {ReleaseDate}\nSoftcore: {Softcore}\nVideo: {Video}\nVoteAverage: {VoteAverage}\nVoteCount: {VoteCount}\nCharacter: {Character}\nCreditId: {CreditId}\nOrder: {Order}";
    }
}