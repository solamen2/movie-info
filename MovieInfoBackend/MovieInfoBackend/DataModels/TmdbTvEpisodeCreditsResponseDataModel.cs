using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbTvEpisodeCreditsResponseDataModel
{
    [JsonPropertyName("cast")]
    public required TmdbTvEpisodeCreditsCastDataModel[] Cast { get; init; }
    [JsonPropertyName("crew")]
    public required TmdbTvEpisodeCreditsCrewDataModel[] Crew { get; init; }
    [JsonPropertyName("guest_stars")]
    public required TmdbTvEpisodeCreditsGuestStarDataModel[] GuestStars { get; init; }
    [JsonPropertyName("id")]
    public required int Id { get; init; }

    public override string ToString()
    {
        return $"Cast:\n*****\n{string.Join("\n\n", Cast)}\n*****\nCrew:\n*****\n{string.Join("\n\n", Crew)}\n*****\nGuestStars:\n*****\n{string.Join("\n\n", GuestStars)}\n*****\nId: {Id}";
    }
}