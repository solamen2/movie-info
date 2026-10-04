import { beforeAll, afterEach, afterAll } from "vitest";
import { server } from "./mocks/node.ts";
import { clearDetailDataCache } from "../src/shared/useDetailData.ts";
import "@testing-library/jest-dom/vitest";

beforeAll(() => {
  server.listen({ onUnhandledRequest: "error" });
});
afterEach(() => {
  server.resetHandlers();
  // Tests mock different responses for the same URL
  clearDetailDataCache();
});
afterAll(() => {
  server.close();
});

// jsdom doesn't implement scrolling, and would log an error for every call.
// Opening a panel from a card inside another panel scrolls to the top of the
// page (see useOpenDetail).
window.scrollTo = () => undefined;
