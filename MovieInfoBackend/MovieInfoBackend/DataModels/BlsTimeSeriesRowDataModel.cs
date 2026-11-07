namespace MovieInfoBackend.DataModels;

// One year's row of a BLS time series table (see BlsTimeSeriesResponseDataModel)
public record BlsTimeSeriesRowDataModel
{
    public required int Year { get; init; }
    // The index value of each month, Jan to Dec, null where the series has none: not published yet (the months after
    // the latest release), or "-(X)" for a month the BLS never collected (e.g. Oct 2025, during the lapse in appropriations)
    public required decimal?[] Months { get; init; }

    public override string ToString()
    {
        return $"Year: {Year}\nMonths: {string.Join(" | ", Months.Select(month => month?.ToString() ?? "-"))}";
    }
}
