using System.Collections.Frozen;
using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;

namespace MovieInfoBackend.Helpers;

// Which Academy Award categories the app shows, under which (modern) names, and how the dataset's categories fold into them.
public static class OscarCategoriesHelper
{
    // In display order. The mapping follows the dataset's CanonicalCategory, extended where Wikipedia's page for the
    // modern category counts an older award as part of its history:
    // - Best Sound: Sound Recording (1930-57) and Sound Editing / Sound Effects (Editing), merged into Sound in 2020, plus
    //   the Special Achievement Awards given for sound effects (editing) when no competitive award was
    // - Best Original Screenplay: Original / Motion Picture Story (1928-56), merged into Original Screenplay in 1957
    // - Best Original Score: the parallel musical / adaptation scoring awards (1934-85, 1995-99)
    // - Best Visual Effects: the Special Achievement Awards given when no competitive award was
    // - Best International Feature Film: the special / honorary foreign language film awards of 1947-55
    // A category never appears twice in a year: where it had two divisions at once (black-and-white and color, sound
    // mixing and sound effects editing, …) the year's listing shows a subsection for each.
    public static readonly OscarDisplayCategoryViewModel[] Displayed =
    [
        new("Best Picture", ["BEST PICTURE"], ["BEST PICTURE"]),
        new("Best Director", ["DIRECTING"], ["DIRECTING"],
            [new("Dramatic Picture", "Best Director (Dramatic Picture)", ["DIRECTING (Dramatic Picture)"], ["DIRECTING (Dramatic Picture)"]),
             new("Comedy Picture", "Best Director (Comedy Picture)", ["DIRECTING (Comedy Picture)"], ["DIRECTING (Comedy Picture)"])]),
        new("Best Actor", ["ACTOR IN A LEADING ROLE"], ["ACTOR", "ACTOR IN A LEADING ROLE"]),
        new("Best Supporting Actor", ["ACTOR IN A SUPPORTING ROLE"], ["ACTOR IN A SUPPORTING ROLE"]),
        new("Best Actress", ["ACTRESS IN A LEADING ROLE"], ["ACTRESS", "ACTRESS IN A LEADING ROLE"]),
        new("Best Supporting Actress", ["ACTRESS IN A SUPPORTING ROLE"], ["ACTRESS IN A SUPPORTING ROLE"]),
        new("Best Animated Feature Film", ["ANIMATED FEATURE FILM"], ["ANIMATED FEATURE FILM"]),
        new("Best International Feature Film", ["INTERNATIONAL FEATURE FILM", "SPECIAL FOREIGN LANGUAGE FILM AWARD"], ["INTERNATIONAL FEATURE FILM"]),
        new("Best Documentary Feature", ["DOCUMENTARY (Feature)"], ["DOCUMENTARY (Feature)", "DOCUMENTARY FEATURE FILM"]),
        new("Best Cinematography", ["CINEMATOGRAPHY"], ["CINEMATOGRAPHY"],
            [new("Black-and-White", "Best Cinematography (Black-and-White)", ["CINEMATOGRAPHY (Black-and-White)"], ["CINEMATOGRAPHY (Black-and-White)"]),
             new("Color", "Best Cinematography (Color)", ["CINEMATOGRAPHY (Color)"], ["CINEMATOGRAPHY (Color)"])]),
        new("Best Original Screenplay", [], ["WRITING (Original Screenplay)"],
            [new("Best Original Screenplay", "Best Original Screenplay", ["WRITING (Original Screenplay)"], ["WRITING (Original Screenplay)"]),
             new("Best Story", "Best Story", ["WRITING (Original Story)"], [])]),
        new("Best Adapted Screenplay", ["WRITING (Adapted Screenplay)"], ["WRITING (Adapted Screenplay)"]),
        new("Best Film Editing", ["FILM EDITING"], ["FILM EDITING"]),
        new("Best Visual Effects", ["VISUAL EFFECTS", "SPECIAL ACHIEVEMENT AWARD (Visual Effects)"], ["VISUAL EFFECTS"]),
        // Non-musical and musical as on Wikipedia's history of the award, which the dataset's canonical categories follow
        new("Best Original Score", [], ["MUSIC (Original Score)"],
            [new("Best Original Non-Musical Score", "Best Original Non-Musical Score", ["MUSIC (Original Score)"], []),
             new("Best Original Musical Score", "Best Original Musical Score", ["MUSIC (Original Song Score or Adaptation Score)"], [])]),
        new("Best Original Song", ["MUSIC (Original Song)"], ["MUSIC (Original Song)"]),
        new("Best Sound", [], ["SOUND"],
            [new("Best Sound Mixing", "Best Sound Mixing", ["SOUND MIXING", "SOUND RECORDING"], ["SOUND MIXING"]),
             new("Best Sound Editing", "Best Sound Editing",
                 ["SOUND EDITING", "SPECIAL ACHIEVEMENT AWARD (Sound Effects)", "SPECIAL ACHIEVEMENT AWARD (Sound Effects Editing)", "SPECIAL ACHIEVEMENT AWARD (Sound Editing)"],
                 ["SOUND EDITING"])]),
        new("Best Production Design", ["ART DIRECTION"], ["PRODUCTION DESIGN"],
            [new("Black-and-White", "Best Production Design (Black-and-White)", ["ART DIRECTION (Black-and-White)"], ["ART DIRECTION (Black-and-White)"]),
             new("Color", "Best Production Design (Color)", ["ART DIRECTION (Color)"], ["ART DIRECTION (Color)"])]),
        new("Best Makeup And Hairstyling", ["MAKEUP AND HAIRSTYLING"], ["MAKEUP AND HAIRSTYLING"]),
        new("Best Costume Design", ["COSTUME DESIGN"], ["COSTUME DESIGN"],
            [new("Black-and-White", "Best Costume Design (Black-and-White)", ["COSTUME DESIGN (Black-and-White)"], ["COSTUME DESIGN (Black-and-White)"]),
             new("Color", "Best Costume Design (Color)", ["COSTUME DESIGN (Color)"], ["COSTUME DESIGN (Color)"])]),
        new("Best Casting", ["CASTING"], ["CASTING"]),  // New at the 98th ceremony (2025), so only in years from then on
    ];

