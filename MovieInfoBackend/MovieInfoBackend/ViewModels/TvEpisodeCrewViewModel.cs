using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TvEpisodeCrewViewModel
{
    public TvEpisodeCrewViewModel(TmdbTvEpisodeCrewDataModel tvEpisodeCrewDataModel)
    {
        this.ID = Guid.NewGuid();
        this.Gender = (TmdbGenderType)tvEpisodeCrewDataModel.Gender;
        this.TmdbId = tvEpisodeCrewDataModel.Id;
        this.Name = tvEpisodeCrewDataModel.Name;
        this.OriginalName = tvEpisodeCrewDataModel.OriginalName;
        this.Popularity = tvEpisodeCrewDataModel.Popularity;
        this.ProfilePath = tvEpisodeCrewDataModel.ProfilePath;
        this.Department = tvEpisodeCrewDataModel.Department;
        this.Job = tvEpisodeCrewDataModel.Job;
    }

    public Guid ID { get; }
    public TmdbGenderType Gender { get; }
    public int TmdbId { get; }
    public string Name { get; }
    public string OriginalName { get; }
    public double Popularity { get; }
    public string? ProfilePath { get; }
    public string Department { get; }
    public string Job { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nGender: {Gender}\nTmdbId: {TmdbId}\nName: {Name}\nOriginalName: {OriginalName}\nPopularity: {Popularity}\nProfilePath: {ProfilePath}\nDepartment: {Department}\nJob: {Job}";
    }
}
