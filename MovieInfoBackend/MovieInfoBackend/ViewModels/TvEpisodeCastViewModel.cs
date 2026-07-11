using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TvEpisodeCastViewModel
{    
    public TvEpisodeCastViewModel(TmdbTvEpisodeCastDataModel tvEpisodeCastDataModel)
    {
        this.ID = Guid.NewGuid();
        this.Gender = (TmdbGenderType)tvEpisodeCastDataModel.Gender;
        this.TmdbId = tvEpisodeCastDataModel.Id;
        this.Name = tvEpisodeCastDataModel.Name;
        this.OriginalName = tvEpisodeCastDataModel.OriginalName;
        this.Popularity = tvEpisodeCastDataModel.Popularity;
        this.ProfilePath = tvEpisodeCastDataModel.ProfilePath;
        this.Character = tvEpisodeCastDataModel.Character;
        this.BilledOrder = tvEpisodeCastDataModel.Order;
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
