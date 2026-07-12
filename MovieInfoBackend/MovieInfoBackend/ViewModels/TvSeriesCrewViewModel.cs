using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TvSeriesCrewViewModel
{
    public TvSeriesCrewViewModel(TmdbTvSeriesAggregateCrewDataModel tmdbTvSeriesAggregateCrewDataModel)
    {
        this.ID = Guid.NewGuid();
        this.Gender = (TmdbGenderType)tmdbTvSeriesAggregateCrewDataModel.Gender;
        this.TmdbId = tmdbTvSeriesAggregateCrewDataModel.Id;
        this.KnownForDepartment = tmdbTvSeriesAggregateCrewDataModel.KnownForDepartment;
        this.Name = tmdbTvSeriesAggregateCrewDataModel.Name;
        this.OriginalName = tmdbTvSeriesAggregateCrewDataModel.OriginalName;
        this.Popularity = tmdbTvSeriesAggregateCrewDataModel.Popularity;
        this.ProfilePath = tmdbTvSeriesAggregateCrewDataModel.ProfilePath;
        this.Jobs = tmdbTvSeriesAggregateCrewDataModel.Jobs.Select(ttsacjdm => ttsacjdm.Job).ToList();
        this.Department = tmdbTvSeriesAggregateCrewDataModel.Department;
        this.TotalEpisodeCount = tmdbTvSeriesAggregateCrewDataModel.TotalEpisodeCount;
    }

    public Guid ID { get; }
    public TmdbGenderType Gender { get; }
    public int TmdbId { get; }
    public string KnownForDepartment { get; }
    public string Name { get; }
    public string OriginalName { get; }
    public double Popularity { get; }
    public string? ProfilePath { get; }
    public List<string> Jobs { get; }
    public string Department { get; }
    public int TotalEpisodeCount { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nGender: {Gender}\nTmdbId: {TmdbId}\nKnownForDepartment: {KnownForDepartment}\nName: {Name}\nOriginalName: {OriginalName}\nPopularity: {Popularity}\nProfilePath: {ProfilePath}\nJobs:{string.Join(",", Jobs)}\nDepartment: {Department}\nTotalEpisodeCount: {TotalEpisodeCount}";
    }
}
