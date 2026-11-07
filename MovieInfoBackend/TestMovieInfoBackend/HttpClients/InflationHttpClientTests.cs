using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using MovieInfoBackend.DataModels;
using System.Net;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.HttpClients;

public class InflationHttpClientTests
{
    private string seasonallyAdjustedResponse;
    private string notSeasonallyAdjustedResponse;

    private const string SeasonallyAdjustedUri = "https://data.bls.gov/timeseries/CUSR0000SA0?years_option=all_years";
    private const string NotSeasonallyAdjustedUri = "https://data.bls.gov/timeseries/CUUR0000SA0?years_option=all_years";

    public InflationHttpClientTests(ITestOutputHelper output)
    {
        // Arrange

        seasonallyAdjustedResponse = ReadTestData("InflationHttpClientSeasonallyAdjustedResponse.html");
        notSeasonallyAdjustedResponse = ReadTestData("InflationHttpClientNotSeasonallyAdjustedResponse.html");
    }

    private static string ReadTestData(string filename)
    {
        string testData;
        using (StreamReader sr = File.OpenText($"../../../TestData/{filename}"))
        {
            testData = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(testData))
        {
            throw new ArgumentException($"{filename} is not valid test data.");
        }
        return testData;
    }

    // Each test gets its own (empty) cache, like the app's singleton IMemoryCache
    private static IMemoryCache GetMemoryCache()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddMemoryCache();
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IMemoryCache>();
    }

    // A client whose requests get the response for the series they ask for (by URI); recordRequest is called once per request made
    private static InflationHttpClient GetInflationHttpClient(IDictionary<string, (HttpStatusCode statusCode, string content)> responsesByUri, IMemoryCache cache, Action<Uri?>? recordRequest = null)
    {
        Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => recordRequest?.Invoke(request.RequestUri))
            .ReturnsAsync((HttpRequestMessage request, CancellationToken _) =>
            {
                (HttpStatusCode statusCode, string content) = responsesByUri.TryGetValue(request.RequestUri?.AbsoluteUri ?? "", out var response)
                    ? response
                    : (HttpStatusCode.NotFound, "Not Found");
                return new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content)
                };
            });

        HttpClient httpClient = new HttpClient(httpMessageHandlerMock.Object);
        return new InflationHttpClient(httpClient, cache);
    }

    // A client that answers both series' requests with their pages
    private InflationHttpClient GetInflationHttpClient(IMemoryCache cache, Action<Uri?>? recordRequest = null)
    {
        return GetInflationHttpClient(new Dictionary<string, (HttpStatusCode, string)>
        {
            [SeasonallyAdjustedUri] = (HttpStatusCode.OK, seasonallyAdjustedResponse),
            [NotSeasonallyAdjustedUri] = (HttpStatusCode.OK, notSeasonallyAdjustedResponse),
        }, cache, recordRequest);
    }

    private static BlsTimeSeriesRowDataModel FindRow(BlsTimeSeriesResponseDataModel model, int year)
    {
        BlsTimeSeriesRowDataModel? row = model.Rows.Find(r => r.Year == year);
        Assert.NotNull(row);
        return row;
    }

    [Fact]
    public void GetModelFromResponse_SeasonallyAdjustedResponse_ParsesTheCatalog()
    {
        // Act
        BlsTimeSeriesResponseDataModel? model = InflationHttpClient.GetModelFromResponse(seasonallyAdjustedResponse);

        // Assert
        Assert.NotNull(model);
        Assert.Equal("CUSR0000SA0", model.SeriesId);
        Assert.Equal("All items in U.S. city average, all urban consumers, seasonally adjusted", model.SeriesTitle);
        Assert.True(model.IsSeasonallyAdjusted);
    }

    [Fact]
    public void GetModelFromResponse_NotSeasonallyAdjustedResponse_ParsesTheCatalog()
    {
        // Act
        BlsTimeSeriesResponseDataModel? model = InflationHttpClient.GetModelFromResponse(notSeasonallyAdjustedResponse);

        // Assert
        Assert.NotNull(model);
        Assert.Equal("CUUR0000SA0", model.SeriesId);
        Assert.Equal("All items in U.S. city average, all urban consumers, not seasonally adjusted", model.SeriesTitle);
        Assert.False(model.IsSeasonallyAdjusted);
    }

    [Fact]
    public void GetModelFromResponse_SeasonallyAdjustedResponse_ParsesOneRowPerYearFrom1947()
    {
        // Act
        BlsTimeSeriesResponseDataModel? model = InflationHttpClient.GetModelFromResponse(seasonallyAdjustedResponse);

        // Assert
        Assert.NotNull(model);
        Assert.Equal(1947, model.Rows[0].Year);
        Assert.True(model.Rows.Count >= 80);  // Through 2026 when the page was saved, and the series only grows
        Assert.Equal(model.Rows.Select(row => row.Year), Enumerable.Range(1947, model.Rows.Count));  // Ascending, without gaps
        Assert.All(model.Rows, row => Assert.Equal(12, row.Months.Length));
    }

    [Fact]
    public void GetModelFromResponse_NotSeasonallyAdjustedResponse_ParsesOneRowPerYearFrom1913()
    {
        // Act
        BlsTimeSeriesResponseDataModel? model = InflationHttpClient.GetModelFromResponse(notSeasonallyAdjustedResponse);

        // Assert
        Assert.NotNull(model);
        Assert.Equal(1913, model.Rows[0].Year);
        Assert.True(model.Rows.Count >= 114);
        Assert.Equal(model.Rows.Select(row => row.Year), Enumerable.Range(1913, model.Rows.Count));
        Assert.All(model.Rows, row => Assert.Equal(12, row.Months.Length));
    }

    [Fact]
    public void GetModelFromResponse_ValidResponses_ParseTheMonthlyValues()
    {
        // Act
        BlsTimeSeriesResponseDataModel? seasonallyAdjusted = InflationHttpClient.GetModelFromResponse(seasonallyAdjustedResponse);
        BlsTimeSeriesResponseDataModel? notSeasonallyAdjusted = InflationHttpClient.GetModelFromResponse(notSeasonallyAdjustedResponse);

        // Assert
        Assert.NotNull(seasonallyAdjusted);
        Assert.NotNull(notSeasonallyAdjusted);
        Assert.Equal(new decimal?[] { 21.48m, 21.62m, 22.00m, 22.00m, 21.95m, 22.08m, 22.23m, 22.40m, 22.84m, 22.91m, 23.06m, 23.41m }, FindRow(seasonallyAdjusted, 1947).Months);
        Assert.Equal(new decimal?[] { 9.8m, 9.8m, 9.8m, 9.8m, 9.7m, 9.8m, 9.9m, 9.9m, 10.0m, 10.0m, 10.1m, 10.0m }, FindRow(notSeasonallyAdjusted, 1913).Months);
        Assert.Equal(318.961m, FindRow(seasonallyAdjusted, 2025).Months[0]);  // Three decimals nowadays
        Assert.Equal(317.671m, FindRow(notSeasonallyAdjusted, 2025).Months[0]);
    }

    [Fact]
    public void GetModelFromResponse_MonthWithoutAValue_IsNull()
    {
        // Act
        BlsTimeSeriesResponseDataModel? model = InflationHttpClient.GetModelFromResponse(seasonallyAdjustedResponse);

        // Assert
        Assert.NotNull(model);
        BlsTimeSeriesRowDataModel row2025 = FindRow(model, 2025);
        Assert.Null(row2025.Months[9]);  // Oct 2025 is "-(X)": not collected during the lapse in appropriations
        Assert.Equal(324.245m, row2025.Months[8]);
        Assert.Equal(325.063m, row2025.Months[10]);

        BlsTimeSeriesRowDataModel latestRow = model.Rows[^1];  // The months not published yet are "&nbsp;"
        Assert.Contains(null, latestRow.Months);
        Assert.NotNull(latestRow.Months[0]);
    }

    [Fact]
    public void GetModelFromResponse_WindowsLineEndings_AreParsed()
    {
        // Arrange
        string windowsResponse = seasonallyAdjustedResponse.Replace("\n", "\r\n");

        // Act
        BlsTimeSeriesResponseDataModel? model = InflationHttpClient.GetModelFromResponse(windowsResponse);

        // Assert
        Assert.NotNull(model);
        Assert.Equal("CUSR0000SA0", model.SeriesId);
        Assert.Equal(21.48m, FindRow(model, 1947).Months[0]);
    }

    [Fact]
    public void GetModelFromResponse_MinimalPage_IsParsed()
    {
        // Arrange (the parts of the page the parser needs, in lower case and with extra whitespace)
        string response = "<html><body><table class=\"catalog\"><tr><th class=\"catalog\" scope=\"row\">Series Id:</th><td class=\"catalog\"> CUUR0000SA0 </td></tr>"
                        + "<tr><td class=\"catalog\" colspan=\"2\">Not Seasonally Adjusted</td></tr></table>\n"
                        + "<table id=\"table0\"><thead><tr><th scope=\"col\"> Year </th><th scope=\"col\"> Jan </th><th scope=\"col\"> Feb </th><th scope=\"col\"> Mar </th>"
                        + "<th scope=\"col\"> Apr </th><th scope=\"col\"> May </th><th scope=\"col\"> Jun </th><th scope=\"col\"> Jul </th><th scope=\"col\"> Aug </th>"
                        + "<th scope=\"col\"> Sep </th><th scope=\"col\"> Oct </th><th scope=\"col\"> Nov </th><th scope=\"col\"> Dec </th></tr></thead>\n"
                        + "<tbody><tr><th scope=\"row\">2024</th>\n<td>1</td><td>2</td><td>3</td><td>4</td><td>5</td><td>6</td><td>7</td><td>8</td><td>9</td><td>10</td><td>11</td><td>12.5</td></tr></tbody></table></body></html>";

        // Act
        BlsTimeSeriesResponseDataModel? model = InflationHttpClient.GetModelFromResponse(response);

        // Assert
        Assert.NotNull(model);
        Assert.Equal("CUUR0000SA0", model.SeriesId);
        Assert.Equal("", model.SeriesTitle);
        Assert.False(model.IsSeasonallyAdjusted);
        BlsTimeSeriesRowDataModel row = Assert.Single(model.Rows);
        Assert.Equal(2024, row.Year);
        Assert.Equal(new decimal?[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12.5m }, row.Months);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("<html>Not Found</html>")]
    [InlineData("{\"status\":\"REQUEST_SUCCEEDED\"}")]  // The BLS API's JSON, not the page
    public void GetModelFromResponse_WithoutTheExpectedPage_ReturnsNull(string response)
    {
        // Act
        BlsTimeSeriesResponseDataModel? model = InflationHttpClient.GetModelFromResponse(response);

        // Assert
        Assert.Null(model);
    }

    [Fact]
    public void GetModelFromResponse_WithoutTheDataTable_ReturnsNull()
    {
        // Arrange (the catalog without the data table)
        string response = seasonallyAdjustedResponse.Replace("id=\"table0\"", "id=\"table1\"");

        // Act
        BlsTimeSeriesResponseDataModel? model = InflationHttpClient.GetModelFromResponse(response);

        // Assert
        Assert.Null(model);
    }

    [Fact]
    public void GetModelFromResponse_WithOtherColumns_ReturnsNull()
    {
        // Arrange (a quarterly series' columns instead of months)
        string response = seasonallyAdjustedResponse
            .Replace("<TH scope=\"col\"> Jan </TH><TH scope=\"col\"> Feb </TH><TH scope=\"col\"> Mar </TH>", "<TH scope=\"col\"> Qtr1 </TH>");

        // Act
        BlsTimeSeriesResponseDataModel? model = InflationHttpClient.GetModelFromResponse(response);

        // Assert
        Assert.Null(model);
    }

    [Fact]
    public void GetModelFromResponse_WithoutTheCatalog_ReturnsNull()
    {
        // Arrange
        string response = seasonallyAdjustedResponse.Replace("Series Id:", "Series:");

        // Act
        BlsTimeSeriesResponseDataModel? model = InflationHttpClient.GetModelFromResponse(response);

        // Assert
        Assert.Null(model);
    }

    [Fact]
    public async Task GetInflation_Ok_ReturnsBothSeriesFromTheirPages()
    {
        // Arrange
        List<Uri?> requestUris = new List<Uri?>();
        InflationHttpClient inflationHttpClient = GetInflationHttpClient(GetMemoryCache(), uri => { lock (requestUris) { requestUris.Add(uri); } });

        // Act
        InflationDataModel? inflationDataModel = await inflationHttpClient.GetInflation();

        // Assert
        Assert.NotNull(inflationDataModel);
        Assert.Equal("CUSR0000SA0", inflationDataModel.SeasonallyAdjusted.SeriesId);
        Assert.True(inflationDataModel.SeasonallyAdjusted.IsSeasonallyAdjusted);
        Assert.Equal(1947, inflationDataModel.SeasonallyAdjusted.Rows[0].Year);
        Assert.Equal("CUUR0000SA0", inflationDataModel.NotSeasonallyAdjusted.SeriesId);
        Assert.False(inflationDataModel.NotSeasonallyAdjusted.IsSeasonallyAdjusted);
        Assert.Equal(1913, inflationDataModel.NotSeasonallyAdjusted.Rows[0].Year);
        Assert.Equal(2, requestUris.Count);
        Assert.Contains(requestUris, uri => uri?.AbsoluteUri == SeasonallyAdjustedUri);
        Assert.Contains(requestUris, uri => uri?.AbsoluteUri == NotSeasonallyAdjustedUri);
    }

    [Fact]
    public async Task GetInflation_CalledAgain_ReturnsTheCachedDataWithoutDownloading()
    {
        // Arrange
        int requests = 0;
        IMemoryCache cache = GetMemoryCache();
        InflationHttpClient inflationHttpClient = GetInflationHttpClient(cache, _ => Interlocked.Increment(ref requests));

        // Act
        InflationDataModel? first = await inflationHttpClient.GetInflation();
        InflationDataModel? second = await inflationHttpClient.GetInflation();
        // The app's HttpClients are transient, so another instance (with the same cache) must share the download too
        InflationDataModel? third = await GetInflationHttpClient(cache, _ => Interlocked.Increment(ref requests)).GetInflation();

        // Assert
        Assert.NotNull(first);
        Assert.Same(first, second);
        Assert.Same(first, third);
        Assert.Equal(2, requests);  // One per series
        Assert.True(cache.TryGetValue(InflationHttpClient.CacheKey, out InflationDataModel? cached));
        Assert.Same(first, cached);
        Assert.Equal(TimeSpan.FromDays(7), inflationHttpClient.AbsoluteExpirationTime);
    }

    [Fact]
    public async Task GetInflation_ConcurrentCalls_DownloadOnce()
    {
        // Arrange
        int requests = 0;
        InflationHttpClient inflationHttpClient = GetInflationHttpClient(GetMemoryCache(), _ => Interlocked.Increment(ref requests));

        // Act
        InflationDataModel?[] results = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => inflationHttpClient.GetInflation()));

        // Assert
        Assert.All(results, result => Assert.Same(results[0], result));
        Assert.NotNull(results[0]);
        Assert.Equal(2, requests);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetInflation_OneSeriesUnparseable_ReturnsNullWithoutCaching(bool seasonallyAdjustedFails)
    {
        // Arrange
        int requests = 0;
        IMemoryCache cache = GetMemoryCache();
        InflationHttpClient inflationHttpClient = GetInflationHttpClient(new Dictionary<string, (HttpStatusCode, string)>
        {
            [SeasonallyAdjustedUri] = (HttpStatusCode.OK, seasonallyAdjustedFails ? "<html>Not the series</html>" : seasonallyAdjustedResponse),
            [NotSeasonallyAdjustedUri] = (HttpStatusCode.OK, seasonallyAdjustedFails ? notSeasonallyAdjustedResponse : "<html>Not the series</html>"),
        }, cache, _ => Interlocked.Increment(ref requests));

        // Act
        InflationDataModel? first = await inflationHttpClient.GetInflation();
        InflationDataModel? second = await inflationHttpClient.GetInflation();

        // Assert
        Assert.Null(first);
        Assert.Null(second);
        Assert.Equal(4, requests);  // Tried again, since a failure isn't cached
        Assert.False(cache.TryGetValue(InflationHttpClient.CacheKey, out InflationDataModel? _));
    }

    [Fact]
    public async Task GetInflation_PageForAnotherSeries_ReturnsNull()
    {
        // Arrange (the seasonally adjusted page served for both series)
        IMemoryCache cache = GetMemoryCache();
        InflationHttpClient inflationHttpClient = GetInflationHttpClient(new Dictionary<string, (HttpStatusCode, string)>
        {
            [SeasonallyAdjustedUri] = (HttpStatusCode.OK, seasonallyAdjustedResponse),
            [NotSeasonallyAdjustedUri] = (HttpStatusCode.OK, seasonallyAdjustedResponse),
        }, cache);

        // Act
        InflationDataModel? inflationDataModel = await inflationHttpClient.GetInflation();

        // Assert
        Assert.Null(inflationDataModel);
        Assert.False(cache.TryGetValue(InflationHttpClient.CacheKey, out InflationDataModel? _));
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    public async Task GetInflation_OneSeriesNotOk_ReturnsNull(HttpStatusCode statusCode)
    {
        // Arrange
        IMemoryCache cache = GetMemoryCache();
        InflationHttpClient inflationHttpClient = GetInflationHttpClient(new Dictionary<string, (HttpStatusCode, string)>
        {
            [SeasonallyAdjustedUri] = (HttpStatusCode.OK, seasonallyAdjustedResponse),
            [NotSeasonallyAdjustedUri] = (statusCode, "Not Found"),
        }, cache);

        // Act
        InflationDataModel? inflationDataModel = await inflationHttpClient.GetInflation();

        // Assert
        Assert.Null(inflationDataModel);
        Assert.False(cache.TryGetValue(InflationHttpClient.CacheKey, out InflationDataModel? _));
    }
}
