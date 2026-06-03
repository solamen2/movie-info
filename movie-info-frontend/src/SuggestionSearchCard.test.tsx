import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { MemoryRouter } from "react-router-dom";
import SuggestionSearch from "./SuggestionSearch";

// screen.logTestingPlaygroundURL();
describe("SuggestionSearchCard", () => {
  // First search: 8 cards

  describe("When search card contains a person", () => {
    it("Should only show person search card data fields", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const personText = await screen.findByText("Example Smith");
      const personCard = personText.closest("#search-card");
      expect(personCard?.textContent).toMatch(
        "Example SmithSearch Type: PersonRank: 3Known For: Actress, Example Film",
      );
    });
  });
  describe("When search card contains a movie", () => {
    it("Should only show movie search card data fields", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const movieText = await screen.findByText("Example Movie");
      const movieCard = movieText.closest("#search-card");
      expect(movieCard?.textContent).toMatch(
        "Example MovieSearch Type: MediaMedia Type: MovieRank: 4444Known For: Example Jones, Example BrownYear: 2016",
      );
    });
  });
  describe("When search card contains a TV series", () => {
    it("Should only show TV series search card data fields", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const tvSeriesText = await screen.findByText("Example TV Series");
      const tvSeriesCard = tvSeriesText.closest("#search-card");
      expect(tvSeriesCard?.textContent).toMatch(
        "Example TV SeriesSearch Type: MediaMedia Type: TV SeriesRank: 4444Known For: John Smith, James JohnsonYear: 2001Years: 2001-2003",
      );
    });
  });
  describe("When search card contains a TV mini series", () => {
    it("Should only show TV mini series search card data fields", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const tvMiniSeriesText = await screen.findByText(
        "Example TV Mini Series",
      );
      const tvMiniSeriesCard = tvMiniSeriesText.closest("#search-card");
      expect(tvMiniSeriesCard?.textContent).toMatch(
        "Example TV Mini SeriesSearch Type: MediaMedia Type: TV Mini SeriesRank: 4444Known For: Maria Garcia, James SmithYear: 1982Years: 1982-1982",
      );
    });
  });
  describe("When search card contains a TV movie", () => {
    it("Should only show TV movie search card data fields", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const tvMovieText = await screen.findByText("Example TV Movie");
      const tvMovieCard = tvMovieText.closest("#search-card");
      expect(tvMovieCard?.textContent).toMatch(
        "No imageExample TV MovieSearch Type: MediaMedia Type: TV MovieRank: 188333Known For: Michael Smith, David SmithYear: 2017",
      );
    });
  });
  describe("When search card contains a TV special", () => {
    it("Should only show TV special card data fields", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const tvSpecialText = await screen.findByText("Example TV Special");
      const tvSpecialCard = tvSpecialText.closest("#search-card");
      expect(tvSpecialCard?.textContent).toMatch(
        "Example TV SpecialSearch Type: MediaMedia Type: TV SpecialRank: 1930011Known For: Maria Rodriguez, Mary SmithYear: 1999",
      );
    });
  });
  describe("When search card contains a TV short", () => {
    it("Should only show TV short search card data fields", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const tvShortText = await screen.findByText("Example TV Short");
      const tvShortCard = tvShortText.closest("#search-card");
      expect(tvShortCard?.textContent).toMatch(
        "Example TV ShortSearch Type: MediaMedia Type: TV ShortRank: 2000Known For: Maria Martinez, James JohnsonYear: 1982",
      );
    });
  });
  describe("When search card contains a short", () => {
    it("Should only show short search card data fields", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const shortText = await screen.findByText("Example Short");
      const shortCard = shortText.closest("#search-card");
      expect(shortCard?.textContent).toMatch(
        "Example ShortSearch Type: MediaMedia Type: ShortRank: 1124Known For: David Johnson, Maria HernandezYear: 1934",
      );
    });
  });

  // Second search: 6 cards (5 are tested)

  describe("When search card contains a video game", () => {
    it("Should only show video game search card data fields", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "2" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const videoGameText = await screen.findByText("Example Video Game");
      const videoGameCard = videoGameText.closest("#search-card");
      expect(videoGameCard?.textContent).toMatch(
        "Example Video GameSearch Type: MediaMedia Type: Video GameRank: 989Known For: Action, Adventure, Sci-FiYear: 1996",
      );
    });
  });
  describe("When search card contains a video", () => {
    it("Should only show video search card data fields", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "2" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const videoText = await screen.findByText("Example Video");
      const videoCard = videoText.closest("#search-card");
      expect(videoCard?.textContent).toMatch(
        "Example VideoSearch Type: MediaMedia Type: VideoRank: 65439Known For: Robert Johnson, James WilliamsYear: 2016",
      );
    });
  });
  describe("When search card contains a music video", () => {
    it("Should only show music video search card data fields", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "2" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const musicVideoText = await screen.findByText("Example Music Video");
      const musicVideoCard = musicVideoText.closest("#search-card");
      expect(musicVideoCard?.textContent).toMatch(
        "Example Music VideoSearch Type: MediaMedia Type: Music VideoRank: 4455Known For: James Brown, Jose GarciaYear: 2001",
      );
    });
  });
  describe("When search card contains a podcast series", () => {
    it("Should only show podcast series search card data fields", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "2" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const podcastSeriesText = await screen.findByText(
        "Example Podcast Series",
      );
      const podcastSeriesCard = podcastSeriesText.closest("#search-card");
      expect(podcastSeriesCard?.textContent).toMatch(
        "Example Podcast SeriesSearch Type: MediaMedia Type: Podcast SeriesRank: 27Known For: Maria Gonzalez, David BrownYear: 1992",
      );
    });
  });
  describe("When search card contains a spotlight", () => {
    it("Should only show spotlight search card data fields", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "2" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const spotlightText = await screen.findByText("Lemur History Month");
      const spotlightCard = spotlightText.closest("#search-card");
      expect(spotlightCard?.textContent).toMatch(
        "Lemur History MonthSearch Type: —Media Type: —Rank: —Known For: A Celebration of Lemur Storytellers and StoriesYear: —",
      );
    });
  });

  // Selection / deselection animation tests

  describe("When a search card is selected", () => {
    it("Should mark the results container with has-selection so other cards fade via CSS", async () => {
      const { container } = render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const resultsContainer = container.querySelector(".results-container");
      if (!resultsContainer) {
        throw new Error("results-container not found");
      }
      const personText = await screen.findByText("Example Smith");
      const personCard = personText.closest("#search-card");
      if (!personCard) {
        throw new Error("Person search card not found");
      }

      expect(resultsContainer.classList.contains("has-selection")).toBe(false);
      fireEvent.click(personCard);
      expect(resultsContainer.classList.contains("has-selection")).toBe(true);
      expect(personCard.classList.contains("selected")).toBe(true);
    });

    it("Should set --orig-x and --orig-y CSS variables on the card so CSS can translate it to the upper-left", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const personText = await screen.findByText("Example Smith");
      const personCard = personText.closest<HTMLDivElement>("#search-card");
      if (!personCard) {
        throw new Error("Person search card not found");
      }

      expect(personCard.style.getPropertyValue("--orig-x")).toBe("");
      expect(personCard.style.getPropertyValue("--orig-y")).toBe("");
      fireEvent.click(personCard);
      // The variables are set unconditionally on selection (values come from
      // layout offsets, which are 0 in JSDOM but the property must exist).
      expect(personCard.style.getPropertyValue("--orig-x")).toMatch(/px$/);
      expect(personCard.style.getPropertyValue("--orig-y")).toMatch(/px$/);
    });
  });

  describe("When a selected search card is deselected by clicking it again", () => {
    it("Should remove has-selection from the container and selected from the card so CSS reverses the animation", async () => {
      const { container } = render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const resultsContainer = container.querySelector(".results-container");
      if (!resultsContainer) {
        throw new Error("results-container not found");
      }
      const personText = await screen.findByText("Example Smith");
      const personCard = personText.closest("#search-card");
      if (!personCard) {
        throw new Error("Person search card not found");
      }

      fireEvent.click(personCard);
      expect(resultsContainer.classList.contains("has-selection")).toBe(true);
      expect(personCard.classList.contains("selected")).toBe(true);
      expect(personCard.classList.contains("deselecting")).toBe(false);
      fireEvent.click(personCard);
      expect(resultsContainer.classList.contains("has-selection")).toBe(false);
      expect(personCard.classList.contains("selected")).toBe(false);
      expect(personCard.classList.contains("deselecting")).toBe(true);
      fireEvent.click(personCard);
      expect(resultsContainer.classList.contains("has-selection")).toBe(true);
      expect(personCard.classList.contains("selected")).toBe(true);
      expect(personCard.classList.contains("deselecting")).toBe(false);
    });
  });

  describe("When a selected search card is deselected via the ESC key", () => {
    it("Should clear the selection state so CSS reverses the animation", async () => {
      const { container } = render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const resultsContainer = container.querySelector(".results-container");
      if (!resultsContainer) {
        throw new Error("results-container not found");
      }
      const personText = await screen.findByText("Example Smith");
      const personCard = personText.closest("#search-card");
      if (!personCard) {
        throw new Error("Person search card not found");
      }

      fireEvent.click(personCard);
      expect(personCard.classList.contains("selected")).toBe(true);
      expect(resultsContainer.classList.contains("has-selection")).toBe(true);

      fireEvent.keyDown(window, { key: "Escape" });
      expect(personCard.classList.contains("selected")).toBe(false);
      expect(resultsContainer.classList.contains("has-selection")).toBe(false);
    });

    it("Should be a no-op when ESC is pressed and no card is selected", () => {
      const { container } = render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const resultsContainer = container.querySelector(".results-container");
      if (!resultsContainer) {
        throw new Error("results-container not found");
      }

      expect(resultsContainer.classList.contains("has-selection")).toBe(false);
      fireEvent.keyDown(window, { key: "Escape" });
      expect(resultsContainer.classList.contains("has-selection")).toBe(false);
    });
  });

  describe("When the search button is clicked while a card is selected", () => {
    it("Should clear the selection and run the new search", async () => {
      const { container } = render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const personText = await screen.findByText("Example Smith");
      const personCard = personText.closest("#search-card");
      if (!personCard) {
        throw new Error("Person search card not found");
      }
      fireEvent.click(personCard);
      expect(personCard.classList.contains("selected")).toBe(true);

      const resultsContainer = container.querySelector(".results-container");
      if (!resultsContainer) {
        throw new Error("results-container not found");
      }
      expect(resultsContainer.classList.contains("has-selection")).toBe(true);

      fireEvent.change(searchQueryInput, { target: { value: "2" } });
      fireEvent.click(searchButton);

      // New results render, and nothing should be selected.
      const videoGameText = await screen.findByText("Example Video Game");
      expect(videoGameText).toBeInTheDocument();
      const resultsContainerAfter =
        container.querySelector(".results-container");
      expect(resultsContainerAfter?.classList.contains("has-selection")).toBe(
        false,
      );
      const selectedCards = container.querySelectorAll(".result-card.selected");
      expect(selectedCards.length).toBe(0);
    });
  });

  describe("When a card is deselected via the ESC key", () => {
    it("Should add a .deselecting class to that card so the exit animation plays", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const personText = await screen.findByText("Example Smith");
      const personCard = personText.closest<HTMLDivElement>("#search-card");
      if (!personCard) {
        throw new Error("Person search card not found");
      }

      fireEvent.click(personCard);
      expect(personCard.classList.contains("selected")).toBe(true);

      fireEvent.keyDown(window, { key: "Escape" });
      expect(personCard.classList.contains("selected")).toBe(false);
      expect(personCard.classList.contains("deselecting")).toBe(true);
    });
  });

  describe("When a card has never been selected", () => {
    it("Should not have the .deselecting class", async () => {
      const { container } = render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      await screen.findByText("Example Smith");
      const deselecting = container.querySelectorAll(
        ".result-card.deselecting",
      );
      expect(deselecting.length).toBe(0);
    });
  });

  describe("When re-selecting a card that was just deselected", () => {
    it("Should clear .deselecting and add .selected so the entry animation plays cleanly", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const personText = await screen.findByText("Example Smith");
      const personCard = personText.closest<HTMLDivElement>("#search-card");
      if (!personCard) {
        throw new Error("Person search card not found");
      }

      fireEvent.click(personCard); // select
      fireEvent.click(personCard); // deselect
      expect(personCard.classList.contains("deselecting")).toBe(true);

      fireEvent.click(personCard); // re-select
      expect(personCard.classList.contains("selected")).toBe(true);
      expect(personCard.classList.contains("deselecting")).toBe(false);
    });
  });

  describe("When a new search runs after a card was previously deselected", () => {
    it("Should clear the deselect tracking so a re-rendered card with a matching id does not animate", async () => {
      const { container } = render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      const searchQueryInput = screen.getByRole("textbox", {
        name: "search-query-input",
      });
      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      const searchButton = screen.getByRole("button", { name: "search" });
      fireEvent.click(searchButton);

      const personText = await screen.findByText("Example Smith");
      const personCard = personText.closest<HTMLDivElement>("#search-card");
      if (!personCard) {
        throw new Error("Person search card not found");
      }
      fireEvent.click(personCard); // select
      fireEvent.click(personCard); // deselect
      expect(personCard.classList.contains("deselecting")).toBe(true);

      fireEvent.change(searchQueryInput, { target: { value: "1" } });
      fireEvent.click(searchButton);
      await screen.findByText("Example Smith");
      const deselectingCards = container.querySelectorAll(
        ".result-card.deselecting",
      );
      expect(deselectingCards.length).toBe(0);
    });
  });
});
