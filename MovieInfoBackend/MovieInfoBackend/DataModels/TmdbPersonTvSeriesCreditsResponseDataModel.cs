using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbPersonTvSeriesCreditsResponseDataModel
{
    [JsonPropertyName("cast")]
    public required TmdbPersonTvSeriesCastDataModel[] Cast { get; init; }
    [JsonPropertyName("crew")]
    public required TmdbPersonTvSeriesCrewDataModel[] Crew { get; init; }
    [JsonPropertyName("id")]
    public required int Id { get; init; }

    public override string ToString()
    {
        return $"Cast:\n*****\n{string.Join("\n\n", Cast)}\n*****\nCrew:\n*****\n{string.Join("\n\n", Crew)}\n*****\nId: {Id}";
    }
}