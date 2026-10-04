import { useEffect, useState } from "react";
import type { Movie } from "../movie/movieTypes";
import type { Person } from "../person/personTypes";
import type { TvEpisode } from "../tvepisode/tvEpisodeTypes";
import type { TvSeason } from "../tvseason/tvSeasonTypes";
import type { TvSeries } from "../tvseries/tvSeriesTypes";
import { describeFailedLoad } from "../utilities/utilities";

// The kinds of detail a panel can load, by the name used for them in messages
// (e.g. "Loading TV season failed…"). The name picks the response's type.
export interface DetailKinds {
  movie: Movie;
  person: Person;
  "TV series": TvSeries;
  "TV season": TvSeason;
  "TV episode": TvEpisode;
}

// Responses by URL, for as long as the page is open. A panel the user comes
// back to (e.g. with the browser's back button, from a panel opened from one
// of its cards) is then shown fully formed at once, which the panel
// transitions rely on (see ReplaceMode in useCardSelection). The backend
// caches the same responses for longer than this.
const cache = new Map<string, unknown>();

// For tests, which mock different responses for the same URL.
export function clearDetailDataCache() {
  cache.clear();
}

interface DetailData<K extends keyof DetailKinds> {
  url: string;
  data: DetailKinds[K] | null;
  error: string;
}

// Loads a detail panel's data from one of the /api/… URLs. `what` names the
// kind of item, for messages and for the type of the data (see DetailKinds).
export function useDetailData<K extends keyof DetailKinds>(
  url: string,
  what: K,
): { data: DetailKinds[K] | null; error: string } {
  const [state, setState] = useState<DetailData<K>>(() => ({
    url,
    data: (cache.get(url) as DetailKinds[K] | undefined) ?? null,
    error: "",
  }));

  // A new URL means new data; sync during render so the very next paint
  // already reflects it (see "Adjusting some state when a prop changes" in
  // the React docs).
  if (state.url !== url) {
    setState({
      url,
      data: (cache.get(url) as DetailKinds[K] | undefined) ?? null,
      error: "",
    });
  }

  useEffect(() => {
    if (cache.has(url)) return;
    const controller = new AbortController();

    async function load() {
      try {
        const response = await fetch(url, { signal: controller.signal });
        if (!response.ok) {
          setState({
            url,
            data: null,
            error: await describeFailedLoad(`Loading ${what}`, response),
          });
          return;
        }
        const data = (await response.json()) as DetailKinds[K] | null; // TODO: Maybe someday make this validation more robust
        if (data == null) {
          setState({
            url,
            data: null,
            error: `No ${what} details were found.`,
          });
          return;
        }
        cache.set(url, data);
        setState({ url, data, error: "" });
      } catch {
        if (!controller.signal.aborted) {
          setState({
            url,
            data: null,
            error: `An unexpected error occurred while loading the ${what}.`,
          });
        }
      }
    }

    void load();
    return () => {
      controller.abort();
    };
  }, [url, what]);

  return state;
}
