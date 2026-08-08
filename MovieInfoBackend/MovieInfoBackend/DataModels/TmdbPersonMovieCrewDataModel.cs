using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbPersonMovieCrewDataModel  // NOTE: More of a movie description than a person description 
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
    [JsonPropertyName("credit_id")]
    public required string CreditId { get; init; }
    [JsonPropertyName("department")]
    public required string Department { get; init; }
    [JsonPropertyName("job")]
    public required string Job { get; init; }

    public override string ToString()
    {
        return $"Adult: {Adult}\nBackdropPath: {BackdropPath}\nGenreIds: {string.Join(", ", GenreIds)}\nId: {Id}\nTitle: {Title}\nOriginalLanguage: {OriginalLanguage}\nOriginalTitle: {OriginalTitle}\nOverview: {Overview}\nPopularity: {Popularity}\nPosterPath: {PosterPath}\nReleaseDate: {ReleaseDate}\nSoftcore: {Softcore}\nVideo: {Video}\nVoteAverage: {VoteAverage}\nVoteCount: {VoteCount}\nCreditId: {CreditId}\nDepartment: {Department}\nJob: {Job}";
    }
}