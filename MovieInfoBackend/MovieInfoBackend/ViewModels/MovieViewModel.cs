using System.Collections.Frozen;
using System.Globalization;
using MovieInfoBackend.DataModels;
using static MovieInfoBackend.DataModels.TmdbConfigurationCountriesResponseDataModel;
using static MovieInfoBackend.DataModels.TmdbConfigurationLanguagesResponseDataModel;

namespace MovieInfoBackend.ViewModels;

public record MovieViewModel
{
    public MovieViewModel(SuggestionViewModel suggestionViewModel, 
                          OmdbResponseDataModel omdbDataModel,
                          TmdbMovieResponseDataModel tmdbMovieDataModel, 
                          TmdbMovieCreditsResponseDataModel tmdbMovieCreditsDataModel,
                          TmdbWatchProvidersResponseDataModel tmdbWatchProvidersDataModel,
                          ConfigurationCountriesDictionary configurationCountriesDictionary,
                          ConfigurationLanguagesDictionary configurationLanguagesDictionary,
                          Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
        this.Image = suggestionViewModel.Image;
        this.ImdbId = suggestionViewModel.ItemID;
        this.Title = suggestionViewModel.Name;
        this.ImdbRank = suggestionViewModel.Rank;
        this.KnownForActors = suggestionViewModel.KnownFor;
        this.Year = suggestionViewModel.Year;
        this.Rated = omdbDataModel.Rated;
        this.OmdbGenres = omdbDataModel.Genre;
        this.OmdbPlot = omdbDataModel.Plot;
        this.Awards = omdbDataModel.Awards;
        this.ImdbRating = omdbDataModel.ImdbRating;
        this.ImdbVotes = omdbDataModel.ImdbVotes;
        this.BoxOfficeString = omdbDataModel.BoxOffice;
        if (String.IsNullOrWhiteSpace(omdbDataModel.BoxOffice) || omdbDataModel.BoxOffice == "N/A")
        {
            this.BoxOfficeNumber = 0;
        }
        else
        {
            this.BoxOfficeNumber = long.Parse(omdbDataModel.BoxOffice.TrimStart("$"), NumberStyles.AllowThousands);
        }
        this.Budget = tmdbMovieDataModel.Budget;
        this.TmdbGenres = String.Join(", ", tmdbMovieDataModel.Genres.Select(tgdm => tgdm.Name));
        
        this.Homepage = tmdbMovieDataModel.Homepage;
        this.TmdbId = tmdbMovieDataModel.TmdbId;
        this.OriginCountries = "";
        FrozenDictionary<string, string>? iso31661ToEnglishCountryNameDictionary = configurationCountriesDictionary.iso31661ToEnglishCountryNameDictionary;
        if (iso31661ToEnglishCountryNameDictionary != null)
        {            
            this.OriginCountries = String.Join(", ", tmdbMovieDataModel.OriginCountry.Select(oc => iso31661ToEnglishCountryNameDictionary[oc]));
        }
        this.OriginLanguage = "";
        FrozenDictionary<string, string>? iso6391ToEnglishLanguageNameDictionary = configurationLanguagesDictionary.iso6391ToEnglishLanguageNameDictionary;
        if (iso6391ToEnglishLanguageNameDictionary != null)
        {            
            string iso6391LanguageCode = tmdbMovieDataModel.OriginalLanguage;
            this.OriginLanguage = iso6391ToEnglishLanguageNameDictionary[iso6391LanguageCode];
        }
        this.OriginalTitle = tmdbMovieDataModel.OriginalTitle;
        this.TmdbPlot = tmdbMovieDataModel.Overview;
        if (tmdbMovieDataModel.ProductionCompanies != null)
        {
            this.ProductionCompanies = String.Join("; ", tmdbMovieDataModel.ProductionCompanies.Select(tpcdm => tpcdm.Name));
        }
        this.ProductionCountries = String.Join(", ", tmdbMovieDataModel.ProductionCountries.Select(tpcdm => tpcdm.Name));
        this.ReleaseDate = tmdbMovieDataModel.ReleaseDate;
        this.Revenue = tmdbMovieDataModel.Revenue;
        this.Runtime = tmdbMovieDataModel.Runtime;
        this.SpokenLanguages = String.Join(", ", tmdbMovieDataModel.SpokenLanguages.Select(tsldm => tsldm.EnglishName));
        this.Status = tmdbMovieDataModel.Status;
        this.Tagline = tmdbMovieDataModel.Tagline;
        this.Cast = tmdbMovieCreditsDataModel.Cast
                        .Select(tmcdm => new MovieCastViewModel(tmcdm, testGuid))
                        .ToList();
        this.Directors = tmdbMovieCreditsDataModel.Crew
                            .Where(tmcdm => tmcdm.Job == "Director")
                            .Select(tmcdm => new MovieCrewViewModel(tmcdm, testGuid))
                            .ToList();
        this.Writers = tmdbMovieCreditsDataModel.Crew
                            .Where(tmcdm => tmcdm.Department == "Writing")
                            .Select(tmcdm => new MovieCrewViewModel(tmcdm, testGuid))
                            .ToList();
        this.Producers = tmdbMovieCreditsDataModel.Crew
                            .Where(tmcdm => tmcdm.Job.EndsWith("Producer"))
                            .Select(tmcdm => new MovieCrewViewModel(tmcdm, testGuid))
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
    public string Title { get; }
    public int? ImdbRank { get; }
    public string KnownForActors { get; }
    public int? Year { get; }
    public string Rated { get; }
    public string OmdbGenres { get; }
    public string OmdbPlot { get; }
    public string Awards { get; }
    public string ImdbRating { get; }
    public string ImdbVotes { get; }
    public string? BoxOfficeString { get; }
    public long BoxOfficeNumber { get; }
    public long Budget { get; }
    public string TmdbGenres { get; }
    public string? Homepage { get; }
    public int TmdbId { get; }
    public string OriginCountries { get; }
    public string OriginLanguage { get; }
    public string OriginalTitle { get; }
    public string TmdbPlot { get; }
    public string? ProductionCompanies { get; }
    public string ProductionCountries { get; }
    public string ReleaseDate { get; }  // NOTE: Actually a date, of course
    public long Revenue { get; }
    public int Runtime { get; }
    public string SpokenLanguages { get; }
    public string Status { get; }
    public string? Tagline { get; }
    public List<MovieCastViewModel> Cast { get; }
    public List<MovieCrewViewModel> Directors { get; }
    public List<MovieCrewViewModel> Writers { get; }
    public List<MovieCrewViewModel> Producers { get; }
    public List<WatchProviderViewModel> WatchProvidersBuy { get; }
    public List<WatchProviderViewModel> WatchProvidersFlatrate { get; }
    public List<WatchProviderViewModel> WatchProvidersRent { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nImage:\n*****\n{Image}\n*****\nImdbId: {ImdbId}\nTitle: {Title}\nImdbRank: {ImdbRank}\nKnownForActors: {KnownForActors}\nYear: {Year}\nRated: {Rated}\nOmdbGenres: {OmdbGenres}\nOmdbPlot: {OmdbPlot}\nAwards: {Awards}\nImdbRating: {ImdbRating}\nImdbVotes: {ImdbVotes}\nBoxOfficeString: {BoxOfficeString}\nBoxOfficeNumber: {BoxOfficeNumber}\nBudget: {Budget}\nTmdbGenres: {TmdbGenres}\nHomepage: {Homepage}\nTmdbId: {TmdbId}\nOriginCountries: {OriginCountries}\nOriginLanguage: {OriginLanguage}\nOriginalTitle: {OriginalTitle}\nTmdbPlot: {TmdbPlot}\nProductionCompanies: {ProductionCompanies}\nProductionCountries: {ProductionCountries}\nReleaseDate: {ReleaseDate}\nRevenue: {Revenue}\nRuntime: {Runtime}\nSpokenLanguages: {SpokenLanguages}\nStatus: {Status}\nTagline: {Tagline}\nCast:\n*****\n{string.Join("\n\n", Cast)}\n*****\nDirectors:\n*****\n{string.Join("\n\n", Directors)}\n*****\nWriters:\n*****\n{string.Join("\n\n", Writers)}\n*****\nProducers:\n*****\n{string.Join("\n\n", Producers)}\n*****\nWatchProvidersBuy:\n*****\n{string.Join("\n\n", WatchProvidersBuy)}\n*****\nWatchProvidersFlatrate:\n*****\n{string.Join("\n\n", WatchProvidersFlatrate)}\n*****\nWatchProvidersRent:\n*****\n{string.Join("\n\n", WatchProvidersRent)}";
    }
}
