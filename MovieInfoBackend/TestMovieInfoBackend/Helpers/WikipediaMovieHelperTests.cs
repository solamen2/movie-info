using MovieInfoBackend.DataModels;
using MovieInfoBackend.Helpers;

namespace TestMovieInfoBackend.Helpers;

public class WikipediaMovieHelperTests
{
    // The text of a real page (Heat, 1995), as the Wikipedia API returned it
    private static string ReadMoviePageText()
    {
        using StreamReader sr = File.OpenText("../../../TestData/WikipediaHttpClientMovieExtractResponse.json");
        WikipediaExtractResponseDataModel? model = WikipediaHttpClient.GetExtractModelFromResponse(sr.ReadToEnd());
        string? pageText = model?.Query?.Pages?[0].Extract;
        Assert.NotNull(pageText);
        return pageText;
    }

    // A page whose Reception section is the given paragraph
    private static string PageWithReception(string receptionParagraph)
    {
        return $"Example Movie is a 2016 American drama film.\n\n\n== Plot ==\nThings happen.\n\n\n== Reception ==\n{receptionParagraph}\nA critic wrote, \"I liked it.\"\n\n\n== References ==";
    }

    // ---------- GetPageTitle ----------

    [Theory]
    [InlineData("https://en.wikipedia.org/wiki/Heat_(1995_film)", "Heat (1995 film)")]
    [InlineData("https://en.wikipedia.org/wiki/The_Shawshank_Redemption", "The Shawshank Redemption")]
    [InlineData("https://en.wikipedia.org/wiki/Am%C3%A9lie", "Amélie")]
    [InlineData("https://en.wikipedia.org/wiki/Amélie", "Amélie")]
    [InlineData("https://en.wikipedia.org/wiki/What%27s_Up,_Doc%3F_(1972_film)", "What's Up, Doc? (1972 film)")]
    [InlineData("https://en.wikipedia.org/wiki/Face/Off", "Face/Off")]
    [InlineData("https://en.wikipedia.org/wiki/Heat_(1995_film)#Plot", "Heat (1995 film)")]
    public void GetPageTitle_EnglishWikipediaLink_ReturnsThePageTitle(string wikipediaLink, string expectedTitle)
    {
        Assert.Equal(expectedTitle, WikipediaMovieHelper.GetPageTitle(wikipediaLink));
    }

    [Theory]
    [InlineData("")]
    [InlineData("Heat (1995 film)")]
    [InlineData("https://en.wikipedia.org/wiki/")]
    [InlineData("https://en.wikipedia.org/w/index.php?title=Heat_(1995_film)")]
    // Only English Wikipedia will do
    [InlineData("https://fr.wikipedia.org/wiki/Heat_(film,_1995)")]
    [InlineData("https://en.m.wikipedia.org/wiki/Heat_(1995_film)")]
    [InlineData("http://en.wikipedia.org/wiki/Heat_(1995_film)")]
    [InlineData("https://en.wikipedia.org.example.com/wiki/Heat_(1995_film)")]
    [InlineData("https://www.wikidata.org/wiki/Q42198")]
    public void GetPageTitle_AnyOtherLink_ReturnsNull(string link)
    {
        Assert.Null(WikipediaMovieHelper.GetPageTitle(link));
    }

    // ---------- GetPlot ----------

    [Fact]
    public void GetPlot_FailedLookup_ReturnsNull()
    {
        Assert.Null(WikipediaMovieHelper.GetPlot(null));
    }

    [Fact]
    public void GetPlot_NoPage_ReturnsEmptyString()
    {
        Assert.Equal("", WikipediaMovieHelper.GetPlot(""));
    }

    [Fact]
    public void GetPlot_RealPage_ReturnsThePlotSectionOnly()
    {
        string? plot = WikipediaMovieHelper.GetPlot(ReadMoviePageText());

        Assert.NotNull(plot);
        Assert.StartsWith("Neil McCauley, a Los Angeles professional thief, and his crew", plot);
        Assert.EndsWith("takes McCauley's hand as he dies of his wounds.", plot);
        // One paragraph per line, and nothing of the sections around it
        string[] paragraphs = plot.Split('\n');
        Assert.Equal(9, paragraphs.Length);
        Assert.All(paragraphs, paragraph => Assert.Equal(paragraph.Trim(), paragraph));
        Assert.All(paragraphs, paragraph => Assert.NotEqual("", paragraph));
        Assert.DoesNotContain("==", plot);
        Assert.DoesNotContain("Al Pacino as", plot);  // (The Cast section follows the plot)
    }

