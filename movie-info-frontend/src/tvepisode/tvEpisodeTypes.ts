import type { TmdbGender } from "../shared/sharedTypes";

// Mirrors MovieInfoBackend's TvEpisodeViewModel (and child view models) as
// serialized by System.Text.Json: camelCase properties, enums as strings,
// DateOnly as "yyyy-MM-dd". TMDB returns null for many image paths and dates
// even where the backend declares them non-nullable, so they are nullable here.

// TODO: Fix the non-nullable image paths and dates

export interface TvEpisodeCast {
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

export interface TvEpisodeCrew {
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

export interface TvEpisodeGuestStar {
  id: string;
  character: string;
  billedOrder: number;
  gender: TmdbGender;
  tmdbId: number;
  name: string;
  originalName: string;
  popularity: number;
  profilePath: string | null;
}

export interface TvEpisode {
  id: string;
  year: number;
  rated: string;
  omdbAverageEpisodeRuntimeString: string;
  omdbAverageEpisodeRuntimeNumber: number;
  omdbGenres: string;
  knownForActors: string;
  omdbOverview: string;
  awards: string;
  imdbRating: string;
  imdbVotes: string;
  imdbId: string;
  airDateString: string | null;
  airDate: string | null;
  episodeNumber: number;
  episodeType: string;
  title: string;
  tmdbOverview: string | null;
  tmdbId: number;
  runtime: number;
  seasonNumber: number;
  stillPath: string | null;
  cast: TvEpisodeCast[];
  directors: TvEpisodeCrew[];
  writers: TvEpisodeCrew[];
  producers: TvEpisodeCrew[];
  guestStars: TvEpisodeGuestStar[];
}
