import { act, fireEvent, screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { renderApp } from "../../tests/renderApp";
import { SEARCH_AUTO_SUBMIT_MS } from "../utilities/constants";

function typeQuery(value: string) {
  fireEvent.change(
    screen.getByRole("textbox", { name: "search-query-input" }),
    { target: { value } },
  );
}

function wait(milliseconds: number) {
  act(() => {
    vi.advanceTimersByTime(milliseconds);
  });
}

// screen.logTestingPlaygroundURL();
describe("SuggestionSearch", () => {
  describe("When using search terms with results", () => {
    it("Should return search results", async () => {
      renderApp();
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);
      const searchResultsNumber = await screen.findByText("8 results.");
      expect(searchResultsNumber).toBeInTheDocument();

      fireEvent.change(searchQueryInput, { target: { value: "2" } });
      fireEvent.click(searchButton);
      const searchResultsNumber2 = await screen.findByText("6 results.");
      expect(searchResultsNumber2).toBeInTheDocument();
    });
  });
  describe("When using search terms with no results", () => {
    it("Should show 'No results. Please try another search.'", async () => {
      renderApp();
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "empty" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);
      const noSearchResults = await screen.findByText(
        "No results. Please try another search.",
      );
      expect(noSearchResults).toBeInTheDocument();
    });
  });
  describe("When using a bad search term", () => {
    it("Should show the 404 error and suggest another search", async () => {
      renderApp();
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "error" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);
      const errorSearchResults = await screen.findByText(
        "Search failed with status 404: Not a valid search query for mock. Please try another search.",
      );
      expect(errorSearchResults).toBeInTheDocument();
    });
  });
  describe("When a search query is typed without being submitted", () => {
    afterEach(() => {
      vi.useRealTimers();
    });

    it("Should submit the query once there has been no input for a while", async () => {
      vi.useFakeTimers({ toFake: ["setTimeout", "clearTimeout"] });
      const { currentUrl } = renderApp();
      typeQuery("1");
      wait(SEARCH_AUTO_SUBMIT_MS - 1);
      expect(currentUrl()).toBe("/search");
      wait(1);
      expect(currentUrl()).toBe("/search?q=1");

      vi.useRealTimers();
      expect(await screen.findByText("8 results.")).toBeInTheDocument();
    });

    it("Should start waiting again on every input", () => {
      vi.useFakeTimers({ toFake: ["setTimeout", "clearTimeout"] });
      const { currentUrl } = renderApp();
      typeQuery("1");
      wait(SEARCH_AUTO_SUBMIT_MS - 1);
      typeQuery("2");
      wait(SEARCH_AUTO_SUBMIT_MS - 1);
      expect(currentUrl()).toBe("/search");
      wait(1);
      expect(currentUrl()).toBe("/search?q=2");
    });

    it("Should not submit an empty query", () => {
      vi.useFakeTimers({ toFake: ["setTimeout", "clearTimeout"] });
      const { currentUrl } = renderApp("/search?q=1");
      typeQuery(" ");
      wait(SEARCH_AUTO_SUBMIT_MS * 2);
      expect(currentUrl()).toBe("/search?q=1");
    });

    it("Should not submit a query again that the Search button already submitted", () => {
      vi.useFakeTimers({ toFake: ["setTimeout", "clearTimeout"] });
      const { router } = renderApp();
      typeQuery("1");
      fireEvent.click(screen.getByRole("button", { name: "search" }));
      const locationKey = router.state.location.key;
      wait(SEARCH_AUTO_SUBMIT_MS * 2);
      expect(router.state.location.key).toBe(locationKey);
    });
  });
  describe("When valid user clicks the log out button", () => {
    it("Should log out user successfully", () => {
      renderApp();
      const logoutButton = screen.getByRole("button", { name: "logout" });
      fireEvent.click(logoutButton);
    });
  });
});
