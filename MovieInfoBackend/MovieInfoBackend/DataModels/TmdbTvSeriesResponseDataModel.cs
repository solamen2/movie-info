using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbTvSeriesResponseDataModel
{
    [JsonPropertyName("adult")]
    public required bool Adult { get; init; }
    [JsonPropertyName("backdrop_path")]
    public required string BackdropPath { get; init; }
    [JsonPropertyName("created_by")]
    public required TmdbCreatorDataModel[] CreatedBy { get; init; }
    [JsonPropertyName("episode_run_time")]
    public required int[] EpisodeRunTime { get; init; }
    [JsonPropertyName("first_air_date")]
    public string? FirstAirDate { get; init; }
    [JsonPropertyName("genres")]
    public required TmdbGenreDataModel[]? Genres { get; init; }
    [JsonPropertyName("homepage")]
    public required string Homepage { get; init; }
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("in_production")]
    public required bool InProduction { get; init; }
    [JsonPropertyName("languages")]
    public required string[] Languages { get; init; }
    [JsonPropertyName("last_air_date")]
    public string? LastAirDate { get; init; }
    [JsonPropertyName("last_episode_to_air")]
    public TmdbTvEpisodeDataModel? LastEpisodeToAir { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("next_episode_to_air")]
    public TmdbTvEpisodeDataModel? NextEpisodeToAir { get; init; }
    [JsonPropertyName("networks")]
    public required TmdbNetworkDataModel[] Networks { get; init; }
    [JsonPropertyName("number_of_episodes")]
    public required int NumberOfEpisodes { get; init; }
    [JsonPropertyName("number_of_seasons")]
    public required int NumberOfSeasons { get; init; }
    [JsonPropertyName("origin_country")]
    public required string[] OriginCountry { get; init; }
    [JsonPropertyName("original_language")]
    public required string OriginalLanguage { get; init; }
    [JsonPropertyName("original_name")]
    public required string OriginalName { get; init; }
    [JsonPropertyName("overview")]
    public required string Overview { get; init; }
    [JsonPropertyName("popularity")]
    public required double Popularity { get; init; }
    [JsonPropertyName("poster_path")]
    public required string PosterPath { get; init; }
    [JsonPropertyName("production_companies")]
    public required TmdbProductionCompanyDataModel[] ProductionCompanies { get; init; }
    [JsonPropertyName("production_countries")]
    public required TmdbProductionCountryDataModel[] ProductionCountries { get; init; }
    [JsonPropertyName("seasons")]
    public required TmdbTvSeasonDataModel[] Seasons { get; init; }
    [JsonPropertyName("softcore")]
    public required bool Softcore { get; init; }
    [JsonPropertyName("spoken_languages")]
    public required TmdbSpokenLanguageDataModel[] SpokenLanguages { get; init; }
    [JsonPropertyName("status")]
    public required string Status { get; init; }
    [JsonPropertyName("tagline")]
    public required string Tagline { get; init; }
    [JsonPropertyName("type")]
    public required string Type { get; init; }  // NOTE: Currently one of: Documentary, News, Miniseries, Reality, Scripted, Talk Show, Video
    [JsonPropertyName("vote_average")]
    public required double VoteAverage { get; init; }
    [JsonPropertyName("vote_count")]
    public required int VoteCount { get; init; }

    public override string ToString()
    {
        return $"Adult: {Adult}\nBackdropPath: {BackdropPath}\nCreatedBy:\n*****\n{string.Join("\n\n", CreatedBy)}\n*****\nEpisodeRunTime: {string.Join(",", EpisodeRunTime)}\nFirstAirDate: {FirstAirDate}\nGenres:\n*****\n{string.Join("\n\n", Genres)}\n*****\nHomepage: {Homepage}\nId: {Id}\nInProduction: {InProduction}\nLanguages: {string.Join(",", Languages)}\nLastAirDate: {LastAirDate}\nLastEpisodeToAir:\n*****\n{LastEpisodeToAir}\n*****\nName: {Name}\nNextEpisodeToAir:\n*****\n{NextEpisodeToAir}\n*****\nNetworks:\n*****\n{string.Join("\n\n", Networks)}\n*****\nNumberOfEpisodes: {NumberOfEpisodes}\nNumberOfSeasons: {NumberOfSeasons}\nOriginCountry: {string.Join(",", OriginCountry)}\nOriginalLanguage: {OriginalLanguage}\nOriginalName: {OriginalName}\nOverview: {Overview}\nPopularity: {Popularity}\nPosterPath: {PosterPath}\nProductionCompanies:\n*****\n{string.Join("\n\n", ProductionCompanies)}\n*****\nProductionCountries:\n*****\n{string.Join("\n\n", ProductionCountries)}\n*****\nSeasons:\n*****\n{string.Join("\n\n", Seasons)}\n*****\nSoftcore: {Softcore}\nSpokenLanguages:\n*****\n{string.Join("\n\n", SpokenLanguages)}\n*****\nStatus: {Status}\nTagline: {Tagline}\nType: {Type}\nVoteAverage: {VoteAverage}\nVoteCount: {VoteCount}";
    }

}