using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record SuggestionImageViewModel
{
    public SuggestionImageViewModel(SuggestionImageDataModel suggestionImageDataModel,
                                    Guid? testGuid = null)
    {
        this.ID = testGuid ?? Guid.NewGuid();
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
    
    public Guid ID { get; }
    public int Height { get; }
    public string ImageURL { get; }
    public int Width { get; }

    public override string ToString()
    {
        return $"ID: {ID}\nHeight: {Height}\nImageURL: {ImageURL}\nWidth: {Width}";
    }
}
