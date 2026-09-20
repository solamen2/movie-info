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
