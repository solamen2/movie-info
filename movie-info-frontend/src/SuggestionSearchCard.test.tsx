import { fireEvent, render, screen, waitFor } from '@testing-library/react'
import { describe, expect, it } from "vitest";
import { MemoryRouter } from "react-router-dom"
import SuggestionSearch from './SuggestionSearch';
 
  // TODO: Card tests:
  //          Test all good card types as separate tests
  //          Test card selection and deselection

describe("SuggestionSearchCard", async () => {
  
  // First search: 8 cards
  
  describe("When search card contains a person", async () => {
    it("Should only show person search card data fields", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "1" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      await fireEvent.click(searchButton);

      const personText = await screen.findByText("Example Smith");
      const personCard = personText.closest("#search-card");
      expect(personCard?.textContent).toMatch("Example SmithSearch Type: PersonRank: 3Known For: Actress, Example Film");
    });
  });
  describe("When search card contains a movie", async () => {
    it("Should only show movie search card data fields", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "1" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      await fireEvent.click(searchButton);

      const movieText = await screen.findByText("Example Movie");
      const movieCard = movieText.closest("#search-card");
      expect(movieCard?.textContent).toMatch("Example MovieSearch Type: MediaMedia Type: MovieRank: 4444Known For: Example Jones, Example BrownYear: 2016");
    });
  });
  describe("When search card contains a TV series", async () => {
    it("Should only show TV series search card data fields", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "1" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      await fireEvent.click(searchButton);

      const tvSeriesText = await screen.findByText("Example TV Series");
      const tvSeriesCard = tvSeriesText.closest("#search-card");
      expect(tvSeriesCard?.textContent).toMatch("Example TV SeriesSearch Type: MediaMedia Type: TV SeriesRank: 4444Known For: John Smith, James JohnsonYear: 2001Years: 2001-2003");
    });
  });
  describe("When search card contains a TV mini series", async () => {
    it("Should only show TV mini series search card data fields", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "1" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      await fireEvent.click(searchButton);

      const tvMiniSeriesText = await screen.findByText("Example TV Mini Series");
      const tvMiniSeriesCard = tvMiniSeriesText.closest("#search-card");
      expect(tvMiniSeriesCard?.textContent).toMatch("Example TV Mini SeriesSearch Type: MediaMedia Type: TV Mini SeriesRank: 4444Known For: Maria Garcia, James SmithYear: 1982Years: 1982-1982");
    });
  });
  describe("When search card contains a TV movie", async () => {
    it("Should only show TV movie search card data fields", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "1" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      await fireEvent.click(searchButton);

      const tvMovieText = await screen.findByText("Example TV Movie");
      const tvMovieCard = tvMovieText.closest("#search-card");
      expect(tvMovieCard?.textContent).toMatch("No imageExample TV MovieSearch Type: MediaMedia Type: TV MovieRank: 188333Known For: Michael Smith, David SmithYear: 2017");
    });
  });
  describe("When search card contains a TV special", async () => {
    it("Should only show TV special card data fields", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "1" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      await fireEvent.click(searchButton);

      const tvSpecialText = await screen.findByText("Example TV Special");
      const tvSpecialCard = tvSpecialText.closest("#search-card");
      expect(tvSpecialCard?.textContent).toMatch("Example TV SpecialSearch Type: MediaMedia Type: TV SpecialRank: 1930011Known For: Maria Rodriguez, Mary SmithYear: 1999");
    });
  });
  describe("When search card contains a TV short", async () => {
    it("Should only show TV short search card data fields", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "1" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      await fireEvent.click(searchButton);

      const tvShortText = await screen.findByText("Example TV Short");
      const tvShortCard = tvShortText.closest("#search-card");
      expect(tvShortCard?.textContent).toMatch("Example TV ShortSearch Type: MediaMedia Type: TV ShortRank: 2000Known For: Maria Martinez, James JohnsonYear: 1982");
    });
  });
  describe("When search card contains a short", async () => {
    it("Should only show short search card data fields", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "1" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      await fireEvent.click(searchButton);

      const shortText = await screen.findByText("Example Short");
      const shortCard = shortText.closest("#search-card");
      expect(shortCard?.textContent).toMatch("Example ShortSearch Type: MediaMedia Type: ShortRank: 1124Known For: David Johnson, Maria HernandezYear: 1934");
    });
  });

  // Second search: 6 cards (5 are tested)

  describe("When search card contains a video game", async () => {
    it("Should only show video game search card data fields", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "2" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      await fireEvent.click(searchButton);

      const videoGameText = await screen.findByText("Example Video Game");
      const videoGameCard = videoGameText.closest("#search-card");
      expect(videoGameCard?.textContent).toMatch("Example Video GameSearch Type: MediaMedia Type: Video GameRank: 989Known For: Action, Adventure, Sci-FiYear: 1996");
    });
  });
  describe("When search card contains a video", async () => {
    it("Should only show video search card data fields", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "2" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      await fireEvent.click(searchButton);

      const videoText = await screen.findByText("Example Video");
      const videoCard = videoText.closest("#search-card");
      expect(videoCard?.textContent).toMatch("Example VideoSearch Type: MediaMedia Type: VideoRank: 65439Known For: Robert Johnson, James WilliamsYear: 2016");
    });
  });
  describe("When search card contains a music video", async () => {
    it("Should only show music video search card data fields", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "2" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      await fireEvent.click(searchButton);

      const musicVideoText = await screen.findByText("Example Music Video");
      const musicVideoCard = musicVideoText.closest("#search-card");
      expect(musicVideoCard?.textContent).toMatch("Example Music VideoSearch Type: MediaMedia Type: Music VideoRank: 4455Known For: James Brown, Jose GarciaYear: 2001");
    });
  });
  describe("When search card contains a podcast series", async () => {
    it("Should only show podcast series search card data fields", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "2" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      await fireEvent.click(searchButton);

      const podcastSeriesText = await screen.findByText("Example Podcast Series");
      const podcastSeriesCard = podcastSeriesText.closest("#search-card");
      expect(podcastSeriesCard?.textContent).toMatch("Example Podcast SeriesSearch Type: MediaMedia Type: Podcast SeriesRank: 27Known For: Maria Gonzalez, David BrownYear: 1992");
    });
  });
  describe("When search card contains a spotlight", async () => {
    it("Should only show spotlight search card data fields", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "2" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      await fireEvent.click(searchButton);

      const spotlightText = await screen.findByText("Lemur History Month");
      const spotlightCard = spotlightText.closest("#search-card");
      expect(spotlightCard?.textContent).toMatch("Lemur History MonthSearch Type: —Media Type: —Rank: —Known For: A Celebration of Lemur Storytellers and StoriesYear: —");
    });
  });

  // Selection test

  describe("When search card is selected and deselected", async () => {
    it("Should show card appearance as selected and deselected", async () => {
      render(<MemoryRouter><SuggestionSearch /></MemoryRouter>);
      const searchQueryInput = screen.getByRole("textbox", { name: "search-query-input" });
      fireEvent.change(searchQueryInput, { target: { value: "1" }});
      const searchButton = screen.getByRole("button", { name: "search" });
      await fireEvent.click(searchButton);

      const personText = await screen.findByText("Example Smith");
      const personCard = personText.closest("#search-card");
      if (!personCard) {
        throw new Error("Person search card not found in selection test");
      }
      const movieText = await screen.findByText("Example Movie");
      const movieCard = movieText.closest("#search-card");
      if (!movieCard) {
        throw new Error("Movie search card not found in selection test");
      }

      expect(personCard.classList.contains('selected')).toBe(false);
      await fireEvent.click(personCard);
      expect(personCard.classList.contains('selected')).toBe(true);
      await fireEvent.click(movieCard);
      expect(personCard.classList.contains('selected')).toBe(false);
      expect(movieCard.classList.contains('selected')).toBe(true);
      await fireEvent.click(movieCard);
      expect(movieCard.classList.contains('selected')).toBe(false);
    });
  });
});