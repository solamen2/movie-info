using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TmdbCrewViewModel
{
    public TmdbCrewViewModel(TmdbCrewDataModel crewDataModel)
    {
        this.ID = Guid.NewGuid();
        this.TmdbId = crewDataModel.Id;
        this.Gender = (TmdbGenderType)crewDataModel.Gender;
        this.Name = crewDataModel.Name;
        this.OriginalName = crewDataModel.OriginalName;
        this.ProfilePath = crewDataModel.ProfilePath;
        this.Department = crewDataModel.Department;
        this.Job = crewDataModel.Job;
    }

    public Guid ID { get; init; }
    public int TmdbId { get; init; }
    public TmdbGenderType Gender { get; init; }
    public string Name { get; init; }
    public string OriginalName { get; init; }
    public string? ProfilePath { get; init; }
    public string Department { get; init; }
    public string Job { get; init; }

    public override string ToString()
    {
        return $"ID: {ID}\nTmdbId: {TmdbId}\nGender: {Gender}\nName: {Name}\nOriginalName: {OriginalName}\nProfilePath: {ProfilePath}\nDepartment: {Department}\nJob: {Job}";
    }
}
