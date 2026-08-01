namespace MovieInfoBackend.DataModels;

public record WatchProviderViewModel
{
    public WatchProviderViewModel(TmdbWatchProviderDataModel tmdbWatchProviderDataModel,
                                  Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
        this.LogoPath = tmdbWatchProviderDataModel.LogoPath;
        this.ProviderName = tmdbWatchProviderDataModel.ProviderName;
        this.DisplayPriority = tmdbWatchProviderDataModel.DisplayPriority;
    }
    
    public Guid ID { get; }
    public string? LogoPath { get; }
    public string ProviderName { get; }
    public int DisplayPriority { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nLogoPath: {LogoPath}\nProviderName: {ProviderName}\nDisplayPriority: {DisplayPriority}";
    }

}
