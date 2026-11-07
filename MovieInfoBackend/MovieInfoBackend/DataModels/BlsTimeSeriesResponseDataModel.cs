namespace MovieInfoBackend.DataModels;

// One Bureau of Labor Statistics time series, as shown at https://data.bls.gov/timeseries/<series id>?years_option=all_years:
// an HTML page with a catalog table describing the series and a data table of one row per year, with the months as columns,
// so this model is filled in by InflationHttpClient.GetModelFromResponse rather than deserialized directly.
public record BlsTimeSeriesResponseDataModel
{
    public required string SeriesId { get; init; }  // e.g. "CUSR0000SA0"
    public required string SeriesTitle { get; init; }  // e.g. "All items in U.S. city average, all urban consumers, seasonally adjusted"
    public required bool IsSeasonallyAdjusted { get; init; }
    public required List<BlsTimeSeriesRowDataModel> Rows { get; init; }  // In the page's order, which is by year ascending

    public override string ToString()
    {
        return $"SeriesId: {SeriesId}\nSeriesTitle: {SeriesTitle}\nIsSeasonallyAdjusted: {IsSeasonallyAdjusted}\nRows:\n*****\n{string.Join("\n\n", Rows)}";
    }
}
