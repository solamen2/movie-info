import { type SubmitEvent, useEffect, useEffectEvent, useState } from "react";
import {
  useLocation,
  useNavigate,
  useParams,
  useSearchParams,
} from "react-router-dom";
import SuggestionSearchCard, { type Suggestion } from "./SuggestionSearchCard";
import {
  detailPath,
  getPanelKind,
  type PanelKind,
  searchPath,
} from "../utilities/utilities";
import { MediaTypes } from "../utilities/constants";

// Keep in sync with the `select-fly` / `deselect-fly` animation duration and
// the `.result-card` width transition duration in App.css.
const CARD_FLY_MS = 500;
const CARD_RESIZE_MS = 300;

// Set as navigation state when a card click pushes a detail URL, so the
// in-app back button knows it can pop history (matching the browser's back
// button) instead of pushing a fresh results URL.
interface DetailLocationState {
  fromSearch?: boolean;
}

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

// Delayed second half of a two-phase card animation: expand the selected card
// into its panel once it has flown to the corner, or fly the card home once
// its panel has shrunk back to card size.
type PendingPhase = "expand" | "deselect";

// Which card is selected and where it is in its animation. Always replaced as
// a whole via the helpers below, since the fields only make sense together.
interface Selection {
  selectedItemId: string | null;
  // The card the user just transitioned out of selected. It gets a
  // `.deselecting` class which fires the `deselect-fly` keyframe animation in
  // CSS — required because CSS animations only run on class addition, not on
  // class removal, so we can't rely on .selected alone to animate both
  // directions.
  previouslySelectedItemId: string | null;
  // A selected card with a detail panel animates in two phases: first it flies
  // to the upper-left (selectedItemId), then it grows into the panel
  // (expandedItemId). Going back runs the phases in reverse.
  expandedItemId: string | null;
  // A fresh object per transition so the timer effect restarts even when two
  // transitions in a row wait on the same phase.
  pendingPhase: { kind: PendingPhase } | null;
}

const NO_SELECTION: Selection = {
  selectedItemId: null,
  previouslySelectedItemId: null,
  expandedItemId: null,
  pendingPhase: null,
};

// Shown already expanded, with no animation (e.g. a directly loaded URL).
function expandedAtOnce(itemId: string | null): Selection {
  return { ...NO_SELECTION, selectedItemId: itemId, expandedItemId: itemId };
}

// The card flies to the corner and, if it opens a panel, expands afterwards.
function selectCard(
  prev: Selection,
  itemId: string,
  opensPanel: boolean,
): Selection {
  return {
    selectedItemId: itemId,
    previouslySelectedItemId: prev.selectedItemId,
    expandedItemId: null,
    pendingPhase: opensPanel ? { kind: "expand" } : null,
  };
}

// The card flies back home.
function deselectCard(prev: Selection): Selection {
  return { ...NO_SELECTION, previouslySelectedItemId: prev.selectedItemId };
}

// The panel shrinks back to card size, after which the card is deselected.
function shrinkPanel(prev: Selection): Selection {
  return { ...prev, expandedItemId: null, pendingPhase: { kind: "deselect" } };
}

function expandCard(prev: Selection): Selection {
  return { ...prev, expandedItemId: prev.selectedItemId, pendingPhase: null };
}