    [Theory]
    [InlineData("Plot")]
    [InlineData("Synopsis")]
    [InlineData("Premise")]
    [InlineData("Plot summary")]
    [InlineData("Summary")]
    [InlineData("Story")]
    [InlineData("Plot synopsis")]
    [InlineData("Storyline")]
    // Less common, and mainly on the pages of documentaries
    [InlineData("Content")]
    [InlineData("Film description")]
    [InlineData("PLOT")]
    [InlineData("plot Summary")]
    public void GetPlot_PlotSectionUnderAnyOfItsNames_ReturnsIt(string heading)
    {
        string pageText = $"Example Movie is a 2016 film.\n\n\n== {heading} ==\nThings happen.\nMore things happen.\n\n\n== Cast ==\nExample Jones as Lead";

        Assert.Equal("Things happen.\nMore things happen.", WikipediaMovieHelper.GetPlot(pageText));
    }

    [Theory]
    [InlineData("==Plot==")]
    [InlineData("==  Plot  ==")]
    [InlineData("== Plot == ")]
    [InlineData("== Plot ==\r")]
    public void GetPlot_HeadingSpacedDifferently_ReturnsThePlot(string headingLine)
    {
        string pageText = $"Example Movie is a 2016 film.\n\n\n{headingLine}\nThings happen.\n\n\n== Cast ==\nExample Jones as Lead";

        Assert.Equal("Things happen.", WikipediaMovieHelper.GetPlot(pageText));
    }

    [Fact]
    public void GetPlot_PlotIsTheLastSection_ReturnsItToTheEndOfThePage()
    {
        Assert.Equal("Things happen.\nThe end.", WikipediaMovieHelper.GetPlot("Example Movie is a 2016 film.\n\n\n== Plot ==\nThings happen.\nThe end.\n"));
    }

    [Fact]
    public void GetPlot_PlotWithSubsections_KeepsTheirHeadingsAsLinesOfTheirOwn()
    {
        string pageText = "Example Movie is a 2016 film.\n\n\n== Plot ==\nThe film is told in two parts.\n\n\n=== Part One: The Departure ===\nThings happen.\n\n\n==== Epilogue ====\nThe end.\n\n\n== Cast ==\nExample Jones as Lead";

        Assert.Equal("The film is told in two parts.\nPart One: The Departure\nThings happen.\nEpilogue\nThe end.", WikipediaMovieHelper.GetPlot(pageText));
    }

    [Theory]
    // No plot section at all
    [InlineData("Example Movie is a 2016 film.\n\n\n== Cast ==\nExample Jones as Lead\n\n\n== References ==")]
    [InlineData("Example Movie is a 2016 film.")]
    // An empty one (as when the article only has a "this section needs a plot summary" notice there)
    [InlineData("Example Movie is a 2016 film.\n\n\n== Plot ==\n\n\n== Cast ==\nExample Jones as Lead")]
    [InlineData("Example Movie is a 2016 film.\n\n\n== Plot ==\n")]
    // Sections that only sound like one, or that belong to something else
    [InlineData("Example Movie is a 2016 film.\n\n\n== Plot holes ==\nThere are many.\n\n\n== Production ==\nIt was made.")]
    [InlineData("Example Movie is a 2016 film.\n\n\n== Production ==\nIt was made.\n\n\n=== Plot ===\nThe writers changed it.")]
    [InlineData("Example Movie is a 2016 film. Critics said \"== Plot ==\" was missing.")]
    public void GetPlot_NoPlotSection_ReturnsEmptyString(string pageText)
    {
        Assert.Equal("", WikipediaMovieHelper.GetPlot(pageText));
    }

    [Fact]
    public void GetPlot_SeveralPlotLikeSections_PrefersThePlotSection()
    {
        string pageText = "Example Movie is a 2016 film.\n\n\n== Premise ==\nA short premise.\n\n\n== Plot ==\nThe whole plot.\n\n\n== Cast ==\nExample Jones as Lead";

        Assert.Equal("The whole plot.", WikipediaMovieHelper.GetPlot(pageText));
    }

