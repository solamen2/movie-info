using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbGenreViewModel
{
    public TmdbGenreViewModel(TmdbGenreDataModel tmdbGenreDataModel)
    {
        this.ID = Guid.NewGuid();
        this.TmdbId = tmdbGenreDataModel.Id;
        this.Name = tmdbGenreDataModel.Name;
    }
    
    public Guid ID { get; }
    public int TmdbId { get; }
    public string Name { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nTmdbId: {TmdbId}\nName: {Name}";
    }

}