// Mirrors MediaResultType on backend
export const MediaTypes = {
  Movie: "Movie",
  TvSeries: "TV Series",
  TvMiniSeries: "TV Mini Series",
  TvMovie: "TV Movie",
  TvEpisode: "TV Episode", // Not used by Suggestions API
  TvSpecial: "TV Special",
  TvShort: "TV Short",
  Short: "Short",
  VideoGame: "Video Game",
  Video: "Video",
  MusicVideo: "Music Video",
  PodcastEpisode: "Podcast Episode", // Not used by Suggestions API
  PodcastSeries: "Podcast Series",
} as const;
export type MediaType = (typeof MediaTypes)[keyof typeof MediaTypes];

export interface MediaResultType {
  value: MediaType;
}

// Mirrors SearchResultType on backend, which may be serialized as either its
// name or its number.
export const SEARCH_TYPE_LABELS: Record<number, string> = {
  0: "Person",
  1: "Media",
};

// TMDB returns image paths relative to its image CDN (e.g. "/abc123.jpg").
// See https://developer.themoviedb.org/docs/image-basics
export const TMDB_IMAGE_BASE_URL = "https://image.tmdb.org/t/p";
