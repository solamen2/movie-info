import { useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import "./footer.css";

// Shown at the bottom of every page once logged in: links to the app's
// pages, and the logout button. The login and register pages are the only
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
      <nav className="app-footer-links" aria-label="footer">
        <Link to="/search">Search</Link>
        <Link to="/oscars">Oscars By Year</Link>
        <button
          aria-label="logout"
          className="logout-button"
          onClick={handleLogout}
        >
          Logout
        </button>
      </nav>
      {error && <p className="error-message">{error}</p>}
    </footer>
  );
}

export default Footer;
