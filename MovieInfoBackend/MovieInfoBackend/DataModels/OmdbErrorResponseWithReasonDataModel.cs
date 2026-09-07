using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record OmdbErrorResponseWithReasonDataModel
{
    [JsonPropertyName("Response")]  // False when IMDB ID is not found
    public required string Response { get; init; }  // Note for ViewModel: this is a bool

    [JsonPropertyName("Error")]  // Actual error text
    public required string Error { get; init; }

    public override string ToString()
    {
        return $"Response: {Response}\nError: {Error}";
    }
}