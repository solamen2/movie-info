namespace MovieInfoBackend.DataModels;

// The two Consumer Price Index series (CPI-U, all items, U.S. city average) that dollar amounts are adjusted for inflation with,
// downloaded by InflationHttpClient. Served to the frontend as is (there's no view model, since none of this is shown directly):
// the frontend does the adjusting, so that the user can pick the month and year to adjust to.
public record InflationDataModel
{
    public required BlsTimeSeriesResponseDataModel SeasonallyAdjusted { get; init; }  // CUSR0000SA0, from Jan 1947
    public required BlsTimeSeriesResponseDataModel NotSeasonallyAdjusted { get; init; }  // CUUR0000SA0, from Jan 1913

    public override string ToString()
    {
        return $"SeasonallyAdjusted:\n*****\n{SeasonallyAdjusted}\n*****\nNotSeasonallyAdjusted:\n*****\n{NotSeasonallyAdjusted}";
    }
}
