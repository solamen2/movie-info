using System.Globalization;
using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TvEpisodeViewModel
{
    // NOTE: This exists at the episode level, after an episode is selected; TvSeasonEpisodeViewModel is shown at the season level, when no episode is selected
    // TODO: No way to get omdbDataModel without knowing the IMDB ID! So there will have to be a call to the TMDB External IDs API to get it in the HTTP Client
    public TvEpisodeViewModel(OmdbResponseDataModel omdbDataModel,
                              TmdbTvEpisodeResponseDataModel tmdbTvEpisodeDataModel,
                              TmdbTvEpisodeCreditsResponseDataModel tmdbTvEpisodeCreditsDataModel,
                              Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
        if (String.IsNullOrWhiteSpace(omdbDataModel.Year) || omdbDataModel.Year == "N/A")
        {
            this.Year = 0;
        }
        else
        {
            this.Year = int.Parse(new string(omdbDataModel.Year));
        }
        this.Rated = omdbDataModel.Rated;
        this.OmdbAverageEpisodeRuntimeString = omdbDataModel.Runtime;
        if (String.IsNullOrWhiteSpace(omdbDataModel.Runtime) || omdbDataModel.Runtime == "N/A")
        {
            this.OmdbAverageEpisodeRuntimeNumber = 0;
        }
        else
        {
            this.OmdbAverageEpisodeRuntimeNumber = int.Parse(new string(omdbDataModel.Runtime.Where(char.IsDigit).ToArray()));
        }
        this.OmdbGenres = omdbDataModel.Genre;
        this.KnownForActors = omdbDataModel.Actors;
        this.OmdbOverview = omdbDataModel.Plot;
        this.Awards = omdbDataModel.Awards;
        this.ImdbRating = omdbDataModel.ImdbRating;
        this.ImdbVotes = omdbDataModel.ImdbVotes;
        this.ImdbId = omdbDataModel.ImdbId;
        this.AirDateString = tmdbTvEpisodeDataModel.AirDate;
        if (String.IsNullOrWhiteSpace(AirDateString) || this.AirDateString == "N/A")
        {
            this.AirDate = null;
        }
        else
        {
            this.AirDate = DateOnly.ParseExact(AirDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        this.EpisodeNumber = tmdbTvEpisodeDataModel.EpisodeNumber;
        this.EpisodeType = tmdbTvEpisodeDataModel.EpisodeType;
        this.Title = tmdbTvEpisodeDataModel.Name;
        this.TmdbOverview = tmdbTvEpisodeDataModel.Overview;
        this.TmdbId = tmdbTvEpisodeDataModel.Id;
        this.Runtime = tmdbTvEpisodeDataModel.Runtime;
        this.SeasonNumber = tmdbTvEpisodeDataModel.SeasonNumber;
        this.StillPath = tmdbTvEpisodeDataModel.StillPath;
        this.Cast = tmdbTvEpisodeCreditsDataModel.Cast
                        .Select(tteccdm => new TvEpisodeCastViewModel(tteccdm, testGuid))
                        .ToList();
        this.Directors = tmdbTvEpisodeCreditsDataModel.Crew
                            .Where(tteccdm => tteccdm.Job == "Director")
                            .Select(tteccdm => new TvEpisodeCrewViewModel(tteccdm, testGuid))
                            .ToList();
        this.Writers = tmdbTvEpisodeCreditsDataModel.Crew
                            .Where(tteccdm => tteccdm.Department == "Writing")
                            .Select(tteccdm => new TvEpisodeCrewViewModel(tteccdm, testGuid))
                            .ToList();
        this.Producers = tmdbTvEpisodeCreditsDataModel.Crew
                            .Where(tteccdm => tteccdm.Job.EndsWith("Producer"))
                            .Select(tteccdm => new TvEpisodeCrewViewModel(tteccdm, testGuid))
                            .ToList();
        this.GuestStars = tmdbTvEpisodeCreditsDataModel.GuestStars
                            .Select(ttecgsdm => new TvEpisodeGuestStarViewModel(ttecgsdm, testGuid))
                            .ToList();
    }

    public Guid ID { get; }
    public int Year { get; }
    public string Rated { get; }
    public string OmdbAverageEpisodeRuntimeString { get; }
    public int OmdbAverageEpisodeRuntimeNumber { get; }
    public string OmdbGenres { get; }
    public string KnownForActors { get; }
    public string OmdbOverview { get; }
    public string Awards { get; }
    public string ImdbRating { get; }
    public string ImdbVotes { get; }
    public string ImdbId { get; }
    public string? AirDateString { get; }
    public DateOnly? AirDate { get; }
    public int EpisodeNumber { get; }
    public string EpisodeType { get; }
    public string Title { get; } 
    public string TmdbOverview { get; }
    public int TmdbId { get; }
    public int Runtime { get; }
    public int SeasonNumber { get; }
    public string StillPath { get; }
    public List<TvEpisodeCastViewModel> Cast { get; }
    public List<TvEpisodeCrewViewModel> Directors { get; }
    public List<TvEpisodeCrewViewModel> Writers { get; }
    public List<TvEpisodeCrewViewModel> Producers { get; }
    public List<TvEpisodeGuestStarViewModel> GuestStars { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nYear: {Year}\nRated: {Rated}\nOmdbAverageEpisodeRuntimeString: {OmdbAverageEpisodeRuntimeString}\nOmdbAverageEpisodeRuntimeNumber: {OmdbAverageEpisodeRuntimeNumber}\nOmdbGenres: {OmdbGenres}\nKnownForActors: {KnownForActors}\nOmdbOverview: {OmdbOverview}\nAwards: {Awards}\nImdbRating: {ImdbRating}\nImdbVotes: {ImdbVotes}\nImdbId: {ImdbId}\nAirDateString: {AirDateString}\nAirDate: {AirDate:MM/dd/yyy}\nEpisodeNumber: {EpisodeNumber}\nEpisodeType: {EpisodeType}\nTitle: {Title}\nTmdbOverview: {TmdbOverview}\nTmdbId: {TmdbId}\nRuntime: {Runtime}\nSeasonNumber: {SeasonNumber}\nStillPath: {StillPath}\nCast:\n*****\n{string.Join("\n\n", Cast)}\n*****\nDirectors:\n*****\n{string.Join("\n\n", Directors)}\n*****\nWriters:\n*****\n{string.Join("\n\n", Writers)}\n*****\nProducers:\n*****\n{string.Join("\n\n", Producers)}\n*****\nGuestStars:\n*****\n{string.Join("\n\n", GuestStars)}";
    }
}
