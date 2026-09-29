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

interface CalendarDate {
  year: number;
  month: number;
  day: number;
}

function parseIsoDate(isoDate: string): CalendarDate | null {
  const match = /^(\d{4})-(\d{2})-(\d{2})$/.exec(isoDate);
  if (!match) return null;
  const [year, month, day] = match.slice(1).map(Number);
  return { year, month, day };
}

// A person's age in whole years, rounded down: at their death if they have
// died, otherwise as of `today` in the user's own time zone. Null when the age
// can't be known (no birthday, or a date that can't be read) or would be
// negative.
export function getAge(
  birthday: string | null,
  deathday: string | null,
  today: Date = new Date(),
): number | null {
  if (birthday == null) return null;
  const born = parseIsoDate(birthday);
  const until =
    deathday == null
      ? {
          year: today.getFullYear(),
          month: today.getMonth() + 1,
          day: today.getDate(),
        }
      : parseIsoDate(deathday);
  if (!born || !until) return null;

  const hadBirthdayThatYear =
    until.month > born.month ||
    (until.month === born.month && until.day >= born.day);
  const age = until.year - born.year - (hadBirthdayThatYear ? 0 : 1);
  return age < 0 ? null : age;
}

// e.g. "(49)", or "(<1)" for someone who has not reached their first birthday.
export function displayAge(age: number): string {
  return `(${age < 1 ? "<1" : String(age)})`;
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

// The message carried by an error response, if any. MovieInfoBackend returns
// `Results.NotFound("...")`, which serializes as a JSON string; other errors
// come as JSON objects (e.g. `{ message }`, or ASP.NET problem details with
// `title` / `detail`) or as plain text.
export async function getErrorResponseMessage(
  response: Response,
): Promise<string | null> {
  const text = (await response.text().catch(() => "")).trim();
  if (text === "") return null;
  try {
    const body: unknown = JSON.parse(text);
    if (typeof body === "string") return body || null;
    if (body !== null && typeof body === "object") {
      for (const key of ["message", "error", "title", "detail"]) {
        const value = (body as Record<string, unknown>)[key];
        if (typeof value === "string" && value !== "") return value;
      }
    }
    return null;
  } catch {
    return text;
  }
}

// A sentence describing a failed response, e.g.
// "Loading movie failed with status 404: Movie 'tt1' was not found."
export async function describeFailedResponse(
  action: string,
  response: Response,
): Promise<string> {
  const status = `${action} failed with status ${String(response.status)}`;
  const message = await getErrorResponseMessage(response);
  if (message === null) return `${status}.`;
  return `${status}: ${message}${/[.!?]$/.test(message) ? "" : "."}`;
}

// describeFailedResponse followed by advice on what to do next. Retrying
// can't help when the item doesn't exist (404), so that case suggests
// searching for something else instead.
export async function describeFailedLoad(
  action: string,
  response: Response,
): Promise<string> {
  const advice =
    response.status === 404
      ? "Please try another search."
      : "Please try again.";
  return `${await describeFailedResponse(action, response)} ${advice}`;
}
