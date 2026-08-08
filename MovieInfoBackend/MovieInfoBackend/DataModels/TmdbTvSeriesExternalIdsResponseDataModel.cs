using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbTvSeriesExternalIdsResponseDataModel
{
    [JsonPropertyName("id")]
    public required int TmdbId { get; init; }
    [JsonPropertyName("imdb_id")]
    public required string ImdbId { get; init; }
    [JsonPropertyName("freebase_mid")]
    public required string FreebaseMid { get; init; }
    [JsonPropertyName("freebase_id")]
    public required string FreebaseId { get; init; }
    [JsonPropertyName("tvdb_id")]
    public int? TvdbId { get; init; }
    [JsonPropertyName("tvrage_id")]
    public int? TvRageId { get; init; }
    [JsonPropertyName("wikidata_id")]
    public required string WikidataId { get; init; }
    [JsonPropertyName("facebook_id")]
    public string? FacebookId { get; init; }
    [JsonPropertyName("instagram_id")]
    public string? InstagramId { get; init; }
    [JsonPropertyName("twitter_id")]
    public string? TwitterId { get; init; }

    public override string ToString()
    {
        return $"TmdbId: {TmdbId}\nImdbId: {ImdbId}\nFreebaseMid: {FreebaseMid}\nFreebaseId: {FreebaseId}\nTvdbId: {TvdbId}\nTvRageId: {TvRageId}\nWikidataId: {WikidataId}\nFacebookId: {FacebookId}\nInstagramId: {InstagramId}\nTwitterId: {TwitterId}";
    }

}
