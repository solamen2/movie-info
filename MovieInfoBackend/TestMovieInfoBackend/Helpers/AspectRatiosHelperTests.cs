using MovieInfoBackend.DataModels;
using MovieInfoBackend.Helpers;
using MovieInfoBackend.ViewModels;

namespace TestMovieInfoBackend.Helpers;

public class AspectRatiosHelperTests
{
    private static WikidataSparqlResponseDataModel GetResponse(params (string wikidataId, string? label)[] aspectRatios)
    {
        return new WikidataSparqlResponseDataModel
        {
            Results = new WikidataSparqlResultsDataModel
            {
                Bindings = aspectRatios.Select(aspectRatio => new WikidataAspectRatioBindingDataModel
                {
                    AspectRatio = new WikidataSparqlValueDataModel { Type = "uri", Value = $"http://www.wikidata.org/entity/{aspectRatio.wikidataId}" },
                    AspectRatioLabel = aspectRatio.label == null ? null : new WikidataSparqlValueDataModel { Type = "literal", Value = aspectRatio.label },
                }).ToArray(),
            },
        };
    }

    private static string ReadTestData(string filename)
    {
        using StreamReader sr = File.OpenText($"../../../TestData/{filename}");
        return sr.ReadToEnd();
    }

    [Fact]
    public void FromWikidata_FailedLookup_ReturnsNull()
    {
        Assert.Null(AspectRatiosHelper.FromWikidata(null));
    }

    [Fact]
    public void FromWikidata_NoBindings_ReturnsEmptyList()
    {
        List<AspectRatioViewModel>? aspectRatios = AspectRatiosHelper.FromWikidata(GetResponse());

        Assert.NotNull(aspectRatios);
        Assert.Empty(aspectRatios);
    }

    // Every label Wikidata uses for more than a couple of titles
    [Theory]
    [InlineData("4:3", "4:3", "1.33", "Fullscreen")]
    [InlineData("16:9", "16:9", "1.78", "Widescreen")]
    [InlineData("2.35:1", "2.35:1", "2.35", "CinemaScope")]
    [InlineData("1.85:1", "1.85:1", "1.85", "Flat")]
    [InlineData("2.39:1", "2.39:1", "2.39", "Scope")]
    [InlineData("1.66:1", "1.66:1", "1.66", "European widescreen")]
    [InlineData("2:1", "2:1", "2.00", "Univisium")]
    [InlineData("2.2:1", "2.20:1", "2.20", "70 mm")]
    [InlineData("9:16", "9:16", "0.56", "Vertical")]
    [InlineData("1.90:1", "1.90:1", "1.90", "IMAX Digital")]
    [InlineData("2.55:1", "2.55:1", "2.55", "Early CinemaScope")]
    [InlineData("7:4", "1.75:1", "1.75", null)]
    [InlineData("5:3", "5:3", "1.67", "European widescreen")]
    [InlineData("2.4:1", "2.40:1", "2.40", "Scope")]
    [InlineData("2.40:1", "2.40:1", "2.40", "Scope")]
    [InlineData("3:2", "3:2", "1.50", null)]
    [InlineData("19:16", "1.19:1", "1.19", "Movietone")]
    [InlineData("21:9", "21:9", "2.33", "Ultrawide")]
    [InlineData("2.37:1", "2.37:1", "2.37", "Ultrawide")]
    [InlineData("64:27", "2.37:1", "2.37", "Ultrawide")]
    [InlineData("2.76:1", "2.76:1", "2.76", "Ultra Panavision 70")]
    [InlineData("1.375:1", "1.375:1", "1.38", "Academy ratio")]
    [InlineData("1.77:1", "1.77:1", "1.77", "Widescreen")]
    [InlineData("2.21:1", "2.21:1", "2.21", "70 mm")]
    [InlineData("1:1", "1:1", "1.00", "Square")]
    [InlineData("16:10", "16:10", "1.60", null)]
    [InlineData("2.66:1", "2.66:1", "2.66", null)]
    [InlineData("4:1", "4:1", "4.00", "Polyvision")]
    public void FromWikidata_KnownRatio_GetsItsUsualWritingNameAndDescription(string label, string expectedRatio, string expectedDecimal, string? expectedName)
    {
        List<AspectRatioViewModel>? aspectRatios = AspectRatiosHelper.FromWikidata(GetResponse(("Q1", label)));

        Assert.NotNull(aspectRatios);
        AspectRatioViewModel aspectRatio = Assert.Single(aspectRatios);
        Assert.Equal(expectedRatio, aspectRatio.Ratio);
        Assert.Equal(expectedDecimal, aspectRatio.Decimal);
        Assert.Equal(expectedName, aspectRatio.Name);
        Assert.False(string.IsNullOrWhiteSpace(aspectRatio.Description));
        Assert.DoesNotContain("uncommon aspect ratio", aspectRatio.Description);
    }

