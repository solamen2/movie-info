// Types shared by several of MovieInfoBackend's view models, as serialized by
// System.Text.Json: camelCase properties, enums as strings.

export type TmdbGender = "NotSetNotSpecified" | "Female" | "Male" | "NonBinary";

export interface WatchProvider {
  id: string;
  logoPath: string | null;
  providerName: string;
  displayPriority: number;
}

// One of the aspect ratios a movie or TV series was made or shown in, from
// Wikidata, with the name and description the backend keeps for it.
export interface AspectRatio {
  // As usually written, e.g. "4:3" or "2.39:1"
  ratio: string;
  // The width divided by the height, to two decimal places, e.g. "1.33"
  decimal: string;
  // A short common name, e.g. "Fullscreen"; null when the ratio has none
  name: string | null;
  // A sentence or two about the ratio (there is one even without a name)
  description: string;
  // The Wikipedia page about the ratio (or the format it is named for); null
  // when there is none
  wikipediaLink: string | null;
}
