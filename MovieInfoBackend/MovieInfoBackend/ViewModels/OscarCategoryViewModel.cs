namespace MovieInfoBackend.ViewModels;

// One category at one ceremony: its awards as one undivided subsection, or in a titled subsection each in the years
// the category was split (e.g. into black-and-white and color)
public record OscarCategoryViewModel(string Category, List<OscarCategorySubsectionViewModel> Subsections)
{
    public override string ToString()
    {
        return $"Category: {Category}\nSubsections:\n*****\n{string.Join("\n\n", Subsections)}\n*****";
    }
}
