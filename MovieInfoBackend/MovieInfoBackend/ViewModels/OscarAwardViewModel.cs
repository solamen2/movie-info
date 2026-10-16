using MovieInfoBackend.DataModels;
using MovieInfoBackend.Helpers;

namespace MovieInfoBackend.ViewModels;

// One Academy Award nomination (or win): a film and / or the people honored, in one category at one ceremony
public record OscarAwardViewModel
{
    private const string ActingClass = "Acting";
    private const string OriginalSongCanonicalCategory = "MUSIC (Original Song)";

    public OscarAwardViewModel(OscarsRowDataModel row, string category, string? originalCategory, bool isSpecial)
    {
        this.Ceremony = row.Ceremony;
        this.Year = row.Year;
        this.Category = category;
        this.OriginalCategory = originalCategory;
        this.IsSpecial = isSpecial;
        this.Winner = row.Winner;
        this.Films = row.Films.Select((title, i) => new OscarFilmViewModel(title, OscarAwardsHelper.FilmImdbIdOrNull(row.FilmIds[i]))).ToList();
        this.Name = row.Name;
        this.Nominees = row.Nominees.Select((name, i) => new OscarNomineeViewModel(name, OscarAwardsHelper.PersonImdbIdOrNull(row.NomineeIds[i]))).ToList();
        this.Characters = row.Class == ActingClass && row.Detail != null ? row.Detail.Split('|').ToList() : new List<string>();
        this.Song = row.CanonicalCategory == OriginalSongCanonicalCategory ? row.Detail : null;
        this.Note = row.Note;
        this.Citation = row.Citation;
    }

    public int Ceremony { get; }
    public string Year { get; }  // The year of the films honored, e.g. "1927/28" or "2023"
    public string Category { get; }  // As shown, e.g. "Best Picture", "Honorary Award", or "Best Sound Mixing" in a year that category was split
    public string? OriginalCategory { get; }  // The category's name at the time when it differs from the modern one, e.g. "Outstanding Production"; otherwise null
    public bool IsSpecial { get; }  // A special (non-competitive) award, shown after the categories
    public bool Winner { get; }
    public List<OscarFilmViewModel> Films { get; }
    public string Name { get; }  // The credit as announced, e.g. "Emma Thomas, Charles Roven and Christopher Nolan, Producers"; "" for some special awards
    public List<OscarNomineeViewModel> Nominees { get; }  // The individual people (or companies) in Name
    public List<string> Characters { get; }  // Acting awards: the character played, one per film
    public string? Song { get; }  // Original Song awards: the song's title
    public string? Note { get; }
    public string? Citation { get; }  // The text of special awards

    public override string ToString()
    {
        return $"Ceremony: {Ceremony}\nYear: {Year}\nCategory: {Category}\nOriginalCategory: {OriginalCategory}\nIsSpecial: {IsSpecial}\nWinner: {Winner}\nFilms:\n*****\n{string.Join("\n\n", Films)}\n*****\nName: {Name}\nNominees:\n*****\n{string.Join("\n\n", Nominees)}\n*****\nCharacters: {string.Join(" | ", Characters)}\nSong: {Song}\nNote: {Note}\nCitation: {Citation}";
    }
}