    [Fact]
    public void FromWikidata_AcademyRatioItem_IsReadByItsWikidataId()
    {
        // Wikidata labels this one "Academy ratio" rather than "1.37:1"
        List<AspectRatioViewModel>? aspectRatios = AspectRatiosHelper.FromWikidata(GetResponse(("Q1541540", "Academy ratio")));

        Assert.NotNull(aspectRatios);
        AspectRatioViewModel aspectRatio = Assert.Single(aspectRatios);
        Assert.Equal("1.37:1", aspectRatio.Ratio);
        Assert.Equal("1.37", aspectRatio.Decimal);
        Assert.Equal("Academy ratio", aspectRatio.Name);
    }

    [Theory]
    [InlineData("5:1", "5:1", "5.00")]
    [InlineData("5:7", "5:7", "0.71")]
    [InlineData("3:4", "3:4", "0.75")]
    [InlineData("2,1 : 1", "2.1:1", "2.10")]
    public void FromWikidata_UnknownRatio_HasNoNameButStillADescription(string label, string expectedRatio, string expectedDecimal)
    {
        List<AspectRatioViewModel>? aspectRatios = AspectRatiosHelper.FromWikidata(GetResponse(("Q1", label)));

        Assert.NotNull(aspectRatios);
        AspectRatioViewModel aspectRatio = Assert.Single(aspectRatios);
        Assert.Equal(expectedRatio, aspectRatio.Ratio);
        Assert.Equal(expectedDecimal, aspectRatio.Decimal);
        Assert.Null(aspectRatio.Name);
        Assert.Null(aspectRatio.WikipediaLink);
        Assert.Equal($"An uncommon aspect ratio with no widely used name: the picture is {expectedDecimal} times as wide as it is tall.", aspectRatio.Description);
    }

    // Values some titles carry that aren't a ratio at all, and Wikidata's "unknown value"
    [Theory]
    [InlineData("Q752245", "widescreen")]
    [InlineData("Q133454", "high-definition television")]
    [InlineData("Q1788758", "VistaVision")]
    [InlineData("Q1441762", "aspect ratio")]
    [InlineData("c62f9457bfed350d066b676fcf16a992", "http://www.wikidata.org/.well-known/genid/c62f9457bfed350d066b676fcf16a992")]
    [InlineData("Q1", "0:1")]
    [InlineData("Q1", "1:0")]
    [InlineData("Q1", "")]
    [InlineData("Q1", null)]
    public void FromWikidata_NotARatio_IsLeftOut(string wikidataId, string? label)
    {
        List<AspectRatioViewModel>? aspectRatios = AspectRatiosHelper.FromWikidata(GetResponse((wikidataId, label), ("Q1383069", "16:9")));

        Assert.NotNull(aspectRatios);
        AspectRatioViewModel aspectRatio = Assert.Single(aspectRatios);
        Assert.Equal("16:9", aspectRatio.Ratio);
    }

