using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TvSeriesCastViewModel
{    
    public TvSeriesCastViewModel(TmdbTvSeriesAggregateCastDataModel castDataModel)
    {
        this.ID = Guid.NewGuid();
        this.Gender = (TmdbGenderType)castDataModel.Gender;
        this.TmdbId = castDataModel.Id;
        this.Name = castDataModel.Name;
        this.OriginalName = castDataModel.OriginalName;
        this.Popularity = castDataModel.Popularity;
        this.ProfilePath = castDataModel.ProfilePath;
        this.Characters = castDataModel.Roles.Select(ttsacrdm => ttsacrdm.Character).ToList();
        this.TotalEpisodeCount = castDataModel.TotalEpisodeCount;
        this.BilledOrder = castDataModel.Order;
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
        return $"ID: {ID}\nGender: {Gender}\nTmdbId: {TmdbId}\nName: {Name}\nOriginalName: {OriginalName}\nPopularity: {Popularity}\nProfilePath: {ProfilePath}\nCharacters:{string.Join(",", Characters)}\nTotalEpisodeCount: {TotalEpisodeCount}\nBilledOrder: {BilledOrder}";
    }
}
