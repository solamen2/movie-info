// The buttons that narrow a person's crew credits down to one role. The
// roles are picked out the same way the backend does for a title's
// Directors and Writers (see MovieViewModel and TvSeriesViewModel); a TV
// series' creators come from TMDB's own list there, which the person's
// credits name with the job "Creator".

export const NO_CREW_FILTER = "No filter";

interface CrewCredit {
  department: string;
  job: string;
}

const CREW_FILTERS: Partial<Record<string, (credit: CrewCredit) => boolean>> = {
  Director: (credit) => credit.job === "Director",
  Writer: (credit) => credit.department === "Writing",
  Creator: (credit) => credit.job === "Creator",
};

export const MOVIE_CREW_FILTERS = [NO_CREW_FILTER, "Director", "Writer"];
export const TV_SERIES_CREW_FILTERS = [
  NO_CREW_FILTER,
  "Director",
  "Writer",
  "Creator",
];

export function matchesCrewFilter(filter: string, credit: CrewCredit): boolean {
  return CREW_FILTERS[filter]?.(credit) ?? true;
}

// What a filter's button says: the role and how many of the credits it
// leaves, e.g. "Director (5)"; "No filter" stands alone.
export function crewFilterLabel(filter: string, credits: CrewCredit[]): string {
  if (filter === NO_CREW_FILTER) return filter;
  const count = credits.filter((c) => matchesCrewFilter(filter, c)).length;
  return `${filter} (${String(count)})`;
}
