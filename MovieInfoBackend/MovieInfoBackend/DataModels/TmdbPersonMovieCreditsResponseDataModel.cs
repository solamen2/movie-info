using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbPersonMovieCreditsResponseDataModel
{
    [JsonPropertyName("cast")]
    public required TmdbPersonMovieCastCreditDataModel[] Cast { get; init; }
    [JsonPropertyName("crew")]
    public required TmdbPersonMovieCrewCreditDataModel[] Crew { get; init; }
    [JsonPropertyName("id")]
    public required int Id { get; init; }

    public override string ToString()
    {
        return $"Cast:\n*****\n{string.Join("\n\n", Cast)}\n*****\nCrew:\n*****\n{string.Join("\n\n", Crew)}\n*****\nId: {Id}";
    }
}