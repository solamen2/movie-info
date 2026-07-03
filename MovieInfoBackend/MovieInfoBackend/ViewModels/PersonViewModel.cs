using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record PersonViewModel
{
    public PersonViewModel(SuggestionViewModel suggestionViewModel, 
                          TmdbPersonResponseDataModel tmdbPersonDataModel, 
                          TmdbPersonMovieCreditsResponseDataModel tmdbPersonMovieCreditsDataModel,
                          TmdbPersonTvSeriesCreditsResponseDataModel tmdbPersonTvSeriesCreditsDataModel,
                          TmdbPersonImagesResponseDataModel tmdbPersonImagesDataModel)
    {
        this.ID = Guid.NewGuid();
        this.Image = suggestionViewModel.Image;
        this.ImdbId = suggestionViewModel.ItemID;
        this.Name = suggestionViewModel.Name;
        this.ImdbRank = suggestionViewModel.Rank;
        this.KnownForMovies = suggestionViewModel.KnownFor;
        this.AlsoKnownAs = tmdbPersonDataModel.AlsoKnownAs.ToList();
        this.Biography = tmdbPersonDataModel.Biography;
        this.Birthday = tmdbPersonDataModel.Birthday;
        this.Deathday = tmdbPersonDataModel.Deathday;
        this.Gender = (TmdbGenderType)tmdbPersonDataModel.Gender;
        this.Homepage = tmdbPersonDataModel.Homepage;
        this.TmdbId = tmdbPersonDataModel.Id;
        this.KnownForDepartment = tmdbPersonDataModel.KnownForDepartment;
        this.PlaceOfBirth = tmdbPersonDataModel.PlaceOfBirth;
        this.ProfilePath = tmdbPersonDataModel.ProfilePath;
        this.MovieCastCredits = tmdbPersonMovieCreditsDataModel.Cast.ToList()
                                    .Select(tpmcdm => new PersonMovieCastCreditViewModel(tpmcdm))
                                    .ToList();
        this.MovieCrewCredits = tmdbPersonMovieCreditsDataModel.Crew.ToList()
                                    .Select(tpmcdm => new PersonMovieCrewCreditViewModel(tpmcdm))
                                    .ToList();
        this.TvSeriesCastCredits = tmdbPersonTvSeriesCreditsDataModel.Cast.ToList()
                                        .Select(tptscdm => new PersonTvSeriesCastCreditViewModel(tptscdm))
                                        .ToList();
        this.TvSeriesCrewCredits = tmdbPersonTvSeriesCreditsDataModel.Crew.ToList()
                                        .Select(tptscdm => new PersonTvSeriesCrewCreditViewModel(tptscdm))
                                        .ToList();
        this.ProfileImages = tmdbPersonImagesDataModel.Profiles.ToList()
                                .Select(tpidm => new PersonProfileImageViewModel(tpidm))
                                .ToList();
    }

    public Guid ID { get; init; }
    public SuggestionImageViewModel? Image { get; init; }
    public string ImdbId { get; init; }
    public string Name { get; init; }
    public int? ImdbRank { get; init; }
    public string KnownForMovies { get; init; }
    public List<string> AlsoKnownAs { get; init; }
    public string Biography { get; init; }
    public string Birthday { get; init; }  // Actually a date, of course
    public string Deathday { get; init; }  // Actually a date, of course
    public TmdbGenderType Gender { get; init; }
    public string Homepage { get; init; }
    public int TmdbId { get; init; }
    public string KnownForDepartment { get; init; }
    public string PlaceOfBirth { get; init; }
    public string ProfilePath { get; init; }
    public List<PersonMovieCastCreditViewModel> MovieCastCredits { get; init; }
    public List<PersonMovieCrewCreditViewModel> MovieCrewCredits { get; init; }
    public List<PersonTvSeriesCastCreditViewModel> TvSeriesCastCredits { get; init; }
    public List<PersonTvSeriesCrewCreditViewModel> TvSeriesCrewCredits { get; init; }
    public List<PersonProfileImageViewModel> ProfileImages { get; init; }

    public override string ToString()
    {
        return $"ID: {ID}\nImage:\n*****\n{Image}\n*****\nImdbId: {ImdbId}\nName: {Name}\nImdbRank: {ImdbRank}\nKnownForMovies: {KnownForMovies}\nAlsoKnownAs:{string.Join(",", AlsoKnownAs)}\nBiography: {Biography}\nBirthday: {Birthday}\nDeathday: {Deathday}\nGender: {Gender}\nHomepage: {Homepage}\nTmdbId: {TmdbId}\nKnownForDepartment: {KnownForDepartment}\nPlaceOfBirth: {PlaceOfBirth}\nProfilePath: {ProfilePath}\nMovieCastCredits:\n*****\n{string.Join("\n\n", MovieCastCredits)}\n*****\nMovieCrewCredits:\n*****\n{string.Join("\n\n", MovieCrewCredits)}\n*****\nTvSeriesCastCredits:\n*****\n{string.Join("\n\n", TvSeriesCastCredits)}\n*****\nTvSeriesCrewCredits:\n*****\n{string.Join("\n\n", TvSeriesCrewCredits)}\n*****\nProfileImages:\n*****\n{string.Join("\n\n", ProfileImages)}";
    }
}
