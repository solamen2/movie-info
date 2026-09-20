import { MediaTypes, type MediaType } from "./constants";

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
