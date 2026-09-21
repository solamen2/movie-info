import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { MemoryRouter } from "react-router-dom";
import SuggestionSearch from "../suggestion/SuggestionSearch";
import MoviePanel from "./MoviePanel";

async function searchAndSelectExampleMovie() {
  const utils = render(
    <MemoryRouter>
      <SuggestionSearch />
    </MemoryRouter>,
  );
  const searchQueryInput = screen.getByRole("textbox", {
    name: "search-query-input",
  });
  fireEvent.change(searchQueryInput, { target: { value: "1" } });
  fireEvent.click(screen.getByRole("button", { name: "search" }));

  const movieText = await screen.findByText("Example Movie");
  const movieCard = movieText.closest<HTMLDivElement>("#search-card");
  if (!movieCard) {
    throw new Error("Movie search card not found");
  }
  fireEvent.click(movieCard);
  return { ...utils, movieCard };
}

describe("MoviePanel", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe("When a movie search card is selected", () => {
    it("Should expand the card into the movie panel after the fly animation", async () => {
      const { movieCard } = await searchAndSelectExampleMovie();

      expect(movieCard.classList.contains("selected")).toBe(true);
      expect(movieCard.classList.contains("expanded")).toBe(false);

      const panel = await screen.findByTestId("movie-panel");
      expect(movieCard.classList.contains("expanded")).toBe(true);
      expect(movieCard).toContainElement(panel);
    });

    it("Should render the visible movie data members", async () => {
      await searchAndSelectExampleMovie();
      const panel = await screen.findByTestId("movie-panel");
      const text = panel.textContent;

      for (const expected of [
        "Example Movie",
        "An example tagline.",
        "Original TitleExample Movie Original",
        "Release DateSep 23, 2016",
        "Runtime2h 22m",
        "RatedPG-13",
        "StatusReleased",
        "IMDB: 7.9 (123,456)Rank: 4444LinkCopy",
        "Known ForExample Jones, Example Brown",
        "GenresMystery, Thriller, Drama, Science Fiction, Horror",
        "Budget$25,000,000",
        "Revenue$58,000,000",
        "Homepagehttps://example.com/example-movie",
        "Origin CountriesUnited States of America",
        "Production CountriesUnited States of America",
        "Origin LanguageEnglish",
        "Spoken LanguagesEnglish, Spanish",
      ]) {
        expect(text).toContain(expected);
      }
    });

    it("Should not render the hidden movie data members", async () => {
      await searchAndSelectExampleMovie();
      const panel = await screen.findByTestId("movie-panel");
      const text = panel.textContent;

      for (const hidden of [
        "90001",
        "12345678",
        "Year",
        "Genres (",
        "Sci-Fi",
        "IMDb Rank",
        "IMDb Rating",
        "IMDb Votes",
        "Awards",
        "Won 2 Example Awards",
        "Box Office",
        "$12,345,678",
        "Production Companies",
        "Example Studios",
        "Producers",
      ]) {
        expect(text).not.toContain(hidden);
      }
    });

    it("Should order the collapsed sections with credits first and watch providers last", async () => {
      await searchAndSelectExampleMovie();
      const panel = await screen.findByTestId("movie-panel");

      const summaries = [...panel.querySelectorAll("summary")].map((s) =>
        s.textContent.replace(/\(\d+\)$/, ""),
      );
      expect(summaries).toEqual([
        "Cast",
        "Directors",
        "Writers",
        "Plot (TMDB)",
        "Plot (OMDB)",
        "Where to Stream",
        "Where to Rent",
        "Where to Buy",
      ]);
    });

    it("Should hide plots, credits, and watch providers behind collapsed sections", async () => {
      await searchAndSelectExampleMovie();
      await screen.findByTestId("movie-panel");

      for (const [summary, content] of [
        ["Plot (OMDB)", "An example plot from OMDB."],
        ["Plot (TMDB)", "An example plot from TMDB."],
        ["Cast", "Example Jones"],
        ["Directors", "Example Director"],
        ["Writers", "Example Writer"],
        ["Where to Buy", "Example Buy Store"],
        ["Where to Stream", "Example Stream"],
      ]) {
        const details = screen.getByText(summary).closest("details");
        if (!details) {
          throw new Error(`Collapsible for ${summary} not found`);
        }
        expect(details.open).toBe(false);
        expect(details.textContent).toContain(content);
      }
      expect(
        screen.getByText("Where to Rent").closest("details")?.textContent,
      ).toContain("None");
    });

    it("Should render cast in billed order and crew / watch providers as cards", async () => {
      await searchAndSelectExampleMovie();
      await screen.findByTestId("movie-panel");

      const castCards = screen.getAllByTestId("cast-card");
      expect(castCards.map((c) => c.textContent)).toEqual([
        "Example JonesLead",
        "No imageExample BrownSecond Lead",
      ]);
      expect(screen.getByAltText("Example Jones")).toHaveAttribute(
        "src",
        "https://image.tmdb.org/t/p/w185/exampleJones.jpg",
      );

      expect(screen.getAllByTestId("crew-card")).toHaveLength(2);
      expect(screen.getAllByTestId("watch-provider-card")).toHaveLength(2);
      expect(screen.getByAltText("Example Stream logo")).toHaveAttribute(
        "src",
        "https://image.tmdb.org/t/p/w92/exampleStream.jpg",
      );
    });

    it("Should link to the IMDB title page in a new tab", async () => {
      await searchAndSelectExampleMovie();
      await screen.findByTestId("movie-panel");

      const link = screen.getByRole("link", { name: "Link" });
      expect(link).toHaveAttribute(
        "href",
        "https://www.imdb.com/title/tt0000001",
      );
      expect(link).toHaveAttribute("target", "_blank");
      expect(link).toHaveAttribute("rel", "noopener");
    });

    it("Should copy the IMDB link as an HTML anchor when Copy is clicked", async () => {
      const writeText = vi.fn().mockResolvedValue(undefined);
      Object.defineProperty(navigator, "clipboard", {
        value: { writeText },
        configurable: true,
      });

      await searchAndSelectExampleMovie();
      await screen.findByTestId("movie-panel");

      fireEvent.click(screen.getByRole("button", { name: "copy-imdb-link" }));
      expect(writeText).toHaveBeenCalledWith(
        '<a href="https://www.imdb.com/title/tt0000001">Link</a>',
      );
      expect(await screen.findByText("Copied!")).toBeInTheDocument();
    });

    it("Should not deselect the card when clicking inside the panel", async () => {
      const { movieCard } = await searchAndSelectExampleMovie();
      const panel = await screen.findByTestId("movie-panel");

      fireEvent.click(panel);
      expect(movieCard.classList.contains("selected")).toBe(true);
      expect(movieCard.classList.contains("expanded")).toBe(true);
    });
  });

  describe("When a card is selected", () => {
    it("Should replace the results message with a back arrow", async () => {
      await searchAndSelectExampleMovie();

      expect(screen.queryByText("8 results.")).not.toBeInTheDocument();
      expect(screen.getByRole("button", { name: "back" })).toBeInTheDocument();
    });
  });

  describe.each([
    [
      "the back arrow is clicked",
      () => {
        fireEvent.click(screen.getByRole("button", { name: "back" }));
      },
    ],
    [
      "the ESC key is pressed",
      () => {
        fireEvent.keyDown(window, { key: "Escape" });
      },
    ],
  ])("When %s while the movie panel is expanded", (_, goBack) => {
    it("Should shrink the panel, then deselect the card and restore the results message", async () => {
      const { container, movieCard } = await searchAndSelectExampleMovie();
      await screen.findByTestId("movie-panel");

      goBack();
      expect(movieCard.classList.contains("expanded")).toBe(false);
      expect(movieCard.classList.contains("selected")).toBe(true);
      expect(screen.queryByTestId("movie-panel")).not.toBeInTheDocument();

      await waitFor(() => {
        expect(movieCard.classList.contains("selected")).toBe(false);
      });
      expect(movieCard.classList.contains("deselecting")).toBe(true);
      expect(
        container
          .querySelector(".results-container")
          ?.classList.contains("has-selection"),
      ).toBe(false);
      expect(screen.getByText("8 results.")).toBeInTheDocument();
      expect(
        screen.queryByRole("button", { name: "back" }),
      ).not.toBeInTheDocument();
    });
  });

  describe("When a search card without a detail panel is selected", () => {
    it("Should not expand into a movie panel", async () => {
      render(
        <MemoryRouter>
          <SuggestionSearch />
        </MemoryRouter>,
      );
      fireEvent.change(
        screen.getByRole("textbox", { name: "search-query-input" }),
        { target: { value: "1" } },
      );
      fireEvent.click(screen.getByRole("button", { name: "search" }));

      const tvSeriesCard = (
        await screen.findByText("Example TV Series")
      ).closest("#search-card");
      if (!tvSeriesCard) {
        throw new Error("TV series search card not found");
      }
      fireEvent.click(tvSeriesCard);

      await new Promise((resolve) => setTimeout(resolve, 700));
      expect(tvSeriesCard.classList.contains("selected")).toBe(true);
      expect(tvSeriesCard.classList.contains("expanded")).toBe(false);
      expect(screen.queryByTestId("movie-panel")).not.toBeInTheDocument();
    });
  });

  describe("When the movie API returns an error", () => {
    it("Should show an error message", async () => {
      render(<MoviePanel imdbId="tt9999999" />);

      expect(
        await screen.findByText(
          "Loading movie failed with status 404. Please try again.",
        ),
      ).toBeInTheDocument();
    });
  });
});
