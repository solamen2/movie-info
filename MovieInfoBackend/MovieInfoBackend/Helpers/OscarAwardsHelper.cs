using MovieInfoBackend.ViewModels;

namespace MovieInfoBackend.Helpers;

// Finding a movie's or a person's awards among all of them, and which of the dataset's ids are shown
public static class OscarAwardsHelper
{
    private const string FilmImdbIdPrefix = "tt";
    private const string PersonImdbIdPrefix = "nm";

    // Only an IMDB id of the right kind (tt… for a film, nm… for a person): the dataset also has company ids (co…) for the
    // studios named in the earliest years, and a pair of ids for a pseudonym shared by two people
    public static string? FilmImdbIdOrNull(string? id)
    {
        return ImdbIdOrNull(id, FilmImdbIdPrefix);
    }

    public static string? PersonImdbIdOrNull(string? id)
    {
        return ImdbIdOrNull(id, PersonImdbIdPrefix);
    }

    private static string? ImdbIdOrNull(string? id, string prefix)
    {
        return id != null && id.Length > prefix.Length && id.StartsWith(prefix) && id[prefix.Length..].All(char.IsDigit) ? id : null;
    }

    // Every award in the by-year listing's order: each ceremony's categories, then its special awards
    public static IEnumerable<OscarAwardViewModel> All(OscarsViewModel oscars)
    {
        return oscars.Years.SelectMany(year => year.Categories.SelectMany(category => category.Subsections.SelectMany(subsection => subsection.Entries))
                                                   .Concat(year.SpecialAwards));
    }

    // A movie's awards, in by-year order. Only awards whose film has no IMDB id in the dataset are matched by title, so
    // that a film with the same title as another one can't pick up the other's awards.
    public static List<OscarAwardViewModel> ForMovie(OscarsViewModel oscars, string imdbId, string title)
    {
        string titleKey = NameKey(title);
        return All(oscars).Where(award => award.Films.Any(film => film.ImdbId != null ? film.ImdbId == imdbId : NameKey(film.Title) == titleKey))
                          .ToList();
    }

    // A person's awards, in by-year order; matched by name only when the dataset has no IMDB id for the nominee, as above
    public static List<OscarAwardViewModel> ForPerson(OscarsViewModel oscars, string imdbId, string name)
    {
        string nameKey = NameKey(name);
        return All(oscars).Where(award => award.Nominees.Any(nominee => nominee.ImdbId != null ? nominee.ImdbId == imdbId : NameKey(nominee.Name) == nameKey))
                          .ToList();
    }

    private static string NameKey(string name)
    {
        return name.Trim().ToLowerInvariant();
    }
}
