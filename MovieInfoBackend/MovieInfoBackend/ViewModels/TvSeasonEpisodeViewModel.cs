using System.Globalization;
using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TvSeasonEpisodeViewModel
{
    // NOTE: This exists at the season level; TvEpisodeViewModel is shown after selecting a single episode
    public TvSeasonEpisodeViewModel(TmdbTvSeasonEpisodeDataModel tmdbTvSeasonEpisodeDataModel,
                                    Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
        this.AirDateString = tmdbTvSeasonEpisodeDataModel.AirDate;
        if (String.IsNullOrWhiteSpace(AirDateString) || this.AirDateString == "N/A")
        {
            this.AirDate = null;
        }
        else
        {
            this.AirDate = DateOnly.ParseExact(AirDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        this.EpisodeNumber = tmdbTvSeasonEpisodeDataModel.EpisodeNumber;
        this.EpisodeType = tmdbTvSeasonEpisodeDataModel.EpisodeType;
        this.TmdbId = tmdbTvSeasonEpisodeDataModel.Id;
        this.Title = tmdbTvSeasonEpisodeDataModel.Name;
        this.TmdbOverview = tmdbTvSeasonEpisodeDataModel.Overview;
        this.Runtime = tmdbTvSeasonEpisodeDataModel.Runtime;
        this.SeasonNumber = tmdbTvSeasonEpisodeDataModel.SeasonNumber;
        this.TmdbTvSeriesId = tmdbTvSeasonEpisodeDataModel.ShowId;
        this.StillPath = tmdbTvSeasonEpisodeDataModel.StillPath;
        this.Directors = tmdbTvSeasonEpisodeDataModel.Crew
                            .Where(ttscdm => ttscdm.Job == "Director")
                            .Select(ttscdm => new TvSeasonEpisodeCrewViewModel(ttscdm, testGuid))
                            .ToList();
        this.Writers = tmdbTvSeasonEpisodeDataModel.Crew
                            .Where(ttscdm => ttscdm.Department == "Writing")
                            .Select(ttscdm => new TvSeasonEpisodeCrewViewModel(ttscdm, testGuid))
                            .ToList();
        this.Producers = tmdbTvSeasonEpisodeDataModel.Crew
                            .Where(ttscdm => ttscdm.Job.EndsWith("Producer"))
                            .Select(ttscdm => new TvSeasonEpisodeCrewViewModel(ttscdm, testGuid))
                            .ToList();
        this.GuestStars = tmdbTvSeasonEpisodeDataModel.GuestStars
                            .Select(ttsegsdm => new TvSeasonEpisodeGuestStarViewModel(ttsegsdm, testGuid))
                            .ToList();
    }

    public Guid ID { get; }
    public string? AirDateString { get; }
    public DateOnly? AirDate { get; }
    public int EpisodeNumber { get; }
    public string EpisodeType { get; }
    public int TmdbId { get; }
    public string Title { get; } 
    public string TmdbOverview { get; }
    public int Runtime { get; }
    public int SeasonNumber { get; }
    public int TmdbTvSeriesId { get; }
    public string StillPath { get; }
    // NOTE: Yes, there is no Cast here, because the season HTTP request does not provide it. The episode must be selected, triggering another call to the TMDB, to get it
    public List<TvSeasonEpisodeCrewViewModel> Directors { get; }
    public List<TvSeasonEpisodeCrewViewModel> Writers { get; }
    public List<TvSeasonEpisodeCrewViewModel> Producers { get; }
    public List<TvSeasonEpisodeGuestStarViewModel> GuestStars { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nAirDateString: {AirDateString}\nAirDate: {AirDate:MM/dd/yyy}\nEpisodeNumber: {EpisodeNumber}\nEpisodeType: {EpisodeType}\nTmdbId: {TmdbId}\nTitle: {Title}\nTmdbOverview: {TmdbOverview}\nRuntime: {Runtime}\nSeasonNumber: {SeasonNumber}\nTmdbTvSeriesId: {TmdbTvSeriesId}\nStillPath: {StillPath}\nDirectors:\n*****\n{string.Join("\n\n", Directors)}\n*****\nWriters:\n*****\n{string.Join("\n\n", Writers)}\n*****\nProducers:\n*****\n{string.Join("\n\n", Producers)}\n*****\nGuestStars:\n*****\n{string.Join("\n\n", GuestStars)}";
    }
}
