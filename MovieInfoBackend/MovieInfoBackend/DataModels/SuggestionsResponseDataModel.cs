using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public class SuggestionsResponseDataModel
{
    [JsonPropertyName("d")]
    public SuggestionDataModel[]? Suggestions { get; set; }

    public SuggestionDataModel? FindByItemId(string itemId)
    {
        return Suggestions?.FirstOrDefault(s => string.Equals(s.ItemID, itemId, StringComparison.OrdinalIgnoreCase));
    }

    public override string ToString()
    {
        return string.Join("\n\n", Suggestions);
    }
}
