using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

// NOTE: This model is not used by this app currently, because in all endpoints used by this app, the data models have the genre names already in them and don't need lookups
public record TmdbGenresResponseDataModel
{
    [JsonPropertyName("genres")]
    public required TmdbGenreDataModel[] Genres { get; init; }

    public override string ToString()
    {
        return $"Genres:\n*****\n{string.Join("\n\n", Genres)}";
    }

    public Dictionary<int, string>? GetGenresDictionary()
    {
        if (Genres == null || Genres.Length <= 0)
        {
            return null;
        }
        
        Dictionary<int, string> genreIdToGenreNameDictionary = new Dictionary<int, string>();

        foreach (TmdbGenreDataModel genre in Genres)
        {
            genreIdToGenreNameDictionary[genre.Id] = genre.Name;
        }

        return genreIdToGenreNameDictionary;
    }
}