using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record PersonMovieCrewViewModel
{
    public PersonMovieCrewViewModel(TmdbPersonMovieCrewDataModel tmdbPersonMovieCrewDataModel)
    {
        this.ID = Guid.NewGuid();
        this.StillPath = tmdbPersonMovieCrewDataModel.BackdropPath;
        this.TmdbId = tmdbPersonMovieCrewDataModel.Id;
        this.Title = tmdbPersonMovieCrewDataModel.Title;
        this.OriginalTitle = tmdbPersonMovieCrewDataModel.OriginalTitle;
        this.Popularity = tmdbPersonMovieCrewDataModel.Popularity;
        this.PosterPath = tmdbPersonMovieCrewDataModel.PosterPath;
        this.ReleaseDate = tmdbPersonMovieCrewDataModel.ReleaseDate;
        this.IsVideo = tmdbPersonMovieCrewDataModel.Video;
        this.Department = tmdbPersonMovieCrewDataModel.Department;
        this.Job = tmdbPersonMovieCrewDataModel.Job;
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
    public string Department { get; }
    public string Job { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nStillPath: {StillPath}\nTmdbId: {TmdbId}\nTitle: {Title}\nOriginalTitle: {OriginalTitle}\nPopularity: {Popularity}\nPosterPath: {PosterPath}\nReleaseDate: {ReleaseDate}\nIsVideo: {IsVideo}\nDepartment: {Department}\nJob: {Job}";
    }
}
