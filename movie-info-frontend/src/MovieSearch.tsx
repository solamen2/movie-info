import { useState } from "react";
import { useNavigate } from "react-router-dom";

function MovieSearch() {
  const [error, setError] = useState("");
  const navigate = useNavigate();

  async function handleLogout() {
    setError("");

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
      {error && <p className="error-message">{error}</p>}
      <button onClick={handleLogout}>Logout</button>
    </div>
  );
}

export default MovieSearch;
