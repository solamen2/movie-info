using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbWatchProviderCountryDataModel
{
    [JsonPropertyName("link")]
    public required string Link { get; init; }
    [JsonPropertyName("flatrate")]
    public TmdbWatchProviderDataModel[]? Flatrate { get; init; }
    [JsonPropertyName("buy")]
    public TmdbWatchProviderDataModel[]? Buy { get; init; }
    [JsonPropertyName("rent")]
    public TmdbWatchProviderDataModel[]? Rent { get; init; }

    public override string ToString()
    {
        string FlatrateString = (Flatrate == null) ? "Flatrate: []\n" : $"Flatrate:\n*****\n{string.Join("\n\n", Flatrate)}\n*****\n";
        string BuyString = (Buy == null) ? "Buy: []\n" : $"Buy:\n*****\n{string.Join("\n\n", Buy)}\n*****\n";
        string RentString = (Rent == null) ? "Rent: []" : $"Rent:\n*****\n{string.Join("\n\n", Rent)}";
        
        return $"Link: {Link}\n{FlatrateString}{BuyString}{RentString}";
    }

}
