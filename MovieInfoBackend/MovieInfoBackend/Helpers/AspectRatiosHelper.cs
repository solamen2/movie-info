using System.Collections.Frozen;
using System.Globalization;
using System.Text.RegularExpressions;
using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;

namespace MovieInfoBackend.Helpers;

// Turns the aspect ratios Wikidata lists for a movie or TV series into the ones shown, with their common names and a short
// description of each. Wikidata only says which ratios a title has, so the names and descriptions are kept here.
public static partial class AspectRatiosHelper
{
    // WikipediaLink is the page about the ratio or the format it is named for; null when Wikipedia has none of its own
    private record KnownAspectRatio(string Ratio, string? Name, string Description, string? WikipediaLink);

    // The names, dates and uses were checked against Wikipedia's "Aspect ratio (image)" article (its list of previous and
    // currently used aspect ratios) and its articles on the individual formats, in November 2026. The links were checked
    // to exist then too; a ratio only links to a page (or section) of its own, not to that general article.
    //
    // By width divided by height, to two decimal places. The ratio is how it is usually written, which is not always how
    // Wikidata labels it (e.g. "2.2:1" for 2.20:1).
    private static readonly FrozenDictionary<string, KnownAspectRatio> KnownAspectRatios = new Dictionary<string, KnownAspectRatio>
    {
        ["0.56"] = new("9:16", "Vertical",
            "Widescreen HD turned on its side: the shape of a phone held upright, used for video made to be watched on phones.",
            "https://en.wikipedia.org/wiki/Vertical_video"),
        ["0.75"] = new("3:4", null,
            "The old 4:3 television shape turned on its side, used for some vertical video.",
            null),
        ["1.19"] = new("1.19:1", "Movietone",
            "The nearly square frame of early sound films (about 1926 to 1932), left over when an optical soundtrack was added beside the silent-era picture. Seen in Sunrise (1927) and M (1931), and revived for The Lighthouse (2019).",
            "https://en.wikipedia.org/wiki/Movietone_sound_system"),
        ["1.25"] = new("5:4", null,
            "Slightly squarer than a classic TV picture: the shape of early 405-line television broadcasts, and later of many computer monitors.",
            null),
        ["1.33"] = new("4:3", "Fullscreen",
            "The shape of silent films and of television before HD. A modern widescreen TV shows it with black bars on the left and right.",
            "https://en.wikipedia.org/wiki/Fullscreen_(aspect_ratio)"),
        ["1.37"] = new("1.37:1", "Academy ratio",
            "The Hollywood standard set by the Academy in 1932, used for nearly all films until widescreen took over in 1953. Almost identical to 4:3.",
            "https://en.wikipedia.org/wiki/Academy_ratio"),
        // 1.375:1, the exact Academy ratio, rounds up
        ["1.38"] = new("1.37:1", "Academy ratio",
            "The Hollywood standard set by the Academy in 1932, used for nearly all films until widescreen took over in 1953. Almost identical to 4:3.",
            "https://en.wikipedia.org/wiki/Academy_ratio"),
        ["1.43"] = new("1.43:1", "IMAX",
            "The tall frame of IMAX 70 mm film, which only the largest IMAX theaters can show at its full height.",
            "https://en.wikipedia.org/wiki/IMAX"),
        ["1.50"] = new("3:2", null,
            "The shape of a 35 mm still photograph and of the VistaVision film frame. Uncommon for a finished film or show.",
            null),
        ["1.56"] = new("14:9", null,
            "A compromise between 4:3 and 16:9, devised after BBC audience tests, that broadcasters used to show a picture acceptably on both old and widescreen TVs.",
            "https://en.wikipedia.org/wiki/14:9_aspect_ratio"),
        ["1.60"] = new("16:10", null,
            "The shape of many widescreen computer monitors and laptops of the 2000s, before they moved to 16:9. Rare for a film or show.",
            "https://en.wikipedia.org/wiki/16:10_aspect_ratio"),
        ["1.66"] = new("1.66:1", "European widescreen",
            "An early widescreen shape introduced by Paramount, which became the standard for theatrical films in several European countries. Also the native shape of Super 16 film.",
            null),
        ["1.67"] = new("5:3", "European widescreen",
            "Usually written 1.66:1: an early widescreen shape introduced by Paramount, which became the standard for theatrical films in several European countries. Also the native shape of Super 16 film.",
            null),
        ["1.75"] = new("1.75:1", null,
            "An early widescreen shape used by MGM and Warner Bros. from 1953 to 1955, and for the widescreen versions of some Disney films, before 1.85:1 became the norm.",
            null),
        ["1.78"] = new("16:9", "Widescreen",
            "The shape of HDTVs, and of nearly all television made since the 2000s. It fills a modern TV screen exactly.",
            "https://en.wikipedia.org/wiki/16:9_aspect_ratio"),
        ["1.85"] = new("1.85:1", "Flat",
            "The standard non-anamorphic (\"flat\") widescreen shape for theatrical films in the US and UK, introduced by Universal in 1953. Only slightly wider than a TV screen.",
            "https://en.wikipedia.org/wiki/Aspect_ratio_(image)#1.85:1"),
        ["1.89"] = new("17:9", null,
            "Close to the full frame of a digital cinema projector (256:135, for example 4096 × 2160), a little wider than a TV screen.",
            "https://en.wikipedia.org/wiki/Digital_Cinema_Initiatives"),
        ["1.90"] = new("1.90:1", "IMAX Digital",
            "The shape of digital IMAX screens, introduced in 2008, and of the full frame of a digital cinema projector. A little taller than standard theatrical widescreen, and often used only for selected scenes of a film.",
            "https://en.wikipedia.org/wiki/IMAX"),
        ["2.00"] = new("2:1", "Univisium",
            "First used for RKO's Superscope in the 1950s, then proposed by cinematographer Vittorio Storaro in 1998 as a single shape for both cinema and television. Now common in streaming series.",
            "https://en.wikipedia.org/wiki/Univisium"),
        ["2.20"] = new("2.20:1", "70 mm",
            "The shape of standard 70 mm film prints, introduced with Todd-AO in 1955 and used for many roadshow epics.",
            "https://en.wikipedia.org/wiki/70_mm_film"),
        ["2.33"] = new("21:9", "Ultrawide",
            "The marketing name for ultra-wide TVs and monitors (really about 2.37:1), close to the shape of scope films.",
            "https://en.wikipedia.org/wiki/21:9_aspect_ratio"),
        ["2.35"] = new("2.35:1", "CinemaScope",
            "The anamorphic widescreen shape of CinemaScope and early Panavision films, standard until 1970. Often still used as a label for later films that are really 2.39:1.",
            "https://en.wikipedia.org/wiki/CinemaScope"),
        ["2.37"] = new("2.37:1", "Ultrawide",
            "The true shape of \"21:9\" ultra-wide TVs and monitors (64:27), close to the shape of scope films.",
            "https://en.wikipedia.org/wiki/21:9_aspect_ratio"),
        ["2.39"] = new("2.39:1", "Scope",
            "The modern anamorphic widescreen standard since 1970, often rounded to 2.40:1. A TV shows it with black bars above and below.",
            "https://en.wikipedia.org/wiki/Anamorphic_format"),
        ["2.40"] = new("2.40:1", "Scope",
            "A rounded way of writing 2.39:1, the modern anamorphic widescreen standard, and the shape widescreen films are given on Blu-ray (1920 × 800). A TV shows it with black bars above and below.",
            "https://en.wikipedia.org/wiki/Anamorphic_format"),
        ["2.55"] = new("2.55:1", "Early CinemaScope",
            "The CinemaScope frame of 1954 to 1956 (and of CinemaScope 55), before an optical soundtrack on the film narrowed it to 2.35:1.",
            "https://en.wikipedia.org/wiki/CinemaScope"),
        ["2.59"] = new("2.59:1", "Cinerama",
            "The 1950s process that used three 35 mm projectors side by side to fill a huge, deeply curved screen.",
            "https://en.wikipedia.org/wiki/Cinerama"),
        ["2.66"] = new("2.66:1", "Original CinemaScope",
            "The full width of the first CinemaScope films of 1953, before soundtracks were added to the film itself in 1954.",
            "https://en.wikipedia.org/wiki/CinemaScope"),
        ["2.76"] = new("2.76:1", "Ultra Panavision 70",
            "One of the widest formats ever in regular use: anamorphic 70 mm (first called MGM Camera 65), used for about ten films from 1957 to 1966, such as Ben-Hur (1959), and revived for The Hateful Eight (2015).",
            "https://en.wikipedia.org/wiki/Ultra_Panavision_70"),
        ["4.00"] = new("4:1", "Polyvision",
            "Three ordinary 35 mm pictures projected side by side, first used for Abel Gance's Napoléon (1927).",
            "https://en.wikipedia.org/wiki/Polyvision"),
    }.ToFrozenDictionary();

