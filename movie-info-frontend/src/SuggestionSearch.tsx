import { type SubmitEvent, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import SuggestionSearchCard, { type Suggestion } from "./SuggestionSearchCard";

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
  const [error, setError] = useState("");
  const [searchMessage, setSearchMessage] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    function onKeyDown(e: KeyboardEvent) {
      if (e.key === "Escape") {
        setSelectedItemId((prev) => {
          setPreviouslySelectedItemId(prev);
          return null;
        });
      }
    }
    window.addEventListener("keydown", onKeyDown);
    return () => { window.removeEventListener("keydown", onKeyDown); };
  }, []);

  async function handleSearch(e: SubmitEvent) {
    e.preventDefault();
    setError("");
    setSearchMessage("");
    setResults([]);
    setSelectedItemId(null);
    // Cards unmount on a new search, so any in-flight deselect animation is
    // moot — clearing this prevents a card in the next result set that
    // happens to share an itemID from rendering with `.deselecting` and
    // playing a phantom exit animation.
    setPreviouslySelectedItemId(null);

    try {
      const response = await fetch(
        `/api/search?searchQuery=${encodeURIComponent(searchQuery)}`
      );

      if (!response.ok) {
        setError(`Search failed with status ${String(response.status)}. Please try again.`);
        return;
      }

      const data: Suggestion[] = await response.json() as Suggestion[];  // TODO: Maybe someday make this validation more robust
      if (data.length === 0) {
        setSearchMessage("No results.");
      } else {
        setSearchMessage(`${String(data.length)} results.`)
        setResults(data);
      }
    } catch {
      setError("An unexpected error occurred. Please try again.");
    }
  }

  function handleCardClick(itemId: string) {
    setSelectedItemId((prev) => {
      setPreviouslySelectedItemId(prev);
      return prev === itemId ? null : itemId;
    });
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
        await new Promise(f => setTimeout(f, 5000));
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
            onChange={(e) => { setSearchQuery(e.target.value); }}
          />
          <button aria-label="search" type="submit">Search</button>
        </form>
        <button aria-label="logout" className="logout-button" onClick={handleLogout}>
          Logout
        </button>
      </div>

      {error && <p className="error-message">{error}</p>}
      {searchMessage && <p className="search-message">{searchMessage}</p>}

      <div
        className={`results-container${selectedItemId ? " has-selection" : ""}`}
      >
        {results.map((item) => (
          <SuggestionSearchCard
            key={item.id}
            item={item}
            selected={selectedItemId === item.itemID}
            deselecting={
              previouslySelectedItemId === item.itemID &&
              selectedItemId !== item.itemID
            }
            onClick={() => { handleCardClick(item.itemID); }}
          />
        ))}
      </div>
    </div>
  );
}

export default SuggestionSearch;
