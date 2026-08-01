using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record SuggestionViewModel
{
    public SuggestionViewModel(SuggestionDataModel suggestionDataModel,
                               Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
        if (suggestionDataModel.Image == null)
        {
            this.Image = null;
        }
        else
        {
            this.Image = new SuggestionImageViewModel(suggestionDataModel.Image, testGuid);
        }
        this.ItemID = suggestionDataModel.ItemID;
        this.Name = suggestionDataModel.Name;
        this.SearchType = SearchResultType.GetSearchType(suggestionDataModel.ItemID);
        if (this.SearchType == SearchResultType.Person)
        {
            this.MediaType = null;
        }
        else
        {
            this.MediaType = MediaResultType.GetMediaType(suggestionDataModel.MediaType);
        }
        this.Rank = suggestionDataModel.Rank;
        this.KnownFor = suggestionDataModel.KnownFor;
        this.Year = suggestionDataModel.Year;
        this.Years = suggestionDataModel.Years;
    }

    public Guid ID { get; }
    public SuggestionImageViewModel? Image { get; }
    public string ItemID { get; }
    public string Name { get; }
    public SearchResultType? SearchType { get; }
    public MediaResultType? MediaType { get; }
    public int? Rank { get; }
    public string KnownFor { get; }
    public int? Year { get; }
    public string? Years { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nImage:\n*****\n{Image}\n*****\nItemID: {ItemID}\nName: {Name}\nSearchType: {SearchType}\nMediaType: {MediaType}\nRank: {Rank}\nKnownFor: {KnownFor}\nYear: {Year}\nYears: {Years}";
    }
}
