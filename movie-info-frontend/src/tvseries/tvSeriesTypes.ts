import type { TmdbGender, WatchProvider } from "../shared/sharedTypes";
import type { SuggestionImage } from "../suggestion/SuggestionSearchCard";

// Mirrors MovieInfoBackend's TvSeriesViewModel (and child view models) as
// serialized by System.Text.Json: camelCase properties, enums as strings,
// DateOnly as "yyyy-MM-dd". TMDB returns null for many image paths and dates
// even where the backend declares them non-nullable, so they are nullable here.

// TODO: Fix image / date non-nullable paths

export interface TvSeriesCreator {
  id: string;
  tmdbId: number;
  name: string;
  originalName: string;
  gender: TmdbGender;
  profilePath: string | null;
}

export interface TvSeriesSeason {
  id: string;
  firstAirDateString: string | null;
  firstAirDate: string | null;
  episodeCount: number;
  tmdbId: number;
  name: string;
  overview: string | null;
  posterPath: string | null;
  seasonNumber: number;
}

export interface TvSeriesNetwork {
  id: string;
  tmdbId: number;
  logoPath: string | null;
  name: string;
  originCountry: string;
}

export interface TvSeriesCast {
  id: string;
  gender: TmdbGender;
  tmdbId: number;
  name: string;
  originalName: string;
  popularity: number;
  profilePath: string | null;
  characters: string[];
  totalEpisodeCount: number;
  billedOrder: number;
}

export interface TvSeriesCrew {
  id: string;
  gender: TmdbGender;
  tmdbId: number;
  knownForDepartment: string;
  name: string;
  originalName: string;
  popularity: number;
  profilePath: string | null;
  jobs: string[];
  department: string;
  totalEpisodeCount: number;
}

export interface TvSeries {
  id: string;
  image: SuggestionImage | null;
  imdbId: string;
  name: string;
  imdbRank: number | null;
  knownForActors: string | null;
  firstYear: number | null;
  years: string | null;
  rated: string;
  omdbAverageEpisodeRuntimeString: string;
  omdbAverageEpisodeRuntimeNumber: number;
  omdbGenres: string;
  omdbOverview: string;
  awards: string;
  imdbRating: string;
  imdbVotes: string;
  backdropPath: string | null;
  creators: TvSeriesCreator[];
  tmdbEpisodeRunTimes: number[];
  firstAirDateString: string | null;
  firstAirDate: string | null;
  tmdbGenres: string;
  homepage: string | null;
  tmdbId: number;
  isInProduction: boolean;
  languages: string;
  lastAirDateString: string | null;
  lastAirDate: string | null;
  nextAirDateString: string | null;
  nextAirDate: string | null;
  networks: TvSeriesNetwork[];
  numberOfEpisodes: number;
  numberOfSeasons: number;
  originCountries: string;
  originLanguage: string;
  originalName: string;
  tmdbOverview: string;
  productionCompanies: string;
  productionCountries: string;
  seasons: TvSeriesSeason[];
  spokenLanguages: string;
  status: string;
  tagline: string | null;
  tvSeriesType: string;
  cast: TvSeriesCast[];
  directors: TvSeriesCrew[];
  writers: TvSeriesCrew[];
  producers: TvSeriesCrew[];
  watchProvidersBuy: WatchProvider[];
  watchProvidersFlatrate: WatchProvider[];
  watchProvidersRent: WatchProvider[];
}

export function displayEpisodeCount(count: number): string {
  return `${String(count)} ${count === 1 ? "episode" : "episodes"}`;
}

// TMDB lists specials as season 0; show them after the numbered seasons.
export function sortSeasons(seasons: TvSeriesSeason[]): TvSeriesSeason[] {
  const seasonSortKey = (s: TvSeriesSeason) =>
    s.seasonNumber === 0 ? Number.MAX_SAFE_INTEGER : s.seasonNumber;
  return [...seasons].sort((a, b) => seasonSortKey(a) - seasonSortKey(b));
}
