import type { TmdbGender, WatchProvider } from "../shared/sharedTypes";
import type { TvSeriesNetwork } from "../tvseries/tvSeriesTypes";

// Mirrors MovieInfoBackend's TvSeasonViewModel (and child view models) as
// serialized by System.Text.Json: camelCase properties, enums as strings,
// DateOnly as "yyyy-MM-dd". TMDB returns null for many image paths and dates
// even where the backend declares them non-nullable, so they are nullable here.

export interface TvSeasonEpisodeCrew {
  id: string;
  department: string;
  job: string;
  gender: TmdbGender;
  tmdbId: number;
  knownForDepartment: string;
  name: string;
  originalName: string;
  popularity: number;
  profilePath: string | null;
}

export interface TvSeasonEpisodeGuestStar {
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

// An episode as listed at the season level; TvEpisode (in ../tvepisode) is the
// fuller record loaded once an episode is selected.
export interface TvSeasonEpisode {
  id: string;
  airDateString: string | null;
  airDate: string | null;
  episodeNumber: number;
  episodeType: string;
  tmdbId: number;
  title: string;
  tmdbOverview: string | null;
  runtime: number;
  seasonNumber: number;
  tmdbTvSeriesId: number;
  stillPath: string | null;
  directors: TvSeasonEpisodeCrew[];
  writers: TvSeasonEpisodeCrew[];
  producers: TvSeasonEpisodeCrew[];
  guestStars: TvSeasonEpisodeGuestStar[];
}

export interface TvSeason {
  id: string;
  firstAirDateString: string | null;
  firstAirDate: string | null;
  episodes: TvSeasonEpisode[];
  name: string;
  networks: TvSeriesNetwork[];
  tmdbOverview: string | null;
  tmdbId: number;
  posterPath: string | null;
  seasonNumber: number;
  watchProvidersBuy: WatchProvider[];
  watchProvidersFlatrate: WatchProvider[];
  watchProvidersRent: WatchProvider[];
  numberOfEpisodes: number;
}

// TMDB episode types are lowercase identifiers such as "standard", "finale"
// and "mid_season".
export function displayEpisodeType(episodeType: string | null): string {
  if (!episodeType) return "—";
  return episodeType
    .split("_")
    .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
    .join(" ");
}

export function displaySeasonEpisode(
  seasonNumber: number,
  episodeNumber: number,
): string {
  return `Season ${String(seasonNumber)}, Episode ${String(episodeNumber)}`;
}
