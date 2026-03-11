// NOTE: Yeah, not technically an enum, but I wanted the string mapping to the IMDB API and it's very similar

using Serilog;

public class MediaResultType
{
    private MediaResultType (string value) { Value = value;}

    public string Value { get; private set; }
    public static MediaResultType Movie { get { return new MediaResultType("Movie"); } }
    public static MediaResultType TVSeries { get { return new MediaResultType("TV Series"); } }
    public static MediaResultType TVMiniSeries { get { return new MediaResultType("TV Mini Series"); } }
    public static MediaResultType TVMovie { get { return new MediaResultType("TV Movie"); } }
    public static MediaResultType TVEpisode { get { return new MediaResultType("TV Episode"); } }
    public static MediaResultType TVSpecial { get { return new MediaResultType("TV Special"); } }
    public static MediaResultType TVShort { get { return new MediaResultType("TV Short"); } }
    public static MediaResultType Short { get { return new MediaResultType("Short"); } }
    public static MediaResultType VideoGame { get { return new MediaResultType("Video Game"); } }
    public static MediaResultType Video { get { return new MediaResultType("Video"); } }
    public static MediaResultType MusicVideo { get { return new MediaResultType("Music Video"); } }
    public static MediaResultType PodcastEpisode { get { return new MediaResultType("Podcast Episode"); } }
    public static MediaResultType PodcastSeries { get { return new MediaResultType("Podcast Series"); } }

    public override string ToString()
    {
        return Value;
    }

    public static MediaResultType? GetMediaType(string? mediaType)
    {
        switch (mediaType)  // From IMDB 
        {
            case "movie":
                return Movie;
            case "tvSeries":
                return TVSeries;
            case "tvMiniSeries":
                return TVMiniSeries;
            case "tvMovie":
                return TVMovie;
            case "tvEpisode":  // NOTE: Does not seem to be used in IMDB's suggestion API
                return TVEpisode;
            case "tvSpecial":
                return TVSpecial;
            case "tvShort":
                return TVShort;
            case "short":
                return Short;
            case "videoGame":
                return VideoGame;
            case "video":
                return Video;
            case "musicVideo":
                return MusicVideo;
            case "podcastEpisode":
                return PodcastEpisode;  // NOTE: Does not seem to be used in IMDB's suggestion API
            case "podcastSeries":
                return PodcastSeries;
            default:
            {
                Log.Warning("Media result type is invalid: '" + mediaType + "'.");
                return null;
            }
        }
    }
}