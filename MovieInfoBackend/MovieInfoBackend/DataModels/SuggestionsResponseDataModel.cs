using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public class SuggestionsResponseDataModel
{
    [JsonPropertyName("d")]
    public SuggestionDataModel[]? Suggestions { get; set; }

    public override string ToString()
    {
        return string.Join("\n\n", Suggestions);
    }
}
