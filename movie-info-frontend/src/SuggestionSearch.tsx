import {
  type SubmitEvent,
  useEffect,
  useEffectEvent,
  useRef,
  useState,
} from "react";
import { useNavigate } from "react-router-dom";
import SuggestionSearchCard, { type Suggestion } from "./SuggestionSearchCard";
import type { MediaResultType } from "./utilities/constants";
import { canHaveMoviePanel } from "./utilities/utilities";

// Keep in sync with the `select-fly` / `deselect-fly` animation duration and
// the `.result-card` width transition duration in App.css.
const CARD_FLY_MS = 500;
const CARD_RESIZE_MS = 300;

function hasDetailPanel(mediaType: MediaResultType | null) {
  return canHaveMoviePanel(mediaType?.value ?? null);
}

function SuggestionSearch() {
  const [searchQuery, setSearchQuery] = useState("");
  const [results, setResults] = useState<Suggestion[]>([]);
  const [selectedItemId, setSelectedItemId] = useState<string | null>(null);
  // Tracks the card the user just transitioned out of selected. The card
  // gets a `.deselecting` class which fires the `deselect-fly` keyframe
  // animation in CSS — required because CSS animations only run on class
  // addition, not on class removal, so we can't rely on .selected alone to
  // animate both directions.
  const [previouslySelectedItemId, setPreviouslySelectedItemId] = useState<
    string | null
  >(null);
  // A selected card with a detail panel animates in two phases: first it flies
  // to the upper-left (selectedItemId), then it grows into the panel
  // (expandedItemId). Going back runs the phases in reverse.
  const [expandedItemId, setExpandedItemId] = useState<string | null>(null);
  const phaseTimeoutRef = useRef<number | undefined>(undefined);
  const [error, setError] = useState("");
  const [searchMessage, setSearchMessage] = useState("");
  const navigate = useNavigate();

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
      window.clearTimeout(phaseTimeoutRef.current);
    };
  }, []);

  function deselect() {
    setSelectedItemId((prev) => {
      setPreviouslySelectedItemId(prev);
      return null;
    });
  }

  function handleBack() {
    window.clearTimeout(phaseTimeoutRef.current);
    if (expandedItemId) {
      // Shrink the panel back to card size first, then fly the card home.
      setExpandedItemId(null);
      phaseTimeoutRef.current = window.setTimeout(deselect, CARD_RESIZE_MS);
    } else {
      deselect();
    }
  }

  async function handleSearch(e: SubmitEvent) {
    e.preventDefault();
    setError("");
    setSearchMessage("");
    setResults([]);
    setSelectedItemId(null);
    setExpandedItemId(null);
    window.clearTimeout(phaseTimeoutRef.current);
    // Cards unmount on a new search, so any in-flight deselect animation is
    // moot — clearing this prevents a card in the next result set that
    // happens to share an itemID from rendering with `.deselecting` and
    // playing a phantom exit animation.
    setPreviouslySelectedItemId(null);

    try {
      // TODO: Add a pending indicator while the search is running

      const response = await fetch(
        `/api/search?searchQuery=${encodeURIComponent(searchQuery)}`,
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
      if (data.length === 0) {
        setSearchMessage("No results.");
      } else {
        setSearchMessage(`${String(data.length)} results.`);
        setResults(data);
      }
    } catch {
      setError("An unexpected error occurred. Please try again.");
    }
  }

  function handleCardClick(item: Suggestion) {
    const itemId = item.itemID;
    const isSelecting = selectedItemId !== itemId;
    window.clearTimeout(phaseTimeoutRef.current);
    setExpandedItemId(null);
    setSelectedItemId((prev) => {
      setPreviouslySelectedItemId(prev);
      return prev === itemId ? null : itemId;
    });
    if (isSelecting && hasDetailPanel(item.mediaType)) {
      phaseTimeoutRef.current = window.setTimeout(() => {
        setExpandedItemId(itemId);
      }, CARD_FLY_MS);
    }
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
        {results.map((item) => (
          <SuggestionSearchCard
            key={item.id}
            item={item}
            selected={selectedItemId === item.itemID}
            expanded={expandedItemId === item.itemID}
            deselecting={
              previouslySelectedItemId === item.itemID &&
              selectedItemId !== item.itemID
            }
            mediaType={item.mediaType?.value ?? null}
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
