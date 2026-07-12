using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbIdResponseDataModel
{
    [JsonPropertyName("movie_results")]
    public TmdbMovieIdDataModel[]? MovieResults { get; init; }
    [JsonPropertyName("person_results")]
    public TmdbPersonIdDataModel[]? PersonResults { get; init; }
    [JsonPropertyName("tv_results")]
    public TmdbTvSeriesIdDataModel[]? TvResults { get; init; }
    [JsonPropertyName("tv_episode_results")]
    public TmdbIndividualTvEpisodeDataModel[]? TvEpisodeResults { get; init; }
    [JsonPropertyName("tv_season_results")]
    public TmdbTvSeasonIdDataModel[]? TvSeasonResults { get; init; }

    public override string ToString()
    {
        return $"MovieResults:\n*****\n{string.Join("\n\n", MovieResults)}\n*****\nPersonResults:\n*****\n{string.Join("\n\n", PersonResults)}\n*****\nTvResults:\n*****\n{string.Join("\n\n", TvResults)}\n*****\nTvEpisodeResults:\n*****\n{string.Join("\n\n", TvEpisodeResults)}\n*****\nTvSeasonResults:\n*****\n{string.Join("\n\n", TvSeasonResults)}\n*****\n";
    }
}