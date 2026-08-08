using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record NetworkViewModel
{
    public NetworkViewModel(TmdbNetworkDataModel tmdbNetworkDataModel,
                            Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
        this.TmdbId = tmdbNetworkDataModel.Id;
        this.LogoPath = tmdbNetworkDataModel.LogoPath;
        this.Name = tmdbNetworkDataModel.Name;
        this.OriginCountry = tmdbNetworkDataModel.OriginCountry;
    }
    
    public Guid ID { get; }    [JsonPropertyName("id")]
    public int TmdbId { get; }
    public string? LogoPath { get; }
    public string Name { get; }
    public string OriginCountry { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nTmdbId: {TmdbId}\nLogoPath: {LogoPath}\nName: {Name}\nOriginCountry: {OriginCountry}";
    }

}