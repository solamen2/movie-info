import { fireEvent, render, screen } from '@testing-library/react'
import { describe, expect, it } from "vitest";
import { MemoryRouter } from "react-router-dom"
import SuggestionSearch from './SuggestionSearch';

// screen.logTestingPlaygroundURL();
describe("SuggestionSearch", () => {
  describe("When using search terms with results", () => {
    it("Should return search results", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);  // MemoryRouter required to use navigate()
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "1" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);
      const searchResultsNumber = await screen.findByText("8 results.");
      expect(searchResultsNumber).toBeInTheDocument();

      fireEvent.change(searchQueryInput, { target: { value: "2" }});
      fireEvent.click(searchButton);
      const searchResultsNumber2 = await screen.findByText("6 results.");
      expect(searchResultsNumber2).toBeInTheDocument();
    });
  });
  describe("When using search terms with no results", () => {
    it("Should show 'No results.'", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "empty" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);
      const noSearchResults = await screen.findByText("No results.");
      expect(noSearchResults).toBeInTheDocument();
    });
  });
  describe("When using a bad search term", () => {
    it("Should show error 'An unexpected error occurred. Please try again.'", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "error" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);
      const errorSearchResults = await screen.findByText("Search failed with status 404. Please try again.");
      expect(errorSearchResults).toBeInTheDocument();
    });
  });
  describe("When valid user clicks the log out button", () => {
    it("Should log out user successfully", () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const logoutButton = screen.getByRole("button", { name: "logout" });
      fireEvent.click(logoutButton);
    });
  });
});