// Types shared by several of MovieInfoBackend's view models, as serialized by
// System.Text.Json: camelCase properties, enums as strings.

export type TmdbGender = "NotSetNotSpecified" | "Female" | "Male" | "NonBinary";

export interface WatchProvider {
  id: string;
  logoPath: string | null;
  providerName: string;
  displayPriority: number;
}
