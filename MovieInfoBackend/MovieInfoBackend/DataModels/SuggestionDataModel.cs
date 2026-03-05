using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public class SuggestionDataModel
{
    [JsonPropertyName("i")]
    public SuggestionImageDataModel? Image { get; init; }
    [JsonPropertyName("id")]
    public required string ItemID { get; init;}
    [JsonPropertyName("l")]
    public required string Name { get; init; }  // name of actor, movie, TV show, etc.
    // "q" input parameter intentionally ignored (redundant with "qid", but sometimes containing a different string)
    [JsonPropertyName("qid")]
    public string? MediaType { get; init; }  // only for media, not people
    [JsonPropertyName("rank")]
    public int? Rank { get; init; }  // rank of this item in its category of results (I believe)
    [JsonPropertyName("s")]
    public required string KnownFor { get; init; }  // top two actors for media, best known work (or role) for people, who knows for others
    [JsonPropertyName("y")]
    public int? Year { get; init; }  // only for media, not people
    [JsonPropertyName("yr")]
    public string? Years { get; init; }  // only for TV series or TV mini series

    public override string ToString()
    {
        return $"Image:\n*****\n{Image}\n*****\nItemID: {ItemID}\nName: {Name}\nMediaType: {MediaType}\nRank: {Rank}\nKnownFor: {KnownFor}\nYear: {Year}\nYears: {Years}";
    }
}
