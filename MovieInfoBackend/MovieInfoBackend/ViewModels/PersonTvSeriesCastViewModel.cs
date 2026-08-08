using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record PersonTvSeriesCastViewModel
{
    public PersonTvSeriesCastViewModel(TmdbPersonTvSeriesCastDataModel tmdbPersonTvSeriesCastDataModel,
                                       Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
        this.BackdropPath = tmdbPersonTvSeriesCastDataModel.BackdropPath;
        this.TmdbId = tmdbPersonTvSeriesCastDataModel.Id;
        this.OriginalName = tmdbPersonTvSeriesCastDataModel.OriginalName;
        this.Popularity = tmdbPersonTvSeriesCastDataModel.Popularity;
        this.PosterPath = tmdbPersonTvSeriesCastDataModel.PosterPath;
        this.FirstAirDate = tmdbPersonTvSeriesCastDataModel.FirstAirDate;
        this.Name = tmdbPersonTvSeriesCastDataModel.Name;
        this.Character = tmdbPersonTvSeriesCastDataModel.Character;
        this.EpisodeCount = tmdbPersonTvSeriesCastDataModel.EpisodeCount;
        this.FirstCreditAirDate = tmdbPersonTvSeriesCastDataModel.FirstCreditAirDate;
    }

    public Guid ID { get; }
    public string BackdropPath { get; }
    public int TmdbId { get; }
    public string OriginalName { get; }
    public double Popularity { get; }
    public string PosterPath { get; }
    public string? FirstAirDate { get; }  // Actually a date, of course
    public string Name { get; }
    public string Character { get; }
    public int EpisodeCount { get; }
    public string FirstCreditAirDate { get; }  // Actually a date, of course

    public override string ToString()
    {
        return $"ID: {ID}\nBackdropPath: {BackdropPath}\nTmdbId: {TmdbId}\nOriginalName: {OriginalName}\nPopularity: {Popularity}\nPosterPath: {PosterPath}\nFirstAirDate: {FirstAirDate}\nName: {Name}\nCharacter: {Character}\nEpisodeCount: {EpisodeCount}\nFirstCreditAirDate: {FirstCreditAirDate}";
    }
}
