using MovieInfoBackend.DataModels;
using MovieInfoBackend.Helpers;
using MovieInfoBackend.ViewModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.Helpers;

public class OscarAwardsHelperTests
{
    private readonly OscarsViewModel oscarsViewModel;

    public OscarAwardsHelperTests(ITestOutputHelper output)
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

    private static string FilmTitles(OscarAwardViewModel award)
    {
        return string.Join("|", award.Films.Select(film => film.Title));
    }

    [Theory]
    [InlineData("tt0111161", "tt0111161")]
    [InlineData("tt1", "tt1")]
    [InlineData("nm0000151", null)]  // A person's id
    [InlineData("co0023400", null)]  // A company's id
    [InlineData("tt", null)]
    [InlineData("tt12a", null)]
    [InlineData("", null)]
    [InlineData(null, null)]
    public void FilmImdbIdOrNull_Ids_KeepsOnlyFilmIds(string? id, string? expected)
    {
        Assert.Equal(expected, OscarAwardsHelper.FilmImdbIdOrNull(id));
    }

    [Theory]
    [InlineData("nm0000151", "nm0000151")]
    [InlineData("tt0111161", null)]  // A film's id
    [InlineData("co0023400", null)]  // A company's id
    [InlineData("nm0001227,nm0001054", null)]  // Two people sharing a pseudonym
    [InlineData("?", null)]
    [InlineData(null, null)]
    public void PersonImdbIdOrNull_Ids_KeepsOnlyPersonIds(string? id, string? expected)
    {
        Assert.Equal(expected, OscarAwardsHelper.PersonImdbIdOrNull(id));
    }

    [Fact]
    public void All_Awards_AreInTheByYearOrderWithSpecialAwardsLast()
    {
        List<OscarAwardViewModel> all = OscarAwardsHelper.All(oscarsViewModel).ToList();

        Assert.Equal(oscarsViewModel.Years.Sum(year => year.Categories.Sum(category => category.Subsections.Sum(subsection => subsection.Entries.Count)) + year.SpecialAwards.Count), all.Count);
        Assert.Equal(96, all[0].Ceremony);
        Assert.Equal("Best Picture", all[0].Category);
        Assert.Equal(1, all[^1].Ceremony);
        Assert.True(all[^1].IsSpecial);
        // Each ceremony's awards are together, most recent ceremony first
        Assert.Equal(all.Select(award => award.Ceremony).Distinct(), all.Select(award => award.Ceremony).Distinct().OrderByDescending(ceremony => ceremony));
    }

    [Fact]
    public void ForMovie_ByImdbId_ReturnsTheMoviesAwardsInDisplayOrder()
    {
        List<OscarAwardViewModel> shawshank = OscarAwardsHelper.ForMovie(oscarsViewModel, "tt0111161", "The Shawshank Redemption");

        Assert.Equal(["Best Picture", "Best Actor", "Best Cinematography", "Best Adapted Screenplay", "Best Film Editing", "Best Original Score", "Best Sound Mixing"],
                     shawshank.Select(award => award.Category));  // Sound was split that year
        Assert.All(shawshank, award => Assert.False(award.Winner));
        Assert.All(shawshank, award => Assert.Equal(67, award.Ceremony));
        Assert.Equal("Morgan Freeman", shawshank[1].Name);
        Assert.Equal(["Red"], shawshank[1].Characters);

        List<OscarAwardViewModel> oppenheimer = OscarAwardsHelper.ForMovie(oscarsViewModel, "tt15398776", "Oppenheimer");
        Assert.Equal(["Best Picture", "Best Actor", "Best Sound"], oppenheimer.Select(award => award.Category));
        Assert.Equal([true, true, false], oppenheimer.Select(award => award.Winner));
    }

    [Fact]
    public void ForMovie_ForAMovieWithSeveralFilmsInOneNomination_ReturnsThatNomination()
    {
        List<OscarAwardViewModel> sunrise = OscarAwardsHelper.ForMovie(oscarsViewModel, "tt0018455", "Sunrise");

        Assert.Equal(["Best Actress", "Unique and Artistic Picture"], sunrise.Select(award => award.Category));
        Assert.Equal("Janet Gaynor", sunrise[0].Name);
    }

    [Fact]
    public void ForMovie_UnknownMovie_ReturnsNothing()
    {
        Assert.Empty(OscarAwardsHelper.ForMovie(oscarsViewModel, "tt9999999", "No Such Movie"));
        // The title is only used for films the dataset has no IMDB id for, so another film with the same title doesn't match
        Assert.Empty(OscarAwardsHelper.ForMovie(oscarsViewModel, "tt9999999", "Oppenheimer"));
    }

    [Fact]
    public void ForPerson_ByImdbId_ReturnsThePersonsAwards()
    {
        List<OscarAwardViewModel> newman = OscarAwardsHelper.ForPerson(oscarsViewModel, "nm0002353", "Thomas Newman");
        Assert.Equal(["Little Women", "The Shawshank Redemption"], newman.Select(FilmTitles));
        Assert.All(newman, award => Assert.Equal("Best Original Score", award.Category));

        List<OscarAwardViewModel> robinson = OscarAwardsHelper.ForPerson(oscarsViewModel, "nm0732656", "Glen Robinson");
        Assert.Equal(["King Kong", "Logan's Run"], robinson.Select(FilmTitles));
        Assert.All(robinson, award => Assert.True(award.Winner));

        // Producers are nominees for Best Picture, and special awards count too
        List<OscarAwardViewModel> nolan = OscarAwardsHelper.ForPerson(oscarsViewModel, "nm0634240", "Christopher Nolan");
        Assert.Equal(["Best Picture"], nolan.Select(award => award.Category));
        List<OscarAwardViewModel> bassett = OscarAwardsHelper.ForPerson(oscarsViewModel, "nm0000291", "Angela Bassett");
        Assert.Equal(["Honorary Award"], bassett.Select(award => award.Category));
        Assert.True(bassett[0].IsSpecial);
    }

    [Fact]
    public void ForPerson_ByName_OnlyForNomineesWithoutAnImdbId()
    {
        // Citizen Kane's producer "Mercury" has no id in the dataset
        List<OscarAwardViewModel> mercury = OscarAwardsHelper.ForPerson(oscarsViewModel, "", "mercury");
        Assert.Equal(["Citizen Kane"], mercury.Select(FilmTitles));
        Assert.Equal(mercury, OscarAwardsHelper.ForPerson(oscarsViewModel, "nm9999999", " Mercury "));

        Assert.Empty(OscarAwardsHelper.ForPerson(oscarsViewModel, "nm9999999", "Cillian Murphy"));
        Assert.Empty(OscarAwardsHelper.ForPerson(oscarsViewModel, "nm9999999", "No Such Person"));
    }
}
