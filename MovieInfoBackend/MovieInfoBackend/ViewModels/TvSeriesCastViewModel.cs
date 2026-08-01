using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TvSeriesCastViewModel
{    
    public TvSeriesCastViewModel(TmdbTvSeriesAggregateCastDataModel tmdbTvSeriesAggregateCastDataModel,
                                 Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
        this.Gender = (TmdbGenderType)tmdbTvSeriesAggregateCastDataModel.Gender;
        this.TmdbId = tmdbTvSeriesAggregateCastDataModel.Id;
        this.Name = tmdbTvSeriesAggregateCastDataModel.Name;
        this.OriginalName = tmdbTvSeriesAggregateCastDataModel.OriginalName;
        this.Popularity = tmdbTvSeriesAggregateCastDataModel.Popularity;
        this.ProfilePath = tmdbTvSeriesAggregateCastDataModel.ProfilePath;
        this.Characters = tmdbTvSeriesAggregateCastDataModel.Roles.Select(ttsacrdm => ttsacrdm.Character).ToList();
        this.TotalEpisodeCount = tmdbTvSeriesAggregateCastDataModel.TotalEpisodeCount;
        this.BilledOrder = tmdbTvSeriesAggregateCastDataModel.Order;
    }

    public Guid ID { get; }
    public TmdbGenderType Gender { get; }
    public int TmdbId { get; }
    public string Name { get; }
    public string OriginalName { get; }
    public double Popularity { get; }
    public string? ProfilePath { get; }
    public List<string> Characters { get; }
    public int TotalEpisodeCount { get; }
    public int BilledOrder { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nGender: {Gender}\nTmdbId: {TmdbId}\nName: {Name}\nOriginalName: {OriginalName}\nPopularity: {Popularity}\nProfilePath: {ProfilePath}\nCharacters: {string.Join(", ", Characters)}\nTotalEpisodeCount: {TotalEpisodeCount}\nBilledOrder: {BilledOrder}";
    }
}
