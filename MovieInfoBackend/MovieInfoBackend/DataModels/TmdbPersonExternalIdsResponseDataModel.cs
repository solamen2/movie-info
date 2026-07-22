using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbPersonExternalIdsResponseDataModel
{
    [JsonPropertyName("id")]
    public required int TmdbId { get; init; }
    [JsonPropertyName("freebase_mid")]
    public required string FreebaseMid { get; init; }
    [JsonPropertyName("freebase_id")]
    public required string FreebaseId { get; init; }
    [JsonPropertyName("imdb_id")]
    public required string ImdbId { get; init; }
    [JsonPropertyName("tvrage_id")]
    public int? TvRageId { get; init; }
    [JsonPropertyName("wikidata_id")]
    public string? WikidataId { get; init; }
    [JsonPropertyName("facebook_id")]
    public string? FacebookId { get; init; }
    [JsonPropertyName("instagram_id")]
    public string? InstagramId { get; init; }
    [JsonPropertyName("tiktok_id")]
    public string? TiktokId { get; init; }
    [JsonPropertyName("twitter_id")]
    public string? TwitterId { get; init; }
    [JsonPropertyName("youtube_id")]
    public string? YoutubeId { get; init; }

    public override string ToString()
    {
        return $"TmdbId: {TmdbId}\nFreebaseMid: {FreebaseMid}\nFreebaseId: {FreebaseId}\nImdbId: {ImdbId}\nTvRageId: {TvRageId}\nWikidataId: {WikidataId}\nFacebookId: {FacebookId}\nInstagramId: {InstagramId}\nTiktokId: {TiktokId}\nTwitterId: {TwitterId}\nYoutubeId: {YoutubeId}";
    }

}
