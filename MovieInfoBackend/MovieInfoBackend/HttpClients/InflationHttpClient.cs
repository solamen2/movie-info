using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;
using MovieInfoBackend.DataModels;

// Downloads the two Bureau of Labor Statistics Consumer Price Index series (CPI-U, all items, U.S. city average) used to adjust
// dollar amounts for inflation: CUSR0000SA0 (seasonally adjusted, Jan 1947 onwards) and CUUR0000SA0 (not seasonally adjusted,
// Jan 1913 onwards), from the HTML tables at https://data.bls.gov/timeseries/<series id>?years_option=all_years
public partial class InflationHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    public static string CacheKey = "inflation";
    public readonly TimeSpan AbsoluteExpirationTime = TimeSpan.FromDays(7);  // The index is published monthly, so weekly is plenty
    // So that concurrent requests while the series aren't cached download and parse them only once
    private static readonly SemaphoreSlim LoadLock = new SemaphoreSlim(1, 1);

    public const string SeasonallyAdjustedSeriesId = "CUSR0000SA0";
    public const string NotSeasonallyAdjustedSeriesId = "CUUR0000SA0";

    private static readonly string[] MonthColumns = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    public InflationHttpClient(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;

        _httpClient.BaseAddress = new Uri("https://data.bls.gov/timeseries/");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "text/html");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Referer, "https://movieinfo.dev/");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "movie-info/1.0 (https://movieinfo.dev/)");
    }

    // Both series, downloaded and parsed only when they aren't cached (or the cache has expired, after a week).
    // Null when either download or parse failed, which isn't cached.
    public async Task<InflationDataModel?> GetInflation()
    {
        InflationDataModel? inflationDataModel;

        if (!_cache.TryGetValue(CacheKey, out inflationDataModel))
        {
            await LoadLock.WaitAsync();
            try
            {
                if (!_cache.TryGetValue(CacheKey, out inflationDataModel))  // Not loaded by the request that held the lock
                {
                    inflationDataModel = await DownloadInflation();
                    if (inflationDataModel == null)
                    {
                        return null;
                    }

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(AbsoluteExpirationTime);
                    _cache.Set(CacheKey, inflationDataModel, cacheEntryOptions);
                }
            }
            finally
            {
                LoadLock.Release();
            }
        }

        return inflationDataModel;
    }

    private async Task<InflationDataModel?> DownloadInflation()
    {
        Task<BlsTimeSeriesResponseDataModel?> seasonallyAdjustedTask = DownloadSeries(SeasonallyAdjustedSeriesId);
        Task<BlsTimeSeriesResponseDataModel?> notSeasonallyAdjustedTask = DownloadSeries(NotSeasonallyAdjustedSeriesId);
        await Task.WhenAll(seasonallyAdjustedTask, notSeasonallyAdjustedTask);

        BlsTimeSeriesResponseDataModel? seasonallyAdjusted = await seasonallyAdjustedTask;
        BlsTimeSeriesResponseDataModel? notSeasonallyAdjusted = await notSeasonallyAdjustedTask;
        if (seasonallyAdjusted == null || notSeasonallyAdjusted == null)
        {
            return null;
        }

        return new InflationDataModel
        {
            SeasonallyAdjusted = seasonallyAdjusted,
            NotSeasonallyAdjusted = notSeasonallyAdjusted,
        };
    }

    // Null when the download failed, the page couldn't be parsed, or it describes some other series than the one asked for
    private async Task<BlsTimeSeriesResponseDataModel?> DownloadSeries(string seriesId)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"{seriesId}?years_option=all_years");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }

        string responseHtmlString = await response.Content.ReadAsStringAsync();

        BlsTimeSeriesResponseDataModel? seriesResponseDataModel = GetModelFromResponse(responseHtmlString);
        return seriesResponseDataModel?.SeriesId == seriesId ? seriesResponseDataModel : null;
    }

    // Parses the series page. Returns null when the page doesn't have the catalog and data table expected (see
    // BlsTimeSeriesResponseDataModel). A month's cell holds its index value, or "&nbsp;" when it isn't published yet, or a
    // footnote marker such as "-(X)" when the BLS has no value for it; the table's HALF1 / HALF2 columns are ignored.
    public static BlsTimeSeriesResponseDataModel? GetModelFromResponse(string responseHtmlString)
    {
        Match seriesIdMatch = SeriesIdRegex().Match(responseHtmlString);
        Match seriesTitleMatch = SeriesTitleRegex().Match(responseHtmlString);
        Match seasonalAdjustmentMatch = SeasonalAdjustmentRegex().Match(responseHtmlString);
        Match tableMatch = DataTableRegex().Match(responseHtmlString);
        if (!seriesIdMatch.Success || !seasonalAdjustmentMatch.Success || !tableMatch.Success)
        {
            return null;
        }

        string table = tableMatch.Groups[1].Value;
        string[] headerColumns = HeaderCellRegex().Matches(table).Select(match => match.Groups[1].Value.Trim()).ToArray();
        if (headerColumns.Length < 1 + MonthColumns.Length
            || headerColumns[0] != "Year"
            || !headerColumns.Skip(1).Take(MonthColumns.Length).SequenceEqual(MonthColumns))
        {
            return null;
        }

        List<BlsTimeSeriesRowDataModel> rows = new List<BlsTimeSeriesRowDataModel>();
        foreach (Match rowMatch in RowRegex().Matches(table))
        {
            string[] cells = DataCellRegex().Matches(rowMatch.Groups[2].Value).Select(match => match.Groups[1].Value.Trim()).ToArray();
            if (cells.Length < MonthColumns.Length)
            {
                continue;  // Not a data row
            }
            decimal?[] months = new decimal?[MonthColumns.Length];
            for (int i = 0; i < MonthColumns.Length; i++)
            {
                months[i] = decimal.TryParse(cells[i], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal value) ? value : null;
            }
            rows.Add(new BlsTimeSeriesRowDataModel
            {
                Year = int.Parse(rowMatch.Groups[1].Value),
                Months = months,
            });
        }
        if (rows.Count == 0)
        {
            return null;
        }

        return new BlsTimeSeriesResponseDataModel
        {
            SeriesId = seriesIdMatch.Groups[1].Value.Trim(),
            SeriesTitle = seriesTitleMatch.Success ? seriesTitleMatch.Groups[1].Value.Trim() : "",
            IsSeasonallyAdjusted = !seasonalAdjustmentMatch.Groups[1].Value.StartsWith("Not", StringComparison.OrdinalIgnoreCase),
            Rows = rows,
        };
    }

    // The catalog table above the data, e.g. <th class="catalog" scope="row">Series Id:</th><td class="catalog">CUSR0000SA0</td>
    [GeneratedRegex(@"Series Id:\s*</th>\s*<td[^>]*>\s*([^<]*?)\s*</td>", RegexOptions.IgnoreCase)]
    private static partial Regex SeriesIdRegex();

    [GeneratedRegex(@"Series Title:\s*</th>\s*<td[^>]*>\s*([^<]*?)\s*</td>", RegexOptions.IgnoreCase)]
    private static partial Regex SeriesTitleRegex();

    // A row of its own in the catalog, either "Seasonally Adjusted" or "Not Seasonally Adjusted"
    [GeneratedRegex(@"<td[^>]*>\s*((?:Not )?Seasonally Adjusted)\s*</td>", RegexOptions.IgnoreCase)]
    private static partial Regex SeasonalAdjustmentRegex();

    [GeneratedRegex(@"<table\s+id=""table0""[^>]*>(.*?)</table>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex DataTableRegex();

    [GeneratedRegex(@"<th\s+scope=""col""[^>]*>([^<]*)</th>", RegexOptions.IgnoreCase)]
    private static partial Regex HeaderCellRegex();

    // A data row: the year as its heading cell, then the months' cells
    [GeneratedRegex(@"<th\s+scope=""row""[^>]*>\s*(\d{4})\s*</th>((?:\s*<td[^>]*>[^<]*</td>)+)", RegexOptions.IgnoreCase)]
    private static partial Regex RowRegex();

    [GeneratedRegex(@"<td[^>]*>([^<]*)</td>", RegexOptions.IgnoreCase)]
    private static partial Regex DataCellRegex();
}
