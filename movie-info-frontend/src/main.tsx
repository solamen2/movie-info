import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";

async function enableMocking() {
  if (
    import.meta.env.DEV &&
    import.meta.env.VITE_USE_MOCK_HTTP_CALLS === "true"
  ) {
    const { worker } = await import("../tests/mocks/browser");

    // `worker.start()` returns a Promise that resolves
    // once the Service Worker is up and ready to intercept requests.
    return worker.start();
  }
}

void enableMocking().then(() => {
  createRoot(document.getElementById("root") ?? new HTMLElement()).render(
    <StrictMode>
      <App />
    </StrictMode>,
  );
});
