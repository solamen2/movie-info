using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record PersonTvSeriesCrewViewModel
{
    public PersonTvSeriesCrewViewModel(TmdbPersonTvSeriesCrewDataModel tmdbPersonTvSeriesCrewDataModel)
    {
        this.ID = Guid.NewGuid();
        this.BackdropPath = tmdbPersonTvSeriesCrewDataModel.BackdropPath;
        this.TmdbId = tmdbPersonTvSeriesCrewDataModel.Id;
        this.OriginalName = tmdbPersonTvSeriesCrewDataModel.OriginalName;
        this.Popularity = tmdbPersonTvSeriesCrewDataModel.Popularity;
        this.PosterPath = tmdbPersonTvSeriesCrewDataModel.PosterPath;
        this.FirstAirDate = tmdbPersonTvSeriesCrewDataModel.FirstAirDate;
        this.Name = tmdbPersonTvSeriesCrewDataModel.Name;
        this.Department = tmdbPersonTvSeriesCrewDataModel.Department;
        this.EpisodeCount = tmdbPersonTvSeriesCrewDataModel.EpisodeCount;
        this.FirstCreditAirDate = tmdbPersonTvSeriesCrewDataModel.FirstCreditAirDate;
        this.Job = tmdbPersonTvSeriesCrewDataModel.Job;
    }

    public Guid ID { get; }
    public string BackdropPath { get; }
    public int TmdbId { get; }
    public string OriginalName { get; }
    public double Popularity { get; }
    public string PosterPath { get; }
    public string? FirstAirDate { get; }  // Actually a date, of course
    public string Name { get; }
    public string Department { get; }
    public int EpisodeCount { get; }
    public string FirstCreditAirDate { get; }  // Actually a date, of course
    public string Job { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nBackdropPath: {BackdropPath}\nTmdbId: {TmdbId}\nOriginalName: {OriginalName}\nPopularity: {Popularity}\nPosterPath: {PosterPath}\nFirstAirDate: {FirstAirDate}\nName: {Name}\nDepartment: {Department}\nEpisodeCount: {EpisodeCount}\nFirstCreditAirDate: {FirstCreditAirDate}\nJob: {Job}";
    }
}
