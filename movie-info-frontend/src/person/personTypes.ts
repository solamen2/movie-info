import type { TmdbGender } from "../movie/movieTypes";
import type { SuggestionImage } from "../suggestion/SuggestionSearchCard";

// Mirrors MovieInfoBackend's PersonViewModel (and child view models) as
// serialized by System.Text.Json: camelCase properties, enums as strings.
// TMDB returns null for many of the image paths and dates even where the
// backend declares them non-nullable, so they are nullable here.
// TODO: Fix the backend's non-nullable dates / image paths

export interface PersonMovieCast {
  id: string;
  stillPath: string | null;
  tmdbId: number;
  title: string;
  originalTitle: string;
  popularity: number;
  posterPath: string | null;
  releaseDate: string | null;
  isVideo: boolean;
  character: string;
}

export interface PersonMovieCrew {
  id: string;
  stillPath: string | null;
  tmdbId: number;
  title: string;
  originalTitle: string;
  popularity: number;
  posterPath: string | null;
  releaseDate: string | null;
  isVideo: boolean;
  department: string;
  job: string;
}

export interface PersonTvSeriesCast {
  id: string;
  backdropPath: string | null;
  tmdbId: number;
  originalName: string;
  popularity: number;
  posterPath: string | null;
  firstAirDate: string | null;
  name: string;
  character: string;
  episodeCount: number;
  firstCreditAirDate: string | null;
}

export interface PersonTvSeriesCrew {
  id: string;
  backdropPath: string | null;
  tmdbId: number;
  originalName: string;
  popularity: number;
  posterPath: string | null;
  firstAirDate: string | null;
  name: string;
  department: string;
  episodeCount: number;
  firstCreditAirDate: string | null;
  job: string;
}

export interface PersonProfileImage {
  id: string;
  height: number;
  filePath: string;
  width: number;
}

export interface Person {
  id: string;
  image: SuggestionImage | null;
  imdbId: string;
  name: string;
  imdbRank: number | null;
  knownForMovies: string | null;
  alsoKnownAs: string[];
  biography: string | null;
  birthday: string | null;
  deathday: string | null;
  gender: TmdbGender;
  homepage: string | null;
  tmdbId: number;
  knownForDepartment: string;
  placeOfBirth: string | null;
  profilePath: string | null;
  movieCastCredits: PersonMovieCast[];
  movieCrewCredits: PersonMovieCrew[];
  tvSeriesCastCredits: PersonTvSeriesCast[];
  tvSeriesCrewCredits: PersonTvSeriesCrew[];
  profileImages: PersonProfileImage[];
}

export function imdbNameUrl(imdbId: string): string {
  return `https://www.imdb.com/name/${imdbId}`;
}

// Newest first, with undated credits (usually unreleased) at the end.
export function sortByDateDescending<T>(
  credits: T[],
  getDate: (credit: T) => string | null,
): T[] {
  return [...credits].sort((a, b) =>
    (getDate(b) ?? "").localeCompare(getDate(a) ?? ""),
  );
}
