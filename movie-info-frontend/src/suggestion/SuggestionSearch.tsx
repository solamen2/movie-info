import { type SubmitEvent, useEffect, useEffectEvent, useState } from "react";
import { useNavigate, useParams, useSearchParams } from "react-router-dom";
import SuggestionSearchCard, { type Suggestion } from "./SuggestionSearchCard";
import { type ReplaceMode, useCardSelection } from "../shared/useCardSelection";
import {
  describeFailedLoad,
  detailPath,
  getPanelKind,
  searchPath,
  selectedCardPath,
  type PanelKind,
} from "../utilities/utilities";
import { MediaTypes, SEARCH_AUTO_SUBMIT_MS } from "../utilities/constants";

function panelKindOf(item: Suggestion): PanelKind | null {
  return getPanelKind(item.searchType, item.mediaType?.value ?? null);
}

// Stand-in result for a detail URL whose item isn't in the current results
// (e.g. a movie link opened directly, or a person opened from a movie's cast,
// who is only known by TMDB id). It only ever renders expanded, so the card's
// own details are never shown.
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
  // from the route's :itemId param (an IMDB or TMDB id, see isTmdbId).
  panelKind?: PanelKind;
}

// The URL is the source of truth for the search (`?q=`) and the selected card:
// a detail item (`/movie/:itemId` etc.) or a merely highlighted card without a
// panel (`?selected=`). Submitting a search or clicking a card only navigates;
// the component then syncs to the new URL, so browser back / forward and
// directly loaded URLs behave exactly like clicks. Cards inside a panel (a
// movie's cast, say) navigate to their own detail URL the same way (see
// useOpenDetail), so the new panel replaces the old one here, hosted by a
// stand-in card, and browser back returns to the old one, with the clicked
// card highlighted (`?cardType=…&cardId=…`, see usePanelCards).
function SuggestionSearch({ panelKind }: SuggestionSearchProps) {
  const { itemId } = useParams();
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const query = searchParams.get("q") ?? "";
  const targetItemId = panelKind
    ? (itemId ?? null)
    : searchParams.get("selected");
  // A URL that highlights a card inside the panel is where going back from a
  // panel opened from that card lands: the panel then shrinks into the card.
  // Any other switch between panels grows the new panel out of the card that
  // was flown to the corner at the previous URL (see useOpenDetail).
  const replaceMode: ReplaceMode =
    searchParams.get("cardType") !== null && searchParams.get("cardId") !== null
      ? "shrink"
      : "grow";

  const [searchQuery, setSearchQuery] = useState(query);
  // null until a search for the current query has succeeded.
  const [results, setResults] = useState<Suggestion[] | null>(null);
  // Bumped to re-run a search whose query is already in the URL.
  const [searchAttempt, setSearchAttempt] = useState(0);
  const [error, setError] = useState("");
  // The query the results above were last synced to.
  const [syncedQuery, setSyncedQuery] = useState(query);

  const { selectedId, previouslySelectedId, expandedId, growingId } =
    useCardSelection({
      targetId: targetItemId,
      opensPanel: panelKind !== undefined,
      hasCardFor: (id) => results?.some((r) => r.itemID === id) ?? false,
      // Every search (including re-running the same query) replaces the cards
      resetKey: `${String(searchAttempt)}:${query}`,
      replaceMode,
    });

  const searchMessage =
    results === null
      ? ""
      : results.length === 0
        ? "No results. Please try another search."
        : `${String(results.length)} results.`;
  const hasCardForSelected =
    results?.some((r) => r.itemID === selectedId) ?? false;
  const standIn =
    selectedId && panelKind && !hasCardForSelected
      ? placeholderSuggestion(selectedId, panelKind)
      : null;
  const displayedResults = standIn
    ? [standIn, ...(results ?? [])]
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

  // Typing a query and then pausing submits it, exactly like the Search
  // button. Every keystroke restarts the wait, and so does anything else that
  // changes the URL's query (e.g. submitting by hand), since the box is then
  // in sync with the URL again.
  const onTypingPaused = useEffectEvent(() => {
    void navigate(searchPath(searchQuery));
  });

  useEffect(() => {
    if (searchQuery === query || searchQuery.trim() === "") return;
    const timeoutId = window.setTimeout(() => {
      onTypingPaused();
    }, SEARCH_AUTO_SUBMIT_MS);
    return () => {
      window.clearTimeout(timeoutId);
    };
  }, [searchQuery, query]);

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
            growIn={growingId === item.itemID}
            standIn={item === standIn}
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
