using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbMovieExternalIdsResponseDataModel
{
    [JsonPropertyName("id")]
    public required int TmdbId { get; init; }
    [JsonPropertyName("imdb_id")]
    public required string ImdbId { get; init; }
    [JsonPropertyName("wikidata_id")]
    public string? WikidataId { get; init; }
    [JsonPropertyName("facebook_id")]
    public string? FacebookId { get; init; }
    [JsonPropertyName("instagram_id")]
    public string? InstagramId { get; init; }
    [JsonPropertyName("twitter_id")]
    public string? TwitterId { get; init; }

    public override string ToString()
    {
        return $"TmdbId: {TmdbId}\nImdbId: {ImdbId}\nWikidataId: {WikidataId}\nFacebookId: {FacebookId}\nInstagramId: {InstagramId}\nTwitterId: {TwitterId}";
    }

}
