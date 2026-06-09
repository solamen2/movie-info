using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbTvSeriesIdDataModel
{
    [JsonPropertyName("adult")]
    public required bool Adult { get; init; }
    [JsonPropertyName("backdrop_path")]
    public required string BackdropPath { get; init; }
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("original_name")]
    public required string OriginalName { get; init; }
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
    [JsonPropertyName("first_air_date")]
    public string? FirstAirDate { get; init; }
    [JsonPropertyName("softcore")]
    public required bool Softcore { get; init; }
    [JsonPropertyName("vote_average")]
    public required double VoteAverage { get; init; }
    [JsonPropertyName("vote_count")]
    public required int VoteCount { get; init; }
    [JsonPropertyName("origin_country")]
    public required string[] OriginCountry { get; init; }

    public override string ToString()
    {
        return $"Adult: {Adult}\nBackdropPath: {BackdropPath}\nId: {Id}\nName: {Name}\nOriginalName: {OriginalName}\nOverview: {Overview}\nPosterPath: {PosterPath}\nMediaType: {MediaType}\nOriginalLanguage: {OriginalLanguage}\nGenreIds: {string.Join(",", GenreIds)}\nPopularity: {Popularity}\nFirstAirDate: {FirstAirDate}\nSoftcore: {Softcore}\nVoteAverage: {VoteAverage}\nVoteCount: {VoteCount}\nOriginCountry: {string.Join(",", OriginCountry)}";
    }
}