import { type SubmitEvent, useState } from "react";
import { useNavigate } from "react-router-dom";

function LoginUser() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  async function handleLogin(e: SubmitEvent) {
    e.preventDefault();
    setError("");

    try {
      const response = await fetch("/api/login?useCookies=true", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
      });

      if (response.ok) {
        await navigate("/search");
      } else {
        setError("Login failed. Please check your credentials and try again."); // TODO: Make this more specific (like when backend is down)
      }
    } catch {
      setError("An unexpected error occurred. Please try again.");
    }
  }

  return (
    <div className="auth-page">
      <h1>Login</h1>
      <form onSubmit={handleLogin}>
        <div className="form-field">
          <label htmlFor="email">Email</label>
          <input
            id="email"
            type="text"
            value={email}
            onChange={(e) => {
              setEmail(e.target.value);
            }}
            required
          />
        </div>
        <div className="form-field">
          <label htmlFor="password">Password</label>
          <input
            id="password"
            type="password"
            value={password}
            onChange={(e) => {
              setPassword(e.target.value);
            }}
            required
          />
        </div>
        {error && <p className="error-message">{error}</p>}
        <div className="form-actions">
          <button aria-label="login" id="login" type="submit">
            Login
          </button>
          {import.meta.env.DEV && (
            <button type="button" onClick={() => navigate("/register")}>
              Register
            </button>
          )}
        </div>
      </form>
      <p className="data-credits">
        Uses data from:
        <a href="https://www.imdb.com/" target="_blank" rel="noopener">
          <img src="/imdb-logo.svg" alt="IMDb" />
        </a>
        <a href="https://www.themoviedb.org/" target="_blank" rel="noopener">
          <img src="/tmdb-logo.svg" alt="TMDB" />
        </a>
        {/* OMDB has no logo of its own */}
        <a href="https://www.omdbapi.com/" target="_blank" rel="noopener">
          OMDb API
        </a>
        <a href="https://www.wikipedia.org/" target="_blank" rel="noopener">
          <img src="/wikipedia-logo.svg" alt="Wikipedia" />
        </a>
        <a href="https://www.wikidata.org/" target="_blank" rel="noopener">
          <img src="/wikidata-logo.svg" alt="Wikidata" />
        </a>
        <a href="https://www.bls.gov/" target="_blank" rel="noopener">
          <img src="/bls-logo.svg" alt="U.S. Bureau of Labor Statistics" />
        </a>
      </p>
    </div>
  );
}

export default LoginUser;
