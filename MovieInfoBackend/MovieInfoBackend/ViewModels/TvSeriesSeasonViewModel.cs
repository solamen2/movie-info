using System.Globalization;

namespace MovieInfoBackend.DataModels;

public record TvSeriesSeasonViewModel
{
    public TvSeriesSeasonViewModel(TmdbTvSeasonDataModel tmdbTvSeasonDataModel,
                                   Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
        this.FirstAirDateString = tmdbTvSeasonDataModel.AirDate;
        if (String.IsNullOrWhiteSpace(FirstAirDateString) || this.FirstAirDateString == "N/A")
        {
            this.FirstAirDate = null;
        }
        else
        {
            this.FirstAirDate = DateOnly.ParseExact(FirstAirDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        this.EpisodeCount = tmdbTvSeasonDataModel.EpisodeCount;
        this.TmdbId = tmdbTvSeasonDataModel.Id;
        this.Name = tmdbTvSeasonDataModel.Name;
        this.Overview = tmdbTvSeasonDataModel.Overview;
        this.PosterPath = tmdbTvSeasonDataModel.PosterPath;
        this.SeasonNumber = tmdbTvSeasonDataModel.SeasonNumber;
    }
    
    public Guid ID { get; }
    public string? FirstAirDateString { get; }
    public DateOnly? FirstAirDate { get; }
    public int EpisodeCount { get; }
    public int TmdbId { get; }
    public string Name { get; }
    public string Overview { get; }
    public string PosterPath { get; }
    public int SeasonNumber { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nFirstAirDateString: {FirstAirDateString}\nFirstAirDate: {FirstAirDate}\nEpisodeCount: {EpisodeCount}\nTmdbId: {TmdbId}\nName: {Name}\nOverview: {Overview}\nPosterPath: {PosterPath}\nSeasonNumber: {SeasonNumber}";
    }

}