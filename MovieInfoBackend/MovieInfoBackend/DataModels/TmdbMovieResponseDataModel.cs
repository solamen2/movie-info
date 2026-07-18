using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbMovieResponseDataModel
{
    [JsonPropertyName("adult")]
    public required bool Adult { get; init; }
    [JsonPropertyName("backdrop_path")]
    public required string BackdropPath { get; init; }
    [JsonPropertyName("belongs_to_collection")]
    public required string BelongsToCollection { get; init; }
    [JsonPropertyName("budget")]
    public required long Budget { get; init; }
    [JsonPropertyName("genres")]
    public required TmdbGenreDataModel[]? Genres { get; init; }
    [JsonPropertyName("homepage")]
    public required string Homepage { get; init; }
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("imdb_id")]
    public required string ImdbId { get; init; }
    [JsonPropertyName("origin_country")]
    public required string[] OriginCountry { get; init; }
    [JsonPropertyName("original_language")]
    public required string OriginalLanguage { get; init; }
    [JsonPropertyName("original_title")]
    public required string OriginalTitle { get; init; }
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
    [JsonPropertyName("release_date")]
    public required string ReleaseDate { get; init; }
    [JsonPropertyName("revenue")]
    public required long Revenue { get; init; }
    [JsonPropertyName("runtime")]
    public required int Runtime { get; init; }
    [JsonPropertyName("softcore")]
    public required bool Softcore { get; init; }
    [JsonPropertyName("spoken_languages")]
    public required TmdbSpokenLanguageDataModel[] SpokenLanguages { get; init; }
    [JsonPropertyName("status")]
    public required string Status { get; init; }
    [JsonPropertyName("tagline")]
    public required string Tagline { get; init; }
    [JsonPropertyName("title")]
    public required string Title { get; init; }
    [JsonPropertyName("video")]
    public required bool Video { get; init; }
    [JsonPropertyName("vote_average")]
    public required double VoteAverage { get; init; }
    [JsonPropertyName("vote_count")]
    public required int VoteCount { get; init; }

    public override string ToString()
    {
        return $"Adult: {Adult}\nBackdropPath: {BackdropPath}\nBelongsToCollection: {BelongsToCollection}\nBudget: {Budget}\nGenres:\n*****\n{string.Join("\n\n", Genres)}\n*****\nHomepage: {Homepage}\nId: {Id}\nImdbId: {ImdbId}\nOriginCountry: {string.Join(", ", OriginCountry)}\nOriginalLanguage: {OriginalLanguage}\nOriginalTitle: {OriginalTitle}\nOverview: {Overview}\nPopularity: {Popularity}\nPosterPath: {PosterPath}\nProductionCompanies:\n*****\n{string.Join("\n\n", ProductionCompanies)}\n*****\nProductionCountries:\n*****\n{string.Join("\n\n", ProductionCountries)}\n*****\nReleaseDate: {ReleaseDate}\nRevenue: {Revenue}\nRuntime: {Runtime}\nSoftcore: {Softcore}\nSpokenLanguages:\n*****\n{string.Join("\n\n", SpokenLanguages)}\n*****\nStatus: {Status}\nTagline: {Tagline}\nTitle: {Title}\nVideo: {Video}\nVoteAverage: {VoteAverage}\nVoteCount: {VoteCount}";
    }

}