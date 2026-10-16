namespace MovieInfoBackend.ViewModels;

// The awards in one category, or in one subsection of a category split that year, at one ceremony: the winner(s)
// first, then the other nominees
public record OscarCategorySubsectionViewModel(string? Section, string? OriginalCategory, List<OscarAwardViewModel> Entries)
{
    // Section: the subsection's title when the category was split that year, e.g. "Color" or "Best Sound Mixing"; otherwise null
    // OriginalCategory: the name(s) at the time when differing from the modern one, e.g. "Outstanding Production"; otherwise null

    public override string ToString()
    {
        return $"Section: {Section}\nOriginalCategory: {OriginalCategory}\nEntries:\n*****\n{string.Join("\n\n", Entries)}\n*****";
    }
}
