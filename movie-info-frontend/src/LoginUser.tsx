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
        setError("Login failed. Please check your credentials and try again.");  // TODO: Make this more specific (like when backend is down)
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
            onChange={(e) => { setEmail(e.target.value); }}
            required
          />
        </div>
        <div className="form-field">
          <label htmlFor="password">Password</label>
          <input
            id="password"
            type="password"
            value={password}
            onChange={(e) => { setPassword(e.target.value); }}
            required
          />
        </div>
        {error && <p className="error-message">{error}</p>}
        <div className="form-actions">
          <button aria-label="login" id="login" type="submit">Login</button>
          {import.meta.env.DEV && <button type="button" onClick={() => navigate("/register")}>Register</button>}
        </div>
      </form>
    </div>
  );
}

export default LoginUser;
