using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbMovieExternalIdsResponseDataModel
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("imdb_id")]
    public required string ImdbId { get; init; }
    [JsonPropertyName("wikidata_id")]
    public required string WikidataId { get; init; }
    [JsonPropertyName("facebook_id")]
    public required string FacebookId { get; init; }
    [JsonPropertyName("instagram_id")]
    public required string InstagramId { get; init; }
    [JsonPropertyName("twitter_id")]
    public required string TwitterId { get; init; }

    public override string ToString()
    {
        return $"Id: {Id}\nImdbId: {ImdbId}\nWikidataId: {WikidataId}\nFacebookId: {FacebookId}\nInstagramId: {InstagramId}\nTwitterId: {TwitterId}";
    }

}