    // Wikidata items used as aspect ratios whose labels aren't written as a ratio (by item ID)
    private static readonly FrozenDictionary<string, (double Width, double Height)> RatiosByWikidataId = new Dictionary<string, (double, double)>
    {
        ["Q1541540"] = (1.37, 1),  // "Academy ratio"
    }.ToFrozenDictionary();

    // Every aspect ratio with a name or description here, narrowest first, for the reference page. A ratio listed under
    // two decimals (1.37:1 is also filed under 1.38, for the exact 1.375:1) appears once.
    public static List<AspectRatioViewModel> All()
    {
        return KnownAspectRatios.OrderBy(known => double.Parse(known.Key, CultureInfo.InvariantCulture))
                                .Select(known => new AspectRatioViewModel(known.Value.Ratio, known.Key, known.Value.Name, known.Value.Description, known.Value.WikipediaLink))
                                .DistinctBy(aspectRatio => aspectRatio.Ratio)
                                .ToList();
    }

    // The aspect ratios to show, narrowest first: empty when Wikidata lists none that can be read as a ratio (a few titles
    // are only tagged with something like "widescreen" or "high-definition television"), and null when the lookup failed.
    public static List<AspectRatioViewModel>? FromWikidata(WikidataSparqlResponseDataModel? wikidataResponse)
    {
        if (wikidataResponse == null)
        {
            return null;
        }

        return wikidataResponse.Results.Bindings
                               .Select(ParseRatio)
                               .OfType<(double Width, double Height, string Written)>()
                               .Select(ratio => (Value: ratio.Width / ratio.Height, ratio.Written))
                               .OrderBy(ratio => ratio.Value)
                               .Select(ratio => FromRatio(ratio.Value, ratio.Written))
                               .DistinctBy(aspectRatio => aspectRatio.Ratio)
                               .ToList();
    }

