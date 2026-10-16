namespace MovieInfoBackend.ViewModels;

// A film an Academy Award nomination was for; ImdbId is null when the dataset has none
public record OscarFilmViewModel(string Title, string? ImdbId)
{
    public override string ToString()
    {
        return $"Title: {Title}\nImdbId: {ImdbId}";
    }
}
