using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record PersonTvSeriesCrewCreditViewModel
{
    public PersonTvSeriesCrewCreditViewModel(TmdbPersonTvSeriesCrewCreditDataModel tmdbPersonTvSeriesCrewCreditDataModel)
    {
        this.ID = Guid.NewGuid();
        this.StillPath = tmdbPersonTvSeriesCrewCreditDataModel.BackdropPath;
        this.TmdbId = tmdbPersonTvSeriesCrewCreditDataModel.Id;
        this.OriginalName = tmdbPersonTvSeriesCrewCreditDataModel.OriginalName;
        this.Popularity = tmdbPersonTvSeriesCrewCreditDataModel.Popularity;
        this.PosterPath = tmdbPersonTvSeriesCrewCreditDataModel.PosterPath;
        this.FirstAirDate = tmdbPersonTvSeriesCrewCreditDataModel.FirstAirDate;
        this.Name = tmdbPersonTvSeriesCrewCreditDataModel.Name;
        this.Department = tmdbPersonTvSeriesCrewCreditDataModel.Department;
        this.EpisodeCount = tmdbPersonTvSeriesCrewCreditDataModel.EpisodeCount;
        this.FirstCreditAirDate = tmdbPersonTvSeriesCrewCreditDataModel.FirstCreditAirDate;
        this.Job = tmdbPersonTvSeriesCrewCreditDataModel.Job;
    }

    public Guid ID { get; init; }
    public string StillPath { get; init; }
    public int TmdbId { get; init; }
    public string OriginalName { get; init; }
    public double Popularity { get; init; }
    public string PosterPath { get; init; }
    public string? FirstAirDate { get; init; }  // Actually a date, of course
    public string Name { get; init; }
    public string Department { get; init; }
    public int EpisodeCount { get; init; }
    public string FirstCreditAirDate { get; init; }  // Actually a date, of course
    public string Job { get; init; }

    public override string ToString()
    {
        return $"ID: {ID}\nStillPath: {StillPath}\nTmdbId: {TmdbId}\nOriginalName: {OriginalName}\nPopularity: {Popularity}\nPosterPath: {PosterPath}\nFirstAirDate: {FirstAirDate}\nName: {Name}\nDepartment: {Department}\nEpisodeCount: {EpisodeCount}\nFirstCreditAirDate: {FirstCreditAirDate}\nJob: {Job}";
    }
}
