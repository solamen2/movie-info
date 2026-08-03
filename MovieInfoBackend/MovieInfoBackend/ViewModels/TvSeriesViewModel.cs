using System.Collections.Frozen;
using System.Globalization;
using MovieInfoBackend.DataModels;
using static MovieInfoBackend.DataModels.TmdbConfigurationCountriesResponseDataModel;
using static MovieInfoBackend.DataModels.TmdbConfigurationLanguagesResponseDataModel;

namespace MovieInfoBackend.ViewModels;

public record TvSeriesViewModel
{
    // TODO: Don't forget to only download all episodes when searching for an episode title in a TV show
    public TvSeriesViewModel(SuggestionViewModel suggestionViewModel, 
                             OmdbResponseDataModel omdbDataModel,
                             TmdbTvSeriesResponseDataModel tmdbTvSeriesDataModel, 
                             TmdbTvSeriesAggregateCreditsResponseDataModel tmdbTvSeriesAggregateCreditsDataModel,
                             TmdbWatchProvidersResponseDataModel tmdbWatchProvidersDataModel,
                             ConfigurationCountriesDictionary configurationCountriesDictionary,
                             ConfigurationLanguagesDictionary configurationLanguagesDictionary,
                             Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
        this.Image = suggestionViewModel.Image;
        this.ImdbId = suggestionViewModel.ItemID;
        this.Name = suggestionViewModel.Name;
        this.ImdbRank = suggestionViewModel.Rank;
        this.KnownForActors = suggestionViewModel.KnownFor;
        this.FirstYear = suggestionViewModel.Year;
        this.Years = suggestionViewModel.Years;
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
        this.OmdbOverview = omdbDataModel.Plot;
        this.Awards = omdbDataModel.Awards;
        this.ImdbRating = omdbDataModel.ImdbRating;
        this.ImdbVotes = omdbDataModel.ImdbVotes;
        this.BackdropPath = tmdbTvSeriesDataModel.BackdropPath;
        this.Creators = tmdbTvSeriesDataModel.CreatedBy
                            .Select(tcdm => new TmdbCreatorViewModel(tcdm, testGuid))
                            .ToList();
        this.TmdbEpisodeRunTimes = tmdbTvSeriesDataModel.EpisodeRunTime.ToList();
        this.FirstAirDateString = tmdbTvSeriesDataModel.FirstAirDate;
        if (String.IsNullOrWhiteSpace(FirstAirDateString) || this.FirstAirDateString == "N/A")
        {
            this.FirstAirDate = null;
        }
        else
        {
            this.FirstAirDate = DateOnly.ParseExact(FirstAirDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        this.TmdbGenres = String.Join(", ", tmdbTvSeriesDataModel.Genres.Select(tgdm => tgdm.Name));
        
        this.Homepage = tmdbTvSeriesDataModel.Homepage;
        this.TmdbId = tmdbTvSeriesDataModel.Id;
        this.IsInProduction = tmdbTvSeriesDataModel.InProduction;
        FrozenDictionary<string, string>? iso6391ToEnglishLanguageNameDictionary = configurationLanguagesDictionary.iso6391ToEnglishLanguageNameDictionary;
        if (iso6391ToEnglishLanguageNameDictionary == null)
        {            
            this.Languages = "[]";
        }
        else
        {
            this.Languages = String.Join(", ", tmdbTvSeriesDataModel.Languages.Select(l => iso6391ToEnglishLanguageNameDictionary[l]));
        }
        this.LastAirDateString = tmdbTvSeriesDataModel.LastAirDate;
        if (String.IsNullOrWhiteSpace(LastAirDateString) || this.LastAirDateString == "N/A")
        {
            this.LastAirDate = null;
        }
        else
        {
            this.LastAirDate = DateOnly.ParseExact(LastAirDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        this.NextAirDateString = tmdbTvSeriesDataModel.NextEpisodeToAir?.AirDate;
        if (String.IsNullOrWhiteSpace(NextAirDateString) || this.NextAirDateString == "N/A")
        {
            this.NextAirDate = null;
        }
        else
        {
            this.NextAirDate = DateOnly.ParseExact(NextAirDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        this.Networks = tmdbTvSeriesDataModel.Networks
                            .Select(tndm => new NetworkViewModel(tndm, testGuid))
                            .ToList();
        this.NumberOfEpisodes = tmdbTvSeriesDataModel.NumberOfEpisodes;
        this.NumberOfSeasons = tmdbTvSeriesDataModel.NumberOfSeasons;
        this.OriginCountries = "";
        FrozenDictionary<string, string>? iso31661ToEnglishCountryNameDictionary = configurationCountriesDictionary.iso31661ToEnglishCountryNameDictionary;
        if (iso31661ToEnglishCountryNameDictionary != null)
        {            
            this.OriginCountries = String.Join(", ", tmdbTvSeriesDataModel.OriginCountry.Select(oc => iso31661ToEnglishCountryNameDictionary[oc]));
        }
        this.OriginLanguage = "";
        if (iso6391ToEnglishLanguageNameDictionary != null)
        {            
            string iso6391LanguageCode = tmdbTvSeriesDataModel.OriginalLanguage;
            this.OriginLanguage = iso6391ToEnglishLanguageNameDictionary[iso6391LanguageCode];
        }
        this.OriginalName = tmdbTvSeriesDataModel.OriginalName;
        this.TmdbOverview = tmdbTvSeriesDataModel.Overview;
        this.ProductionCompanies = String.Join("; ", tmdbTvSeriesDataModel.ProductionCompanies.Select(tpcdm => tpcdm.Name));
        this.ProductionCountries = String.Join(", ", tmdbTvSeriesDataModel.ProductionCountries.Select(tpcdm => tpcdm.Name));
        this.Seasons = tmdbTvSeriesDataModel.Seasons
                            .Select(ttsdm => new TvSeriesSeasonViewModel(ttsdm, testGuid))
                            .ToList();
        this.SpokenLanguages = String.Join(", ", tmdbTvSeriesDataModel.SpokenLanguages.Select(tsldm => tsldm.EnglishName));
        this.Status = tmdbTvSeriesDataModel.Status;
        this.Tagline = tmdbTvSeriesDataModel.Tagline;
        this.TvSeriesType = tmdbTvSeriesDataModel.Type;
        this.Cast = tmdbTvSeriesAggregateCreditsDataModel.Cast
                        .Select(ttsacdm => new TvSeriesCastViewModel(ttsacdm, testGuid))
                        .ToList();
        this.Directors = tmdbTvSeriesAggregateCreditsDataModel.Crew
                            .Where(ttsacdm => ttsacdm.Jobs.Any(j => j.Job == "Director"))
                            .Select(ttsacdm => new TvSeriesCrewViewModel(ttsacdm, testGuid))
                            .ToList();
        this.Writers = tmdbTvSeriesAggregateCreditsDataModel.Crew
                            .Where(ttsacdm => ttsacdm.Department == "Writing")
                            .Select(ttsacdm => new TvSeriesCrewViewModel(ttsacdm, testGuid))
                            .ToList();
        this.Producers = tmdbTvSeriesAggregateCreditsDataModel.Crew
                            .Where(ttsacdm => ttsacdm.Jobs.Any(j => j.Job.EndsWith("Producer")))
                            .Select(ttsacdm => new TvSeriesCrewViewModel(ttsacdm, testGuid))
                            .ToList();
        TmdbWatchProviderCountryDataModel? tmdbWatchProviderCountryDataModel = tmdbWatchProvidersDataModel?.Results?.US;
        if (tmdbWatchProviderCountryDataModel?.Buy != null)
        {
            this.WatchProvidersBuy = tmdbWatchProviderCountryDataModel.Buy
                                        .Select(twpdm => new WatchProviderViewModel(twpdm, testGuid))
                                        .ToList();
        }
        else
        {
            this.WatchProvidersBuy = new List<WatchProviderViewModel>();
        }
        if (tmdbWatchProviderCountryDataModel?.Flatrate != null)
        {
            this.WatchProvidersFlatrate = tmdbWatchProviderCountryDataModel.Flatrate
                                        .Select(twpdm => new WatchProviderViewModel(twpdm, testGuid))
                                        .ToList();
        }
        else
        {
            this.WatchProvidersFlatrate = new List<WatchProviderViewModel>();
        }
        if (tmdbWatchProviderCountryDataModel?.Rent != null)
        {
            this.WatchProvidersRent = tmdbWatchProviderCountryDataModel.Rent
                                        .Select(twpdm => new WatchProviderViewModel(twpdm, testGuid))
                                        .ToList();
        }
        else
        {
            this.WatchProvidersRent = new List<WatchProviderViewModel>();
        }
    }

    public Guid ID { get; }
    public SuggestionImageViewModel? Image { get; }
    public string ImdbId { get; }
    public string Name { get; }
    public int? ImdbRank { get; }
    public string KnownForActors { get; }
    public int? FirstYear { get; }
    public string? Years { get; }
    public string Rated { get; }
    public string OmdbAverageEpisodeRuntimeString { get; }
    public int OmdbAverageEpisodeRuntimeNumber { get; }
    public string OmdbGenres { get; }
    public string OmdbOverview { get; }
    public string Awards { get; }
    public string ImdbRating { get; }
    public string ImdbVotes { get; }
    public string BackdropPath { get; }
    public List<TmdbCreatorViewModel> Creators { get; }
    public List<int> TmdbEpisodeRunTimes { get; }
    public string? FirstAirDateString { get; }
    public DateOnly? FirstAirDate { get; }
    public string TmdbGenres { get; }
    public string? Homepage { get; }
    public int TmdbId { get; }
    public bool IsInProduction { get; }
    public string Languages { get; }
    public string? LastAirDateString { get; }
    public DateOnly? LastAirDate { get; }
    public string? NextAirDateString { get; }
    public DateOnly? NextAirDate { get; }
    public List<NetworkViewModel> Networks { get; }
    public int NumberOfEpisodes { get; }
    public int NumberOfSeasons { get; }
    public string OriginCountries { get; }
    public string OriginLanguage { get; }
    public string OriginalName { get; }
    public string TmdbOverview { get; }
    public string ProductionCompanies { get; }
    public string ProductionCountries { get; }
    public List<TvSeriesSeasonViewModel> Seasons { get; }
    public string SpokenLanguages { get; }
    public string Status { get; }
    public string Tagline { get; }
    public string TvSeriesType { get; }
    public List<TvSeriesCastViewModel> Cast { get; }
    public List<TvSeriesCrewViewModel> Directors { get; }
    public List<TvSeriesCrewViewModel> Writers { get; }
    public List<TvSeriesCrewViewModel> Producers { get; }
    public List<WatchProviderViewModel> WatchProvidersBuy { get; }
    public List<WatchProviderViewModel> WatchProvidersFlatrate { get; }
    public List<WatchProviderViewModel> WatchProvidersRent { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nImage:\n*****\n{Image}\n*****\nImdbId: {ImdbId}\nName: {Name}\nImdbRank: {ImdbRank}\nKnownForActors: {KnownForActors}\nFirstYear: {FirstYear}\nYears: {Years}\nRated: {Rated}\nOmdbAverageEpisodeRuntimeString: {OmdbAverageEpisodeRuntimeString}\nOmdbAverageEpisodeRuntimeNumber: {OmdbAverageEpisodeRuntimeNumber}\nOmdbGenres: {OmdbGenres}\nOmdbOverview: {OmdbOverview}\nAwards: {Awards}\nImdbRating: {ImdbRating}\nImdbVotes: {ImdbVotes}\nBackdropPath: {BackdropPath}\nCreators:\n*****\n{string.Join("\n\n", Creators)}\n*****\nTmdbEpisodeRunTimes: {string.Join(", ", TmdbEpisodeRunTimes)}\nFirstAirDateString: {FirstAirDateString}\nFirstAirDate: {FirstAirDate}\nTmdbGenres: {TmdbGenres}\nHomepage: {Homepage}\nTmdbId: {TmdbId}\nIsInProduction: {IsInProduction}\nLanguages: {string.Join("\n\n", Languages)}\nLastAirDateString: {LastAirDateString}\nLastAirDate: {LastAirDate}\nNextAirDateString: {NextAirDateString}\nNextAirDate: {NextAirDate}\nNetworks:\n*****\n{string.Join("\n\n", Networks)}\n*****\nNumberOfEpisodes: {NumberOfEpisodes}\nNumberOfSeasons: {NumberOfSeasons}\nOriginCountries: {OriginCountries}\nOriginLanguage: {OriginLanguage}\nOriginalName: {OriginalName}\nTmdbOverview: {TmdbOverview}\nProductionCompanies: {ProductionCompanies}\nProductionCountries: {ProductionCountries}\nSeasons:\n*****\n{string.Join("\n\n", Seasons)}\n*****\nSpokenLanguages: {SpokenLanguages}\nStatus: {Status}\nTagline: {Tagline}\nTvSeriesType: {TvSeriesType}\nCast:\n*****\n{string.Join("\n\n", Cast)}\n*****\nDirectors:\n*****\n{string.Join("\n\n", Directors)}\n*****\nWriters:\n*****\n{string.Join("\n\n", Writers)}\n*****\nProducers:\n*****\n{string.Join("\n\n", Producers)}\n*****\nWatchProvidersBuy:\n*****\n{string.Join("\n\n", WatchProvidersBuy)}\n*****\nWatchProvidersFlatrate:\n*****\n{string.Join("\n\n", WatchProvidersFlatrate)}\n*****\nWatchProvidersRent:\n*****\n{string.Join("\n\n", WatchProvidersRent)}";
    }
}
