using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record MovieCrewViewModel
{
    public MovieCrewViewModel(TmdbMovieCrewDataModel movieCrewDataModel)
    {
        this.ID = Guid.NewGuid();
        this.Gender = (TmdbGenderType)movieCrewDataModel.Gender;
        this.TmdbId = movieCrewDataModel.Id;
        this.Name = movieCrewDataModel.Name;
        this.OriginalName = movieCrewDataModel.OriginalName;
        this.Popularity = movieCrewDataModel.Popularity;
        this.ProfilePath = movieCrewDataModel.ProfilePath;
        this.Department = movieCrewDataModel.Department;
        this.Job = movieCrewDataModel.Job;
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
