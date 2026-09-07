using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public record OmdbErrorResponseDataModel
{
    [JsonPropertyName("Response")]  // False when IMDB ID is not found
    public required string Response { get; init; }  // Note for ViewModel: this is a bool

    public override string ToString()
    {
        return $"Response: {Response}";
    }
}