using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TvEpisodeCastViewModel
{    
    public TvEpisodeCastViewModel(TmdbTvEpisodeCreditsCastDataModel tmdbTvEpisodeCreditsCastDataModel,
                                  Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
        this.Gender = (TmdbGenderType)tmdbTvEpisodeCreditsCastDataModel.Gender;
        this.TmdbId = tmdbTvEpisodeCreditsCastDataModel.Id;
        this.Name = tmdbTvEpisodeCreditsCastDataModel.Name;
        this.OriginalName = tmdbTvEpisodeCreditsCastDataModel.OriginalName;
        this.Popularity = tmdbTvEpisodeCreditsCastDataModel.Popularity;
        this.ProfilePath = tmdbTvEpisodeCreditsCastDataModel.ProfilePath;
        this.Character = tmdbTvEpisodeCreditsCastDataModel.Character;
        this.BilledOrder = tmdbTvEpisodeCreditsCastDataModel.Order;
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
