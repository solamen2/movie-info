using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TvSeasonEpisodeGuestStarViewModel
{    
    public TvSeasonEpisodeGuestStarViewModel(TmdbTvSeasonEpisodeGuestStarDataModel tmdbTvSeasonEpisodeGuestStarDataModel,
                                             Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
        this.Character = tmdbTvSeasonEpisodeGuestStarDataModel.Character;
        this.BilledOrder = tmdbTvSeasonEpisodeGuestStarDataModel.Order;
        this.Gender = (TmdbGenderType)tmdbTvSeasonEpisodeGuestStarDataModel.Gender;
        this.TmdbId = tmdbTvSeasonEpisodeGuestStarDataModel.Id;
        this.Name = tmdbTvSeasonEpisodeGuestStarDataModel.Name;
        this.OriginalName = tmdbTvSeasonEpisodeGuestStarDataModel.OriginalName;
        this.Popularity = tmdbTvSeasonEpisodeGuestStarDataModel.Popularity;
        this.ProfilePath = tmdbTvSeasonEpisodeGuestStarDataModel.ProfilePath;
    }

    public Guid ID { get; }
    public string Character { get; }
    public int BilledOrder { get; }
    public TmdbGenderType Gender { get; }
    public int TmdbId { get; }
    public string Name { get; }
    public string OriginalName { get; }
    public double Popularity { get; }
    public string? ProfilePath { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nCharacter: {Character}\nBilledOrder: {BilledOrder}\nGender: {Gender}\nTmdbId: {TmdbId}\nName: {Name}\nOriginalName: {OriginalName}\nPopularity: {Popularity}\nProfilePath: {ProfilePath}";
    }
}
