import { expect, selectors } from "@playwright/test";
import { test } from "./e2ebase.ts";
import { config } from "../../src/movieinfo.config.ts";

test.beforeEach(async ({ page }) => {
  console.log("Logging in...");
  await page.goto("");
  await expect(page).toHaveTitle("Movie Search");
  const email = page.getByLabel("Email");
  const emailText = process.env.CI ? process.env.E2E_TEST_USERNAME as string : process.env.E2E_TEST_LOCAL_USERNAME as string;
  await email.fill(emailText);
  const password = page.getByLabel("Password");
  const passwordText = process.env.CI ? process.env.E2E_TEST_PASSWORD as string : process.env.E2E_TEST_LOCAL_PASSWORD as string;
  await password.fill(passwordText);
  const loginButton = page.getByRole("button", {"name": "Login"});
  await loginButton.click();
  console.log("Login finished.");
});

test.afterEach(async ({ page }) => {
  console.log("Logging out...");
  const logoutButton = page.getByRole("button", {"name": "Logout"});
  await logoutButton.click();
  console.log("Logout finished.")
});

test("Basic happy path: search, check results are valid, and select a search card", async ({ page }) => {
  console.log("Starting basic happy path test...");
  const searchText = config.useMockHttpCalls === true ? "1" : "The Shawshank Redemption";
  const searchQueryInput = page.getByRole("textbox", { name: "search-query-input" });
  await searchQueryInput.fill(searchText);
  const searchButton = page.getByRole("button", { name: "search" });
  await searchButton.click();

  const movieNameTextToSearch = config.useMockHttpCalls === true ? "Example Movie" : "The Shawshank Redemption";
  const movieTextElement = await page.getByText(movieNameTextToSearch, { exact: true });
  const movieCard = movieTextElement.locator("ancestor=#search-card");
  const movieCardText = config.useMockHttpCalls === true
    ? "Example MovieSearch Type: MediaMedia Type: MovieRank: 4444Known For: Example Jones, Example BrownYear: 2016"
    : "The Shawshank RedemptionSearch Type: MediaMedia Type: MovieRank: 109Known For: Tim Robbins, Morgan FreemanYear: 1994";
  expect(await movieCard.textContent()).toBe(movieCardText);
  console.log("Basic happy path test finished.");
});
