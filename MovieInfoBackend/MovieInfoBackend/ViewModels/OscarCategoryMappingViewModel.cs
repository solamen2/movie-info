namespace MovieInfoBackend.ViewModels;

// Where one of the dataset's canonical categories is shown: in which category, and in which of its subsections if any
// (see OscarCategoriesHelper.ByCanonicalCategory)
public record OscarCategoryMappingViewModel(OscarDisplayCategoryViewModel Display, OscarDisplaySubsectionViewModel? Subsection)
{
    public override string ToString()
    {
        return $"Display:\n*****\n{Display}\n*****\nSubsection:\n*****\n{Subsection}\n*****";
    }
}
