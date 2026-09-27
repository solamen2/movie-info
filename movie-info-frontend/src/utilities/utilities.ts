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
  size: "w92" | "w185" | "w300" | "w500",
): string | null {
  return path ? `${TMDB_IMAGE_BASE_URL}/${size}${path}` : null;
}

// Which detail panel (and therefore which URL path) a search result opens.
// The segment doubles as the route path, e.g. "/movie/tt0111161".
export type PanelKind = "movie" | "tvseries" | "person";

export function getPanelKind(
  searchType: number | string | null,
  mediaType: MediaType | null,
): PanelKind | null {
  if (canHaveMoviePanel(mediaType)) return "movie";
  if (canHaveTvSeriesPanel(mediaType)) return "tvseries";
  const isPerson = getSearchTypeLabel(searchType) === "Person";
  if (canHavePersonPanel(isPerson, mediaType)) return "person";
  return null;
}

// The search query travels with every route as `?q=` so that the results grid
// can be rebuilt when a detail URL is loaded directly, and so that in-app and
// browser back both land on the same results.
function querySuffix(query: string): string {
  return query ? `?${new URLSearchParams({ q: query }).toString()}` : "";
}

export function searchPath(query: string): string {
  return `/search${querySuffix(query)}`;
}

// The current URL with some query parameters added, replaced (string) or
// removed (null). Used to select a card within an already open panel, e.g.
// `?tmdbTvSeriesId=95&seasonNumber=1`, without disturbing the rest.
export function withQueryParams(
  pathname: string,
  search: string,
  updates: Record<string, string | null>,
): string {
  const params = new URLSearchParams(search);
  for (const [name, value] of Object.entries(updates)) {
    if (value === null) {
      params.delete(name);
    } else {
      params.set(name, value);
    }
  }
  const suffix = params.toString();
  return suffix ? `${pathname}?${suffix}` : pathname;
}

// Reads a positive integer query parameter such as a season number.
export function getIntParam(
  params: URLSearchParams,
  name: string,
): number | null {
  const value = params.get(name);
  if (value === null || !/^\d+$/.test(value)) return null;
  return Number(value);
}

// A results card without a detail panel, highlighted in place.
export function selectedCardPath(query: string, itemId: string): string {
  return withQueryParams("/search", querySuffix(query), { selected: itemId });
}

export function detailPath(
  panelKind: PanelKind,
  imdbId: string,
  query: string,
): string {
  return `/${panelKind}/${encodeURIComponent(imdbId)}${querySuffix(query)}`;
}
