using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record SuggestionImageViewModel
{
    public SuggestionImageViewModel(SuggestionImageDataModel suggestionImageDataModel)
    {
        this.Height = suggestionImageDataModel.Height;
        this.ImageURL = suggestionImageDataModel.ImageURL;
        this.Width = suggestionImageDataModel.Width;
    }
    
    public int Height;
    public string ImageURL;
    public int Width;

    public override string ToString()
    {
        return $"Height: {Height}\nImageURL: {ImageURL}\nWidth: {Width}";
    }
}
