import { defineConfig, devices } from "@playwright/test";

// NOTE: This is only used for testing dev docker image locally
//dotenv.config({ path: "../../movie-info-secrets/dev.env" });

/**
 * Read environment variables from file.
 * https://github.com/motdotla/dotenv
 */
// import dotenv from "dotenv";
// import path from "path";
// dotenv.config({ path: path.resolve(__dirname, ".env") });

/**
 * See https://playwright.dev/docs/test-configuration.
 */
export default defineConfig({
  testDir: "./tests/e2e/",
  // This avoids running Vitest tests on accident
  testMatch: '*.spec.ts',
  /* Run tests in files in parallel */
  fullyParallel: true,
  /* Fail the build on CI if you accidentally left test.only in the source code. */
  forbidOnly: !!process.env.CI,
  /* Retry on CI only */
  retries: process.env.CI ? 2 : 0,
  /* Opt out of parallel tests on CI. */
  workers: process.env.CI ? 1 : undefined,
  /* Reporter to use. See https://playwright.dev/docs/test-reporters */
  reporter: [["html"], ["list"]],
  /* Shared settings for all the projects below. See https://playwright.dev/docs/api/class-testoptions. */
  use: {
    /* Base URL to use in actions like `await page.goto("")`. */
    baseURL: process.env.CI ? process.env.E2E_TEST_BASE_URL : process.env.E2E_TEST_LOCAL_BASE_URL,
    headless: process.env.CI ? true : false,
    /* Collect trace when retrying the failed test. See https://playwright.dev/docs/trace-viewer */
    trace: "retain-on-failure",
  },

  projects: [
    {
       name: "Mobile Safari",
       use: { ...devices["iPhone 12"] },
     },
    {
       name: "Chromium",
       use: { ...devices["Desktop Chrome"]},
    },
  ]
});
