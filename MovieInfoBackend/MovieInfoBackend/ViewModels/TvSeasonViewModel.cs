using System.Globalization;
using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TvSeasonViewModel
{
    public TvSeasonViewModel(TmdbTvSeasonResponseDataModel tmdbTvSeasonDataModel, 
                             TmdbWatchProvidersResponseDataModel tmdbWatchProvidersDataModel,
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
        this.Episodes = tmdbTvSeasonDataModel.Episodes
                            .Select(ttsedm => new TvSeasonEpisodeViewModel(ttsedm))
                            .ToList();
        this.Name = tmdbTvSeasonDataModel.Name;
        this.Networks = tmdbTvSeasonDataModel.Networks
                            .Select(tndm => new NetworkViewModel(tndm))
                            .ToList();
        this.TmdbOverview = tmdbTvSeasonDataModel.Overview;
        this.TmdbId = tmdbTvSeasonDataModel.Id2;
        this.PosterPath = tmdbTvSeasonDataModel.PosterPath;
        this.SeasonNumber = tmdbTvSeasonDataModel.SeasonNumber;
        TmdbWatchProviderCountryDataModel? tmdbWatchProviderCountryDataModel = tmdbWatchProvidersDataModel?.Results?.US;
        if (tmdbWatchProviderCountryDataModel?.Buy != null)
        {
            this.WatchProvidersBuy = tmdbWatchProviderCountryDataModel.Buy
                                        .Select(twpdm => new WatchProviderViewModel(twpdm))
                                        .ToList();
        }
        else
        {
            this.WatchProvidersBuy = new List<WatchProviderViewModel>();
        }
        if (tmdbWatchProviderCountryDataModel?.Flatrate != null)
        {
            this.WatchProvidersFlatrate = tmdbWatchProviderCountryDataModel.Flatrate
                                        .Select(twpdm => new WatchProviderViewModel(twpdm))
                                        .ToList();
        }
        else
        {
            this.WatchProvidersFlatrate = new List<WatchProviderViewModel>();
        }
        if (tmdbWatchProviderCountryDataModel?.Rent != null)
        {
            this.WatchProvidersRent = tmdbWatchProviderCountryDataModel.Rent
                                        .Select(twpdm => new WatchProviderViewModel(twpdm))
                                        .ToList();
        }
        else
        {
            this.WatchProvidersRent = new List<WatchProviderViewModel>();
        }
        this.NumberOfEpisodes = this.Episodes.Count;
    }

    public Guid ID { get; }
    public string? FirstAirDateString { get; }
    public DateOnly? FirstAirDate { get; }
    public List<TvSeasonEpisodeViewModel> Episodes { get; }
    public string Name { get; }
    public List<NetworkViewModel> Networks { get; }
    public string TmdbOverview { get; }
    public int TmdbId { get; }
    public string PosterPath { get; }
    public int SeasonNumber { get; }
    public List<WatchProviderViewModel> WatchProvidersBuy { get; }
    public List<WatchProviderViewModel> WatchProvidersFlatrate { get; }
    public List<WatchProviderViewModel> WatchProvidersRent { get; }
    public int NumberOfEpisodes { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nFirstAirDateString: {FirstAirDateString}\nFirstAirDate: {FirstAirDate}\nEpisodes:\n*****\n{string.Join("\n\n", Episodes)}\n*****\nName: {Name}\nNetworks:\n*****\n{string.Join("\n\n", Networks)}\n*****\nTmdbOverview: {TmdbOverview}\nTmdbId: {TmdbId}\nPosterPath: {PosterPath}\nSeasonNumber: {SeasonNumber}\nWatchProvidersBuy:\n*****\n{string.Join("\n\n", WatchProvidersBuy)}\n*****\nWatchProvidersFlatrate:\n*****\n{string.Join("\n\n", WatchProvidersFlatrate)}\n*****\nWatchProvidersRent:\n*****\n{string.Join("\n\n", WatchProvidersRent)}\n*****\nNumberOfEpisodes: {NumberOfEpisodes}";
    }
}
