import { type SubmitEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import { usePageTitle } from "../utilities/pageTitle";

export interface RegisterResponse {
  type: string;
  title: string;
  status: number;
  errors: { error: string[] };
}

function RegisterUser() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  usePageTitle("Register");

  async function handleRegister(e: SubmitEvent) {
    e.preventDefault();
    setError("");

    try {
      const response = await fetch("/api/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
      });

      if (response.ok) {
        // TODO: Change this to say success message and add a link to go back to login
        await navigate("/login");
      } else {
        const data = (await response.json()) as RegisterResponse | null;
        if (data?.errors) {
          const messages = Object.values(
            data.errors as Record<string, string[]>,
          )
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
      {/* A note, not a heading */}
      <p className="auth-note">
        Hey there! This page is technically accessible to the public, and you
        can make accounts here, but since accounts have to be manually enabled
        in the database, it's pointless for anyone but me to make them. It's
        just a convenience page for me, the admin.
      </p>
      <form onSubmit={handleRegister}>
        <div className="form-field">
          <label htmlFor="email">Email</label>
          <input
            id="email"
            type="text"
            autoComplete="username"
            inputMode="email"
            autoCapitalize="none"
            spellCheck={false}
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
            autoComplete="new-password"
            value={password}
            onChange={(e) => {
              setPassword(e.target.value);
            }}
            required
          />
        </div>
        {error && (
          <p className="error-message" role="alert">
            {error}
          </p>
        )}
        <div className="form-actions">
          <button type="submit">Register</button>
        </div>
      </form>
    </div>
  );
}

export default RegisterUser;
