using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TmdbCreatorViewModel
{
    public TmdbCreatorViewModel(TmdbCreatorDataModel tmdbCreatorDataModel)
    {
        this.ID = Guid.NewGuid();
        this.TmdbId = tmdbCreatorDataModel.Id;
        this.Name = tmdbCreatorDataModel.Name;
        this.OriginalName = tmdbCreatorDataModel.OriginalName;
        this.Gender = (TmdbGenderType)tmdbCreatorDataModel.Gender;
        this.ProfilePath = tmdbCreatorDataModel.ProfilePath;
    }
    
    public Guid ID { get; }
    public int TmdbId { get; }
    public string Name { get; }
    public string OriginalName { get; }
    public TmdbGenderType Gender { get; }
    public string ProfilePath { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nTmdbId: {TmdbId}\nName: {Name}\nOriginalName: {OriginalName}\nGender: {Gender}\nProfilePath: {ProfilePath}";
    }
}
