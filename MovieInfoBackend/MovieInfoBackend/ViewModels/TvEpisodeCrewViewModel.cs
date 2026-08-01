using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TvEpisodeCrewViewModel
{
    public TvEpisodeCrewViewModel(TmdbTvEpisodeCreditsCrewDataModel tmdbTvEpisodeCreditsCrewDataModel,
                                  Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
        this.Gender = (TmdbGenderType)tmdbTvEpisodeCreditsCrewDataModel.Gender;
        this.TmdbId = tmdbTvEpisodeCreditsCrewDataModel.Id;
        this.KnownForDepartment = tmdbTvEpisodeCreditsCrewDataModel.KnownForDepartment;
        this.Name = tmdbTvEpisodeCreditsCrewDataModel.Name;
        this.OriginalName = tmdbTvEpisodeCreditsCrewDataModel.OriginalName;
        this.Popularity = tmdbTvEpisodeCreditsCrewDataModel.Popularity;
        this.ProfilePath = tmdbTvEpisodeCreditsCrewDataModel.ProfilePath;
        this.Department = tmdbTvEpisodeCreditsCrewDataModel.Department;
        this.Job = tmdbTvEpisodeCreditsCrewDataModel.Job;
    }

    public Guid ID { get; }
    public TmdbGenderType Gender { get; }
    public int TmdbId { get; }
    public string KnownForDepartment { get; }
    public string Name { get; }
    public string OriginalName { get; }
    public double Popularity { get; }
    public string? ProfilePath { get; }
    public string Department { get; }
    public string Job { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nGender: {Gender}\nTmdbId: {TmdbId}\nKnownForDepartment: {KnownForDepartment}\nName: {Name}\nOriginalName: {OriginalName}\nPopularity: {Popularity}\nProfilePath: {ProfilePath}\nDepartment: {Department}\nJob: {Job}";
    }
}
