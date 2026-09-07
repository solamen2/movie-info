using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbMovieBelongsToCollectionDataModel
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("poster_path")]
    public required string PosterPath { get; init; }
    [JsonPropertyName("backdrop_path")]
    public required string BackdropPath { get; init; }

    public override string ToString()
    {
        return $"Id: {Id}\nName: {Name}\nPosterPath: {PosterPath}\nBackdropPath: {BackdropPath}";
    }

}