using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbPersonExternalIdsResponseDataModel
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("freebase_mid")]
    public required string FreebaseMid { get; init; }
    [JsonPropertyName("freebase_id")]
    public required string FreebaseId { get; init; }
    [JsonPropertyName("imdb_id")]
    public required string ImdbId { get; init; }
    [JsonPropertyName("tvrage_id")]
    public int? TvRageId { get; init; }
    [JsonPropertyName("wikidata_id")]
    public required string WikidataId { get; init; }
    [JsonPropertyName("facebook_id")]
    public required string FacebookId { get; init; }
    [JsonPropertyName("instagram_id")]
    public required string InstagramId { get; init; }
    [JsonPropertyName("tiktok_id")]
    public required string TiktokId { get; init; }
    [JsonPropertyName("twitter_id")]
    public required string TwitterId { get; init; }
    [JsonPropertyName("youtube_id")]
    public required string YoutubeId { get; init; }

    public override string ToString()
    {
        return $"Id: {Id}\nFreebaseMid: {FreebaseMid}\nFreebaseId: {FreebaseId}\nImdbId: {ImdbId}\nTvRageId: {TvRageId}\nWikidataId: {WikidataId}\nFacebookId: {FacebookId}\nInstagramId: {InstagramId}\nTiktokId: {TiktokId}\nTwitterId: {TwitterId}\nYoutubeId: {YoutubeId}";
    }

}