    private static (double Width, double Height, string Written)? ParseRatio(WikidataAspectRatioBindingDataModel binding)
    {
        string wikidataId = binding.AspectRatio.Value[(binding.AspectRatio.Value.LastIndexOf('/') + 1)..];
        if (RatiosByWikidataId.TryGetValue(wikidataId, out (double Width, double Height) knownRatio))
        {
            return (knownRatio.Width, knownRatio.Height, $"{FormatNumber(knownRatio.Width)}:{FormatNumber(knownRatio.Height)}");
        }

        Match match = RatioRegex().Match(binding.AspectRatioLabel?.Value ?? "");
        if (!match.Success)
        {
            return null;
        }
        string writtenWidth = match.Groups[1].Value.Replace(',', '.');
        string writtenHeight = match.Groups[2].Value.Replace(',', '.');
        double width = double.Parse(writtenWidth, CultureInfo.InvariantCulture);
        double height = double.Parse(writtenHeight, CultureInfo.InvariantCulture);
        if (width <= 0 || height <= 0)
        {
            return null;
        }
        return (width, height, $"{writtenWidth}:{writtenHeight}");
    }

    private static AspectRatioViewModel FromRatio(double value, string written)
    {
        string decimalRatio = value.ToString("0.00", CultureInfo.InvariantCulture);
        if (KnownAspectRatios.TryGetValue(decimalRatio, out KnownAspectRatio? known))
        {
            return new AspectRatioViewModel(known.Ratio, decimalRatio, known.Name, known.Description, known.WikipediaLink);
        }
        return new AspectRatioViewModel(written, decimalRatio, null,
            $"An uncommon aspect ratio with no widely used name: the picture is {decimalRatio} times as wide as it is tall.", null);
    }

    private static string FormatNumber(double number)
    {
        return number.ToString("0.##", CultureInfo.InvariantCulture);
    }

    // "16:9", "2.35:1", "2,35 : 1"
    [GeneratedRegex(@"^\s*(\d+(?:[.,]\d+)?)\s*:\s*(\d+(?:[.,]\d+)?)\s*$")]
    private static partial Regex RatioRegex();
}
