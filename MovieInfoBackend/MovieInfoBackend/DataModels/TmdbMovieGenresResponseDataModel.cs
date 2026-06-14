using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbGenresResponseDataModel
{
    [JsonPropertyName("genres")]
    public required TmdbGenreDataModel[] Genres { get; init; }

    public override string ToString()
    {
        return $"Genres:\n*****\n{string.Join("\n\n", Genres)}";
    }
}