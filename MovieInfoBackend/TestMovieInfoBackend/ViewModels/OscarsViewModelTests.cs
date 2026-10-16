using System.Text.Json;
using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.ViewModels;

public class OscarsViewModelTests
{
    private readonly OscarsViewModel oscarsViewModel;

    public OscarsViewModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testDataFilename = "OscarsHttpClientResponse.tsv";
        string oscarsResponse;
        using (StreamReader sr = File.OpenText($"../../../TestData/{testDataFilename}"))
        {
            oscarsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(oscarsResponse))
        {
            throw new ArgumentException($"{testDataFilename} is not valid test data.");
        }
        OscarsResponseDataModel? oscarsResponseDataModel = OscarsHttpClient.GetModelFromResponse(oscarsResponse);
        Assert.NotNull(oscarsResponseDataModel);
        oscarsViewModel = new OscarsViewModel(oscarsResponseDataModel.Rows);
    }

    private OscarYearViewModel Year(int ceremony)
    {
        OscarYearViewModel? year = oscarsViewModel.Years.Find(y => y.Ceremony == ceremony);
        Assert.NotNull(year);
        return year;
    }

    // A category undivided that year
    private static OscarCategorySubsectionViewModel Whole(OscarCategoryViewModel category)
    {
        OscarCategorySubsectionViewModel subsection = Assert.Single(category.Subsections);
        Assert.Null(subsection.Section);
        return subsection;
    }

    private static List<OscarAwardViewModel> Entries(OscarCategoryViewModel category)
    {
        return Whole(category).Entries;
    }

    private static string FilmTitles(OscarAwardViewModel award)
    {
        return string.Join("|", award.Films.Select(film => film.Title));
    }

    [Fact]
    public void Years_InTheListing_AreMostRecentFirst()
    {
        Assert.Equal([96, 85, 67, 49, 14, 13, 11, 1], oscarsViewModel.Years.Select(year => year.Ceremony));
        Assert.Equal("2023", oscarsViewModel.Years[0].Year);
        Assert.Equal("1927/28", oscarsViewModel.Years[^1].Year);
    }

    [Fact]
    public void Categories_AreInDisplayOrder_WithoutShortFilmsOrSciTechAwards()
    {
        // The test data for the 96th ceremony also has animated and documentary short films and a SciTech award
        OscarYearViewModel year = Year(96);

        Assert.Equal(["Best Picture", "Best Actor", "Best International Feature Film", "Best Original Song", "Best Sound"],
                     year.Categories.Select(category => category.Category));
        Assert.All(year.Categories, category => Assert.Null(Whole(category).OriginalCategory));
        Assert.All(year.Categories.SelectMany(Entries), entry => Assert.False(entry.IsSpecial));
    }

    [Fact]
    public void Categories_ListTheWinnerFirst_ThenTheNomineesInTheDatasetsOrder()
    {
        List<OscarAwardViewModel> bestPicture = Entries(Year(96).Categories[0]);

        Assert.Equal(10, bestPicture.Count);
        OscarAwardViewModel winner = bestPicture[0];
        Assert.True(winner.Winner);
        Assert.Equal([new OscarFilmViewModel("Oppenheimer", "tt15398776")], winner.Films);
        Assert.Equal("Emma Thomas, Charles Roven and Christopher Nolan, Producers", winner.Name);
        Assert.Equal([new OscarNomineeViewModel("Emma Thomas", "nm0858799"), new OscarNomineeViewModel("Charles Roven", "nm0746273"), new OscarNomineeViewModel("Christopher Nolan", "nm0634240")],
                     winner.Nominees);
        Assert.Equal(96, winner.Ceremony);
        Assert.Equal("2023", winner.Year);
        Assert.Equal("Best Picture", winner.Category);
        Assert.Null(winner.OriginalCategory);
        Assert.Empty(winner.Characters);
        Assert.Null(winner.Song);
        Assert.All(bestPicture.Skip(1), entry => Assert.False(entry.Winner));
        Assert.Equal(["American Fiction", "Anatomy of a Fall", "Barbie", "The Holdovers", "Killers of the Flower Moon", "Maestro", "Past Lives", "Poor Things", "The Zone of Interest"],
                     bestPicture.Skip(1).Select(FilmTitles));
    }

    [Fact]
    public void ActingAwards_WithADetail_HaveTheCharacterPlayed()
    {
        OscarAwardViewModel winner = Entries(Year(96).Categories[1])[0];

        Assert.Equal("Best Actor", winner.Category);
        Assert.Equal("Cillian Murphy", winner.Name);
        Assert.Equal(["J. Robert Oppenheimer"], winner.Characters);
        Assert.Null(winner.Song);

        // One character per film in the earliest years, when a nomination could be for several films
        OscarAwardViewModel gaynor = Entries(Year(1).Categories.Single(category => category.Category == "Best Actress"))[0];
        Assert.True(gaynor.Winner);
        Assert.Equal("7th Heaven|Street Angel|Sunrise", FilmTitles(gaynor));
        Assert.Equal(["Diane", "Angela", "The Wife"], gaynor.Characters);
    }

    [Fact]
    public void OriginalSongAwards_WithADetail_HaveTheSongTitle()
    {
        OscarAwardViewModel winner = Entries(Year(96).Categories[3])[0];

        Assert.Equal("Best Original Song", winner.Category);
        Assert.Equal("What Was I Made For?", winner.Song);
        Assert.Equal("Music and Lyric by Billie Eilish and Finneas O'Connell", winner.Name);
        Assert.Equal(["Billie Eilish", "Finneas O'Connell"], winner.Nominees.Select(nominee => nominee.Name));
        Assert.Empty(winner.Characters);
    }

    [Fact]
    public void InternationalFeatureFilmAwards_NameTheCountry_WithoutNominees()
    {
        OscarAwardViewModel winner = Entries(Year(96).Categories[2])[0];

        Assert.Equal("Best International Feature Film", winner.Category);
        Assert.Equal("The Zone of Interest", FilmTitles(winner));
        Assert.Equal("United Kingdom", winner.Name);
        Assert.Empty(winner.Nominees);
    }

    [Fact]
    public void Categories_UnderAnOlderName_SayWhatItWas()
    {
        OscarCategoryViewModel bestPicture1941 = Year(14).Categories.Single();
        Assert.Equal("Best Picture", bestPicture1941.Category);
        Assert.Equal("Outstanding Motion Picture", Whole(bestPicture1941).OriginalCategory);
        Assert.All(Entries(bestPicture1941), entry => Assert.Equal("Outstanding Motion Picture", entry.OriginalCategory));

        OscarCategoryViewModel adaptedScreenplay1994 = Year(67).Categories.Single(category => category.Category == "Best Adapted Screenplay");
        Assert.Equal("Writing (Screenplay Based on Material Previously Produced or Published)", Whole(adaptedScreenplay1994).OriginalCategory);

        OscarCategoryViewModel bestPicture1928 = Year(1).Categories.Single(category => category.Category == "Best Picture");
        Assert.Equal("Outstanding Picture", Whole(bestPicture1928).OriginalCategory);

        OscarCategoryViewModel bestActress1928 = Year(1).Categories.Single(category => category.Category == "Best Actress");
        Assert.Null(Whole(bestActress1928).OriginalCategory);  // "Actress" is one of the category's modern names, like "Actress in a Leading Role"

        OscarCategoryViewModel bestSound2023 = Year(96).Categories.Single(category => category.Category == "Best Sound");
        Assert.Null(Whole(bestSound2023).OriginalCategory);  // "Sound" is one of the category's modern names
    }

    [Fact]
    public void Categories_SplitInSomeYears_HaveASubsectionEach()
    {
        // Black-and-white and color art direction in 1940
        OscarCategoryViewModel productionDesign1940 = Year(13).Categories.Single(category => category.Category == "Best Production Design");
        Assert.Equal(["Black-and-White", "Color"], productionDesign1940.Subsections.Select(subsection => subsection.Section));
        Assert.All(productionDesign1940.Subsections, subsection => Assert.Null(subsection.OriginalCategory));  // The subsections' titles are the names at the time
        Assert.Equal("Pride and Prejudice", FilmTitles(productionDesign1940.Subsections[0].Entries[0]));
        Assert.True(productionDesign1940.Subsections[0].Entries[0].Winner);
        Assert.Equal("Best Production Design (Black-and-White)", productionDesign1940.Subsections[0].Entries[0].Category);
        Assert.Equal("The Thief of Bagdad", FilmTitles(productionDesign1940.Subsections[1].Entries[0]));
        Assert.True(productionDesign1940.Subsections[1].Entries[0].Winner);
        Assert.Equal("Best Production Design (Color)", productionDesign1940.Subsections[1].Entries[0].Category);

        // Original screenplay and original story in 1940
        OscarCategoryViewModel screenplay1940 = Year(13).Categories.Single(category => category.Category == "Best Original Screenplay");
        Assert.Equal(["Best Original Screenplay", "Best Story"], screenplay1940.Subsections.Select(subsection => subsection.Section));
        Assert.Equal([null, "Writing (Original Story)"], screenplay1940.Subsections.Select(subsection => subsection.OriginalCategory));
        Assert.Equal("The Great McGinty", FilmTitles(screenplay1940.Subsections[0].Entries[0]));
        Assert.Equal("Best Original Screenplay", screenplay1940.Subsections[0].Entries[0].Category);
        Assert.Equal("Arise, My Love", FilmTitles(screenplay1940.Subsections[1].Entries[0]));
        Assert.Equal("Best Story", screenplay1940.Subsections[1].Entries[0].Category);

        // Original score and original song score / adaptation score in 1976
        OscarCategoryViewModel score1976 = Year(49).Categories.Single(category => category.Category == "Best Original Score");
        Assert.Equal(["Best Original Non-Musical Score", "Best Original Musical Score"], score1976.Subsections.Select(subsection => subsection.Section));
        Assert.Equal(["Music (Original Score)", "Music (Original Song Score and Its Adaptation or Adaptation Score)"],
                     score1976.Subsections.Select(subsection => subsection.OriginalCategory));
        Assert.Equal("The Omen", FilmTitles(score1976.Subsections[0].Entries[0]));
        Assert.Equal("Bound for Glory", FilmTitles(score1976.Subsections[1].Entries[0]));

        // Sound and sound effects editing in 1994 (the latter named differently from its section); sound mixing and sound editing in 2012, with a tie for sound editing
        OscarCategoryViewModel sound1994 = Year(67).Categories.Single(category => category.Category == "Best Sound");
        Assert.Equal(["Best Sound Mixing", "Best Sound Editing"], sound1994.Subsections.Select(subsection => subsection.Section));
        Assert.Equal(["Sound", "Sound Effects Editing"], sound1994.Subsections.Select(subsection => subsection.OriginalCategory));
        Assert.Equal("Speed", FilmTitles(sound1994.Subsections[0].Entries[0]));
        Assert.Equal("Speed", FilmTitles(sound1994.Subsections[1].Entries[0]));
        OscarCategoryViewModel sound2012 = Year(85).Categories.Single(category => category.Category == "Best Sound");
        Assert.Equal(["Best Sound Mixing", "Best Sound Editing"], sound2012.Subsections.Select(subsection => subsection.Section));
        Assert.Equal([null, null], sound2012.Subsections.Select(subsection => subsection.OriginalCategory));
        Assert.Equal(["Skyfall", "Zero Dark Thirty", "Argo", "Django Unchained", "Life of Pi"], sound2012.Subsections[1].Entries.Select(FilmTitles));
        Assert.Equal([true, true, false, false, false], sound2012.Subsections[1].Entries.Select(entry => entry.Winner));
        Assert.StartsWith("NOTE: A tie.", sound2012.Subsections[1].Entries[0].Note);
        Assert.All(sound2012.Subsections[1].Entries, entry => Assert.Equal("Best Sound Editing", entry.Category));

        // The category is one undivided subsection when only one of its divisions was awarded, like sound in 2023 (and all other categories here)
        Assert.All(oscarsViewModel.Years.SelectMany(year => year.Categories).Where(category => category.Subsections.Count == 1),
                   category => Assert.Null(category.Subsections[0].Section));
        Assert.Equal("Best Sound", Entries(Year(96).Categories.Single(category => category.Category == "Best Sound"))[0].Category);
    }

    [Fact]
    public void SpecialAchievementAwards_ForVisualEffects_CountAsBestVisualEffects()
    {
        OscarYearViewModel year1976 = Year(49);

        Assert.Equal(["Best Visual Effects", "Best Original Score"], year1976.Categories.Select(category => category.Category));
        OscarCategoryViewModel visualEffects = year1976.Categories[0];
        Assert.Equal("Special Achievement Award (Visual Effects)", Whole(visualEffects).OriginalCategory);
        Assert.Equal(["King Kong", "Logan's Run"], Entries(visualEffects).Select(FilmTitles));
        Assert.All(Entries(visualEffects), entry => Assert.True(entry.Winner));
        Assert.All(Entries(visualEffects), entry => Assert.False(entry.IsSpecial));
        Assert.Empty(year1976.SpecialAwards);
    }

    [Fact]
    public void SpecialAwards_ComeAfterTheCategories_InTheDatasetsOrder()
    {
        OscarYearViewModel year1928 = Year(1);

        Assert.Equal(["Best Picture", "Best Actress", "Best Visual Effects"], year1928.Categories.Select(category => category.Category));  // No title writing
        Assert.Equal(["Unique and Artistic Picture", "Unique and Artistic Picture", "Unique and Artistic Picture", "Special Award", "Special Award"],
                     year1928.SpecialAwards.Select(award => award.Category));
        Assert.All(year1928.SpecialAwards, award => Assert.True(award.IsSpecial));
        Assert.All(year1928.SpecialAwards, award => Assert.Null(award.OriginalCategory));
        Assert.Equal(["Sunrise", "The Crowd", "Chang", "The Jazz Singer", "The Circus"], year1928.SpecialAwards.Select(FilmTitles));
        Assert.Equal([true, false, false, true, true], year1928.SpecialAwards.Select(award => award.Winner));

        OscarAwardViewModel chaplin = year1928.SpecialAwards[4];
        Assert.Equal("", chaplin.Name);
        Assert.Equal([new OscarNomineeViewModel("Charles Chaplin", "nm0000122")], chaplin.Nominees);
        Assert.Equal("To Charles Chaplin, for acting, writing, directing and producing The Circus.", chaplin.Citation);
        Assert.StartsWith("NOTE: \"The Academy Board of Judges", chaplin.Note);

        // Honorary awards name the person in Nominees only
        OscarYearViewModel year2023 = Year(96);
        Assert.Equal(["Honorary Award", "Honorary Award", "Honorary Award"], year2023.SpecialAwards.Select(award => award.Category));
        Assert.Equal(["Angela Bassett", "Mel Brooks", "Carol Littleton"], year2023.SpecialAwards.Select(award => award.Nominees.Single().Name));
        Assert.All(year2023.SpecialAwards, award => Assert.Empty(award.Films));
        Assert.All(year2023.SpecialAwards, award => Assert.StartsWith("To ", award.Citation));
    }

    [Fact]
    public void SpecialAwards_IncludeTheNominees_WhenAnyWereAnnounced()
    {
        // The Thalberg award's only year with nominees
        OscarYearViewModel year1938 = Year(11);

        Assert.Empty(year1938.Categories);
        Assert.Equal(7, year1938.SpecialAwards.Count);
        Assert.All(year1938.SpecialAwards, award => Assert.Equal("Irving G. Thalberg Memorial Award", award.Category));
        OscarAwardViewModel winner = Assert.Single(year1938.SpecialAwards, award => award.Winner);
        Assert.Equal("Hal B. Wallis", winner.Name);
    }

    [Fact]
    public void Nominees_WithACompanyId_HaveNoImdbId()
    {
        // The studios named in the earliest years have company ids (co…) in the dataset, which aren't people the app can show
        OscarAwardViewModel wings = Entries(Year(1).Categories.Single(category => category.Category == "Best Picture"))[0];
        Assert.Equal("Wings", FilmTitles(wings));
        Assert.Equal([new OscarNomineeViewModel("Paramount Famous Lasky", null)], wings.Nominees);
        Assert.Equal([new OscarFilmViewModel("Wings", "tt0018578")], wings.Films);
    }

    [Fact]
    public void FilmsAndNominees_ValidModelToString_ReturnCorrectValue()
    {
        OscarAwardViewModel wings = Entries(Year(1).Categories.Single(category => category.Category == "Best Picture"))[0];
        OscarAwardViewModel murphy = Entries(Year(96).Categories.Single(category => category.Category == "Best Actor"))[0];

        Assert.Equal("Title: Wings\nImdbId: tt0018578", wings.Films[0].ToString());
        Assert.Equal("Name: Paramount Famous Lasky\nImdbId: ", wings.Nominees[0].ToString());
        Assert.Equal("Title: Oppenheimer\nImdbId: tt15398776", murphy.Films[0].ToString());
        Assert.Equal("Name: Cillian Murphy\nImdbId: nm0614165", murphy.Nominees[0].ToString());
    }

    [Fact]
    public void Awards_ValidModelToString_ReturnsCorrectValue()
    {
        OscarAwardViewModel gaynor = Entries(Year(1).Categories.Single(category => category.Category == "Best Actress"))[0];

        Assert.Equal(
            "Ceremony: 1\nYear: 1927/28\nCategory: Best Actress\nOriginalCategory: \nIsSpecial: False\nWinner: True\n"
            + "Films:\n*****\nTitle: 7th Heaven\nImdbId: tt0018379\n\nTitle: Street Angel\nImdbId: tt0019429\n\nTitle: Sunrise\nImdbId: tt0018455\n*****\n"
            + "Name: Janet Gaynor\nNominees:\n*****\nName: Janet Gaynor\nImdbId: nm0310980\n*****\n"
            + "Characters: Diane | Angela | The Wife\nSong: \nNote: \nCitation: "
            , gaynor.ToString());
    }

    [Fact]
    public void Years_SerializedForTheApi_IsCamelCase()
    {
        string json = JsonSerializer.Serialize(oscarsViewModel.Years, JsonSerializerOptions.Web);

        Assert.StartsWith("[{\"ceremony\":96,\"year\":\"2023\",\"categories\":[{\"category\":\"Best Picture\",\"subsections\":[{\"section\":null,\"originalCategory\":null,\"entries\":[{\"ceremony\":96,", json);
        Assert.Contains("\"specialAwards\":[", json);
        Assert.Contains("\"characters\":[\"J. Robert Oppenheimer\"]", json);
    }
}