    [Theory]
    [InlineData("Content", "Synopsis")]
    [InlineData("Film description", "Premise")]
    [InlineData("Content", "Storyline")]
    public void GetPlot_DocumentaryHeadingBesideAnotherPlotLikeSection_PrefersTheOther(string documentaryHeading, string otherHeading)
    {
        string pageText = $"Example Movie is a 2016 documentary film.\n\n\n== {documentaryHeading} ==\nWhat the film shows.\n\n\n== {otherHeading} ==\nThe story it tells.\n\n\n== Release ==\nIt was released.";

        Assert.Equal("The story it tells.", WikipediaMovieHelper.GetPlot(pageText));
    }

    [Theory]
    // Close to the documentary headings, but something else
    [InlineData("Contents")]
    [InlineData("Content rating")]
    [InlineData("Overview")]
    [InlineData("Description")]
    public void GetPlot_OtherDescriptiveSections_ReturnsEmptyString(string heading)
    {
        string pageText = $"Example Movie is a 2016 documentary film.\n\n\n== {heading} ==\nSomething about the film.\n\n\n== Release ==\nIt was released.";

        Assert.Equal("", WikipediaMovieHelper.GetPlot(pageText));
    }

    [Fact]
    public void GetPlot_EmptyPlotSection_FallsBackToAnotherPlotLikeSection()
    {
        string pageText = "Example Movie is a 2016 film.\n\n\n== Plot ==\n\n\n== Synopsis ==\nA synopsis.\n\n\n== Cast ==\nExample Jones as Lead";

        Assert.Equal("A synopsis.", WikipediaMovieHelper.GetPlot(pageText));
    }

    // ---------- GetRtCriticsConsensus ----------

    [Fact]
    public void GetRtCriticsConsensus_FailedLookup_ReturnsNull()
    {
        Assert.Null(WikipediaMovieHelper.GetRtCriticsConsensus(null));
    }

    [Fact]
    public void GetRtCriticsConsensus_NoPage_ReturnsEmptyString()
    {
        Assert.Equal("", WikipediaMovieHelper.GetRtCriticsConsensus(""));
    }

    [Fact]
    public void GetRtCriticsConsensus_RealPage_ReturnsTheQuotedConsensus()
    {
        Assert.Equal("Though Al Pacino and Robert De Niro share but a handful of screen minutes together, Heat is an engrossing crime drama that draws compelling performances from its stars – and confirms Michael Mann's mastery of the genre.",
                     WikipediaMovieHelper.GetRtCriticsConsensus(ReadMoviePageText()));
    }

