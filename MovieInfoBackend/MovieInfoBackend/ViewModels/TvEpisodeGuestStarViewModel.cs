using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TvEpisodeGuestStarViewModel
{    
    public TvEpisodeGuestStarViewModel(TmdbTvEpisodeCreditsGuestStarDataModel tmdbTvEpisodeCreditsGuestStarDataModel,
                                       Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
        this.Character = tmdbTvEpisodeCreditsGuestStarDataModel.Character;
        this.BilledOrder = tmdbTvEpisodeCreditsGuestStarDataModel.Order;
        this.Gender = (TmdbGenderType)tmdbTvEpisodeCreditsGuestStarDataModel.Gender;
        this.TmdbId = tmdbTvEpisodeCreditsGuestStarDataModel.Id;
        this.Name = tmdbTvEpisodeCreditsGuestStarDataModel.Name;
        this.OriginalName = tmdbTvEpisodeCreditsGuestStarDataModel.OriginalName;
        this.Popularity = tmdbTvEpisodeCreditsGuestStarDataModel.Popularity;
        this.ProfilePath = tmdbTvEpisodeCreditsGuestStarDataModel.ProfilePath;
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
