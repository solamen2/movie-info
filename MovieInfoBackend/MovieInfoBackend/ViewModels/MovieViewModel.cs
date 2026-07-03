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
                          ConfigurationCountriesDictionary configurationCountriesDictionary,
                          ConfigurationLanguagesDictionary configurationLanguagesDictionary)
    {
        this.ID = Guid.NewGuid();
        this.Image = suggestionViewModel.Image;
        this.ImdbId = suggestionViewModel.ItemID;
        this.Name = suggestionViewModel.Name;
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
            this.BoxOfficeNumber = long.Parse(omdbDataModel.BoxOffice, NumberStyles.AllowThousands | NumberStyles.AllowCurrencySymbol);
        }
        this.Budget = tmdbMovieDataModel.Budget;
        this.TmdbGenres = null;
        if (tmdbMovieDataModel.Genres != null)
        {
            this.TmdbGenres = String.Join(", ", tmdbMovieDataModel.Genres.Select(g => g.Name));
        }
        this.Homepage = tmdbMovieDataModel.Homepage;
        this.TmdbId = tmdbMovieDataModel.Id;
        this.OriginCountries = null;
        FrozenDictionary<string, string>? iso31661ToEnglishCountryNameDictionary = configurationCountriesDictionary.iso31661ToEnglishCountryNameDictionary;
        if (tmdbMovieDataModel.OriginCountry != null && iso31661ToEnglishCountryNameDictionary != null)
        {            
            this.OriginCountries = String.Join(", ", tmdbMovieDataModel.OriginCountry.Select(oc => iso31661ToEnglishCountryNameDictionary[oc]));
        }
        this.OriginLanguage = null;
        FrozenDictionary<string, string>? iso6391ToEnglishLanguageNameDictionary = configurationLanguagesDictionary.iso6391ToEnglishLanguageNameDictionary;
        if (tmdbMovieDataModel.OriginalLanguage != null && iso6391ToEnglishLanguageNameDictionary != null)
        {            
            string iso6391LanguageCode = tmdbMovieDataModel.OriginalLanguage;
            this.OriginLanguage = iso6391ToEnglishLanguageNameDictionary[iso6391LanguageCode];
        }
        this.OriginalTitle = tmdbMovieDataModel.OriginalTitle;
        this.TmdbPlot = tmdbMovieDataModel.Overview;
        this.ProductionCompanies = String.Join("; ", tmdbMovieDataModel.ProductionCompanies.Select(pc => pc.Name));
        this.ProductionCountries = String.Join(", ", tmdbMovieDataModel.ProductionCountries.Select(pc => pc.Name));
        this.ReleaseDate = tmdbMovieDataModel.ReleaseDate;
        this.Revenue = tmdbMovieDataModel.Revenue;
        this.Runtime = tmdbMovieDataModel.Runtime;
        this.SpokenLanguages = String.Join(", ", tmdbMovieDataModel.SpokenLanguages.Select(sl => sl.EnglishName));
        this.Status = tmdbMovieDataModel.Status;
        this.Tagline = tmdbMovieDataModel.Tagline;
        this.Cast = tmdbMovieCreditsDataModel.Cast.ToList()
                        .Select(tcdm => new TmdbCastViewModel(tcdm))
                        .ToList();
        this.Directors = tmdbMovieCreditsDataModel.Crew.ToList()
                            .Where(tcdm => tcdm.Job == "Director")
                            .Select(tcdm => new TmdbCrewViewModel(tcdm))
                            .ToList();
        this.Writers = tmdbMovieCreditsDataModel.Crew.ToList()
                            .Where(tcdm => tcdm.Department == "Writing")
                            .Select(tcdm => new TmdbCrewViewModel(tcdm))
                            .ToList();
    }

    public Guid ID { get; init; }
    public SuggestionImageViewModel? Image { get; init; }
    public string ImdbId { get; init; }
    public string Name { get; init; }
    public int? ImdbRank { get; init; }
    public string KnownForActors { get; init; }
    public int? Year { get; init; }
    public string Rated { get; init; }
    public string OmdbGenres { get; init; }
    public string OmdbPlot { get; init; }
    public string Awards { get; init; }
    public string ImdbRating { get; init; }
    public string ImdbVotes { get; init; }
    public string? BoxOfficeString { get; init; }
    public long BoxOfficeNumber { get; init; }
    public long Budget { get; init; }
    public string? TmdbGenres { get; init; }
    public string Homepage { get; init; }
    public int TmdbId { get; init; }
    public string? OriginCountries { get; init; }
    public string? OriginLanguage { get; init; }
    public string OriginalTitle { get; init; }
    public string TmdbPlot { get; init; }
    public string ProductionCompanies { get; init; }
    public string ProductionCountries { get; init; }
    public string ReleaseDate { get; init; }  // NOTE: Actually a date, of course
    public long Revenue { get; init; }
    public int Runtime { get; init; }
    public string SpokenLanguages { get; init; }
    public string Status { get; init; }
    public string Tagline { get; init; }
    public List<TmdbCastViewModel> Cast { get; init; }
    public List<TmdbCrewViewModel> Directors { get; init; }
    public List<TmdbCrewViewModel> Writers { get; init; }

    public override string ToString()
    {
        return $"ID: {ID}\nImage:\n*****\n{Image}\n*****\nImdbId: {ImdbId}\nName: {Name}\nImdbRank: {ImdbRank}\nKnownForActors: {KnownForActors}\nYear: {Year}\nRated: {Rated}\nOmdbGenres: {OmdbGenres}\nOmdbPlot: {OmdbPlot}\nAwards: {Awards}\nImdbRating: {ImdbRating}\nImdbVotes: {ImdbVotes}\nBoxOfficeString: {BoxOfficeString}\nBoxOfficeNumber: {BoxOfficeNumber}\nBudget: {Budget}\nTmdbGenres: {TmdbGenres}\nHomepage: {Homepage}\nTmdbId: {TmdbId}\nOriginCountries: {OriginCountries}\nOriginLanguage: {OriginLanguage}\nOriginalTitle: {OriginalTitle}\nTmdbPlot: {TmdbPlot}\nProductionCompanies: {ProductionCompanies}\nProductionCountries: {ProductionCountries}\nReleaseDate: {ReleaseDate}\nRevenue: {Revenue}\nRuntime: {Runtime}\nSpokenLanguages: {SpokenLanguages}\nStatus: {Status}\nTagline: {Tagline}\nCast:\n*****\n{string.Join("\n\n", Cast)}\n*****\nDirectors:\n*****\n{string.Join("\n\n", Directors)}\n*****\nWriters:\n*****\n{string.Join("\n\n", Writers)}";
    }
}
