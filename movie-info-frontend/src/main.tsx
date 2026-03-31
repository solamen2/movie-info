import { StrictMode } from "react"
import { createRoot } from "react-dom/client"
import "./index.css"
import App from "./App.tsx"
import { config } from "./movieinfo.config.ts"

async function enableMocking() {
  if ((process.env.NODE_ENV === 'development') && config.useMockHttpCalls) {
    const { worker } = await import('../tests/mocks/browser.ts');
 
    // `worker.start()` returns a Promise that resolves
    // once the Service Worker is up and ready to intercept requests.
    return worker.start();
  }
}
 
enableMocking().then(() => {
  createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
})