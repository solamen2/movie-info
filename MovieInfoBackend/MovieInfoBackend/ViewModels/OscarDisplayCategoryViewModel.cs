namespace MovieInfoBackend.ViewModels;

// A category as shown, with the dataset's canonical categories that fold into it when it is undivided, the names the
// category has had over the years that are shown as is, and its subsections in the years it was split (a year with
// awards in only one of them shows them undivided). An award under any other name gets its name at the time shown
// next to the modern name (see OscarCategorySubsectionViewModel.OriginalCategory and OscarCategoriesHelper.Displayed).
public record OscarDisplayCategoryViewModel(string Name, string[] CanonicalCategories, string[] ModernNames, OscarDisplaySubsectionViewModel[]? SplitSubsections = null)
{
    public OscarDisplaySubsectionViewModel[] Subsections { get; } = SplitSubsections ?? [];

    public override string ToString()
    {
        return $"Name: {Name}\nCanonicalCategories: {string.Join(" | ", CanonicalCategories)}\nModernNames: {string.Join(" | ", ModernNames)}\nSubsections:\n*****\n{string.Join("\n\n", Subsections)}\n*****";
    }
}