    // Each consensus is a real one, in a paragraph worded the way that film's page words it (the film is named in the comment)
    [Theory]
    // Parasite
    [InlineData("On review aggregator Rotten Tomatoes, Parasite has an approval rating of 99% based on 485 reviews, with an average rating of 9.40/10. The website's critics consensus reads: \"An urgent, brilliantly layered look at timely social themes, Parasite finds writer-director Bong Joon Ho in near-total command of his craft.\" The site ranked it fifth on their \"300 Best Movies of All Time\" list in 2025. On Metacritic, the film has a weighted average score of 97 out of 100, indicating \"universal acclaim\".",
                "An urgent, brilliantly layered look at timely social themes, Parasite finds writer-director Bong Joon Ho in near-total command of his craft.")]
    // The Shawshank Redemption
    [InlineData("Contemporary review aggregation website Rotten Tomatoes gives the film an 89% approval rating from 146 critics, with an average rating of 8.3/10. The consensus reads, \"Steeped in old-fashioned storytelling and given evergreen humanity by Morgan Freeman and Tim Robbins, The Shawshank Redemption chronicles the hardship of incarceration patiently enough to come by its uplift honestly.\" The film also has a score of 82 out of 100 on Metacritic based on 22 critics indicating \"universal acclaim\".",
                "Steeped in old-fashioned storytelling and given evergreen humanity by Morgan Freeman and Tim Robbins, The Shawshank Redemption chronicles the hardship of incarceration patiently enough to come by its uplift honestly.")]
    // The Leopard: "critics consensus" is itself in quotation marks
    [InlineData("On Rotten Tomatoes, the film has an approval rating of 98% based on 48 reviews, with an average score of 9.1/10; the site's \"critics consensus\" reads: \"Lavish and wistful, The Leopard features epic battles, sumptuous costumes, and a ballroom waltz that competes for most beautiful sequence committed to film.\" On Metacritic, the 2004 re-release of the film holds a perfect 100 out of 100 score based on 12 reviews, indicating \"universal acclaim\".",
                "Lavish and wistful, The Leopard features epic battles, sumptuous costumes, and a ballroom waltz that competes for most beautiful sequence committed to film.")]
    // The Apartment: the full stop comes after the closing quotation mark
    [InlineData("On Rotten Tomatoes, the film holds an approval rating of 94% based on 109 reviews. The website's critical consensus reads, \"Director Billy Wilder's customary cynicism is leavened here by tender humor, romance, and genuine pathos\". On Metacritic, the film has a score of 94 out of 100, indicating \"universal acclaim\".",
                "Director Billy Wilder's customary cynicism is leavened here by tender humor, romance, and genuine pathos")]
    // Holiday Inn: a quotation inside the consensus, in single quotation marks
    [InlineData("On Rotten Tomatoes, the film has a 100% rating based on 23 reviews. The site's consensus states: \"With the combined might of Bing Crosby, Fred Astaire, and Irving Berlin working in its favor, Holiday Inn is a seasonal classic—not least because it introduced 'White Christmas' to the world.\"",
                "With the combined might of Bing Crosby, Fred Astaire, and Irving Berlin working in its favor, Holiday Inn is a seasonal classic—not least because it introduced 'White Christmas' to the world.")]
    // Gotti: a one-word consensus
    [InlineData("On Rotten Tomatoes, the film has an approval rating of 0% based on 59 reviews. The website's critical consensus simply reads, \"Fuhgeddaboudit\". On Metacritic, the film has a score of 24 out of 100, indicating \"generally unfavorable reviews\".",
                "Fuhgeddaboudit")]
    // You Don't Mess with the Zohan: only part of the consensus is quoted
    [InlineData("On Rotten Tomatoes, the film has a rating of 37%, based on 190 reviews, with the site's consensus stating that the film \"features intermittent laughs, and will please Sandler diehards, but after a while the leaky premise wears thin.\" On Metacritic, the film has a score of 54 out of 100, indicating \"mixed or average reviews\".",
                "features intermittent laughs, and will please Sandler diehards, but after a while the leaky premise wears thin.")]
    // The Reaping and Max: the consensus quotes something itself, ending in a question mark
    [InlineData("On Rotten Tomatoes, the film has an approval rating of 8% based on 135 reviews, with the consensus stating: \"It may feature such accomplished actors as Hilary Swank and Stephen Rea, but The Reaping also boasts the apropos tagline \"What hath God wrought?\". It's schlocky, spiritually shallow, and scare-free.\" Metacritic, which uses a weighted average, assigned the film a score of 36 out of 100, indicating \"generally unfavorable\" reviews.",
                "It may feature such accomplished actors as Hilary Swank and Stephen Rea, but The Reaping also boasts the apropos tagline \"What hath God wrought?\". It's schlocky, spiritually shallow, and scare-free.")]
    [InlineData("On Rotten Tomatoes, the film has an approval rating of 65%. The website's consensus reads, \"Well-acted in the execution of its provocative \"what-if?\" premise.\" Metacritic assigned the film a weighted average score of 56 out of 100, indicating \"mixed or average reviews\".",
                "Well-acted in the execution of its provocative \"what-if?\" premise.")]
    // Waiting for "Superman": so does this one, in the middle of a sentence
    [InlineData("On Rotten Tomatoes, the film has an approval rating of 90%. The site's consensus states: \"Gripping, heartbreaking, and ultimately hopeful, Waiting for \"Superman\" is an impassioned indictment of the American school system from An Inconvenient Truth director Davis Guggenheim.\" On Metacritic it has a score of 81%.",
                "Gripping, heartbreaking, and ultimately hopeful, Waiting for \"Superman\" is an impassioned indictment of the American school system from An Inconvenient Truth director Davis Guggenheim.")]
    // The Last Mimzy: no punctuation at all around the closing quotation mark
    [InlineData("On Rotten Tomatoes, the film has an approval rating of 55%. The site's critical consensus states, \"The Last Mimzy makes efforts to be a fun children's movie, but unsuccessfully juggles too many genres and subplots—eventually settling as an unfocused, slightly dull affair\" On Metacritic, the film had a score of 59 out of 100, indicating \"mixed or average reviews\". Audiences gave the film an average grade of \"B+\".",
                "The Last Mimzy makes efforts to be a fun children's movie, but unsuccessfully juggles too many genres and subplots—eventually settling as an unfocused, slightly dull affair")]
    // When We Were Kings: part of the consensus, quoted in the middle of a sentence
    [InlineData("It maintains a 98% positive rating at Rotten Tomatoes, with the website's critics consensus calling it \"an engrossing documentary that's as much about a time and a place as it is about a fight\" and received strong reviews from critics such as Roger Ebert, who called it \"a new documentary of a much-loved subject.\"",
                "an engrossing documentary that's as much about a time and a place as it is about a fight")]
    // Cell 211: the page never closes the quotation
    [InlineData("Cell 211 has an approval rating of 98% on review aggregator website Rotten Tomatoes, based on 45 reviews. The website's critical consensus states: \"A uniquely sharp, well-acted prison drama, Cell 211 avoids exploitative thrills in favor of expertly ratcheted tension.",
                "A uniquely sharp, well-acted prison drama, Cell 211 avoids exploitative thrills in favor of expertly ratcheted tension.")]
    // Make Me Famous: a doubled opening quotation mark, for the title the consensus starts with
    [InlineData("On the review aggregator website Rotten Tomatoes, 100% of 24 critics' reviews are positive. The website’s consensus reads, “”Make Me Famous” brings notoriety to its subject and his peers while delivering a gritty time capsule of New York City's art scene before it was gentrified.”",
                "”Make Me Famous” brings notoriety to its subject and his peers while delivering a gritty time capsule of New York City's art scene before it was gentrified.")]
    public void GetRtCriticsConsensus_ConsensusAsRealPagesWordIt_ReturnsIt(string receptionParagraph, string expectedConsensus)
    {
        Assert.Equal(expectedConsensus, WikipediaMovieHelper.GetRtCriticsConsensus(PageWithReception(receptionParagraph)));
    }