    public static readonly FrozenDictionary<string, OscarCategoryMappingViewModel> ByCanonicalCategory =
        Displayed.SelectMany(display => display.CanonicalCategories.Select(canonical => (canonical, mapping: new OscarCategoryMappingViewModel(display, null)))
                                               .Concat(display.Subsections.SelectMany(subsection => subsection.CanonicalCategories.Select(canonical => (canonical, mapping: new OscarCategoryMappingViewModel(display, subsection))))))
                 .ToFrozenDictionary(pair => pair.canonical, pair => pair.mapping);

    // Where a dataset row is shown, or null when its category isn't one of the displayed ones
    public static OscarCategoryMappingViewModel? MappingOf(OscarsRowDataModel row)
    {
        return ByCanonicalCategory.TryGetValue(row.CanonicalCategory, out OscarCategoryMappingViewModel? mapping) ? mapping : null;
    }

    // Awards outside the competitive categories, shown after them: the dataset's "Special" class (Honorary Award,
    // Special Award, the Thalberg and Hersholt awards, Special Achievement Awards, …) plus the first ceremony's second
    // best picture award.
    private const string SpecialClass = "Special";
    private static readonly FrozenSet<string> SpecialCanonicalCategories = new[] { "UNIQUE AND ARTISTIC PICTURE" }.ToFrozenSet();

    // Never shown: the Scientific and Technical awards, and short films of any kind
    private const string SciTechClass = "SciTech";

    public static bool IsExcluded(OscarsRowDataModel row)
    {
        return row.Class == SciTechClass
            || row.CanonicalCategory.Contains("SHORT", StringComparison.OrdinalIgnoreCase)
            || row.Category.Contains("SHORT", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsSpecialAward(OscarsRowDataModel row)
    {
        return row.Class == SpecialClass || SpecialCanonicalCategories.Contains(row.CanonicalCategory);
    }

    private static readonly FrozenSet<string> SmallWords =
        new[] { "a", "an", "and", "as", "at", "by", "for", "in", "of", "on", "or", "the", "to" }.ToFrozenSet();

    // The dataset's category names are upper case, with any qualifier in parentheses in mixed case:
    // "ACTOR IN A LEADING ROLE" -> "Actor in a Leading Role", "ART DIRECTION (Black-and-White)" -> "Art Direction (Black-and-White)"
    public static string ToTitleCase(string category)
    {
        string[] words = category.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i];
            string letters = new string(word.Where(char.IsLetter).ToArray());
            if (letters.Length == 0 || letters != letters.ToUpperInvariant())
            {
                continue;  // Not an all upper case word, so already as it should be
            }
            if (letters.Length == 1 && word.EndsWith('.'))
            {
                continue;  // An initial, e.g. the "G." of "IRVING G. THALBERG"
            }
            string lower = word.ToLowerInvariant();
            words[i] = i > 0 && SmallWords.Contains(lower) ? lower : CapitalizeFirstLetter(lower);
        }
        return string.Join(' ', words);
    }

    private static string CapitalizeFirstLetter(string word)
    {
        int index = Array.FindIndex(word.ToCharArray(), char.IsLetter);
        return index < 0 ? word : word[..index] + char.ToUpperInvariant(word[index]) + word[(index + 1)..];
    }
}
