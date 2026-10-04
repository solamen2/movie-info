import {
  act,
  fireEvent,
  render,
  screen,
  waitFor,
  within,
} from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { MemoryRouter } from "react-router-dom";
import { type AppRouter, renderApp } from "../../tests/renderApp";
import { http, HttpResponse } from "msw";
import { server } from "../../tests/mocks/node.ts";
import movieDataJson1 from "../../tests/mocks/data/movieData1.json" with { type: "json" };
import MoviePanel from "./MoviePanel";

async function searchAndSelectExampleMovie() {
  const utils = renderApp();
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

// Finds the person (cast / crew / creator / guest star) card with the given
// text inside a panel.
function findPersonCard(panel: HTMLElement, testId: string, text: string) {
  const card = within(panel)
    .getAllByTestId(testId)
    .find((c) => c.textContent.includes(text));
  if (!card) {
    throw new Error(`${testId} containing ${text} not found`);
  }
  return card;
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
    it("Should hide the results message", async () => {
      await searchAndSelectExampleMovie();

      expect(screen.queryByText("8 results.")).not.toBeInTheDocument();
    });
  });

  describe.each([
    [
      "the browser back button is used",
      (router: AppRouter) => act(() => router.navigate(-1)),
    ],
    [
      "the ESC key is pressed",
      () => {
        fireEvent.keyDown(window, { key: "Escape" });
      },
    ],
  ])("When %s while the movie panel is expanded", (_, goBack) => {
    it("Should shrink the panel, then deselect the card and restore the results message", async () => {
      const { container, router, movieCard } =
        await searchAndSelectExampleMovie();
      await screen.findByTestId("movie-panel");

      await goBack(router);
      await waitFor(() => {
        expect(movieCard.classList.contains("expanded")).toBe(false);
      });
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
    });
  });

  describe("When a search card without a detail panel is selected", () => {
    it("Should not expand into a movie panel", async () => {
      renderApp();
      fireEvent.change(
        screen.getByRole("textbox", { name: "search-query-input" }),
        { target: { value: "2" } },
      );
      fireEvent.click(screen.getByRole("button", { name: "search" }));

      const videoGameCard = (
        await screen.findByText("Example Video Game")
      ).closest("#search-card");
      if (!videoGameCard) {
        throw new Error("Video game search card not found");
      }
      fireEvent.click(videoGameCard);

      await new Promise((resolve) => setTimeout(resolve, 700));
      expect(videoGameCard.classList.contains("selected")).toBe(true);
      expect(videoGameCard.classList.contains("expanded")).toBe(false);
      expect(screen.queryByTestId("movie-panel")).not.toBeInTheDocument();
    });
  });

  describe("When the movie has been released", () => {
    it("Should not show the status", async () => {
      render(
        <MemoryRouter>
          <MoviePanel itemId={movieDataJson1.imdbId} />
        </MemoryRouter>,
      );
      const panel = await screen.findByTestId("movie-panel");

      expect(screen.getByRole("heading", { level: 2 }).textContent).toBe(
        "Example Movie",
      );
      expect(screen.queryByTestId("title-status")).toBeNull();
      expect(panel.textContent).not.toContain("Status");
    });
  });

  describe("When the movie has not been released", () => {
    it("Should show the status beside the title's heading instead of as a fact", async () => {
      server.use(
        http.get("/api/movie", () =>
          HttpResponse.json({ ...movieDataJson1, status: "Post Production" }),
        ),
      );

      render(
        <MemoryRouter>
          <MoviePanel itemId={movieDataJson1.imdbId} />
        </MemoryRouter>,
      );
      const panel = await screen.findByTestId("movie-panel");

      const heading = screen.getByRole("heading", { level: 2 });
      expect(heading.textContent).toBe("Example Movie");
      const status = screen.getByTestId("title-status");
      expect(heading.nextElementSibling).toBe(status);
      expect(status.tagName).toBe("I");
      expect(status.textContent).toBe("(post production)");
      expect(panel.textContent).not.toContain("Status");
    });
  });

  describe("When the movie API returns an error", () => {
    it("Should show an error message", async () => {
      render(
        <MemoryRouter>
          <MoviePanel itemId="tt9999999" />
        </MemoryRouter>,
      );

      expect(
        await screen.findByText(
          "Loading movie failed with status 404: Not a valid IMDB ID for mock. Please try another search.",
        ),
      ).toBeInTheDocument();
    });
  });

  describe("When a person card inside the movie panel is clicked", () => {
    it.each([
      ["Cast", "cast-card", "Example Brown", "/person/2?q=1"],
      ["Directors", "crew-card", "Example Director", "/person/3?q=1"],
      ["Writers", "crew-card", "Example Writer", "/person/4?q=1"],
    ])(
      "Should replace the movie panel with the %s person's panel at their TMDB id URL",
      async (_, testId, name, expectedUrl) => {
        const { currentUrl, container, movieCard } =
          await searchAndSelectExampleMovie();
        const moviePanel = await screen.findByTestId("movie-panel");

        fireEvent.click(findPersonCard(moviePanel, testId, name));
        expect(currentUrl()).toBe(expectedUrl);
        // The movie's card is dropped at once rather than flying home...
        expect(movieCard).not.toHaveClass("selected");
        expect(movieCard).not.toHaveClass("expanded");
        expect(movieCard).toHaveClass("deselecting");
        expect(moviePanel).not.toBeInTheDocument();
        // ...and the person (who is no search result) gets a stand-in card
        // that is shown already expanded, with the results still hidden
        const personPanel = await screen.findByTestId("person-panel");
        const personCard = personPanel.closest("#search-card");
        expect(personCard).not.toBe(movieCard);
        expect(personCard).toHaveClass("selected");
        expect(personCard).toHaveClass("expanded");
        expect(
          container.querySelectorAll("#search-card.selected"),
        ).toHaveLength(1);
        expect(container.querySelector(".results-container")).toHaveClass(
          "has-selection",
        );
        expect(
          within(personPanel).getByRole("heading", { name }),
        ).toBeInTheDocument();
        expect(screen.queryByText("8 results.")).not.toBeInTheDocument();
      },
    );

    it("Should scroll to the top of the page", async () => {
      const scrollTo = vi
        .spyOn(window, "scrollTo")
        .mockImplementation(() => undefined);
      await searchAndSelectExampleMovie();
      const moviePanel = await screen.findByTestId("movie-panel");

      fireEvent.click(findPersonCard(moviePanel, "cast-card", "Example Brown"));
      expect(scrollTo).toHaveBeenCalledWith({ top: 0 });
    });

    it("Should bring the movie panel back on browser back, and the person's panel on forward", async () => {
      const { currentUrl, container, movieCard, goBack, goForward } =
        await searchAndSelectExampleMovie();
      const moviePanel = await screen.findByTestId("movie-panel");
      fireEvent.click(findPersonCard(moviePanel, "cast-card", "Example Brown"));
      const personPanel = await screen.findByTestId("person-panel");

      await goBack();
      expect(currentUrl()).toBe("/movie/tt0000001?q=1");
      expect(personPanel).not.toBeInTheDocument();
      // The movie's own card hosts its panel again, shown expanded at once,
      // and the person's stand-in card is gone
      expect(movieCard).toHaveClass("selected");
      expect(movieCard).toHaveClass("expanded");
      expect(movieCard).not.toHaveClass("deselecting");
      expect(movieCard).toContainElement(
        await screen.findByTestId("movie-panel"),
      );
      expect(container.querySelectorAll("#search-card")).toHaveLength(8);

      await goForward();
      expect(currentUrl()).toBe("/person/2?q=1");
      expect(movieCard).toHaveClass("deselecting");
      await screen.findByTestId("person-panel");
      expect(container.querySelectorAll("#search-card")).toHaveLength(9);
    });
  });
});
