using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record MovieCastViewModel
{    
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
