using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record MovieCastViewModel
{    
    // TODO: It would be nice to have a link out to the IMDB for all cast, crew (director / producer / writer), and guest star models. Need to call TMDB External IDs API for that (but only call when a button is pressed in UI! Or the person is somehow selected...)
    public MovieCastViewModel(TmdbMovieCastDataModel movieCastDataModel)
    {
        this.ID = Guid.NewGuid();
        this.Gender = (TmdbGenderType)movieCastDataModel.Gender;
        this.TmdbId = movieCastDataModel.Id;
        this.Name = movieCastDataModel.Name;
        this.OriginalName = movieCastDataModel.OriginalName;
        this.Popularity = movieCastDataModel.Popularity;
        this.ProfilePath = movieCastDataModel.ProfilePath;
        this.Character = movieCastDataModel.Character;
        this.BilledOrder = movieCastDataModel.Order;
    }

    public Guid ID { get; }
    public TmdbGenderType Gender { get; }
    public int TmdbId { get; }
    public string Name { get; }
    public string OriginalName { get; }
    public double Popularity { get; }
    public string? ProfilePath { get; }
    public string Character { get; }
    public int BilledOrder { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nGender: {Gender}\nTmdbId: {TmdbId}\nName: {Name}\nOriginalName: {OriginalName}\nPopularity: {Popularity}\nProfilePath: {ProfilePath}\nCharacter: {Character}\nBilledOrder: {BilledOrder}";
    }
}
