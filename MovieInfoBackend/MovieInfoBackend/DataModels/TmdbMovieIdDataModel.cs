using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbMovieIdDataModel
{
    [JsonPropertyName("adult")]
    public required bool Adult { get; init; }
    [JsonPropertyName("backdrop_path")]
    public required string BackdropPath { get; init; }
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("title")]
    public required string Title { get; init; }
    [JsonPropertyName("original_title")]
    public required string OriginalTitle { get; init; }
    [JsonPropertyName("overview")]
    public required string Overview { get; init; }
    [JsonPropertyName("poster_path")]
    public required string PosterPath { get; init; }
    [JsonPropertyName("media_type")]
    public required string MediaType { get; init; }
    [JsonPropertyName("original_language")]
    public required string OriginalLanguage { get; init; }
    [JsonPropertyName("genre_ids")]
    public required int[] GenreIds { get; init; }
    [JsonPropertyName("popularity")]
    public required double Popularity { get; init; }
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

    public override string ToString()
    {
        return $"Adult: {Adult}\nBackdropPath: {BackdropPath}\nId: {Id}\nTitle: {Title}\nOriginalTitle: {OriginalTitle}\nOverview: {Overview}\nPosterPath: {PosterPath}\nMediaType: {MediaType}\nOriginalLanguage: {OriginalLanguage}\nGenreIds: {string.Join(",", GenreIds)}\nPopularity: {Popularity}\nReleaseDate: {ReleaseDate}\nSoftcore: {Softcore}\nVideo: {Video}\nVoteAverage: {VoteAverage}\nVoteCount: {VoteCount}";
    }
}