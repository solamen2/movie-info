namespace MovieInfoBackend.ViewModels;

// One of the aspect ratios a movie or TV series was made or shown in (see AspectRatiosHelper)
public record AspectRatioViewModel
{
    public AspectRatioViewModel(string ratio, string decimalRatio, string? name, string description, string? wikipediaLink)
    {
        this.Ratio = ratio;
        this.Decimal = decimalRatio;
        this.Name = name;
        this.Description = description;
        this.WikipediaLink = wikipediaLink;
    }

    public string Ratio { get; }  // As usually written, e.g. "4:3" or "2.39:1"
    public string Decimal { get; }  // The width divided by the height, to two decimal places, e.g. "1.33"
    public string? Name { get; }  // A short common name, e.g. "Fullscreen"; null when the ratio has none
    public string Description { get; }  // A sentence or two about the ratio; there is always one, even with no name
    public string? WikipediaLink { get; }  // The Wikipedia page about the ratio (or the format it is named for); null when there is none

    public override string ToString()
    {
        return $"Ratio: {Ratio}\nDecimal: {Decimal}\nName: {Name}\nDescription: {Description}\nWikipediaLink: {WikipediaLink}";
    }
}
