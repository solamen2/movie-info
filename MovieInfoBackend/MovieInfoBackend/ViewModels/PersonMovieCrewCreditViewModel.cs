using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record PersonMovieCrewCreditViewModel
{
    public PersonMovieCrewCreditViewModel(TmdbPersonMovieCrewCreditDataModel tmdbPersonMovieCrewCreditDataModel)
    {
        this.ID = Guid.NewGuid();
        this.StillPath = tmdbPersonMovieCrewCreditDataModel.BackdropPath;
        this.TmdbId = tmdbPersonMovieCrewCreditDataModel.Id;
        this.Title = tmdbPersonMovieCrewCreditDataModel.Title;
        this.OriginalTitle = tmdbPersonMovieCrewCreditDataModel.OriginalTitle;
        this.Popularity = tmdbPersonMovieCrewCreditDataModel.Popularity;
        this.PosterPath = tmdbPersonMovieCrewCreditDataModel.PosterPath;
        this.ReleaseDate = tmdbPersonMovieCrewCreditDataModel.ReleaseDate;
        this.IsVideo = tmdbPersonMovieCrewCreditDataModel.Video;
        this.Department = tmdbPersonMovieCrewCreditDataModel.Department;
        this.Job = tmdbPersonMovieCrewCreditDataModel.Job;
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
    public string Department { get; init; }
    public string Job { get; init; }

    public override string ToString()
    {
        return $"ID: {ID}\nStillPath: {StillPath}\nTmdbId: {TmdbId}\nTitle: {Title}\nOriginalTitle: {OriginalTitle}\nPopularity: {Popularity}\nPosterPath: {PosterPath}\nReleaseDate: {ReleaseDate}\nIsVideo: {IsVideo}\nDepartment: {Department}\nJob: {Job}";
    }
}
