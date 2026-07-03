using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record PersonTvSeriesCastCreditViewModel
{
    public PersonTvSeriesCastCreditViewModel(TmdbPersonTvSeriesCastCreditDataModel tmdbPersonTvSeriesCastCreditDataModel)
    {
        this.ID = Guid.NewGuid();
        this.StillPath = tmdbPersonTvSeriesCastCreditDataModel.BackdropPath;
        this.TmdbId = tmdbPersonTvSeriesCastCreditDataModel.Id;
        this.OriginalName = tmdbPersonTvSeriesCastCreditDataModel.OriginalName;
        this.Popularity = tmdbPersonTvSeriesCastCreditDataModel.Popularity;
        this.PosterPath = tmdbPersonTvSeriesCastCreditDataModel.PosterPath;
        this.FirstAirDate = tmdbPersonTvSeriesCastCreditDataModel.FirstAirDate;
        this.Name = tmdbPersonTvSeriesCastCreditDataModel.Name;
        this.Character = tmdbPersonTvSeriesCastCreditDataModel.Character;
        this.EpisodeCount = tmdbPersonTvSeriesCastCreditDataModel.EpisodeCount;
        this.FirstCreditAirDate = tmdbPersonTvSeriesCastCreditDataModel.FirstCreditAirDate;
    }

    public Guid ID { get; init; }
    public string StillPath { get; init; }
    public int TmdbId { get; init; }
    public string OriginalName { get; init; }
    public double Popularity { get; init; }
    public string PosterPath { get; init; }
    public string? FirstAirDate { get; init; }  // Actually a date, of course
    public string Name { get; init; }
    public string Character { get; init; }
    public int EpisodeCount { get; init; }
    public string FirstCreditAirDate { get; init; }  // Actually a date, of course

    public override string ToString()
    {
        return $"ID: {ID}\nStillPath: {StillPath}\nTmdbId: {TmdbId}\nOriginalName: {OriginalName}\nPopularity: {Popularity}\nPosterPath: {PosterPath}\nFirstAirDate: {FirstAirDate}\nName: {Name}\nCharacter: {Character}\nEpisodeCount: {EpisodeCount}\nFirstCreditAirDate: {FirstCreditAirDate}";
    }
}
