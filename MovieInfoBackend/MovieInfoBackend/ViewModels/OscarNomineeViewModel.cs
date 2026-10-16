namespace MovieInfoBackend.ViewModels;

// A person (or company) named in an Academy Award nomination; ImdbId is null when the dataset has none
public record OscarNomineeViewModel(string Name, string? ImdbId)
{
    public override string ToString()
    {
        return $"Name: {Name}\nImdbId: {ImdbId}";
    }
}
