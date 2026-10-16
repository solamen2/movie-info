using MovieInfoBackend.DataModels;
using MovieInfoBackend.Helpers;
using MovieInfoBackend.ViewModels;

namespace TestMovieInfoBackend.Helpers;

public class OscarCategoriesHelperTests
{
    private static OscarsRowDataModel Row(string className, string canonicalCategory, string? category = null)
    {
        return new OscarsRowDataModel
        {
            Ceremony = 1, Year = "1927/28", Class = className, CanonicalCategory = canonicalCategory, Category = category ?? canonicalCategory,
            Films = [], FilmIds = [], Name = "", Nominees = [], NomineeIds = [], Winner = false, Detail = null, Note = null, Citation = null,
        };
    }

    [Fact]
    public void Displayed_TheRequestedCategories_AreInDisplayOrder()
    {
        Assert.Equal(
        [
            "Best Picture", "Best Director", "Best Actor", "Best Supporting Actor", "Best Actress", "Best Supporting Actress",
            "Best Animated Feature Film", "Best International Feature Film", "Best Documentary Feature", "Best Cinematography",
            "Best Original Screenplay", "Best Adapted Screenplay", "Best Film Editing", "Best Visual Effects", "Best Original Score",
            "Best Original Song", "Best Sound", "Best Production Design", "Best Makeup And Hairstyling", "Best Costume Design",
            "Best Casting",
        ], OscarCategoriesHelper.Displayed.Select(display => display.Name));
    }

    [Fact]
    public void Displayed_ModernNamesAndCanonicalCategories_EachBelongToOneCategory()
    {
        Assert.All(OscarCategoriesHelper.Displayed, display => Assert.NotEmpty(display.ModernNames));
        Assert.All(OscarCategoriesHelper.Displayed, display => Assert.NotEmpty(display.CanonicalCategories.Concat(display.Subsections.SelectMany(subsection => subsection.CanonicalCategories))));
        Assert.All(OscarCategoriesHelper.Displayed, display => Assert.NotEqual(1, display.Subsections.Length));  // Split into at least two subsections, or not split
        Assert.All(OscarCategoriesHelper.Displayed.SelectMany(display => display.Subsections), subsection => Assert.NotEmpty(subsection.CanonicalCategories));
        Assert.Equal(OscarCategoriesHelper.Displayed.Sum(display => display.ModernNames.Length),
                     OscarCategoriesHelper.Displayed.SelectMany(display => display.ModernNames).Distinct().Count());
        Assert.Equal(OscarCategoriesHelper.Displayed.Sum(display => display.CanonicalCategories.Length + display.Subsections.Sum(subsection => subsection.CanonicalCategories.Length)),
                     OscarCategoriesHelper.ByCanonicalCategory.Count);
    }

    [Theory]
    [InlineData("ACTOR IN A LEADING ROLE", "Best Actor", null)]
    [InlineData("MAKEUP AND HAIRSTYLING", "Best Makeup And Hairstyling", null)]
    [InlineData("CINEMATOGRAPHY", "Best Cinematography", null)]
    [InlineData("CINEMATOGRAPHY (Black-and-White)", "Best Cinematography", "Black-and-White")]
    [InlineData("ART DIRECTION (Color)", "Best Production Design", "Color")]
    [InlineData("SOUND RECORDING", "Best Sound", "Best Sound Mixing")]
    [InlineData("SOUND EDITING", "Best Sound", "Best Sound Editing")]
    [InlineData("SPECIAL ACHIEVEMENT AWARD (Sound Effects Editing)", "Best Sound", "Best Sound Editing")]
    [InlineData("WRITING (Original Screenplay)", "Best Original Screenplay", "Best Original Screenplay")]
    [InlineData("WRITING (Original Story)", "Best Original Screenplay", "Best Story")]
    [InlineData("MUSIC (Original Score)", "Best Original Score", "Best Original Non-Musical Score")]
    [InlineData("MUSIC (Original Song Score or Adaptation Score)", "Best Original Score", "Best Original Musical Score")]
    [InlineData("SPECIAL ACHIEVEMENT AWARD (Visual Effects)", "Best Visual Effects", null)]
    [InlineData("SPECIAL FOREIGN LANGUAGE FILM AWARD", "Best International Feature Film", null)]
    [InlineData("DIRECTING (Comedy Picture)", "Best Director", "Comedy Picture")]
    [InlineData("CASTING", "Best Casting", null)]
    public void ByCanonicalCategory_OlderCategories_FoldIntoTheModernOnes(string canonicalCategory, string expectedDisplayName, string? expectedSection)
    {
        // Act
        bool found = OscarCategoriesHelper.ByCanonicalCategory.TryGetValue(canonicalCategory, out OscarCategoryMappingViewModel? mapping);

        // Assert
        Assert.True(found);
        Assert.Equal(expectedDisplayName, mapping!.Display.Name);
        Assert.Equal(expectedSection, mapping.Subsection?.Section);
    }

    [Fact]
    public void MappingOf_Rows_FollowsTheirCanonicalCategory()
    {
        Assert.Equal("Best Picture", OscarCategoriesHelper.MappingOf(Row("Title", "BEST PICTURE", "OUTSTANDING PRODUCTION"))!.Display.Name);
        Assert.Equal("Color", OscarCategoriesHelper.MappingOf(Row("Production", "CINEMATOGRAPHY (Color)"))!.Subsection!.Section);
        Assert.Null(OscarCategoriesHelper.MappingOf(Row("Special", "HONORARY AWARD")));
    }

