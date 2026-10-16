using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;
using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;

// Downloads the Academy Awards dataset at https://github.com/DLu/oscar_data (one tab separated file of every nomination)
public class OscarsHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    public static string CacheKey = "oscars";
    public readonly TimeSpan AbsoluteExpirationTime = TimeSpan.FromDays(7);  // The dataset is updated after each ceremony, so weekly is plenty
    // So that concurrent requests while the dataset isn't cached download and parse it only once
    private static readonly SemaphoreSlim LoadLock = new SemaphoreSlim(1, 1);

    public const string FileName = "oscars.csv";

    // The columns the parser needs, by their header names in the file
    private static readonly string[] RequiredColumns =
        ["Ceremony", "Year", "Class", "CanonicalCategory", "Category", "Film", "FilmId", "Name", "Nominees", "NomineeIds", "Winner", "Detail", "Note", "Citation"];

    public OscarsHttpClient(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;

        _httpClient.BaseAddress = new Uri("https://raw.githubusercontent.com/DLu/oscar_data/refs/heads/main/");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "text/plain");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Referer, "https://movieinfo.dev/");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "movie-info/1.0 (https://movieinfo.dev/)");
    }

    // The whole dataset, indexed for the by-year listing and for lookups by film or person. Downloaded and parsed only when
    // it isn't cached (or the cache has expired, after a week). Null when the download or parse failed, which isn't cached.
    public async Task<OscarsViewModel?> GetOscars()
    {
        OscarsViewModel? oscarsViewModel;

        if (!_cache.TryGetValue(CacheKey, out oscarsViewModel))
        {
            await LoadLock.WaitAsync();
            try
            {
                if (!_cache.TryGetValue(CacheKey, out oscarsViewModel))  // Not loaded by the request that held the lock
                {
                    OscarsResponseDataModel? oscarsResponseDataModel = await DownloadOscars();
                    if (oscarsResponseDataModel == null)
                    {
                        return null;
                    }
                    oscarsViewModel = new OscarsViewModel(oscarsResponseDataModel.Rows);

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(AbsoluteExpirationTime);
                    _cache.Set(CacheKey, oscarsViewModel, cacheEntryOptions);
                }
            }
            finally
            {
                LoadLock.Release();
            }
        }

        return oscarsViewModel;
    }

    private async Task<OscarsResponseDataModel?> DownloadOscars()
    {
        using HttpResponseMessage response = await _httpClient.GetAsync(FileName);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return null;
        }

        string responseTsvString = await response.Content.ReadAsStringAsync();

        return GetModelFromResponse(responseTsvString);
    }

    // Parses the tab separated file. Returns null when the header row doesn't have the expected columns.
    // Rows are ragged: trailing empty cells are left off, so a row may have fewer cells than the header has columns.
    public static OscarsResponseDataModel? GetModelFromResponse(string responseTsvString)
    {
        string[] lines = responseTsvString.Split('\n');
        string[] header = lines[0].TrimEnd('\r').Split('\t');
        Dictionary<string, int> columns = new Dictionary<string, int>();
        for (int i = 0; i < header.Length; i++)
        {
            columns.TryAdd(header[i].Trim(), i);
        }
        if (RequiredColumns.Any(column => !columns.ContainsKey(column)))
        {
            return null;
        }

        List<OscarsRowDataModel> rows = new List<OscarsRowDataModel>();
        foreach (string rawLine in lines.Skip(1))
        {
            string line = rawLine.TrimEnd('\r');
            if (line.Length == 0)
            {
                continue;
            }
            string[] cells = line.Split('\t');
            string Cell(string column) => columns[column] < cells.Length ? Unescape(cells[columns[column]]) : "";

            if (!int.TryParse(Cell("Ceremony"), out int ceremony))
            {
                continue;  // Not a data row
            }
            string[] films = SplitList(Cell("Film"));
            string[] nominees = SplitList(Cell("Nominees"));
            rows.Add(new OscarsRowDataModel
            {
                Ceremony = ceremony,
                Year = Cell("Year"),
                Class = Cell("Class"),
                CanonicalCategory = Cell("CanonicalCategory"),
                Category = Cell("Category"),
                Films = films,
                FilmIds = SplitIds(Cell("FilmId"), films.Length),
                Name = Cell("Name"),
                Nominees = nominees,
                NomineeIds = SplitIds(Cell("NomineeIds"), nominees.Length),
                Winner = Cell("Winner") == "True",
                Detail = NullIfEmpty(Cell("Detail")),
                Note = NullIfEmpty(Cell("Note")),
                Citation = NullIfEmpty(Cell("Citation")),
            });
        }

        return new OscarsResponseDataModel { Rows = rows };
    }

    // Quotes are backslash escaped in the file (the only escape it uses)
    private static string Unescape(string cell)
    {
        return cell.Replace("\\\"", "\"");
    }

    private static string[] SplitList(string cell)
    {
        return cell.Length == 0 ? [] : cell.Split('|');
    }

    // The ids for a list of the given length; "?" (and a missing id) means the dataset doesn't know it
    private static string?[] SplitIds(string cell, int count)
    {
        string[] ids = SplitList(cell);
        string?[] result = new string?[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = i < ids.Length && ids[i] != "" && ids[i] != "?" ? ids[i] : null;
        }
        return result;
    }

    private static string? NullIfEmpty(string cell)
    {
        return cell.Length == 0 ? null : cell;
    }
}
