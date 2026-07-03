using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record PersonMovieCastCreditViewModel
{
    public PersonMovieCastCreditViewModel(TmdbPersonMovieCastCreditDataModel tmdbPersonMovieCastCreditDataModel)
    {
        this.ID = Guid.NewGuid();
        this.StillPath = tmdbPersonMovieCastCreditDataModel.BackdropPath;
        this.TmdbId = tmdbPersonMovieCastCreditDataModel.Id;
        this.Title = tmdbPersonMovieCastCreditDataModel.Title;
        this.OriginalTitle = tmdbPersonMovieCastCreditDataModel.OriginalTitle;
        this.Popularity = tmdbPersonMovieCastCreditDataModel.Popularity;
        this.PosterPath = tmdbPersonMovieCastCreditDataModel.PosterPath;
        this.ReleaseDate = tmdbPersonMovieCastCreditDataModel.ReleaseDate;
        this.IsVideo = tmdbPersonMovieCastCreditDataModel.Video;
        this.Character = tmdbPersonMovieCastCreditDataModel.Character;
    }

    public Guid ID { get; init; }
    public string StillPath { get; init; }
    public int TmdbId { get; init; }
    public string Title { get; init; }
    public string OriginalTitle { get; init; }
    public double Popularity { get; init; }
    public string PosterPath { get; init; }
    public string ReleaseDate { get; init; }  // Actually a date, of course
    public bool IsVideo { get; init; }
    public string Character { get; init; }

    public override string ToString()
    {
        return $"ID: {ID}\nStillPath: {StillPath}\nTmdbId: {TmdbId}\nTitle: {Title}\nOriginalTitle: {OriginalTitle}\nPopularity: {Popularity}\nPosterPath: {PosterPath}\nReleaseDate: {ReleaseDate}\nIsVideo: {IsVideo}\nCharacter: {Character}";
    }
}
