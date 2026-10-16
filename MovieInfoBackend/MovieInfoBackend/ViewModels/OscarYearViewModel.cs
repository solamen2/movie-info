namespace MovieInfoBackend.ViewModels;

// One ceremony: its categories in display order, then its special awards
public record OscarYearViewModel(int Ceremony, string Year, List<OscarCategoryViewModel> Categories, List<OscarAwardViewModel> SpecialAwards)
{
    public override string ToString()
    {
        return $"Ceremony: {Ceremony}\nYear: {Year}\nCategories:\n*****\n{string.Join("\n\n", Categories)}\n*****\nSpecialAwards:\n*****\n{string.Join("\n\n", SpecialAwards)}\n*****";
    }
}
