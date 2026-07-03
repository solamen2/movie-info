using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record PersonProfileImageViewModel
{
    public PersonProfileImageViewModel(TmdbProfileDataModel tmdbProfileDataModel)
    {
        this.ID = Guid.NewGuid();
        this.Height = tmdbProfileDataModel.Height;
        this.FilePath = tmdbProfileDataModel.FilePath;
        this.Width = tmdbProfileDataModel.Width;
    }

    public Guid ID { get; init; }
    public int Height { get; init; }
    public string FilePath { get; init;}
    public int Width { get; init; }

    public override string ToString()
    {
        return $"ID: {ID}\nHeight: {Height}\nFilePath: {FilePath}\nWidth: {Width}";
    }
}
