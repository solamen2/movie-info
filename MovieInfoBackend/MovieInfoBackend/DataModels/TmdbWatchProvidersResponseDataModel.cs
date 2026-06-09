using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record TmdbWatchProvidersResponseDataModel
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("results")]
    public required TmdbWatchProvidersResultsDataModel Results { get; init; }

    public override string ToString()
    {
        return $"Id: {Id}\nResults:\n*****\n{Results}\n*****\n";
    }

}
