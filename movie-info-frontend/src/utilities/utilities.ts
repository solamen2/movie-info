import {
  MediaTypes,
  type MediaType,
  SEARCH_TYPE_LABELS,
  TMDB_IMAGE_BASE_URL,
} from "./constants";

export function canHaveMoviePanel(mediaType: MediaType | null): boolean {
  return (
    mediaType == MediaTypes.Movie ||
    mediaType == MediaTypes.TvMovie ||
    mediaType == MediaTypes.TvSpecial ||
    mediaType == MediaTypes.TvShort ||
    mediaType == MediaTypes.Short ||
    mediaType == MediaTypes.Video
  );
}

export function canHaveTvSeriesPanel(mediaType: MediaType | null): boolean {
  return (
    mediaType == MediaTypes.TvSeries || mediaType == MediaTypes.TvMiniSeries
  );
}

// Only people have a null media type, but check both to be safe.
export function canHavePersonPanel(
  isPerson: boolean,
  mediaType: MediaType | null,
): boolean {
  return isPerson && mediaType == null;
}

export function getSearchTypeLabel(searchType: number | string | null): string {
  if (searchType == null) return "—";
  return typeof searchType === "number"
    ? (SEARCH_TYPE_LABELS[searchType] ?? String(searchType))
    : searchType;
}

// OMDB uses "N/A" for missing values, and the backend substitutes empty
// strings when OMDB returns nothing at all.
export function displayText(value: string | number | null | undefined): string {
  if (value == null) return "—";
  const text = String(value).trim();
  return text === "" || text === "N/A" ? "—" : text;
}

export function displayRuntime(minutes: number): string {
  if (minutes <= 0) return "—";
  const hours = Math.floor(minutes / 60);
  const mins = minutes % 60;
  return hours > 0 ? `${String(hours)}h ${String(mins)}m` : `${String(mins)}m`;
}

// OMDB genre names (lowercased) that differ from TMDB's name for the same genre.
const OMDB_GENRE_ALIASES: Record<string, string> = {
  "sci-fi": "Science Fiction",
};

// OMDB and TMDB each supply a comma-separated genre list; merge them,
// preferring TMDB's spelling of each genre.
export function displayGenres(tmdbGenres: string, omdbGenres: string): string {
  const genres = new Map<string, string>();
  for (const genreList of [tmdbGenres, omdbGenres]) {
    if (displayText(genreList) === "—") continue;
    for (const genre of genreList.split(",")) {
      const trimmed = genre.trim();
      const name = OMDB_GENRE_ALIASES[trimmed.toLowerCase()] ?? trimmed;
      const key = name.toLowerCase();
      if (name !== "" && !genres.has(key)) {
        genres.set(key, name);
      }
    }
  }
  return displayText([...genres.values()].join(", "));
}

export function displayDate(isoDate: string | null): string {
  if (isoDate == null || !/^\d{4}-\d{2}-\d{2}$/.test(isoDate)) {
    return displayText(isoDate);
  }
  return new Date(`${isoDate}T00:00:00Z`).toLocaleDateString("en-US", {
    timeZone: "UTC",
    dateStyle: "medium",
  });
}

export function getTmdbImageUrl(
  path: string | null,
  size: "w92" | "w185" | "w500",
): string | null {
  return path ? `${TMDB_IMAGE_BASE_URL}/${size}${path}` : null;
}
