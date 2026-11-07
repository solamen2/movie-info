import { useEffect, useState } from "react";
import type { Inflation } from "./inflationTypes";
import { describeFailedResponse } from "../utilities/utilities";

export const INFLATION_URL = "/api/inflation";

// The response, for as long as the page is open (the backend caches it for a
// week), and the request for it while it is on its way, so that a movie's
// budget and revenue (which both need it) share one request. A failure isn't
// kept: the next panel tries again.
let cached: Inflation | null = null;
let pending: Promise<Inflation> | null = null;

// For tests, which mock different responses for the same URL.
export function clearInflationDataCache() {
  cached = null;
  pending = null;
}

async function fetchInflation(): Promise<Inflation> {
  const response = await fetch(INFLATION_URL);
  if (!response.ok) {
    throw new Error(
      await describeFailedResponse("Loading inflation data", response),
    );
  }
  const data = (await response.json()) as Inflation | null;
  if (data == null) {
    throw new Error("No inflation data was found.");
  }
  return data;
}

function loadInflation(): Promise<Inflation> {
  if (cached !== null) return Promise.resolve(cached);
  pending ??= fetchInflation()
    .then((data) => {
      cached = data;
      return data;
    })
    .finally(() => {
      pending = null;
    });
  return pending;
}

export interface InflationData {
  // Null while loading, and when loading failed (then `error` says why)
  data: Inflation | null;
  error: string;
}

// Loads the inflation data from /api/inflation (once per page).
export function useInflationData(): InflationData {
  const [state, setState] = useState<InflationData>(() => ({
    data: cached,
    error: "",
  }));

  useEffect(() => {
    let active = true;
    loadInflation().then(
      (data) => {
        if (active) setState({ data, error: "" });
      },
      (error: unknown) => {
        if (active) {
          setState({
            data: null,
            error:
              error instanceof Error
                ? error.message
                : "An unexpected error occurred while loading the inflation data.",
          });
        }
      },
    );
    return () => {
      active = false;
    };
  }, []);

  return state;
}
