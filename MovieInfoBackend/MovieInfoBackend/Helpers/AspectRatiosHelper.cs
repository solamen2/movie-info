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
    // Decimal is the ratio's width divided by its height, to two decimal places. AlsoWritten are other ways of writing what
    // is in practice the same ratio, each with its own decimal: a rounder form (2.40:1 for 2.39:1), the exact value
    // (1.375:1 for the Academy ratio) or the true figure behind a looser name (the screens sold as 21:9 are 2.37:1).
    // A title labelled one of those ways is shown with that writing, under this ratio's name and description.
    // WikipediaLink is the page (or section) about the ratio or the format it is named for; null when it has none.
    private record KnownAspectRatio(string Ratio, string Decimal, string? Name, string Description, string? WikipediaLink, params (string Ratio, string Decimal)[] AlsoWritten);

    // The names, dates and uses were checked in November 2026 against Wikipedia (its "Aspect ratio (image)" article and
    // its articles on the individual formats) and, where those disagreed or were thin, against the American WideScreen
    // Museum (widescreenmuseum.com) and the Sprocket School projection wiki (sprocketschool.org). The links were checked
    // to exist then too; a ratio only links to a page or section of its own, not to the general article's long list.
    //
    // Narrowest first. The ratio is how it is usually written, which is not always how Wikidata labels it (e.g. "2.2:1"
    // for 2.20:1, or "2.4:1" for 2.39:1).
    private static readonly KnownAspectRatio[] KnownAspectRatios =
    [
        new("9:16", "0.56", "Vertical",
            "Widescreen HD turned on its side: the shape of a phone held upright, used for video made to be watched on phones.",
            "https://en.wikipedia.org/wiki/Vertical_video"),
        new("1:1", "1.00", "Square",
            "A square picture. Rare in film and television, but made popular for phone video by apps such as Instagram and Vine.",
            "https://en.wikipedia.org/wiki/Aspect_ratio_(image)#1:1"),
        new("1.19:1", "1.19", "Movietone",
            "The nearly square frame of early sound films (from the late 1920s until 1932), left over when an optical soundtrack was added beside the silent-era picture. Seen in M (1931) and revived for The Lighthouse (2019).",
            "https://en.wikipedia.org/wiki/Movietone_sound_system", ("1.20:1", "1.20")),
        new("5:4", "1.25", null,
            "Slightly squarer than a classic TV picture: the shape of early 405-line television broadcasts, and later of many computer monitors.",
            null),
        new("4:3", "1.33", "Fullscreen",
            "The shape of silent films and of television before HD. A modern widescreen TV shows it with black bars on the left and right.",
            "https://en.wikipedia.org/wiki/Fullscreen_(aspect_ratio)"),
        new("1.37:1", "1.37", "Academy ratio",
            "The Hollywood standard set by the Academy in 1932, used for nearly all films until widescreen took over in 1953. Almost identical to 4:3.",
            "https://en.wikipedia.org/wiki/Academy_ratio", ("1.375:1", "1.38")),
        new("1.43:1", "1.43", "IMAX",
            "The tall frame of IMAX 70 mm film. Only the largest IMAX theaters, with 70 mm film or dual laser projectors, can show it at its full height.",
            "https://en.wikipedia.org/wiki/IMAX"),
        new("3:2", "1.50", null,
            "The shape of a 35 mm still photograph and of the VistaVision film frame. Uncommon for a finished film or show.",
            null),
        new("14:9", "1.56", null,
            "A compromise between 4:3 and 16:9, devised after BBC audience tests, that broadcasters used to show a picture acceptably on both old and widescreen TVs.",
            "https://en.wikipedia.org/wiki/14:9_aspect_ratio"),
        new("16:10", "1.60", null,
            "The shape of many widescreen computer monitors and laptops of the 2000s, before they moved to 16:9. Rare for a film or show.",
            "https://en.wikipedia.org/wiki/16:10_aspect_ratio"),
        new("1.66:1", "1.66", "European widescreen",
            "The first of the 1953 widescreen shapes, introduced by Paramount with Shane, which became the standard for theatrical films in much of Europe. Also written 5:3, and the native shape of Super 16 film.",
            null, ("5:3", "1.67")),
        new("1.75:1", "1.75", null,
            "An early widescreen shape used by MGM, Warner Bros. and Disney in the mid-1950s, before 1.85:1 became the norm.",
            null),
        new("16:9", "1.78", "Widescreen",
            "The shape of HDTVs, and of nearly all television made since the 2000s. It fills a modern TV screen exactly.",
            "https://en.wikipedia.org/wiki/16:9_aspect_ratio", ("1.77:1", "1.77")),
        new("1.85:1", "1.85", "Flat",
            "The standard non-anamorphic (\"flat\") widescreen shape for theatrical films in the US and UK, introduced by Universal in 1953. Only slightly wider than a TV screen.",
            "https://en.wikipedia.org/wiki/Aspect_ratio_(image)#1.85:1"),
        new("17:9", "1.89", null,
            "Close to the full frame of a digital cinema projector (256:135, for example 4096 × 2160), a little wider than a TV screen.",
            null),
        new("1.90:1", "1.90", "IMAX Digital",
            "The shape of digital IMAX screens, introduced in 2008, and of the full frame of a digital cinema projector. A little taller than standard theatrical widescreen, and often used only for selected scenes of a film.",
            "https://en.wikipedia.org/wiki/IMAX#Digital_projection"),
        new("2:1", "2.00", "Univisium",
            "First used in the 1950s, notably for RKO's Superscope, then proposed by cinematographer Vittorio Storaro in 1998 as a single shape for both cinema and television. Now common in streaming series.",
            "https://en.wikipedia.org/wiki/Univisium"),
        new("2.20:1", "2.20", "70 mm",
            "The shape of standard 70 mm film prints, introduced with Todd-AO in 1955 and used for many roadshow epics.",
            "https://en.wikipedia.org/wiki/70_mm_film", ("2.21:1", "2.21")),
        new("21:9", "2.33", "Ultrawide",
            "The marketing name for ultra-wide TVs and monitors, whose true shape is 64:27 (about 2.37:1). Close to the shape of scope films.",
            "https://en.wikipedia.org/wiki/21:9_aspect_ratio", ("2.37:1", "2.37")),
        new("2.35:1", "2.35", "CinemaScope",
            "The anamorphic widescreen shape of CinemaScope and early Panavision films, from the mid-1950s until 1970. Often still used as a label for later films that are really 2.39:1.",
            "https://en.wikipedia.org/wiki/CinemaScope"),
        new("2.39:1", "2.39", "Scope",
            "The modern anamorphic widescreen standard since 1970. Often rounded to 2.40:1, which is also how widescreen films are framed on Blu-ray. A TV shows it with black bars above and below.",
            "https://en.wikipedia.org/wiki/Anamorphic_format", ("2.38:1", "2.38"), ("2.40:1", "2.40")),
        new("2.55:1", "2.55", "Early CinemaScope",
            "CinemaScope as first released, from The Robe (1953) until 1957, with magnetic stereo sound on the film; adding an optical soundtrack then narrowed it to 2.35:1. Also the shape of CinemaScope 55.",
            "https://en.wikipedia.org/wiki/CinemaScope"),
        new("2.59:1", "2.59", "Cinerama",
            "The 1950s process that used three 35 mm projectors side by side to fill a huge, deeply curved screen.",
            "https://en.wikipedia.org/wiki/Cinerama"),
        new("2.66:1", "2.66", null,
            "The width CinemaScope was designed for in 1953, using the whole silent-film frame with the sound on a separate film. Films were released narrower, at 2.55:1, once the sound was put on the film itself.",
            "https://en.wikipedia.org/wiki/CinemaScope"),
        new("2.76:1", "2.76", "Ultra Panavision 70",
            "One of the widest formats ever in regular use: anamorphic 70 mm (first called MGM Camera 65), used for about ten films from 1957 to 1966, such as Ben-Hur (1959), and revived for The Hateful Eight (2015).",
            "https://en.wikipedia.org/wiki/Ultra_Panavision_70"),
        new("4:1", "4.00", "Polyvision",
            "Three ordinary 35 mm pictures projected side by side, first used for Abel Gance's Napoléon (1927).",
            "https://en.wikipedia.org/wiki/Polyvision"),
    ];

    // Every known ratio by each decimal it goes by, with how it is written at that decimal (see KnownAspectRatio.AlsoWritten)
    private static readonly FrozenDictionary<string, (KnownAspectRatio Known, string Ratio)> KnownAspectRatiosByDecimal =
        KnownAspectRatios.SelectMany(known => known.AlsoWritten.Prepend((known.Ratio, known.Decimal)).Select(written => (written.Decimal, Known: known, written.Ratio)))
                         .ToFrozenDictionary(entry => entry.Decimal, entry => (entry.Known, entry.Ratio));

    // Wikidata items used as aspect ratios whose labels aren't written as a ratio (by item ID)
    private static readonly FrozenDictionary<string, (double Width, double Height)> RatiosByWikidataId = new Dictionary<string, (double, double)>
    {
        ["Q1541540"] = (1.37, 1),  // "Academy ratio"
    }.ToFrozenDictionary();

    // Every aspect ratio with a name or description here, narrowest first, for the reference page
    public static List<AspectRatioViewModel> All()
    {
        return KnownAspectRatios.Select(known => FromKnown(known, known.Ratio, known.Decimal)).ToList();
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
                               // One of each: a title given both 2.39:1 and 2.40:1, say, only needs the first
                               .DistinctBy(aspectRatio => (aspectRatio.Name ?? aspectRatio.Ratio, aspectRatio.Description))
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
        if (KnownAspectRatiosByDecimal.TryGetValue(decimalRatio, out (KnownAspectRatio Known, string Ratio) entry))
        {
            return FromKnown(entry.Known, entry.Ratio, decimalRatio);
        }
        return new AspectRatioViewModel(written, decimalRatio, null,
            $"An uncommon aspect ratio with no widely used name: the picture is {decimalRatio} times as wide as it is tall.", null);
    }

    private static AspectRatioViewModel FromKnown(KnownAspectRatio known, string ratio, string decimalRatio)
    {
        return new AspectRatioViewModel(ratio, decimalRatio, known.Name, known.Description, known.WikipediaLink);
    }

    private static string FormatNumber(double number)
    {
        return number.ToString("0.##", CultureInfo.InvariantCulture);
    }

    // "16:9", "2.35:1", "2,35 : 1"
    [GeneratedRegex(@"^\s*(\d+(?:[.,]\d+)?)\s*:\s*(\d+(?:[.,]\d+)?)\s*$")]
    private static partial Regex RatioRegex();
}
