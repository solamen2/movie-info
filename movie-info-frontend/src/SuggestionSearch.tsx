import { type FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import SuggestionSearchCard, { type Suggestion } from "./SuggestionSearchCard";

function SuggestionSearch() {
  const [searchQuery, setSearchQuery] = useState("");
  const [results, setResults] = useState<Suggestion[]>([]);
  const [selectedItemId, setSelectedItemId] = useState<string | null>(null);
  const [error, setError] = useState("");
  const [noResults, setNoResults] = useState(false);
  const navigate = useNavigate();

  async function handleSearch(e: FormEvent) {
    e.preventDefault();
    setError("");
    setNoResults(false);
    setSelectedItemId(null);

    try {
      const response = await fetch(
        `/api/search?searchQuery=${encodeURIComponent(searchQuery)}`,
        { redirect: "error" }
      );

      if (!response.ok) {
        setError(`Search failed with status ${response.status}. Please try again.`);
        setResults([]);
        return;
      }

      const data: Suggestion[] = await response.json();
      if (!data || data.length === 0) {
        setResults([]);
        setNoResults(true);
      } else {
        setResults(data);
      }
    } catch {
      setError("An unexpected error occurred. Please try again.");
      setResults([]);
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

      if (response.ok) {
        navigate("/login");
      } else {
        setError("Logout failed. Please try again."); // TODO: Maybe redirect to login instead
      }
    } catch {
      setError("An unexpected error occurred. Please try again.");
    }
  }

  return (
    <div className="search-page">
      <div className="search-header">
        <form className="search-bar" onSubmit={handleSearch}>
          <input
            type="text"
            placeholder="Search movies, people..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
          <button type="submit">Search</button>
        </form>
        <button className="logout-button" onClick={handleLogout}>
          Logout
        </button>
      </div>

      {error && <p className="error-message">{error}</p>}
      {noResults && <p className="no-results">No results</p>}

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
