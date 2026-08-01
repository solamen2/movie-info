using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record PersonProfileImageViewModel
{
    public PersonProfileImageViewModel(TmdbProfileDataModel tmdbProfileDataModel,
                                       Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
        this.Height = tmdbProfileDataModel.Height;
        this.FilePath = tmdbProfileDataModel.FilePath;
        this.Width = tmdbProfileDataModel.Width;
    }

    public Guid ID { get; }
    public int Height { get; }
    public string FilePath { get; }
    public int Width { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nHeight: {Height}\nFilePath: {FilePath}\nWidth: {Width}";
    }
}
