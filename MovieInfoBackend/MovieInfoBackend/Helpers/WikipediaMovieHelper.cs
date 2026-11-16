using System.Text.RegularExpressions;

namespace MovieInfoBackend.Helpers;

// Reads a movie's plot and its Rotten Tomatoes critics consensus out of the plain text of its English Wikipedia page
// (see WikipediaHttpClient.GetPageText). In that text a section heading is written as in wikitext ("== Plot =="), each
// paragraph is one line, and there are no references, links or other markup.
//
// Both were checked in November 2026 against the pages of some 2,000 films: popular and obscure, acclaimed and panned,
// American and foreign, from the 1920s to the 2020s. About half of them quote a consensus, and 1,700 have a plot.
public static partial class WikipediaMovieHelper
{
    // Only ever English Wikipedia: the plot and the consensus are shown in English
    private const string EnglishWikipediaPagePrefix = "https://en.wikipedia.org/wiki/";

    // The title of the page an English Wikipedia link (as from WikipediaHttpClient.GetMovieLink) goes to, e.g.
    // "Heat (1995 film)" for "https://en.wikipedia.org/wiki/Heat_(1995_film)". Null for any other link.
    public static string? GetPageTitle(string wikipediaLink)
    {
        if (!wikipediaLink.StartsWith(EnglishWikipediaPagePrefix, StringComparison.Ordinal))
        {
            return null;
        }

        string path = wikipediaLink[EnglishWikipediaPagePrefix.Length..];
        int end = path.IndexOfAny(['#', '?']);
        if (end >= 0)
        {
            path = path[..end];
        }
        string title = Uri.UnescapeDataString(path).Replace('_', ' ').Trim();

        return title == "" ? null : title;
    }

    // The page's plot section, its paragraphs separated by newlines.
    // "" when the page has no such section (or there is no page), null when there is no page text because the lookup failed.
    public static string? GetPlot(string? pageText)
    {
        if (pageText == null)
        {
            return null;
        }

        string[] lines = pageText.Split('\n');

        // In order of preference, should a page have more than one
        foreach (string heading in PlotHeadings)
        {
            for (int headingLine = 0; headingLine < lines.Length; headingLine++)
            {
                if (!string.Equals(GetTopLevelHeadingTitle(lines[headingLine]), heading, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // The section runs to the next top-level heading, or to the end of the page
                List<string> paragraphs = [];
                for (int line = headingLine + 1; line < lines.Length && GetTopLevelHeadingTitle(lines[line]) == null; line++)
                {
                    // A subsection's heading is kept, as a line of its own without the markup
                    string paragraph = GetSubheadingTitle(lines[line]) ?? lines[line].Trim();
                    if (paragraph != "")
                    {
                        paragraphs.Add(paragraph);
                    }
                }

                if (paragraphs.Count > 0)
                {
                    return string.Join("\n", paragraphs);
                }
            }
        }

        return "";
    }

    // Rotten Tomatoes' critics consensus, as the page quotes it (without the quotation marks).
    // "" when the page quotes none (or there is no page), null when there is no page text because the lookup failed.
    public static string? GetRtCriticsConsensus(string? pageText)
    {
        if (pageText == null)
        {
            return null;
        }

        // The consensus is quoted in the paragraph that gives the Rotten Tomatoes score, after the site is named
        foreach (string paragraph in pageText.Split('\n'))
        {
            int rottenTomatoesIndex = paragraph.IndexOf("Rotten Tomatoes", StringComparison.Ordinal);
            if (rottenTomatoesIndex < 0)
            {
                continue;
            }

            for (Match consensusMatch = RtCriticsConsensusRegex().Match(paragraph, rottenTomatoesIndex); consensusMatch.Success; consensusMatch = consensusMatch.NextMatch())
            {
                string consensus = consensusMatch.Groups["text"].Value.Trim();
                if (consensus != "")
                {
                    return consensus;
                }
            }
        }

        return "";
    }

    // What film articles call their plot section. "Plot" is the standard; the others are in order of how common they are.
    // The last two are the least common, and mainly found on the pages of documentaries, which say what the film
    // shows rather than tell a story. (Of some 2,000 film pages checked, 9 without any of the other sections had one.)
    private static readonly string[] PlotHeadings = ["Plot", "Synopsis", "Premise", "Plot summary", "Summary", "Story", "Plot synopsis", "Storyline",
                                                     "Content", "Film description"];

    // The title of a top-level section heading, which is a line of its own: "Plot" for "== Plot ==".
    // Null when the line is not one (which goes for the heading of a section within a section: "=== Act one ===").
    private static string? GetTopLevelHeadingTitle(string line)
    {
        line = line.Trim();
        if (line.Length < 5 || !line.StartsWith("==", StringComparison.Ordinal) || !line.EndsWith("==", StringComparison.Ordinal))
        {
            return null;
        }

        string title = line[2..^2].Trim();
        // More "=" at either end make it a heading further down: a subsection's
        if (title == "" || title.StartsWith('=') || title.EndsWith('='))
        {
            return null;
        }

        return title;
    }

    // The title of the heading of a section within a section (at any depth), which is a line of its own:
    // "Act one" for "=== Act one ===". Null when the line is not one.
    private static string? GetSubheadingTitle(string line)
    {
        line = line.Trim();
        if (line.Length < 6 || !line.StartsWith("===", StringComparison.Ordinal) || !line.EndsWith("===", StringComparison.Ordinal))
        {
            return null;
        }

        return line.Trim('=').Trim();
    }

    // The quoted consensus in a paragraph about the film's Rotten Tomatoes score, e.g.
    //   The website's critics consensus reads, "An engrossing crime drama."
    //   The site's "critics consensus" states: “An engrossing crime drama”.
    // - "consensus", which may itself be in quotation marks, then at most a few words of the same sentence ("reads", "states
    //   that the film is") up to the opening quotation mark.
    // - The text runs to the next quotation mark, or to the end of the paragraph when the page forgets to close the
    //   quotation. A consensus may quote something itself (a "so-called" comedy) without being cut short there: a
    //   quotation mark that follows a space (or a bracket or a dash) and is followed by a word opens a quotation within
    //   the consensus, which runs to its own closing mark. The same goes for a quotation that starts the consensus.
    [GeneratedRegex(@"consensus\b[""”']?[^\n""“”.]{0,80}?[""“”](?<text>(?:[""“”][^\n""“”]+[""”])?(?:[^\n""“”]|(?<=[\s(\[—–\-])[""“](?=\S)[^\n""“”]+[""”])*)(?:[""”]|$)", RegexOptions.IgnoreCase)]
    private static partial Regex RtCriticsConsensusRegex();
}
