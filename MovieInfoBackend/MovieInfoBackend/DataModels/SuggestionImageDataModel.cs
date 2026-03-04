using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public class SuggestionImageDataModel
{
    [JsonPropertyName("height")]
    public required int Height { get; set; }
    [JsonPropertyName("imageUrl")]
    public required string ImageURL { get; set; }
    [JsonPropertyName("width")]
    public required int Width { get; set; }

    public override string ToString()
    {
        return $"Height: {Height}\nImageURL: {ImageURL}\nWidth: {Width}";
    }
}
