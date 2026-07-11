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
        this.MovieCastCredits = tmdbPersonMovieCreditsDataModel.Cast
                                    .Select(tpmcdm => new PersonMovieCastViewModel(tpmcdm))
                                    .ToList();
        this.MovieCrewCredits = tmdbPersonMovieCreditsDataModel.Crew
                                    .Select(tpmcdm => new PersonMovieCrewViewModel(tpmcdm))
                                    .ToList();
        this.TvSeriesCastCredits = tmdbPersonTvSeriesCreditsDataModel.Cast
                                        .Select(tptscdm => new PersonTvSeriesCastViewModel(tptscdm))
                                        .ToList();
        this.TvSeriesCrewCredits = tmdbPersonTvSeriesCreditsDataModel.Crew
                                        .Select(tptscdm => new PersonTvSeriesCrewViewModel(tptscdm))
                                        .ToList();
        this.ProfileImages = tmdbPersonImagesDataModel.Profiles
                                .Select(tpidm => new PersonProfileImageViewModel(tpidm))
                                .ToList();
    }

    public Guid ID { get; }
    public SuggestionImageViewModel? Image { get; }
    public string ImdbId { get; }
    public string Name { get; }
    public int? ImdbRank { get; }
    public string KnownForMovies { get; }
    public List<string> AlsoKnownAs { get; }
    public string Biography { get; }
    public string Birthday { get; }  // Actually a date, of course
    public string Deathday { get; }  // Actually a date, of course
    public TmdbGenderType Gender { get; }
    public string Homepage { get; }
    public int TmdbId { get; }
    public string KnownForDepartment { get; }
    public string PlaceOfBirth { get; }
    public string ProfilePath { get; }
    public List<PersonMovieCastViewModel> MovieCastCredits { get; }
    public List<PersonMovieCrewViewModel> MovieCrewCredits { get; }
    public List<PersonTvSeriesCastViewModel> TvSeriesCastCredits { get; }
    public List<PersonTvSeriesCrewViewModel> TvSeriesCrewCredits { get; }
    public List<PersonProfileImageViewModel> ProfileImages { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nImage:\n*****\n{Image}\n*****\nImdbId: {ImdbId}\nName: {Name}\nImdbRank: {ImdbRank}\nKnownForMovies: {KnownForMovies}\nAlsoKnownAs:{string.Join(",", AlsoKnownAs)}\nBiography: {Biography}\nBirthday: {Birthday}\nDeathday: {Deathday}\nGender: {Gender}\nHomepage: {Homepage}\nTmdbId: {TmdbId}\nKnownForDepartment: {KnownForDepartment}\nPlaceOfBirth: {PlaceOfBirth}\nProfilePath: {ProfilePath}\nMovieCastCredits:\n*****\n{string.Join("\n\n", MovieCastCredits)}\n*****\nMovieCrewCredits:\n*****\n{string.Join("\n\n", MovieCrewCredits)}\n*****\nTvSeriesCastCredits:\n*****\n{string.Join("\n\n", TvSeriesCastCredits)}\n*****\nTvSeriesCrewCredits:\n*****\n{string.Join("\n\n", TvSeriesCrewCredits)}\n*****\nProfileImages:\n*****\n{string.Join("\n\n", ProfileImages)}";
    }
}
