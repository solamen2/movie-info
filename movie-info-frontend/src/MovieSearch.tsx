import { type FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";

interface SuggestionImage {
  height: number;
  imageURL: string;
  width: number;
}

interface Suggestion {
  id: string;
  image: SuggestionImage | null;
  itemID: string;
  name: string;
  searchType: number | null;
  mediaType: { value: string } | null;
  rank: number | null;
  knownFor: string;
  year: number | null;
  years: string | null;
}

const SEARCH_TYPE_LABELS: Record<number, string> = {
  0: "Person",
  1: "Media",
};

function MovieSearch() {
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
        `/api/search?searchQuery=${encodeURIComponent(searchQuery)}`
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
        setError("Logout failed. Please try again.");
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
          <div
            key={item.id}
            className={`result-card${selectedItemId === item.itemID ? " selected" : ""}`}
            onClick={() => handleCardClick(item.itemID)}
          >
            {item.image ? (
              <img
                src={item.image.imageURL}
                alt={item.name} 
                width={item.image.width} 
                height={item.image.height} 
                className="card-image"
              />
            ) : (
              <div className="card-image-placeholder">No image</div>
            )}
            <div className="card-details">
              <h3 className="card-name">{item.name}</h3>
              <p><strong>Item ID:</strong> {item.itemID}</p>
              <p><strong>ID:</strong> {item.id}</p>
              <p>
                <strong>Search Type:</strong>{" "}
                {item.searchType != null
                  ? SEARCH_TYPE_LABELS[item.searchType] ?? String(item.searchType)
                  : "—"}
              </p>
              <p>
                <strong>Media Type:</strong>{" "}
                {item.mediaType?.value ? item.mediaType.value : "—"}
              </p>
              <p><strong>Rank:</strong> {item.rank ?? "—"}</p>
              <p><strong>Known For:</strong> {item.knownFor}</p>
              <p><strong>Year:</strong> {item.year ?? "—"}</p>
              <p><strong>Years:</strong> {item.years ?? "—"}</p>
              {item.image && (
                <p>
                  <strong>Image Size:</strong> {item.image.width} x {item.image.height}
                </p>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

export default MovieSearch;
