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
    
    public int Height { get; init; }
    public string ImageURL { get; init; }
    public int Width { get; init; }

    public override string ToString()
    {
        return $"Height: {Height}\nImageURL: {ImageURL}\nWidth: {Width}";
    }
}
