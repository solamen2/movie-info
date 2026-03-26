import { type SubmitEvent, useState } from "react";
import { useNavigate } from "react-router-dom";

function RegisterUser() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  async function handleRegister(e: SubmitEvent) {
    e.preventDefault();
    setError("");

    try {
      const response = await fetch("/api/register", {  // TODO: Disable this page in prod
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
      });

      if (response.ok) {
        // TODO: Change this to say success message and add a link to go back to login
        navigate("/login");
      } else {
        const data = await response.json().catch(() => null);
        if (data?.errors) {
          const messages = Object.values(data.errors as Record<string, string[]>)
            .flat()
            .join(" ");
          setError(messages);
        } else {
          setError("Registration failed. Please try again.");
        }
      }
    } catch {
      setError("An unexpected error occurred. Please try again.");
    }
  }

  return (
    <div className="auth-page">
      <h1>Register</h1>
      <form onSubmit={handleRegister}>
        <div className="form-field">
          <label htmlFor="email">Email</label>
          <input
            id="email"
            type="text"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>
        <div className="form-field">
          <label htmlFor="password">Password</label>
          <input
            id="password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        {error && <p className="error-message">{error}</p>}
        <div className="form-actions">
          <button aria-label="register" type="submit">Register</button>
        </div>
      </form>
    </div>
  );
}

export default RegisterUser;
