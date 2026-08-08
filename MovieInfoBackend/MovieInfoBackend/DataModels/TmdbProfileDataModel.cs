using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbProfileDataModel
{
    [JsonPropertyName("aspect_ratio")]
    public required double AspectRatio { get; init; }
    [JsonPropertyName("height")]
    public required int Height { get; init; }
    [JsonPropertyName("iso_3166_1")]
    public string? Iso31661 { get; init; }
    [JsonPropertyName("iso_639_1")]
    public string? Iso6391 { get; init; }
    [JsonPropertyName("file_path")]
    public required string FilePath { get; init; }
    [JsonPropertyName("vote_average")]
    public required double VoteAverage { get; init; }
    [JsonPropertyName("vote_count")]
    public required int VoteCount { get; init; }
    [JsonPropertyName("width")]
    public required int Width { get; init; }

    public override string ToString()
    {
        return $"AspectRatio: {AspectRatio}\nHeight: {Height}\nIso31661: {Iso31661}\nIso6391: {Iso6391}\nFilePath: {FilePath}\nVoteAverage: {VoteAverage}\nVoteCount: {VoteCount}\nWidth: {Width}";
    }
}