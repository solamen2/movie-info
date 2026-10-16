using MovieInfoBackend.DataModels;
using MovieInfoBackend.Helpers;
using static MovieInfoBackend.Helpers.OscarCategoriesHelper;

namespace MovieInfoBackend.ViewModels;

// Every Academy Award shown by the app, by ceremony (most recent first)
public record OscarsViewModel
{
    public OscarsViewModel(List<OscarsRowDataModel> rows)
    {
        this.Years = new List<OscarYearViewModel>();
        foreach (IGrouping<int, OscarsRowDataModel> ceremonyRows in rows.Where(row => !IsExcluded(row))
                                                                        .GroupBy(row => row.Ceremony)
                                                                        .OrderByDescending(group => group.Key))
        {
            List<OscarCategoryViewModel> categories = new List<OscarCategoryViewModel>();
            foreach (OscarDisplayCategoryViewModel display in Displayed)
            {
                // The year's awards in the category by subsection (null for the undivided category), in the subsections' order
                List<IGrouping<OscarDisplaySubsectionViewModel?, OscarsRowDataModel>> subsectionRows =
                    ceremonyRows.Select(row => (row, mapping: MappingOf(row)))
                                .Where(pair => pair.mapping?.Display == display)
                                .GroupBy(pair => pair.mapping!.Subsection, pair => pair.row)
                                .OrderBy(group => group.Key == null ? -1 : Array.IndexOf(display.Subsections, group.Key))
                                .ToList();
                if (subsectionRows.Count == 0)
                {
                    continue;
                }
                // Subsection titles only when the category was split that year; awards in a single subsection are the whole category
                bool split = subsectionRows.Count > 1;
                List<OscarCategorySubsectionViewModel> subsections = new List<OscarCategorySubsectionViewModel>();
                foreach (IGrouping<OscarDisplaySubsectionViewModel?, OscarsRowDataModel> group in subsectionRows)
                {
                    OscarDisplaySubsectionViewModel? subsection = split ? group.Key : null;
                    string category = subsection?.Category ?? display.Name;
                    string[] modernNames = subsection?.ModernNames ?? display.ModernNames;
                    List<OscarAwardViewModel> entries = new List<OscarAwardViewModel>();
                    foreach (OscarsRowDataModel row in group.OrderByDescending(row => row.Winner))  // Stable, so otherwise in the dataset's order
                    {
                        string? originalCategory = modernNames.Contains(row.Category) ? null : ToTitleCase(row.Category);
                        entries.Add(new OscarAwardViewModel(row, category, originalCategory, false));
                    }
                    // Usually one name a year; all are shown when there were more
                    string originalCategories = string.Join(" / ", entries.Select(entry => entry.OriginalCategory).OfType<string>().Distinct());
                    subsections.Add(new OscarCategorySubsectionViewModel(subsection?.Section, originalCategories == "" ? null : originalCategories, entries));
                }
                categories.Add(new OscarCategoryViewModel(display.Name, subsections));
            }
            List<OscarAwardViewModel> specialAwards =
                ceremonyRows.Where(row => MappingOf(row) == null && IsSpecialAward(row))
                            .Select(row => new OscarAwardViewModel(row,
                                                                   ToTitleCase(row.CanonicalCategory),
                                                                   row.Category == row.CanonicalCategory ? null : ToTitleCase(row.Category),
                                                                   true))
                            .ToList();
            this.Years.Add(new OscarYearViewModel(ceremonyRows.Key, ceremonyRows.First().Year, categories, specialAwards));
        }
    }

    public List<OscarYearViewModel> Years { get; }

    public override string ToString()
    {
        return $"Years:\n*****\n{string.Join("\n\n", Years)}\n*****";
    }
}
