import { type SubmitEvent, useEffect, useEffectEvent, useState } from "react";
import { useNavigate, useParams, useSearchParams } from "react-router-dom";
import SuggestionSearchCard, { type Suggestion } from "./SuggestionSearchCard";
import { useCardSelection } from "../shared/useCardSelection";
import {
  describeFailedLoad,
  detailPath,
  getPanelKind,
  searchPath,
  selectedCardPath,
  type PanelKind,
} from "../utilities/utilities";
import { MediaTypes } from "../utilities/constants";

function panelKindOf(item: Suggestion): PanelKind | null {
  return getPanelKind(item.searchType, item.mediaType?.value ?? null);
}

// Stand-in result for a detail URL whose item isn't in the current results
// (e.g. a movie link opened directly). It only ever renders expanded, so the
// card's own details are never shown.
function placeholderSuggestion(
  itemId: string,
  panelKind: PanelKind,
): Suggestion {
  return {
    id: itemId,
    image: null,
    itemID: itemId,
    name: "",
    searchType: panelKind === "person" ? 0 : 1,
    mediaType:
      panelKind === "movie"
        ? { value: MediaTypes.Movie }
        : panelKind === "tvseries"
          ? { value: MediaTypes.TvSeries }
          : null,
    rank: null,
    knownFor: "",
    year: null,
    years: null,
  };
}

interface SuggestionSearchProps {
  // Set by the /movie, /tvseries and /person routes; the item to show comes
  // from the route's :imdbId param.
  panelKind?: PanelKind;
}

// The URL is the source of truth for the search (`?q=`) and the selected card:
// a detail item (`/movie/:imdbId` etc.) or a merely highlighted card without a
// panel (`?selected=`). Submitting a search or clicking a card only navigates;
// the component then syncs to the new URL, so browser back / forward and
// directly loaded URLs behave exactly like clicks.
function SuggestionSearch({ panelKind }: SuggestionSearchProps) {
  const { imdbId } = useParams();
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const query = searchParams.get("q") ?? "";
  const targetItemId = panelKind
    ? (imdbId ?? null)
    : searchParams.get("selected");

  const [searchQuery, setSearchQuery] = useState(query);
  // null until a search for the current query has succeeded.
  const [results, setResults] = useState<Suggestion[] | null>(null);
  // Bumped to re-run a search whose query is already in the URL.
  const [searchAttempt, setSearchAttempt] = useState(0);
  const [error, setError] = useState("");
  // The query the results above were last synced to.
  const [syncedQuery, setSyncedQuery] = useState(query);

  const { selectedId, previouslySelectedId, expandedId } = useCardSelection({
    targetId: targetItemId,
    opensPanel: panelKind !== undefined,
    hasCardFor: (id) => results?.some((r) => r.itemID === id) ?? false,
    // Every search (including re-running the same query) replaces the cards
    resetKey: `${String(searchAttempt)}:${query}`,
  });

  const searchMessage =
    results === null
      ? ""
      : results.length === 0
        ? "No results. Please try another search."
        : `${String(results.length)} results.`;
  const hasCardForSelected =
    results?.some((r) => r.itemID === selectedId) ?? false;
  const displayedResults =
    selectedId && panelKind && !hasCardForSelected
      ? [placeholderSuggestion(selectedId, panelKind), ...(results ?? [])]
      : (results ?? []);

  // A new query means a new page of results; sync during render so the very
  // next paint already reflects it (see "Adjusting some state when a prop
  // changes" in the React docs).
  if (query !== syncedQuery) {
    setSyncedQuery(query);
    setSearchQuery(query);
    setResults(null);
    setError("");
  }

  useEffect(() => {
    if (query === "") return;
    const controller = new AbortController();

    async function runSearch() {
      try {
        // TODO: Add a pending indicator while the search is running

        const response = await fetch(
          `/api/search?searchQuery=${encodeURIComponent(query)}`,
          { signal: controller.signal },
        );

        if (!response.ok) {
          setError(await describeFailedLoad("Search", response));
          return;
        }

        const data: Suggestion[] = (await response.json()) as Suggestion[]; // TODO: Maybe someday make this validation more robust
        setResults(data);
      } catch {
        if (!controller.signal.aborted) {
          setError("An unexpected error occurred. Please try again.");
        }
      }
    }

    void runSearch();
    return () => {
      controller.abort();
    };
  }, [query, searchAttempt]);

  // ESC does exactly what the browser's back button does, everywhere.
  const onEscape = useEffectEvent(() => {
    void navigate(-1);
  });

  useEffect(() => {
    function onKeyDown(e: KeyboardEvent) {
      if (e.key === "Escape") {
        onEscape();
      }
    }
    window.addEventListener("keydown", onKeyDown);
    return () => {
      window.removeEventListener("keydown", onKeyDown);
    };
  }, []);

  function handleSearch(e: SubmitEvent) {
    e.preventDefault();
    if (searchQuery !== query || targetItemId) {
      void navigate(searchPath(searchQuery));
    }
    if (searchQuery === query) {
      // Same query as the URL, so the sync above won't fire; reset and
      // re-fetch here instead.
      setResults(null);
      setError("");
      setSearchAttempt((n) => n + 1);
    }
  }

  function handleCardClick(item: Suggestion) {
    const itemId = item.itemID;
    if (targetItemId === itemId) {
      // Clicking the selected card again undoes the navigation that selected
      // it, exactly like the browser's back button.
      void navigate(-1);
      return;
    }
    const itemPanelKind = panelKindOf(item);
    void navigate(
      itemPanelKind
        ? detailPath(itemPanelKind, itemId, query)
        : selectedCardPath(query, itemId),
    );
  }

  async function handleLogout() {
    try {
      const response = await fetch("/api/logout", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({}),
      });

      if (!response.ok) {
        setError("Logout failed. Redirecting to login page...");
        await new Promise((f) => setTimeout(f, 5000));
      }
      await navigate("/login");
    } catch {
      setError("An unexpected logout error occurred. Please try again.");
    }
  }

  return (
    <div className="search-page">
      <div className="search-header">
        <form className="search-bar" onSubmit={handleSearch}>
          <input
            aria-label="search-query-input"
            type="text"
            placeholder="Search movies, people..."
            value={searchQuery}
            onChange={(e) => {
              setSearchQuery(e.target.value);
            }}
          />
          <button aria-label="search" type="submit">
            Search
          </button>
        </form>
        <button
          aria-label="logout"
          className="logout-button"
          onClick={handleLogout}
        >
          Logout
        </button>
      </div>

      {error && <p className="error-message">{error}</p>}
      <div className="results-toolbar">
        {!selectedId && searchMessage && (
          <p className="search-message">{searchMessage}</p>
        )}
      </div>

      <div
        className={`results-container fly-origin${selectedId ? " has-selection" : ""}`}
      >
        {displayedResults.map((item) => (
          <SuggestionSearchCard
            // Keyed by itemID so a placeholder card is seamlessly replaced by
            // the real result once the search finishes, keeping its panel
            // mounted.
            key={item.itemID}
            item={item}
            selected={selectedId === item.itemID}
            expanded={expandedId === item.itemID}
            deselecting={
              previouslySelectedId === item.itemID && selectedId !== item.itemID
            }
            mediaType={item.mediaType?.value ?? null}
            // The URL decides which panel the selected item opens; other cards
            // open whatever their own result type implies.
            panelKind={
              targetItemId === item.itemID && panelKind
                ? panelKind
                : panelKindOf(item)
            }
            onClick={() => {
              handleCardClick(item);
            }}
          />
        ))}
      </div>
    </div>
  );
}

export default SuggestionSearch;
