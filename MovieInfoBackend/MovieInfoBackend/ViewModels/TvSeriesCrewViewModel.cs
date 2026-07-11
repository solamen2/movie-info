using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TvSeriesCrewViewModel
{
    public TvSeriesCrewViewModel(TmdbTvSeriesAggregateCrewDataModel crewDataModel)
    {
        this.ID = Guid.NewGuid();
        this.Gender = (TmdbGenderType)crewDataModel.Gender;
        this.TmdbId = crewDataModel.Id;
        this.Name = crewDataModel.Name;
        this.OriginalName = crewDataModel.OriginalName;
        this.Popularity = crewDataModel.Popularity;
        this.ProfilePath = crewDataModel.ProfilePath;
        this.Jobs = crewDataModel.Jobs.Select(ttsacjdm => ttsacjdm.Job).ToList();
        this.Department = crewDataModel.Department;
        this.TotalEpisodeCount = crewDataModel.TotalEpisodeCount;
    }

    public Guid ID { get; }
    public TmdbGenderType Gender { get; }
    public int TmdbId { get; }
    public string Name { get; }
    public string OriginalName { get; }
    public double Popularity { get; }
    public string? ProfilePath { get; }
    public List<string> Jobs { get; }
    public string Department { get; }
    public int TotalEpisodeCount { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nGender: {Gender}\nTmdbId: {TmdbId}\nName: {Name}\nOriginalName: {OriginalName}\nPopularity: {Popularity}\nProfilePath: {ProfilePath}\nJobs:{string.Join(",", Jobs)}\nDepartment: {Department}\nTotalEpisodeCount: {TotalEpisodeCount}";
    }
}
