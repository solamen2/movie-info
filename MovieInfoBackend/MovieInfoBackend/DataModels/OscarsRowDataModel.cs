namespace MovieInfoBackend.DataModels;

// One row of oscars.csv (which is tab separated, despite its name) from https://github.com/DLu/oscar_data;
// see the README there for the columns. Lists in a cell are "|" separated in the file.
public record OscarsRowDataModel
{
    public required int Ceremony { get; init; }  // 1 for the first ceremony (films of 1927/28), 2 for the second, …
    public required string Year { get; init; }  // The year of the films honored, e.g. "1927/28" or "2023"
    public required string Class { get; init; }  // e.g. "Acting", "Title", "Writing", "Special", "SciTech"
    public required string CanonicalCategory { get; init; }  // The category's modern name, e.g. "ACTOR IN A LEADING ROLE"
    public required string Category { get; init; }  // The category's name at that ceremony, e.g. "ACTOR"
    public required string[] Films { get; init; }  // Usually one; several for some of the earliest nominations, and none for most special awards
    public required string?[] FilmIds { get; init; }  // IMDB ids parallel to Films, null where the dataset has none
    public required string Name { get; init; }  // The credit as announced, e.g. "Emma Thomas, Charles Roven and Christopher Nolan, Producers"
    public required string[] Nominees { get; init; }  // The individual people (or companies) in Name
    public required string?[] NomineeIds { get; init; }  // IMDB ids parallel to Nominees, null where the dataset has none
    public required bool Winner { get; init; }
    public required string? Detail { get; init; }  // Character name(s) for acting awards (one per film, "|" separated), the song title for songs
    public required string? Note { get; init; }
    public required string? Citation { get; init; }  // The text of special awards, e.g. "To Gene Kelly in appreciation of his versatility…"

    public override string ToString()
    {
        return $"Ceremony: {Ceremony}\nYear: {Year}\nClass: {Class}\nCanonicalCategory: {CanonicalCategory}\nCategory: {Category}\nFilms: {string.Join(" | ", Films)}\nFilmIds: {string.Join(" | ", FilmIds)}\nName: {Name}\nNominees: {string.Join(" | ", Nominees)}\nNomineeIds: {string.Join(" | ", NomineeIds)}\nWinner: {Winner}\nDetail: {Detail}\nNote: {Note}\nCitation: {Citation}";
    }
}
