import { beforeAll, afterEach, afterAll } from "vitest";
import { server } from "./mocks/node.ts";
import { clearDetailDataCache } from "../src/shared/useDetailData.ts";
import { clearInflationDataCache } from "../src/movie/useInflationData.ts";
import "@testing-library/jest-dom/vitest";

beforeAll(() => {
  server.listen({ onUnhandledRequest: "error" });
});
afterEach(() => {
  server.resetHandlers();
  // Tests mock different responses for the same URL
  clearDetailDataCache();
  clearInflationDataCache();
});
afterAll(() => {
  server.close();
});

// jsdom doesn't implement scrolling, and would log an error for every call.
// Opening a panel from a card inside another panel scrolls to the top of the
// page (see useOpenDetail).
window.scrollTo = () => undefined;

// Node has a localStorage global of its own, which is undefined unless Node
// is started with a file to keep it in, and which hides jsdom's. Tests get
// jsdom's, as the app does in a browser (the theme is saved there, see
// utilities/theme.ts).
const { jsdom } = globalThis as unknown as {
  jsdom: { window: { localStorage: Storage } };
};
Object.defineProperty(globalThis, "localStorage", {
  value: jsdom.window.localStorage,
  configurable: true,
});

// jsdom has no media queries: none ever matches, which makes the tests' screen
// a wide one (the search box's hint depends on it, see SuggestionSearch).
window.matchMedia = (query: string) =>
  ({
    matches: false,
    media: query,
    addEventListener: () => undefined,
    removeEventListener: () => undefined,
  }) as unknown as MediaQueryList;
