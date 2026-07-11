using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record SuggestionImageViewModel
{
    public SuggestionImageViewModel(SuggestionImageDataModel suggestionImageDataModel)
    {
        if (suggestionImageDataModel.Height < 0)
        {
            throw new ArgumentException("Image height must be a number greater than 0!");
        }
        this.Height = suggestionImageDataModel.Height;
        this.ImageURL = suggestionImageDataModel.ImageURL;
        this.Width = suggestionImageDataModel.Width;
        if (suggestionImageDataModel.Width < 0)
        {
            throw new ArgumentException("Image width must be a number greater than 0!");
        }
    }
    
    public int Height { get; }
    public string ImageURL { get; }
    public int Width { get; }

    public override string ToString()
    {
        return $"Height: {Height}\nImageURL: {ImageURL}\nWidth: {Width}";
    }
}
