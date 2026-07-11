using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record PersonMovieCastViewModel
{
    public PersonMovieCastViewModel(TmdbPersonMovieCastDataModel tmdbPersonMovieCastDataModel)
    {
        this.ID = Guid.NewGuid();
        this.StillPath = tmdbPersonMovieCastDataModel.BackdropPath;
        this.TmdbId = tmdbPersonMovieCastDataModel.Id;
        this.Title = tmdbPersonMovieCastDataModel.Title;
        this.OriginalTitle = tmdbPersonMovieCastDataModel.OriginalTitle;
        this.Popularity = tmdbPersonMovieCastDataModel.Popularity;
        this.PosterPath = tmdbPersonMovieCastDataModel.PosterPath;
        this.ReleaseDate = tmdbPersonMovieCastDataModel.ReleaseDate;
        this.IsVideo = tmdbPersonMovieCastDataModel.Video;
        this.Character = tmdbPersonMovieCastDataModel.Character;
    }

    public Guid ID { get; }
    public string StillPath { get; }
    public int TmdbId { get; }
    public string Title { get; }
    public string OriginalTitle { get; }
    public double Popularity { get; }
    public string PosterPath { get; }
    public string ReleaseDate { get; }  // Actually a date, of course
    public bool IsVideo { get; }
    public string Character { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nStillPath: {StillPath}\nTmdbId: {TmdbId}\nTitle: {Title}\nOriginalTitle: {OriginalTitle}\nPopularity: {Popularity}\nPosterPath: {PosterPath}\nReleaseDate: {ReleaseDate}\nIsVideo: {IsVideo}\nCharacter: {Character}";
    }
}
