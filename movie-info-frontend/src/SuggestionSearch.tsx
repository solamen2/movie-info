import { type SubmitEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import SuggestionSearchCard, { type Suggestion } from "./SuggestionSearchCard";

function SuggestionSearch() {
  const [searchQuery, setSearchQuery] = useState("");
  const [results, setResults] = useState<Suggestion[]>([]);
  const [selectedItemId, setSelectedItemId] = useState<string | null>(null);
  const [error, setError] = useState("");
  const [searchMessage, setSearchMessage] = useState("");
  const navigate = useNavigate();

  async function handleSearch(e: SubmitEvent) {
    e.preventDefault();
    setError("");
    setSearchMessage("");
    setResults([]);
    setSelectedItemId(null);

    try {
      const response = await fetch(
        `/api/search?searchQuery=${encodeURIComponent(searchQuery)}`,
        { redirect: "error" }
      );

      if (!response.ok) {
        setError(`Search failed with status ${response.status}. Please try again.`);
        return;
      }

      const data: Suggestion[] = await response.json();
      if (!data || data.length === 0) {
        setSearchMessage("No results.");
      } else {
        setSearchMessage(`${data.length} results.`)
        setResults(data);
      }
    } catch {
      setError("An unexpected error occurred. Please try again.");
    }
  }

  function handleCardClick(itemId: string) {
    setSelectedItemId((prev) => (prev === itemId ? null : itemId));
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
      navigate("/login");
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
            onChange={(e) => setSearchQuery(e.target.value)}
          />
          <button aria-label="search" type="submit">Search</button>
        </form>
        <button aria-label="logout" className="logout-button" onClick={handleLogout}>
          Logout
        </button>
      </div>

      {error && <p className="error-message">{error}</p>}
      {searchMessage && <p className="search-message">{searchMessage}</p>}

      <div className="results-container">
        {results.map((item) => (
          <SuggestionSearchCard
            key={item.id}
            item={item}
            selected={selectedItemId === item.itemID}
            onClick={() => handleCardClick(item.itemID)}
          />
        ))}
      </div>
    </div>
  );
}

export default SuggestionSearch;
