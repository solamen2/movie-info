import type { SuggestionImage } from "../suggestion/SuggestionSearchCard";

// Mirrors MovieInfoBackend's MovieViewModel (and child view models) as
// serialized by System.Text.Json: camelCase properties, enums as strings.

export type TmdbGender = "NotSetNotSpecified" | "Female" | "Male" | "NonBinary";

export interface MovieCast {
  id: string;
  gender: TmdbGender;
  tmdbId: number;
  name: string;
  originalName: string;
  popularity: number;
  profilePath: string | null;
  character: string;
  billedOrder: number;
}

export interface MovieCrew {
  id: string;
  gender: TmdbGender;
  tmdbId: number;
  knownForDepartment: string;
  name: string;
  originalName: string;
  popularity: number;
  profilePath: string | null;
  department: string;
  job: string;
}

export interface WatchProvider {
  id: string;
  logoPath: string | null;
  providerName: string;
  displayPriority: number;
}

export interface Movie {
  id: string;
  image: SuggestionImage | null;
  imdbId: string;
  title: string;
  imdbRank: number | null;
  knownForActors: string;
  year: number | null;
  rated: string;
  omdbGenres: string;
  omdbPlot: string;
  awards: string;
  imdbRating: string;
  imdbVotes: string;
  boxOfficeString: string | null;
  boxOfficeNumber: number;
  budget: number;
  tmdbGenres: string;
  homepage: string | null;
  tmdbId: number;
  originCountries: string;
  originLanguage: string;
  originalTitle: string;
  tmdbPlot: string;
  productionCompanies: string | null;
  productionCountries: string;
  releaseDate: string;
  revenue: number;
  runtime: number;
  spokenLanguages: string;
  status: string;
  tagline: string | null;
  cast: MovieCast[];
  directors: MovieCrew[];
  writers: MovieCrew[];
  producers: MovieCrew[];
  watchProvidersBuy: WatchProvider[];
  watchProvidersFlatrate: WatchProvider[];
  watchProvidersRent: WatchProvider[];
}

export function imdbTitleUrl(imdbId: string): string {
  return `https://www.imdb.com/title/${imdbId}`;
}
