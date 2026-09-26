import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { MemoryRouter } from "react-router-dom";
import { http, HttpResponse } from "msw";
import { server } from "../../tests/mocks/node.ts";
import tvSeriesDataJson1 from "../../tests/mocks/data/tvSeriesData1.json" with { type: "json" };
import SuggestionSearch from "../suggestion/SuggestionSearch";
import TvSeriesPanel from "./TvSeriesPanel";

async function searchAndSelectExampleTvSeries() {
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

  const tvSeriesText = await screen.findByText("Example TV Series");
  const tvSeriesCard = tvSeriesText.closest<HTMLDivElement>("#search-card");
  if (!tvSeriesCard) {
    throw new Error("TV series search card not found");
  }
  fireEvent.click(tvSeriesCard);
  return { ...utils, tvSeriesCard };
}

describe("TvSeriesPanel", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe("When a TV series search card is selected", () => {
    it("Should expand the card into the TV series panel after the fly animation", async () => {
      const { tvSeriesCard } = await searchAndSelectExampleTvSeries();

      expect(tvSeriesCard.classList.contains("selected")).toBe(true);
      expect(tvSeriesCard.classList.contains("expanded")).toBe(false);

      const panel = await screen.findByTestId("tv-series-panel");
      expect(tvSeriesCard.classList.contains("expanded")).toBe(true);
      expect(tvSeriesCard).toContainElement(panel);
    });

    it("Should render the visible TV series data members", async () => {
      await searchAndSelectExampleTvSeries();
      const panel = await screen.findByTestId("tv-series-panel");
      const text = panel.textContent;

      expect(screen.getByAltText("Example TV Series")).toHaveAttribute(
        "src",
        "https://example.com/example3.jpg",
      );
      for (const expected of [
        "Example TV SeriesAn example TV series tagline.IMDB: 8.3 (172,659)Rank: 4444LinkCopy",
        "Original NameExample TV Series Original",
        "Years2001-2003",
        "First Air DateMar 10, 2001",
        "Last Air DateMay 20, 2003",
        "Next Air Date—",
        "Average Runtime44m42m, 44m",
        "RatedTV-14",
        "StatusEnded",
        "In ProductionNo",
        "TypeScripted",
        "Number of Seasons2",
        "Number of Episodes37",
        "Known ForJohn Smith, James Johnson",
        "GenresComedy, Drama, Sci-Fi & Fantasy, Action, Adventure",
        "Homepagehttps://example.com/example-tv-series",
        "Origin CountriesUnited States of America, Japan",
        "Production CountriesUnited States of America, Japan",
        "Origin LanguageEnglish",
        "Spoken LanguagesEnglish, Japanese",
      ]) {
        expect(text).toContain(expected);
      }
    });

    it("Should separate the OMDB and TMDB average runtimes with a vertical separator", async () => {
      await searchAndSelectExampleTvSeries();
      await screen.findByTestId("tv-series-panel");

      const runtime = screen.getByText("Average Runtime").nextElementSibling;
      expect(runtime?.textContent).toBe("44m42m, 44m");
      expect(runtime?.querySelector('[role="separator"]')).toHaveAttribute(
        "aria-orientation",
        "vertical",
      );
    });

    it("Should not render the hidden TV series data members", async () => {
      await searchAndSelectExampleTvSeries();
      const panel = await screen.findByTestId("tv-series-panel");
      const text = panel.textContent;

      for (const hidden of [
        "7c3e9a1b-5d2f-4e8a-b6c1-0d9f8e7a6b5c",
        "tt10000002",
        "Imdb Id",
        "First Year",
        "2001Years",
        "Awards",
        "Won 2 Example Emmys",
        "Backdrop",
        "/exampleBackdrop.jpg",
        "Tmdb Id",
        "90002",
        "44 min",
        "true",
        "false",
        "Sci-Fi,",
        "Genres (",
        "IMDb Rank",
        "IMDb Rating",
        "IMDb Votes",
        "Production Companies",
        "Example Productions",
        "Producers",
        "Example Producer",
      ]) {
        expect(text).not.toContain(hidden);
      }
      // "Spoken Languages" is shown, so check the label on its own
      expect(screen.queryByText("Languages")).not.toBeInTheDocument();
    });

    it("Should order the collapsed sections with seasons first and watch providers last", async () => {
      await searchAndSelectExampleTvSeries();
      const panel = await screen.findByTestId("tv-series-panel");

      const summaries = [...panel.querySelectorAll("summary")]
        .filter((s) => !s.closest("[data-testid='season-card']"))
        .map((s) => s.textContent.replace(/\(\d+\)$/, ""));
      expect(summaries).toEqual([
        "Seasons",
        "Cast",
        "Creators",
        "Directors",
        "Writers",
        "Overview (TMDB)",
        "Overview (OMDB)",
        "Networks",
        "Where to Stream",
        "Where to Rent",
        "Where to Buy",
      ]);
    });

    it("Should hide seasons, credits, overviews, networks, and watch providers behind collapsed sections", async () => {
      await searchAndSelectExampleTvSeries();
      await screen.findByTestId("tv-series-panel");

      for (const [summary, content] of [
        ["Seasons", "Season 1"],
        ["Cast", "Example Jones"],
        ["Creators", "Example Creator"],
        ["Directors", "Example Director"],
        ["Writers", "Example Writer"],
        ["Overview (TMDB)", "An example overview from TMDB."],
        ["Overview (OMDB)", "An example overview from OMDB."],
        ["Networks", "Example Network"],
        ["Where to Stream", "Example Stream"],
        ["Where to Buy", "Example Buy Store"],
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

    it("Should render seasons in order with specials last, each with its overview collapsed", async () => {
      await searchAndSelectExampleTvSeries();
      await screen.findByTestId("tv-series-panel");

      const seasonCards = screen.getAllByTestId("season-card");
      expect(seasonCards.map((c) => c.textContent)).toEqual([
        "Season 112 episodesMar 10, 2001OverviewAn example overview for season 1.Expand",
        "Season 222 episodesSep 15, 2002Overview—Expand",
        "No imageSpecials3 episodes—OverviewExample specials overview.Expand",
      ]);
      expect(screen.getByAltText("Season 1")).toHaveAttribute(
        "src",
        "https://image.tmdb.org/t/p/w185/exampleSeason1.jpg",
      );
      for (const card of seasonCards) {
        expect(card.querySelector("details")?.open).toBe(false);
      }
    });

    it("Should show an Expand button on each season card", async () => {
      await searchAndSelectExampleTvSeries();
      await screen.findByTestId("tv-series-panel");

      const seasonCards = screen.getAllByTestId("season-card");
      const expandButtons = screen.getAllByRole("button", { name: "Expand" });
      expect(expandButtons).toHaveLength(seasonCards.length);
      for (const [index, card] of seasonCards.entries()) {
        expect(card).toContainElement(expandButtons[index]);
      }
    });

    it("Should render cast in billed order and creators / crew / networks / watch providers as cards", async () => {
      await searchAndSelectExampleTvSeries();
      await screen.findByTestId("tv-series-panel");

      const castCards = screen.getAllByTestId("cast-card");
      expect(castCards.map((c) => c.textContent)).toEqual([
        "Example JonesLead, Lead's Twin37 episodes",
        "No imageExample BrownSecond Lead1 episode",
      ]);
      expect(screen.getByAltText("Example Jones")).toHaveAttribute(
        "src",
        "https://image.tmdb.org/t/p/w185/exampleJones.jpg",
      );

      expect(
        screen.getAllByTestId("creator-card").map((c) => c.textContent),
      ).toEqual(["Example Creator"]);
      expect(
        screen.getAllByTestId("crew-card").map((c) => c.textContent),
      ).toEqual([
        "Example DirectorDirector20 episodes",
        "No imageExample WriterWriter, Story Editor15 episodes",
      ]);
      expect(
        screen.getAllByTestId("network-card").map((c) => c.textContent),
      ).toEqual(["Example NetworkUS", "No logoExample Network 2JP"]);
      expect(screen.getByAltText("Example Network logo")).toHaveAttribute(
        "src",
        "https://image.tmdb.org/t/p/w92/exampleNetwork.png",
      );
      expect(screen.getAllByTestId("watch-provider-card")).toHaveLength(2);
      expect(screen.getByAltText("Example Stream logo")).toHaveAttribute(
        "src",
        "https://image.tmdb.org/t/p/w92/exampleStream.jpg",
      );
    });

    it("Should link to the IMDB title page in a new tab", async () => {
      await searchAndSelectExampleTvSeries();
      await screen.findByTestId("tv-series-panel");

      const link = screen.getByRole("link", { name: "Link" });
      expect(link).toHaveAttribute(
        "href",
        "https://www.imdb.com/title/tt10000002",
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

      await searchAndSelectExampleTvSeries();
      await screen.findByTestId("tv-series-panel");

      fireEvent.click(screen.getByRole("button", { name: "copy-imdb-link" }));
      expect(writeText).toHaveBeenCalledWith(
        '<a href="https://www.imdb.com/title/tt10000002">Link</a>',
      );
      expect(await screen.findByText("Copied!")).toBeInTheDocument();
    });

    it("Should not deselect the card when clicking inside the panel", async () => {
      const { tvSeriesCard } = await searchAndSelectExampleTvSeries();
      const panel = await screen.findByTestId("tv-series-panel");

      fireEvent.click(panel);
      expect(tvSeriesCard.classList.contains("selected")).toBe(true);
      expect(tvSeriesCard.classList.contains("expanded")).toBe(true);
    });

    it("Should replace the results message with a back arrow", async () => {
      await searchAndSelectExampleTvSeries();

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
  ])("When %s while the TV series panel is expanded", (_, goBack) => {
    it("Should shrink the panel, then deselect the card and restore the results message", async () => {
      const { container, tvSeriesCard } =
        await searchAndSelectExampleTvSeries();
      await screen.findByTestId("tv-series-panel");

      goBack();
      expect(tvSeriesCard.classList.contains("expanded")).toBe(false);
      expect(tvSeriesCard.classList.contains("selected")).toBe(true);
      expect(screen.queryByTestId("tv-series-panel")).not.toBeInTheDocument();

      await waitFor(() => {
        expect(tvSeriesCard.classList.contains("selected")).toBe(false);
      });
      expect(tvSeriesCard.classList.contains("deselecting")).toBe(true);
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

  describe("When the TV series has missing data", () => {
    it("Should show placeholders instead of the missing values", async () => {
      server.use(
        http.get("/api/tvseries", () =>
          HttpResponse.json({
            ...tvSeriesDataJson1,
            image: null,
            imdbRank: null,
            imdbRating: "N/A",
            imdbVotes: "N/A",
            tagline: "",
            years: null,
            firstAirDate: null,
            lastAirDate: null,
            nextAirDate: "2030-01-02",
            omdbAverageEpisodeRuntimeNumber: 0,
            tmdbEpisodeRunTimes: [],
            isInProduction: true,
            knownForActors: null,
            omdbGenres: "N/A",
            tmdbGenres: "",
            homepage: null,
            seasons: [],
            creators: [],
            networks: [],
            watchProvidersBuy: [],
            watchProvidersFlatrate: [],
          }),
        ),
      );

      render(<TvSeriesPanel imdbId="tt10000002" />);
      const panel = await screen.findByTestId("tv-series-panel");
      const text = panel.textContent;

      for (const expected of [
        "No imageExample TV SeriesIMDB: —Rank: —LinkCopy",
        "Years—",
        "First Air Date—",
        "Last Air Date—",
        "Next Air DateJan 2, 2030",
        "Average Runtime——",
        "In ProductionYes",
        "Known For—",
        "Genres—",
        "Homepage—",
        "Seasons(0)None",
        "Creators(0)None",
        "Networks(0)None",
        "Where to Stream(0)None",
        "Where to Buy(0)None",
      ]) {
        expect(text).toContain(expected);
      }
      expect(text).not.toContain("An example TV series tagline.");
    });
  });

  describe("When the TV series API returns an error", () => {
    it("Should show an error message", async () => {
      render(<TvSeriesPanel imdbId="tt9999999" />);

      expect(
        await screen.findByText(
          "Loading TV series failed with status 404. Please try again.",
        ),
      ).toBeInTheDocument();
    });
  });
});
