using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record OmdbResponseDataModel
{
    [JsonPropertyName("Title")]
    public required string Title { get; init; }

    [JsonPropertyName("Year")]
    public required string Year { get; init; }

    [JsonPropertyName("Rated")]
    public required string Rated { get; init; }

    [JsonPropertyName("Released")]
    public required string Released { get; init; }

    [JsonPropertyName("Runtime")]
    public required string Runtime { get; init; }

    [JsonPropertyName("Genre")]
    public required string Genre { get; init; }

    [JsonPropertyName("Director")]
    public required string Director { get; init; }

    [JsonPropertyName("Writer")]
    public required string Writer { get; init; }

    [JsonPropertyName("Actors")]
    public required string Actors { get; init; }

    [JsonPropertyName("Plot")]
    public required string Plot { get; init; }

    [JsonPropertyName("Language")]
    public required string Language { get; init; }

    [JsonPropertyName("Country")]
    public required string Country { get; init; }

    [JsonPropertyName("Awards")]
    public required string Awards { get; init; }

    [JsonPropertyName("Poster")]
    public required string Poster { get; init; }

    [JsonPropertyName("Ratings")]
    public required OmdbRatingDataModel[] Ratings { get; init; }

    [JsonPropertyName("Metascore")]
    public required string Metascore { get; init; }

    [JsonPropertyName("imdbRating")]
    public required string ImdbRating { get; init; }

    [JsonPropertyName("imdbVotes")]
    public required string ImdbVotes { get; init; }  // TODO: Note for ViewModel: this is an int

    [JsonPropertyName("imdbID")]
    public required string ImdbId { get; init; }

    [JsonPropertyName("Type")]
    public required string Type { get; init; }

    [JsonPropertyName("DVD")]
    public string? DVD { get; init; }

    [JsonPropertyName("totalSeasons")]
    public string? TotalSeasons { get; init; }  // TODO: Note for ViewModel: this is an int

    [JsonPropertyName("BoxOffice")]
    public string? BoxOffice { get; init; }

    // TODO: Adjusted box office for current year

    [JsonPropertyName("Production")]
    public string? Production { get; init; }

    [JsonPropertyName("Website")]
    public string? Website { get; init; }

    [JsonPropertyName("Response")]
    public required string Response { get; init; }  // TODO: Note for ViewModel: this is a bool

    public override string ToString()
    {
        return $"Title: {Title}\nYear: {Year}\nRated: {Rated}\nReleased: {Released}\nRuntime: {Runtime}\nGenre: {Genre}\nDirector: {Director}\nWriter: {Writer}\nActors: {Actors}\nPlot: {Plot}\nLanguage: {Language}\nCountry: {Country}\nAwards: {Awards}\nPoster: {Poster}\nRatings:\n*****\n{string.Join("\n\n", Ratings)}\n*****\nMetascore: {Metascore}\nImdbRating: {ImdbRating}\nImdbVotes: {ImdbVotes}\nImdbId: {ImdbId}\nType: {Type}\nDVD: {DVD}\nTotal Seasons: {TotalSeasons}\nBoxOffice: {BoxOffice}\nProduction: {Production}\nWebsite: {Website}\nResponse: {Response}\n";
    }
}