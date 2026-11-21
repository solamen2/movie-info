import { useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import ThemeToggle from "./ThemeToggle";
import "./footer.css";

// Shown at the bottom of every page once logged in: links to the app's
// pages, the theme toggle, and the logout button. The login and register pages are the only
// pages seen while logged out, and those have no footer, since everything in
// it needs a logged in user.
function Footer() {
  const navigate = useNavigate();
  const location = useLocation();
  const [error, setError] = useState("");
  const onAuthPage =
    location.pathname === "/login" || location.pathname === "/register";
  if (onAuthPage) {
    return null;
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
    <footer className="app-footer">
      <hr className="app-footer-separator" />
      <nav className="app-footer-links" aria-label="Footer">
        <Link to="/search">Search</Link>
        <Link to="/oscars">Oscars</Link>
        <Link to="/imdb-lists">IMDB Lists</Link>
        <Link to="/aspect-ratios">Aspect Ratios</Link>
        <ThemeToggle />
        <button className="logout-button" onClick={handleLogout}>
          Logout
        </button>
      </nav>
      {error && (
        <p className="error-message" role="alert">
          {error}
        </p>
      )}
    </footer>
  );
}

export default Footer;
