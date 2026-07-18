using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class TmdbConfigurationDataModelTests
{
    private string tmdbHttpClientConfigurationCountriesResponse;
    private string tmdbHttpClientConfigurationLanguagesResponse;

    public TmdbConfigurationDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testCountriesDataFilename = "TmdbHttpClientConfigurationCountriesResponse.json";
        string testLanguagesDataFilename = "TmdbHttpClientConfigurationLanguagesResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testCountriesDataFilename}"))
        {
            tmdbHttpClientConfigurationCountriesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientConfigurationCountriesResponse))
        {
            throw new ArgumentException($"{testCountriesDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testLanguagesDataFilename}"))
        {
            tmdbHttpClientConfigurationLanguagesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientConfigurationLanguagesResponse))
        {
            throw new ArgumentException($"{testLanguagesDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {

    }
}