    [Theory]
    // Typographic quotation marks
    [InlineData("On Rotten Tomatoes, the film has a rating of 90%. The website’s consensus reads, “A fine film, and its star’s best.” Metacritic gave it “universal acclaim”.",
                "A fine film, and its star’s best.")]
    [InlineData("On Rotten Tomatoes, the film has a rating of 90%. The website’s consensus reads, “A fine film”. Metacritic gave it “universal acclaim”.",
                "A fine film")]
    // The consensus quotes something itself, in the same quotation marks
    [InlineData("On Rotten Tomatoes, the film has a rating of 20%. The website's consensus reads, \"This \"comedy\" has few laughs, and fewer ideas.\" Metacritic gave it \"generally unfavorable reviews\".",
                "This \"comedy\" has few laughs, and fewer ideas.")]
    // It ends in something other than a full stop
    [InlineData("On Rotten Tomatoes, the film has a rating of 20%. The website's consensus reads, \"Who asked for this?\" Metacritic gave it \"generally unfavorable reviews\".",
                "Who asked for this?")]
    [InlineData("On Rotten Tomatoes, the film has a rating of 20%. The website's consensus reads, \"Avoid!\" (as of 2020). Metacritic gave it \"generally unfavorable reviews\".",
                "Avoid!")]
    // The consensus ends the paragraph
    [InlineData("On Rotten Tomatoes, the film has a rating of 90%. The website's consensus reads, \"A fine film\"",
                "A fine film")]
    // Different words between "consensus" and the quotation
    [InlineData("On Rotten Tomatoes, the film has a rating of 90%, with the critical consensus: \"A fine film.\"", "A fine film.")]
    [InlineData("On Rotten Tomatoes, the film has a rating of 90%. The site's critics' consensus says that \"A fine film.\"", "A fine film.")]
    [InlineData("On Rotten Tomatoes, the film has a rating of 90%. Its Critics Consensus, as of 2020, is “A fine film.”", "A fine film.")]
    public void GetRtCriticsConsensus_OtherWaysOfQuotingIt_ReturnsIt(string receptionParagraph, string expectedConsensus)
    {
        Assert.Equal(expectedConsensus, WikipediaMovieHelper.GetRtCriticsConsensus(PageWithReception(receptionParagraph)));
    }