    [Fact]
    public void FromWikidata_TheSameRatioWrittenTwoWays_IsShownOnce()
    {
        List<AspectRatioViewModel>? aspectRatios = AspectRatiosHelper.FromWikidata(
            GetResponse(("Q1", "2.39:1"), ("Q2", "2.4:1"), ("Q3", "1.66:1"), ("Q4", "5:3"), ("Q5", "21:9"), ("Q6", "2.37:1")));

        Assert.NotNull(aspectRatios);
        Assert.Equal(["1.66:1", "21:9", "2.39:1"], aspectRatios.Select(aspectRatio => aspectRatio.Ratio));
    }

    // A merged category keeps one name, description and link, but each title shows the number Wikidata gives it
    [Theory]
    [InlineData("1.66:1", "5:3", "1.66:1", "1.66", "5:3", "1.67")]
    [InlineData("2.39:1", "2.40:1", "2.39:1", "2.39", "2.40:1", "2.40")]
    [InlineData("21:9", "2.37:1", "21:9", "2.33", "2.37:1", "2.37")]
    public void FromWikidata_EitherWritingOfAMergedRatio_ShowsItsOwnNumbersUnderTheSharedName(
        string label1, string label2, string expectedRatio1, string expectedDecimal1, string expectedRatio2, string expectedDecimal2)
    {
        AspectRatioViewModel first = Assert.Single(AspectRatiosHelper.FromWikidata(GetResponse(("Q1", label1)))!);
        AspectRatioViewModel second = Assert.Single(AspectRatiosHelper.FromWikidata(GetResponse(("Q2", label2)))!);

        Assert.Equal((expectedRatio1, expectedDecimal1), (first.Ratio, first.Decimal));
        Assert.Equal((expectedRatio2, expectedDecimal2), (second.Ratio, second.Decimal));
        Assert.NotNull(first.Name);
        Assert.Equal(first.Name, second.Name);
        Assert.Equal(first.Description, second.Description);
        Assert.Equal(first.WikipediaLink, second.WikipediaLink);
    }

    [Fact]
    public void FromWikidata_SeveralRatios_AreNarrowestFirstWithoutDuplicates()
    {
        List<AspectRatioViewModel>? aspectRatios = AspectRatiosHelper.FromWikidata(
            GetResponse(("Q24668302", "2.39:1"), ("Q20970432", "4:3"), ("Q28858964", "2.2:1"), ("Q2", "1.33:1"), ("Q3", "2.20:1")));

        Assert.NotNull(aspectRatios);
        Assert.Equal(["4:3", "2.20:1", "2.39:1"], aspectRatios.Select(aspectRatio => aspectRatio.Ratio));
    }

    [Fact]
    public void All_ListsEveryKnownRatioOnce_NarrowestFirst()
    {
        List<AspectRatioViewModel> aspectRatios = AspectRatiosHelper.All();

        Assert.Equal(26, aspectRatios.Count);
        Assert.Equal(aspectRatios.Select(aspectRatio => aspectRatio.Ratio).Distinct(), aspectRatios.Select(aspectRatio => aspectRatio.Ratio));
        List<double> decimals = aspectRatios.Select(aspectRatio => double.Parse(aspectRatio.Decimal, System.Globalization.CultureInfo.InvariantCulture)).ToList();
        Assert.Equal(decimals.Order(), decimals);
        Assert.Equal("9:16", aspectRatios.First().Ratio);
        Assert.Equal("4:1", aspectRatios.Last().Ratio);
        // Ratios that are the same thing written differently are one entry each
        Assert.Equal(aspectRatios.Where(aspectRatio => aspectRatio.Name != null).Select(aspectRatio => aspectRatio.Name).Distinct(),
                     aspectRatios.Where(aspectRatio => aspectRatio.Name != null).Select(aspectRatio => aspectRatio.Name));
        Assert.Equal("1.37", Assert.Single(aspectRatios, aspectRatio => aspectRatio.Name == "Academy ratio").Decimal);
        Assert.Equal("1.66:1", Assert.Single(aspectRatios, aspectRatio => aspectRatio.Name == "European widescreen").Ratio);
        Assert.Equal("2.39:1", Assert.Single(aspectRatios, aspectRatio => aspectRatio.Name == "Scope").Ratio);
        Assert.Equal("21:9", Assert.Single(aspectRatios, aspectRatio => aspectRatio.Name == "Ultrawide").Ratio);
        Assert.DoesNotContain(aspectRatios, aspectRatio => aspectRatio.Ratio is "5:3" or "2.40:1" or "2.37:1");
        Assert.All(aspectRatios, aspectRatio => Assert.False(string.IsNullOrWhiteSpace(aspectRatio.Description)));
    }

