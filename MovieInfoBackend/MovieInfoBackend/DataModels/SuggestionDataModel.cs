using System.Text.Json.Serialization;

namespace MovieInfoBackend.DataModels;

public class SuggestionDataModel
{
    [JsonPropertyName("i")]
    public SuggestionImageDataModel? Image { get; set; }
    [JsonPropertyName("id")]
    public required string ItemID { get; set;}
    [JsonPropertyName("l")]
    public required string Name { get; set; }  // name of actor, movie, TV show, etc.
    // "q" input parameter intentionally ignored (redundant with "qid", but sometimes containing a different string)
    [JsonPropertyName("qid")]
    public string? MediaType { get; set; }  // only for media, not people
    [JsonPropertyName("rank")]
    public required int Rank { get; set; }  // rank of this item in its category of results (I believe)
    [JsonPropertyName("s")]
    public required string KnownFor { get; set; }  // top two actors for media, best known work (or role) for people
    //public string? Plot { get; set; }
    [JsonPropertyName("y")]
    public int? Year { get; set; }  // only for media, not people
    [JsonPropertyName("yr")]
    public string? Years { get; set; }  // only for TV series or TV mini series

    public override string ToString()
    {
        return $"Image:\n*****\n{Image}\n*****\nItemID: {ItemID}\nName: {Name}\nMediaType: {MediaType}\nRank: {Rank}\nKnownFor: {KnownFor}\nYear: {Year}";
    }
}
