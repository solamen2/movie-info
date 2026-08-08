using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TvSeasonEpisodeCrewViewModel
{
    public TvSeasonEpisodeCrewViewModel(TmdbTvSeasonEpisodeCrewDataModel tmdbTvSeasonEpisodeCrewDataModel,
                                        Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
        this.Department = tmdbTvSeasonEpisodeCrewDataModel.Department;
        this.Job = tmdbTvSeasonEpisodeCrewDataModel.Job;
        this.Gender = (TmdbGenderType)tmdbTvSeasonEpisodeCrewDataModel.Gender;
        this.TmdbId = tmdbTvSeasonEpisodeCrewDataModel.Id;
        this.KnownForDepartment = tmdbTvSeasonEpisodeCrewDataModel.KnownForDepartment;
        this.Name = tmdbTvSeasonEpisodeCrewDataModel.Name;
        this.OriginalName = tmdbTvSeasonEpisodeCrewDataModel.OriginalName;
        this.Popularity = tmdbTvSeasonEpisodeCrewDataModel.Popularity;
        this.ProfilePath = tmdbTvSeasonEpisodeCrewDataModel.ProfilePath;
    }

    public Guid ID { get; }
    public string Department { get; }
    public string Job { get; }
    public TmdbGenderType Gender { get; }
    public int TmdbId { get; }
    public string KnownForDepartment { get; }
    public string Name { get; }
    public string OriginalName { get; }
    public double Popularity { get; }
    public string? ProfilePath { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nDepartment: {Department}\nJob: {Job}\nGender: {Gender}\nTmdbId: {TmdbId}\nKnownForDepartment: {KnownForDepartment}\nName: {Name}\nOriginalName: {OriginalName}\nPopularity: {Popularity}\nProfilePath: {ProfilePath}";
    }
}