    [Fact]
    public void All_WikipediaLinks_AreWikipediaPagesOrNull()
    {
        List<AspectRatioViewModel> aspectRatios = AspectRatiosHelper.All();

        Assert.All(aspectRatios, aspectRatio => Assert.True(aspectRatio.WikipediaLink == null || aspectRatio.WikipediaLink.StartsWith("https://en.wikipedia.org/wiki/")));
        Assert.Equal("https://en.wikipedia.org/wiki/Fullscreen_(aspect_ratio)", Assert.Single(aspectRatios, aspectRatio => aspectRatio.Ratio == "4:3").WikipediaLink);
        Assert.Null(Assert.Single(aspectRatios, aspectRatio => aspectRatio.Ratio == "3:2").WikipediaLink);
        // Wikipedia has no page about 17:9 itself, and the digital cinema standards body is not one
        Assert.Null(Assert.Single(aspectRatios, aspectRatio => aspectRatio.Ratio == "17:9").WikipediaLink);
    }

    [Fact]
    public void FromWikidata_KnownRatio_CarriesItsWikipediaLink()
    {
        List<AspectRatioViewModel>? aspectRatios = AspectRatiosHelper.FromWikidata(GetResponse(("Q1383069", "16:9")));

        Assert.NotNull(aspectRatios);
        Assert.Equal("https://en.wikipedia.org/wiki/16:9_aspect_ratio", Assert.Single(aspectRatios).WikipediaLink);
    }

    [Fact]
    public void FromWikidata_RealResponses_GiveTheExpectedRatios()
    {
        WikidataSparqlResponseDataModel? movieResponse = WikidataHttpClient.GetModelFromResponse(ReadTestData("WikidataHttpClientMovieResponse.json"));
        WikidataSparqlResponseDataModel? tvSeriesResponse = WikidataHttpClient.GetModelFromResponse(ReadTestData("WikidataHttpClientTvSeriesResponse.json"));
        WikidataSparqlResponseDataModel? emptyResponse = WikidataHttpClient.GetModelFromResponse(ReadTestData("WikidataHttpClientEmptyResponse.json"));

        List<AspectRatioViewModel>? movieAspectRatios = AspectRatiosHelper.FromWikidata(movieResponse);
        List<AspectRatioViewModel>? tvSeriesAspectRatios = AspectRatiosHelper.FromWikidata(tvSeriesResponse);
        List<AspectRatioViewModel>? emptyAspectRatios = AspectRatiosHelper.FromWikidata(emptyResponse);

        Assert.NotNull(movieAspectRatios);
        Assert.Equal(
            "Ratio: 2.20:1\nDecimal: 2.20\nName: 70 mm\nDescription: The shape of standard 70 mm film prints, introduced with Todd-AO in 1955 and used for many roadshow epics.\nWikipediaLink: https://en.wikipedia.org/wiki/70_mm_film\n\n"
            + "Ratio: 2.39:1\nDecimal: 2.39\nName: Scope\nDescription: The modern anamorphic widescreen standard since 1970. Often rounded to 2.40:1, which is also how widescreen films are framed on Blu-ray. A TV shows it with black bars above and below.\nWikipediaLink: https://en.wikipedia.org/wiki/Anamorphic_format",
            string.Join("\n\n", movieAspectRatios));
        Assert.NotNull(tvSeriesAspectRatios);
        Assert.Equal(["16:9"], tvSeriesAspectRatios.Select(aspectRatio => aspectRatio.Ratio));
        Assert.NotNull(emptyAspectRatios);
        Assert.Empty(emptyAspectRatios);
    }
}