// The URL is the source of truth for both the search (`?q=`) and the selected
// detail item (`/movie/:imdbId` etc.). Submitting a search or clicking a card
// only navigates; the component then syncs its state to the new URL, so that
// browser back / forward and directly loaded URLs behave exactly like clicks.
function SuggestionSearch({ panelKind }: SuggestionSearchProps) {
  const { imdbId } = useParams();
  const [searchParams] = useSearchParams();
  const location = useLocation();
  const navigate = useNavigate();
  const query = searchParams.get("q") ?? "";
  const targetItemId = panelKind && imdbId ? imdbId : null;

  const [searchQuery, setSearchQuery] = useState(query);
  // null until a search for the current query has succeeded.
  const [results, setResults] = useState<Suggestion[] | null>(null);
  // Bumped to re-run a search whose query is already in the URL.
  const [searchAttempt, setSearchAttempt] = useState(0);
  const [error, setError] = useState("");
  const [selection, setSelection] = useState(() =>
    expandedAtOnce(targetItemId),
  );
  // The URL values the selection was last synced to.
  const [syncedUrl, setSyncedUrl] = useState({ query, targetItemId });

  const { selectedItemId, previouslySelectedItemId, expandedItemId } =
    selection;
  const searchMessage =
    results === null
      ? ""
      : results.length === 0
        ? "No results."
        : `${String(results.length)} results.`;
  const hasCardForSelected =
    results?.some((r) => r.itemID === selectedItemId) ?? false;
  const displayedResults =
    selectedItemId && panelKind && !hasCardForSelected
      ? [placeholderSuggestion(selectedItemId, panelKind), ...(results ?? [])]
      : (results ?? []);

  // Sync to the URL during render (see "Adjusting some state when a prop
  // changes" in the React docs), so the very next paint already reflects it.
  if (query !== syncedUrl.query) {
    // A new query means a new page of results, so any selection from the old
    // results is dropped without animating, and the URL's item (if any) is
    // shown expanded right away.
    setSyncedUrl({ query, targetItemId });
    setSearchQuery(query);
    setResults(null);
    setError("");
    setSelection(expandedAtOnce(targetItemId));
  } else if (targetItemId !== syncedUrl.targetItemId) {
    // The URL's item changed while the results stayed put (card click, in-app
    // back, or browser back / forward), so animate the card in or out.
    setSyncedUrl({ query, targetItemId });
    if (targetItemId) {
      setSelection(selectCard(selection, targetItemId, true));
    } else if (expandedItemId && hasCardForSelected) {
      setSelection(shrinkPanel(selection));
    } else {
      // A placeholder card has no home to fly back to, so just drop it.
      setSelection(deselectCard(selection));
    }
  }

  const pendingPhase = selection.pendingPhase;
  useEffect(() => {
    if (!pendingPhase) return;
    const isExpand = pendingPhase.kind === "expand";
    const timeoutId = window.setTimeout(
      () => {
        setSelection(isExpand ? expandCard : deselectCard);
      },
      isExpand ? CARD_FLY_MS : CARD_RESIZE_MS,
    );
    return () => {
      window.clearTimeout(timeoutId);
    };
  }, [pendingPhase]);

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
          // Error responses may carry a JSON body like { message: "..." }
          const body = (await response.json().catch(() => null)) as {
            message?: string;
          } | null;
          const status = `Search failed with status ${String(response.status)}`;
          setError(body?.message ? `${status}: ${body.message}` : `${status}.`);
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

  const onEscape = useEffectEvent(() => {
    handleBack();
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

  function handleBack() {
    if (targetItemId) {
      const state = location.state as DetailLocationState | null;
      if (state?.fromSearch) {
        void navigate(-1);
      } else {
        void navigate(searchPath(query));
      }
      return;
    }
    // Selected card without a detail panel: purely local state.
    setSelection(deselectCard(selection));
  }

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
      setSelection(NO_SELECTION);
      setSearchAttempt((n) => n + 1);
    }
  }

  function handleCardClick(item: Suggestion) {
    const itemId = item.itemID;
    const itemPanelKind = panelKindOf(item);
    if (itemPanelKind) {
      if (targetItemId === itemId) {
        // Clicked again mid-fly: treat it like the back button.
        handleBack();
      } else {
        void navigate(detailPath(itemPanelKind, itemId, query), {
          state: { fromSearch: true } satisfies DetailLocationState,
        });
      }
      return;
    }
    // Cards without a detail panel just toggle a local highlight.
    setSelection(
      selectedItemId === itemId
        ? deselectCard(selection)
        : selectCard(selection, itemId, false),
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
        {selectedItemId ? (
          <button
            type="button"
            aria-label="back"
            title="Back to results (Esc)"
            className="back-button"
            onClick={handleBack}
          >
            <svg
              viewBox="0 0 24 24"
              width="20"
              height="20"
              aria-hidden="true"
              fill="none"
              stroke="currentColor"
              strokeWidth="2.5"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              <path d="M19 12H5M12 19l-7-7 7-7" />
            </svg>
          </button>
        ) : (
          searchMessage && <p className="search-message">{searchMessage}</p>
        )}
      </div>

      <div
        className={`results-container${selectedItemId ? " has-selection" : ""}`}
      >
        {displayedResults.map((item) => (
          <SuggestionSearchCard
            // Keyed by itemID so a placeholder card is seamlessly replaced by
            // the real result once the search finishes, keeping its panel
            // mounted.
            key={item.itemID}
            item={item}
            selected={selectedItemId === item.itemID}
            expanded={expandedItemId === item.itemID}
            deselecting={
              previouslySelectedItemId === item.itemID &&
              selectedItemId !== item.itemID
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
