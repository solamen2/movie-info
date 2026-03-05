using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public class SuggestionImageDataModel
{
    [JsonPropertyName("height")]
    public required int Height { get; init; }
    [JsonPropertyName("imageUrl")]
    public required string ImageURL { get; init; }
    [JsonPropertyName("width")]
    public required int Width { get; init; }

    public override string ToString()
    {
        return $"Height: {Height}\nImageURL: {ImageURL}\nWidth: {Width}";
    }
}
