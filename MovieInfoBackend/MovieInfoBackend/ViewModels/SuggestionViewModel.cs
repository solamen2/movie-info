using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record SuggestionViewModel
{
    public SuggestionViewModel(SuggestionDataModel suggestionDataModel)
    {
        this.ID = Guid.NewGuid();
        if (suggestionDataModel.Image == null)
        {
            this.Image = null;
        }
        else
        {
            this.Image = new SuggestionImageViewModel(suggestionDataModel.Image);
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

    public Guid ID;
    public SuggestionImageViewModel? Image;
    public string ItemID;
    public string Name;
    public SearchResultType? SearchType;
    public MediaResultType? MediaType;
    public int? Rank;
    public string KnownFor;
    public int? Year;
    public string? Years;

    public override string ToString()
    {
        return $"ID: {ID}\nImage:\n*****\n{Image}\n*****\nItemID: {ItemID}\nName: {Name}\nSearchType: {SearchType}\nMediaType: {MediaType}\nRank: {Rank}\nKnownFor: {KnownFor}\nYear: {Year}\nYears: {Years}";
    }
}