    [Theory]
    // The page gives a score, but no consensus
    [InlineData("On Rotten Tomatoes, 95% of 43 reviews are positive. At South by Southwest, the film won the Audience Award.")]
    // The Emoji Movie: the consensus is not text
    [InlineData("On the review aggregator website Rotten Tomatoes, The Emoji Movie has an approval rating of 6% based on 134 professional reviews, with an average rating of 2.9/10. The website's critics' consensus simply displays a no symbol emoji (🚫) in place of text. Metacritic, which uses a weighted average, assigned The Emoji Movie a score of 12 out of 100 based on 26 critics, indicating \"overwhelming dislike\", making it the lowest-rated animated film on the site.")]
    // The consensus is described, not quoted; what is quoted later is someone else's words
    [InlineData("On Rotten Tomatoes, the film has a rating of 20%. The critical consensus is that the film wastes its cast. Metacritic gave it \"generally unfavorable reviews\".")]
    // A consensus, but nothing to say it is Rotten Tomatoes'
    [InlineData("The critical consensus was summed up by one reviewer: \"A fine film.\"")]
    [InlineData("On Metacritic, the film has a score of 80 out of 100. The site's consensus reads, \"A fine film.\"")]
    // Rotten Tomatoes is only named after it
    [InlineData("The critical consensus was summed up by one reviewer: \"A fine film.\" On Rotten Tomatoes, the film has a rating of 90%.")]
    // A Christmas Tale: paraphrased, with someone else's words quoted in the next sentence
    [InlineData("On the review aggregator website Rotten Tomatoes, 86% of 131 critics' reviews are positive, with an average rating of 7.8 out of 10. The website's consensus highlights the film's dark humour and the strength of its ensemble cast. Metacritic, which uses a weighted average, assigned the film a score of 84 out of 100 based on 32 critics, indicating \"universal acclaim\".")]
    // Take Me to the River: New Orleans: "consensus" used for the score
    [InlineData("The review aggregator website Rotten Tomatoes has the critical consensus of the film at 90% out of 100, based on ten reviews. At Metacritic, which uses a weighted average, assigned the film a score of 69 out of 100, based on four critics, indicating \"generally favorable reviews\".")]
    // An empty quotation
    [InlineData("On Rotten Tomatoes, the film has a rating of 90%. The website's consensus reads, \"\".")]
    // No Reception to speak of
    [InlineData("The film was not reviewed.")]
    public void GetRtCriticsConsensus_NoQuotedRottenTomatoesConsensus_ReturnsEmptyString(string receptionParagraph)
    {
        Assert.Equal("", WikipediaMovieHelper.GetRtCriticsConsensus(PageWithReception(receptionParagraph)));
    }

    [Fact]
    public void GetRtCriticsConsensus_RottenTomatoesNamedInAnotherParagraph_ReturnsEmptyString()
    {
        string pageText = PageWithReception("On Rotten Tomatoes, the film has a rating of 90%.\nYears later, the critical consensus was summed up by one reviewer: \"A fine film.\"");

        Assert.Equal("", WikipediaMovieHelper.GetRtCriticsConsensus(pageText));
    }

    [Fact]
    public void GetRtCriticsConsensus_SeveralConsensuses_ReturnsTheFirst()
    {
        string pageText = PageWithReception("On Rotten Tomatoes, the film has a rating of 90%. The website's consensus reads, \"A fine film.\"\nThe director's cut fared worse. On Rotten Tomatoes, it has a rating of 40%. The website's consensus reads, \"A longer film.\"");

        Assert.Equal("A fine film.", WikipediaMovieHelper.GetRtCriticsConsensus(pageText));
    }

    [Fact]
    public void GetRtCriticsConsensus_RottenTomatoesFirstNamedWithoutAConsensus_FindsItInALaterParagraph()
    {
        string pageText = "Example Movie is a 2016 film. It holds a 90% rating on Rotten Tomatoes.\n\n\n== Reception ==\nOn Rotten Tomatoes, the film has a rating of 90%. The website's consensus reads, \"A fine film.\"";

        Assert.Equal("A fine film.", WikipediaMovieHelper.GetRtCriticsConsensus(pageText));
    }
}
