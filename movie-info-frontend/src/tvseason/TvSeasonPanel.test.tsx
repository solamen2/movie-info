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
import { http, HttpResponse } from "msw";
import { type AppRouter, renderApp } from "../../tests/renderApp";
import { server } from "../../tests/mocks/node.ts";
import tvSeasonDataJson1 from "../../tests/mocks/data/tvSeasonData1.json" with { type: "json" };
import TvSeasonPanel from "./TvSeasonPanel";

const SERIES_URL = "/tvseries/tt10000002?q=1";
const SEASON_URL = `${SERIES_URL}&tmdbTvSeriesId=90002&seasonNumber=1`;

function findSeasonCard(name: string) {
  const card = screen
    .getAllByTestId("season-card")
    .find((c) => c.textContent.includes(name));
  if (!card) {
    throw new Error(`Season card for ${name} not found`);
  }
  return card;
}

// Search, open the TV series panel, and click Expand on its first season card.
async function searchAndSelectExampleSeason() {
  const utils = renderApp();
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
  await screen.findByTestId("tv-series-panel");

  const seasonCard = findSeasonCard("Season 1");
  fireEvent.click(within(seasonCard).getByRole("button", { name: "Expand" }));
  return { ...utils, tvSeriesCard, seasonCard };
}

describe("TvSeasonPanel", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe("When a season card's Expand button is clicked", () => {
    it("Should put the API's query parameters in the URL", async () => {
      const { currentUrl } = await searchAndSelectExampleSeason();
      expect(currentUrl()).toBe(SEASON_URL);
    });

    it("Should expand the card into the TV season panel after the fly animation", async () => {
      const { seasonCard, container } = await searchAndSelectExampleSeason();

      expect(seasonCard.classList.contains("selected")).toBe(true);
      expect(seasonCard.classList.contains("expanded")).toBe(false);
      expect(
        container.querySelector(".horizontal-list.has-selection"),
      ).toContainElement(seasonCard);

      const panel = await screen.findByTestId("tv-season-panel");
      expect(seasonCard.classList.contains("expanded")).toBe(true);
      expect(seasonCard).toContainElement(panel);
      // The season lives inside the still-expanded TV series panel
      expect(screen.getByTestId("tv-series-panel")).toContainElement(panel);
    });

    it("Should hand the TV series panel over to the season, hiding the rest of the series", async () => {
      const { seasonCard } = await searchAndSelectExampleSeason();

      // Applied as soon as the card is selected, so the rest of the series
      // fades out while the card flies to the panel's upper-left
      const tvSeriesPanel = screen.getByTestId("tv-series-panel");
      const seasonsSection = screen.getByText("Seasons").closest("details");
      expect(tvSeriesPanel).toHaveClass("has-child-selection");
      expect(seasonsSection).toHaveClass("has-child-selection");
      expect(seasonsSection).toContainElement(seasonCard);

      await screen.findByTestId("tv-season-panel");
      expect(tvSeriesPanel).toHaveClass("has-child-selection");
    });

    it("Should keep the Seasons section open", async () => {
      await searchAndSelectExampleSeason();
      await screen.findByTestId("tv-season-panel");

      expect(screen.getByText("Seasons").closest("details")?.open).toBe(true);
    });

    it("Should render the visible TV season data members", async () => {
      await searchAndSelectExampleSeason();
      const panel = await screen.findByTestId("tv-season-panel");
      const text = panel.textContent;

      expect(within(panel).getByAltText("Season 1")).toHaveAttribute(
        "src",
        "https://image.tmdb.org/t/p/w500/exampleSeason1.jpg",
      );
      for (const expected of [
        "Season 1TV Series IMDB:Link",
        "Season Number1",
        "Number of Episodes3",
        "First Air DateMar 10, 2001",
      ]) {
        expect(text).toContain(expected);
      }
    });

    it("Should not render the hidden TV season data members", async () => {
      await searchAndSelectExampleSeason();
      const panel = await screen.findByTestId("tv-season-panel");
      const text = panel.textContent;

      for (const hidden of [
        "a1b2c3d4-0000-4000-8000-000000000001",
        "59473",
        "Tmdb",
        "Poster",
        "/exampleSeason1.jpg",
        "2001-03-10",
        "Example Producer",
        "Producer",
        "Copy",
      ]) {
        expect(text).not.toContain(hidden);
      }
    });

    it("Should order the collapsed sections with episodes first and watch providers last", async () => {
      await searchAndSelectExampleSeason();
      const panel = await screen.findByTestId("tv-season-panel");

      const summaries = [...panel.querySelectorAll("summary")]
        .filter((s) => !s.closest("[data-testid='episode-card']"))
        .map((s) => s.textContent.replace(/\(\d+\)$/, ""));
      expect(summaries).toEqual([
        "Episodes",
        "Overview (TMDB)",
        "Networks",
        "Where to Stream",
        "Where to Rent",
        "Where to Buy",
      ]);
    });

    it("Should hide episodes, the overview, networks, and watch providers behind collapsed sections", async () => {
      await searchAndSelectExampleSeason();
      const panel = await screen.findByTestId("tv-season-panel");

      for (const [summary, content] of [
        ["Episodes", "Example Pilot"],
        ["Overview (TMDB)", "An example overview for season 1."],
        ["Networks", "Example Network"],
        ["Where to Stream", "Example Stream"],
        ["Where to Buy", "Example Buy Store"],
      ]) {
        const details = within(panel).getByText(summary).closest("details");
        if (!details) {
          throw new Error(`Collapsible for ${summary} not found`);
        }
        expect(details.open).toBe(false);
        expect(details.textContent).toContain(content);
      }
      expect(
        within(panel).getByText("Where to Rent").closest("details")
          ?.textContent,
      ).toContain("None");
    });

    it("Should render episodes in order, each with its overview collapsed and an Expand button", async () => {
      await searchAndSelectExampleSeason();
      const panel = await screen.findByTestId("tv-season-panel");

      const episodeCards = within(panel).getAllByTestId("episode-card");
      expect(episodeCards.map((c) => c.textContent)).toEqual([
        "1. Example PilotMar 10, 200144m · StandardDirected by Example DirectorWritten by Example WriterOverviewAn example overview for episode 1.Expand",
        "No image2. Example Second EpisodeMar 17, 2001— · StandardOverview—Expand",
        "3. Example FinaleMay 22, 200145m · FinaleDirected by Example DirectorWritten by Example Writer, Example DirectorOverviewAn example overview for the finale.Expand",
      ]);
      expect(within(panel).getByAltText("Example Pilot")).toHaveAttribute(
        "src",
        "https://image.tmdb.org/t/p/w185/exampleStill1.jpg",
      );
      for (const card of episodeCards) {
        expect(card.querySelector("details")?.open).toBe(false);
        expect(
          within(card).getByRole("button", { name: "Expand" }),
        ).toBeInTheDocument();
      }
    });

    it("Should render networks and watch providers as cards", async () => {
      await searchAndSelectExampleSeason();
      const panel = await screen.findByTestId("tv-season-panel");

      expect(
        within(panel)
          .getAllByTestId("network-card")
          .map((c) => c.textContent),
      ).toEqual(["Example NetworkUS", "No logoExample Network 2JP"]);
      expect(within(panel).getAllByTestId("watch-provider-card")).toHaveLength(
        2,
      );
      expect(within(panel).getByAltText("Example Stream logo")).toHaveAttribute(
        "src",
        "https://image.tmdb.org/t/p/w92/exampleStream.jpg",
      );
    });

    it("Should link to the TV series' IMDB page in a new tab without a copy button", async () => {
      await searchAndSelectExampleSeason();
      const panel = await screen.findByTestId("tv-season-panel");

      const link = within(panel).getByRole("link", { name: "Link" });
      expect(link).toHaveAttribute(
        "href",
        "https://www.imdb.com/title/tt10000002",
      );
      expect(link).toHaveAttribute("target", "_blank");
      expect(link).toHaveAttribute("rel", "noopener");
      expect(
        within(panel).queryByRole("button", { name: "copy-imdb-link" }),
      ).not.toBeInTheDocument();
    });

    it("Should not deselect the card when clicking inside the panel", async () => {
      const { seasonCard } = await searchAndSelectExampleSeason();
      const panel = await screen.findByTestId("tv-season-panel");

      fireEvent.click(panel);
      expect(seasonCard.classList.contains("selected")).toBe(true);
      expect(seasonCard.classList.contains("expanded")).toBe(true);
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
  ])("When %s while the TV season panel is expanded", (_, goBack) => {
    it("Should shrink the panel, then deselect the season card and bring the whole TV series back", async () => {
      const { container, router, currentUrl, seasonCard, tvSeriesCard } =
        await searchAndSelectExampleSeason();
      await screen.findByTestId("tv-season-panel");
      const tvSeriesPanel = screen.getByTestId("tv-series-panel");

      await goBack(router);
      await waitFor(() => {
        expect(seasonCard.classList.contains("expanded")).toBe(false);
      });
      expect(currentUrl()).toBe(SERIES_URL);
      expect(seasonCard.classList.contains("selected")).toBe(true);
      expect(screen.queryByTestId("tv-season-panel")).not.toBeInTheDocument();
      // The series stays hidden until the card has shrunk back to card size
      expect(tvSeriesPanel).toHaveClass("has-child-selection");

      await waitFor(() => {
        expect(seasonCard.classList.contains("selected")).toBe(false);
      });
      expect(seasonCard.classList.contains("deselecting")).toBe(true);
      expect(
        container.querySelector(".horizontal-list.has-selection"),
      ).toBeNull();
      expect(tvSeriesPanel).not.toHaveClass("has-child-selection");
      expect(screen.getByText("Seasons").closest("details")).not.toHaveClass(
        "has-child-selection",
      );
      expect(screen.getAllByTestId("season-card")).toHaveLength(3);
      expect(screen.getByTestId("tv-series-panel")).toBeInTheDocument();
      expect(tvSeriesCard.classList.contains("expanded")).toBe(true);
    });
  });

  describe("When a TV season URL is loaded directly", () => {
    it("Should show the season panel inside the TV series panel at once", async () => {
      renderApp(SEASON_URL);
      const panel = await screen.findByTestId("tv-season-panel");
      const seasonCard = panel.closest("[data-testid='season-card']");
      expect(seasonCard?.classList.contains("selected")).toBe(true);
      expect(seasonCard?.classList.contains("expanded")).toBe(true);
      expect(screen.getByTestId("tv-series-panel")).toContainElement(panel);
      expect(screen.getByTestId("tv-series-panel")).toHaveClass(
        "has-child-selection",
      );
      expect(screen.getByText("Seasons").closest("details")?.open).toBe(true);
      expect(panel.textContent).toContain("Number of Episodes3");
    });
  });

  describe("When the TV season has missing data", () => {
    it("Should show placeholders instead of the missing values", async () => {
      server.use(
        http.get("/api/tvseason", () =>
          HttpResponse.json({
            ...tvSeasonDataJson1,
            posterPath: null,
            firstAirDate: null,
            episodes: [],
            numberOfEpisodes: 0,
            tmdbOverview: "",
            networks: [],
            watchProvidersBuy: [],
            watchProvidersFlatrate: [],
          }),
        ),
      );

      render(
        <MemoryRouter>
          <TvSeasonPanel
            tmdbTvSeriesId={90002}
            seasonNumber={1}
            tvSeriesImdbId="tt10000002"
          />
        </MemoryRouter>,
      );
      const panel = await screen.findByTestId("tv-season-panel");
      const text = panel.textContent;

      for (const expected of [
        "No imageSeason 1TV Series IMDB:Link",
        "Number of Episodes0",
        "First Air Date—",
        "Episodes(0)None",
        "Overview (TMDB)—",
        "Networks(0)None",
        "Where to Stream(0)None",
        "Where to Buy(0)None",
      ]) {
        expect(text).toContain(expected);
      }
    });
  });

  describe("When the TV season API returns an error", () => {
    it("Should show an error message", async () => {
      render(
        <MemoryRouter>
          <TvSeasonPanel
            tmdbTvSeriesId={90002}
            seasonNumber={9}
            tvSeriesImdbId="tt10000002"
          />
        </MemoryRouter>,
      );

      expect(
        await screen.findByText(
          "Loading TV season failed with status 404. Please try again.",
        ),
      ).toBeInTheDocument();
    });
  });
});
