import { test as base } from "../playwright.setup.ts";

// Implements closest() selector, implementation from https://github.com/microsoft/playwright/issues/6015 (which also discusses why closest() isn't in Playwright)

const ancestorEngine = () => ({
  query(root: Element, selector: string) {
    return root.closest(selector);
  },
  queryAll(root: Element, selector: string) {
    const closest = root.closest(selector);
    return closest ? new Array(closest) : [];
  }
});

export const test = base.extend<object, { selectorRegistration: object }>({
  // Register selectors once per worker.
  selectorRegistration: [async ({ playwright }, use) => {
    // Register the engine. Selectors will be prefixed with "ancestor=".
    await playwright.selectors.register("ancestor", ancestorEngine, { contentScript: true });
    await use({});
  }, { scope: "worker", auto: true }],
});
