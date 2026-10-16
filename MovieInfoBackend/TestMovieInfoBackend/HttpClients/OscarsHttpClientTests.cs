using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using System.Net;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.HttpClients;

public class OscarsHttpClientTests
{
    private string oscarsResponse;
    private int oscarsResponseRowCount;  // The file's lines less its header, so the file can grow

    public OscarsHttpClientTests(ITestOutputHelper output)
    {
        // Arrange

        oscarsResponse = ReadTestData("OscarsHttpClientResponse.tsv");
        oscarsResponseRowCount = oscarsResponse.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length - 1;
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

    // A client whose every request gets the given response; recordRequest is called once per request made
    private static OscarsHttpClient GetOscarsHttpClient(HttpStatusCode statusCode, string content, IMemoryCache cache, Action<Uri?>? recordRequest = null)
    {
        Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => recordRequest?.Invoke(request.RequestUri))
            .ReturnsAsync(() => new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });

        HttpClient httpClient = new HttpClient(httpMessageHandlerMock.Object);
        return new OscarsHttpClient(httpClient, cache);
    }

    private static OscarsRowDataModel FindRow(OscarsResponseDataModel model, int ceremony, string canonicalCategory, string nominee)
    {
        OscarsRowDataModel? row = model.Rows.Find(r => r.Ceremony == ceremony && r.CanonicalCategory == canonicalCategory && r.Nominees.Contains(nominee));
        Assert.NotNull(row);
        return row;
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ParsesEveryDataRow()
    {
        // Act
        OscarsResponseDataModel? model = OscarsHttpClient.GetModelFromResponse(oscarsResponse);

        // Assert
        Assert.NotNull(model);
        Assert.True(oscarsResponseRowCount > 100);  // Enough data to be a real sample of the dataset
        Assert.Equal(oscarsResponseRowCount, model.Rows.Count);
        Assert.All(model.Rows, row => Assert.InRange(row.Ceremony, 1, 96));
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ParsesTheColumns()
    {
        // Act
        OscarsResponseDataModel? model = OscarsHttpClient.GetModelFromResponse(oscarsResponse);

        // Assert
        Assert.NotNull(model);
        OscarsRowDataModel row = FindRow(model, 96, "ACTOR IN A LEADING ROLE", "Cillian Murphy");
        Assert.Equal("2023", row.Year);
        Assert.Equal("Acting", row.Class);
        Assert.Equal("ACTOR IN A LEADING ROLE", row.Category);
        Assert.Equal(["Oppenheimer"], row.Films);
        Assert.Equal(new string?[] { "tt15398776" }, row.FilmIds);
        Assert.Equal("Cillian Murphy", row.Name);
        Assert.Equal(["Cillian Murphy"], row.Nominees);
        Assert.Equal(new string?[] { "nm0614165" }, row.NomineeIds);
        Assert.True(row.Winner);
        Assert.Equal("J. Robert Oppenheimer", row.Detail);
        Assert.Null(row.Note);
        Assert.Null(row.Citation);
    }

    [Fact]
    public void GetModelFromResponse_PipeSeparatedCells_SplitsIntoParallelLists()
    {
        // Act
        OscarsResponseDataModel? model = OscarsHttpClient.GetModelFromResponse(oscarsResponse);

        // Assert
        Assert.NotNull(model);
        OscarsRowDataModel gaynor = FindRow(model, 1, "ACTRESS IN A LEADING ROLE", "Janet Gaynor");
        Assert.Equal(["7th Heaven", "Street Angel", "Sunrise"], gaynor.Films);
        Assert.Equal(new string?[] { "tt0018379", "tt0019429", "tt0018455" }, gaynor.FilmIds);
        Assert.Equal("Diane|Angela|The Wife", gaynor.Detail);
        Assert.True(gaynor.Winner);

        OscarsRowDataModel oppenheimer = FindRow(model, 96, "BEST PICTURE", "Christopher Nolan");
        Assert.Equal("Emma Thomas, Charles Roven and Christopher Nolan, Producers", oppenheimer.Name);
        Assert.Equal(["Emma Thomas", "Charles Roven", "Christopher Nolan"], oppenheimer.Nominees);
        Assert.Equal(new string?[] { "nm0858799", "nm0746273", "nm0634240" }, oppenheimer.NomineeIds);
    }

    [Fact]
    public void GetModelFromResponse_UnknownIds_AreNull()
    {
        // Act
        OscarsResponseDataModel? model = OscarsHttpClient.GetModelFromResponse(oscarsResponse);

        // Assert
        Assert.NotNull(model);
        OscarsRowDataModel citizenKane = FindRow(model, 14, "BEST PICTURE", "Mercury");  // Its NomineeIds cell is "?"
        Assert.Equal(["Mercury"], citizenKane.Nominees);
        Assert.Equal(new string?[] { null }, citizenKane.NomineeIds);
        Assert.Equal(new string?[] { "tt0033467" }, citizenKane.FilmIds);

        OscarsRowDataModel zoneOfInterest = model.Rows.Find(r => r.Ceremony == 96 && r.CanonicalCategory == "INTERNATIONAL FEATURE FILM" && r.Winner)!;
        Assert.Equal("United Kingdom", zoneOfInterest.Name);  // The country stands in for the nominees
        Assert.Empty(zoneOfInterest.Nominees);
        Assert.Empty(zoneOfInterest.NomineeIds);
    }

    [Fact]
    public void GetModelFromResponse_RaggedRows_FillInTheMissingTrailingCells()
    {
        // Act
        OscarsResponseDataModel? model = OscarsHttpClient.GetModelFromResponse(oscarsResponse);

        // Assert
        Assert.NotNull(model);
        OscarsRowDataModel dresser = FindRow(model, 1, "ACTRESS IN A LEADING ROLE", "Louise Dresser");  // Row ends at its Detail cell
        Assert.False(dresser.Winner);
        Assert.Equal("Mrs. Pleznik", dresser.Detail);
        Assert.Null(dresser.Note);
        Assert.Null(dresser.Citation);

        OscarsRowDataModel bassett = FindRow(model, 96, "HONORARY AWARD", "Angela Bassett");  // Empty Film and Name cells
        Assert.Empty(bassett.Films);
        Assert.Equal("", bassett.Name);
        Assert.Equal(new string?[] { "nm0000291" }, bassett.NomineeIds);
        Assert.True(bassett.Winner);
        Assert.StartsWith("To Angela Bassett", bassett.Citation);
    }

    [Fact]
    public void GetModelFromResponse_EscapedQuotes_AreUnescaped()
    {
        // Act
        OscarsResponseDataModel? model = OscarsHttpClient.GetModelFromResponse(oscarsResponse);

        // Assert
        Assert.NotNull(model);
        OscarsRowDataModel chaplin = FindRow(model, 1, "SPECIAL AWARD", "Charles Chaplin");
        Assert.StartsWith("NOTE: \"The Academy Board of Judges", chaplin.Note);
        Assert.DoesNotContain("\\", chaplin.Note);
    }

    [Fact]
    public void GetModelFromResponse_WindowsLineEndings_AreParsed()
    {
        // Arrange
        string windowsResponse = oscarsResponse.Replace("\n", "\r\n");

        // Act
        OscarsResponseDataModel? model = OscarsHttpClient.GetModelFromResponse(windowsResponse);

        // Assert
        Assert.NotNull(model);
        Assert.Equal(oscarsResponseRowCount, model.Rows.Count);
        OscarsRowDataModel bassett = FindRow(model, 96, "HONORARY AWARD", "Angela Bassett");
        Assert.DoesNotContain("\r", bassett.Citation);
    }

    [Fact]
    public void GetModelFromResponse_ColumnsInAnotherOrder_AreFoundByName()
    {
        // Arrange
        string response = "Winner\tCeremony\tYear\tClass\tCanonicalCategory\tCategory\tFilm\tFilmId\tName\tNominees\tNomineeIds\tDetail\tNote\tCitation\n"
                        + "True\t96\t2023\tTitle\tBEST PICTURE\tBEST PICTURE\tOppenheimer\ttt15398776\tEmma Thomas, Producer\tEmma Thomas\tnm0858799\n";

        // Act
        OscarsResponseDataModel? model = OscarsHttpClient.GetModelFromResponse(response);

        // Assert
        Assert.NotNull(model);
        OscarsRowDataModel row = Assert.Single(model.Rows);
        Assert.True(row.Winner);
        Assert.Equal(96, row.Ceremony);
        Assert.Equal(["Oppenheimer"], row.Films);
    }

    [Fact]
    public void GetModelFromResponse_RowWithoutACeremonyNumber_IsSkipped()
    {
        // Arrange
        string response = oscarsResponse + "not a row\tat all\n";

        // Act
        OscarsResponseDataModel? model = OscarsHttpClient.GetModelFromResponse(response);

        // Assert
        Assert.NotNull(model);
        Assert.Equal(oscarsResponseRowCount, model.Rows.Count);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("<html>Not Found</html>")]
    [InlineData("Ceremony\tYear\n1\t1927/28\n")]  // Missing columns
    [InlineData("Ceremony,Year,Class,CanonicalCategory,Category,Film,FilmId,Name,Nominees,NomineeIds,Winner,Detail,Note,Citation\n")]  // Comma, not tab, separated
    public void GetModelFromResponse_WithoutTheExpectedHeader_ReturnsNull(string response)
    {
        // Act
        OscarsResponseDataModel? model = OscarsHttpClient.GetModelFromResponse(response);

        // Assert
        Assert.Null(model);
    }

    [Fact]
    public async Task GetOscars_Ok_ReturnsViewModelFromTheDatasetFile()
    {
        // Arrange
        Uri? requestUri = null;
        OscarsHttpClient oscarsHttpClient = GetOscarsHttpClient(HttpStatusCode.OK, oscarsResponse, GetMemoryCache(), uri => requestUri = uri);
        int ceremonies = OscarsHttpClient.GetModelFromResponse(oscarsResponse)!.Rows.Select(row => row.Ceremony).Distinct().Count();

        // Act
        OscarsViewModel? oscarsViewModel = await oscarsHttpClient.GetOscars();

        // Assert
        Assert.NotNull(oscarsViewModel);
        Assert.Equal(ceremonies, oscarsViewModel.Years.Count);
        Assert.NotNull(requestUri);
        Assert.Equal("https://raw.githubusercontent.com/DLu/oscar_data/refs/heads/main/oscars.csv", requestUri.AbsoluteUri);
    }

    [Fact]
    public async Task GetOscars_CalledAgain_ReturnsTheCachedViewModelWithoutDownloading()
    {
        // Arrange
        int requests = 0;
        IMemoryCache cache = GetMemoryCache();
        OscarsHttpClient oscarsHttpClient = GetOscarsHttpClient(HttpStatusCode.OK, oscarsResponse, cache, _ => requests++);

        // Act
        OscarsViewModel? first = await oscarsHttpClient.GetOscars();
        OscarsViewModel? second = await oscarsHttpClient.GetOscars();
        // The app's HttpClients are transient, so another instance (with the same cache) must share the download too
        OscarsViewModel? third = await GetOscarsHttpClient(HttpStatusCode.OK, oscarsResponse, cache, _ => requests++).GetOscars();

        // Assert
        Assert.NotNull(first);
        Assert.Same(first, second);
        Assert.Same(first, third);
        Assert.Equal(1, requests);
        Assert.True(cache.TryGetValue(OscarsHttpClient.CacheKey, out OscarsViewModel? cached));
        Assert.Same(first, cached);
        Assert.Equal(TimeSpan.FromDays(7), oscarsHttpClient.AbsoluteExpirationTime);
    }

    [Fact]
    public async Task GetOscars_ConcurrentCalls_DownloadOnce()
    {
        // Arrange
        int requests = 0;
        OscarsHttpClient oscarsHttpClient = GetOscarsHttpClient(HttpStatusCode.OK, oscarsResponse, GetMemoryCache(), _ => Interlocked.Increment(ref requests));

        // Act
        OscarsViewModel?[] results = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => oscarsHttpClient.GetOscars()));

        // Assert
        Assert.All(results, result => Assert.Same(results[0], result));
        Assert.NotNull(results[0]);
        Assert.Equal(1, requests);
    }

    [Fact]
    public async Task GetOscars_UnparseableResponse_ReturnsNullWithoutCaching()
    {
        // Arrange
        int requests = 0;
        IMemoryCache cache = GetMemoryCache();
        OscarsHttpClient oscarsHttpClient = GetOscarsHttpClient(HttpStatusCode.OK, "<html>Not the dataset</html>", cache, _ => requests++);

        // Act
        OscarsViewModel? first = await oscarsHttpClient.GetOscars();
        OscarsViewModel? second = await oscarsHttpClient.GetOscars();

        // Assert
        Assert.Null(first);
        Assert.Null(second);
        Assert.Equal(2, requests);  // Tried again, since a failure isn't cached
        Assert.False(cache.TryGetValue(OscarsHttpClient.CacheKey, out OscarsViewModel? _));
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    public async Task GetOscars_NotOk_ReturnsNull(HttpStatusCode statusCode)
    {
        // Arrange
        IMemoryCache cache = GetMemoryCache();
        OscarsHttpClient oscarsHttpClient = GetOscarsHttpClient(statusCode, "Not Found", cache);

        // Act
        OscarsViewModel? oscarsViewModel = await oscarsHttpClient.GetOscars();

        // Assert
        Assert.Null(oscarsViewModel);
        Assert.False(cache.TryGetValue(OscarsHttpClient.CacheKey, out OscarsViewModel? _));
    }
}
