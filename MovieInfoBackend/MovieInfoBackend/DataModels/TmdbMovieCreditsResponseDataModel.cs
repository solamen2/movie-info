using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbMovieCreditsResponseDataModel
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("cast")]
    public required TmdbMovieCastDataModel[] Cast { get; init; }
    [JsonPropertyName("crew")]
    public required TmdbMovieCrewDataModel[] Crew { get; init; }


    public override string ToString()
    {
        return $"Id: {Id}\nCast:\n*****\n{string.Join("\n\n", Cast)}\n*****\nCrew:\n*****\n{string.Join("\n\n", Crew)}";
    }
}