    [Fact]
    public void DisplayedAndMappings_ValidModelToString_ReturnCorrectValue()
    {
        OscarDisplayCategoryViewModel bestPicture = OscarCategoriesHelper.Displayed[0];
        OscarDisplayCategoryViewModel cinematography = OscarCategoriesHelper.Displayed.Single(display => display.Name == "Best Cinematography");

        Assert.Equal("Name: Best Picture\nCanonicalCategories: BEST PICTURE\nModernNames: BEST PICTURE\nSubsections:\n*****\n\n*****", bestPicture.ToString());
        Assert.Equal("Section: Color\nCategory: Best Cinematography (Color)\nCanonicalCategories: CINEMATOGRAPHY (Color)\nModernNames: CINEMATOGRAPHY (Color)",
                     cinematography.Subsections[1].ToString());
        Assert.Equal("Name: Best Cinematography\nCanonicalCategories: CINEMATOGRAPHY\nModernNames: CINEMATOGRAPHY\nSubsections:\n*****\n"
                     + cinematography.Subsections[0] + "\n\n" + cinematography.Subsections[1] + "\n*****", cinematography.ToString());
        Assert.Equal("Display:\n*****\n" + bestPicture + "\n*****\nSubsection:\n*****\n\n*****", OscarCategoriesHelper.ByCanonicalCategory["BEST PICTURE"].ToString());
        Assert.Equal("Display:\n*****\n" + cinematography + "\n*****\nSubsection:\n*****\n" + cinematography.Subsections[1] + "\n*****",
                     OscarCategoriesHelper.ByCanonicalCategory["CINEMATOGRAPHY (Color)"].ToString());
    }

    [Theory]
    [InlineData("ASSISTANT DIRECTOR")]
    [InlineData("DANCE DIRECTION")]
    [InlineData("WRITING (Title Writing)")]
    [InlineData("SHORT FILM (Animated)")]
    [InlineData("HONORARY AWARD")]
    public void ByCanonicalCategory_CategoriesWithoutAModernOne_AreNotDisplayedCategories(string canonicalCategory)
    {
        Assert.False(OscarCategoriesHelper.ByCanonicalCategory.ContainsKey(canonicalCategory));
    }

    [Theory]
    [InlineData("SciTech", "SCIENTIFIC AND TECHNICAL AWARD (Technical Achievement Award)", null)]
    [InlineData("Title", "SHORT FILM (Animated)", null)]
    [InlineData("Title", "SHORT FILM (Live Action)", "LIVE ACTION SHORT FILM")]
    [InlineData("Title", "DOCUMENTARY (Short Subject)", "DOCUMENTARY SHORT FILM")]
    [InlineData("Title", "SHORT SUBJECT (One-reel)", null)]
    public void IsExcluded_SciTechAndShortFilms_AreExcluded(string className, string canonicalCategory, string? category)
    {
        Assert.True(OscarCategoriesHelper.IsExcluded(Row(className, canonicalCategory, category)));
    }

    [Theory]
    [InlineData("Title", "BEST PICTURE")]
    [InlineData("Title", "DOCUMENTARY (Feature)")]
    [InlineData("Special", "HONORARY AWARD")]
    [InlineData("Acting", "ACTOR IN A SUPPORTING ROLE")]
    public void IsExcluded_OtherAwards_AreNotExcluded(string className, string canonicalCategory)
    {
        Assert.False(OscarCategoriesHelper.IsExcluded(Row(className, canonicalCategory)));
    }

    [Theory]
    [InlineData("Special", "HONORARY AWARD", true)]
    [InlineData("Special", "IRVING G. THALBERG MEMORIAL AWARD", true)]
    [InlineData("Special", "SPECIAL AWARD", true)]
    [InlineData("Title", "UNIQUE AND ARTISTIC PICTURE", true)]
    [InlineData("Title", "BEST PICTURE", false)]
    [InlineData("SciTech", "SCIENTIFIC OR TECHNICAL AWARD (Class I)", false)]
    public void IsSpecialAward_TheSpecialClassAndUniqueAndArtisticPicture_AreSpecial(string className, string canonicalCategory, bool expected)
    {
        Assert.Equal(expected, OscarCategoriesHelper.IsSpecialAward(Row(className, canonicalCategory)));
    }

    [Theory]
    [InlineData("ACTOR", "Actor")]
    [InlineData("ACTOR IN A LEADING ROLE", "Actor in a Leading Role")]
    [InlineData("OUTSTANDING PRODUCTION", "Outstanding Production")]
    [InlineData("ART DIRECTION (Black-and-White)", "Art Direction (Black-and-White)")]
    [InlineData("MUSIC (Song)", "Music (Song)")]
    [InlineData("WRITING (Screenplay--based on material from another medium)", "Writing (Screenplay--based on material from another medium)")]
    [InlineData("IRVING G. THALBERG MEMORIAL AWARD", "Irving G. Thalberg Memorial Award")]
    [InlineData("AWARD OF COMMENDATION", "Award of Commendation")]
    [InlineData("JOHN A. BONNER MEDAL OF COMMENDATION", "John A. Bonner Medal of Commendation")]
    [InlineData("UNIQUE AND ARTISTIC PICTURE", "Unique and Artistic Picture")]
    [InlineData("SPECIAL ACHIEVEMENT AWARD (Visual Effects)", "Special Achievement Award (Visual Effects)")]
    [InlineData("Already Title Case", "Already Title Case")]
    [InlineData("", "")]
    public void ToTitleCase_UpperCaseWords_AreTitleCased(string category, string expected)
    {
        Assert.Equal(expected, OscarCategoriesHelper.ToTitleCase(category));
    }
}
