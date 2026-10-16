namespace MovieInfoBackend.ViewModels;

// A division of a category in the years it was split, e.g. the black-and-white and color halves of cinematography
// (1939-66) or sound mixing and sound effects editing (1963-2019): the title of its subsection in the by-year listing,
// its name as a category of its own in a movie's or person's awards, the dataset's canonical categories that fold
// into it, and the names it has had that are shown as is (see OscarCategoriesHelper.Displayed)
public record OscarDisplaySubsectionViewModel(string Section, string Category, string[] CanonicalCategories, string[] ModernNames)
{
    public override string ToString()
    {
        return $"Section: {Section}\nCategory: {Category}\nCanonicalCategories: {string.Join(" | ", CanonicalCategories)}\nModernNames: {string.Join(" | ", ModernNames)}";
    }
}
