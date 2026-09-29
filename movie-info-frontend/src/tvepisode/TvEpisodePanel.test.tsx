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
import tvEpisodeDataJson1 from "../../tests/mocks/data/tvEpisodeData1.json" with { type: "json" };
import TvEpisodePanel from "./TvEpisodePanel";

const SEASON_URL =
  "/tvseries/tt10000002?q=1&tmdbTvSeriesId=90002&seasonNumber=1";
const EPISODE_URL = `${SEASON_URL}&episodeNumber=1`;

function findCardByText(testId: string, text: string) {
  const card = screen
    .getAllByTestId(testId)
    .find((c) => c.textContent.includes(text));
  if (!card) {
    throw new Error(`${testId} containing ${text} not found`);
  }
  return card;
}

// Search, open the TV series panel, expand its first season, and click Expand
// on the season's first episode card.
async function searchAndSelectExampleEpisode() {
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

  const seasonCard = findCardByText("season-card", "Season 1");
  fireEvent.click(within(seasonCard).getByRole("button", { name: "Expand" }));
  await screen.findByTestId("tv-season-panel");

  const episodeCard = findCardByText("episode-card", "Example Pilot");
  fireEvent.click(within(episodeCard).getByRole("button", { name: "Expand" }));
  return { ...utils, tvSeriesCard, seasonCard, episodeCard };
}

describe("TvEpisodePanel", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe("When an episode card's Expand button is clicked", () => {
    it("Should put the API's query parameters in the URL", async () => {
      const { currentUrl } = await searchAndSelectExampleEpisode();
      expect(currentUrl()).toBe(EPISODE_URL);
    });

    it("Should expand the card into the TV episode panel after the fly animation", async () => {
      const { episodeCard, container } = await searchAndSelectExampleEpisode();

      expect(episodeCard.classList.contains("selected")).toBe(true);
      expect(episodeCard.classList.contains("expanded")).toBe(false);
      expect(
        [...container.querySelectorAll(".horizontal-list.has-selection")].some(
          (list) => list.contains(episodeCard),
        ),
      ).toBe(true);

      const panel = await screen.findByTestId("tv-episode-panel");
      expect(episodeCard.classList.contains("expanded")).toBe(true);
      expect(episodeCard).toContainElement(panel);
      // The episode lives inside the still-expanded season and series panels
      expect(screen.getByTestId("tv-season-panel")).toContainElement(panel);
      expect(screen.getByTestId("tv-series-panel")).toContainElement(panel);
    });

    it("Should hand the TV season panel over to the episode, hiding the rest of the season", async () => {
      const { episodeCard } = await searchAndSelectExampleEpisode();

      const tvSeasonPanel = screen.getByTestId("tv-season-panel");
      const episodesSection = screen.getByText("Episodes").closest("details");
      expect(tvSeasonPanel).toHaveClass("has-child-selection");
      expect(episodesSection).toHaveClass("has-child-selection");
      expect(episodesSection).toContainElement(episodeCard);
      // The series is still handed over to the season as well
      expect(screen.getByTestId("tv-series-panel")).toHaveClass(
        "has-child-selection",
      );

      await screen.findByTestId("tv-episode-panel");
      expect(tvSeasonPanel).toHaveClass("has-child-selection");
    });

    it("Should keep the Episodes section open", async () => {
      await searchAndSelectExampleEpisode();
      await screen.findByTestId("tv-episode-panel");

      expect(screen.getByText("Episodes").closest("details")?.open).toBe(true);
    });

    it("Should render the visible TV episode data members", async () => {
      await searchAndSelectExampleEpisode();
      const panel = await screen.findByTestId("tv-episode-panel");
      const text = panel.textContent;

      expect(within(panel).getByAltText("Example Pilot")).toHaveAttribute(
        "src",
        "https://image.tmdb.org/t/p/w300/exampleStill1.jpg",
      );
      for (const expected of [
        "Example PilotSeason 1, Episode 1IMDB: 8.1 (3,456)LinkCopyTV Series IMDB:Link",
        "Air DateMar 10, 2001",
        "Year2001",
        "Runtime44m43m",
        "RatedTV-14",
        "TypeStandard",
        "Known ForExample Jones, Example Brown",
        "GenresAction, Drama, Science Fiction",
      ]) {
        expect(text).toContain(expected);
      }
    });

    it("Should separate the OMDB and TMDB runtimes with a vertical separator", async () => {
      await searchAndSelectExampleEpisode();
      const panel = await screen.findByTestId("tv-episode-panel");

      const runtime = within(panel).getByText("Runtime").nextElementSibling;
      expect(runtime?.textContent).toBe("44m43m");
      expect(runtime?.querySelector('[role="separator"]')).toHaveAttribute(
        "aria-orientation",
        "vertical",
      );
    });

    it("Should not render the hidden TV episode data members", async () => {
      await searchAndSelectExampleEpisode();
      const panel = await screen.findByTestId("tv-episode-panel");
      const text = panel.textContent;

      for (const hidden of [
        "b2c3d4e5-0000-4000-8000-000000000002",
        "Awards",
        "Won 1 Example Award",
        "Imdb Id",
        "tt20000001",
        "Tmdb",
        "7001",
        "44 min",
        "Sci-Fi",
        "Still",
        "/exampleStill1.jpg",
        "2001-03-10",
        "Producer",
        "Example Producer",
        "standard",
      ]) {
        expect(text).not.toContain(hidden);
      }
    });

    it("Should order the collapsed sections with credits first and overviews last", async () => {
      await searchAndSelectExampleEpisode();
      const panel = await screen.findByTestId("tv-episode-panel");

      const summaries = [...panel.querySelectorAll("summary")].map((s) =>
        s.textContent.replace(/\(\d+\)$/, ""),
      );
      expect(summaries).toEqual([
        "Cast",
        "Directors",
        "Writers",
        "Guest Stars",
        "Overview (OMDB)",
        "Overview (TMDB)",
      ]);
    });

    it("Should hide credits and overviews behind collapsed sections", async () => {
      await searchAndSelectExampleEpisode();
      const panel = await screen.findByTestId("tv-episode-panel");

      for (const [summary, content] of [
        ["Cast", "Example Jones"],
        ["Directors", "Example Director"],
        ["Writers", "Example Writer"],
        ["Guest Stars", "Example Guest"],
        ["Overview (OMDB)", "An example episode overview from OMDB."],
        ["Overview (TMDB)", "An example episode overview from TMDB."],
      ]) {
        const details = within(panel).getByText(summary).closest("details");
        if (!details) {
          throw new Error(`Collapsible for ${summary} not found`);
        }
        expect(details.open).toBe(false);
        expect(details.textContent).toContain(content);
      }
    });

    it("Should render cast and guest stars in billed order and crew as cards", async () => {
      await searchAndSelectExampleEpisode();
      const panel = await screen.findByTestId("tv-episode-panel");

      expect(
        within(panel)
          .getAllByTestId("cast-card")
          .map((c) => c.textContent),
      ).toEqual(["Example JonesLead", "No imageExample BrownSecond Lead"]);
      expect(within(panel).getByAltText("Example Jones")).toHaveAttribute(
        "src",
        "https://image.tmdb.org/t/p/w185/exampleJones.jpg",
      );
      expect(
        within(panel)
          .getAllByTestId("crew-card")
          .map((c) => c.textContent),
      ).toEqual(["Example DirectorDirector", "No imageExample WriterWriter"]);
      expect(
        within(panel)
          .getAllByTestId("guest-star-card")
          .map((c) => c.textContent),
      ).toEqual([
        "Example GuestExample Villain",
        "No imageExample Extra(Example Extra Original)Example Bystander",
      ]);
    });

    it("Should link to the episode's IMDB page with a copy button, and to the TV series' IMDB page without one", async () => {
      await searchAndSelectExampleEpisode();
      const panel = await screen.findByTestId("tv-episode-panel");

      const links = within(panel).getAllByRole("link", { name: "Link" });
      expect(links.map((l) => l.getAttribute("href"))).toEqual([
        "https://www.imdb.com/title/tt20000001",
        "https://www.imdb.com/title/tt10000002",
      ]);
      for (const link of links) {
        expect(link).toHaveAttribute("target", "_blank");
        expect(link).toHaveAttribute("rel", "noopener");
      }
      expect(
        within(panel).getAllByRole("button", { name: "copy-imdb-link" }),
      ).toHaveLength(1);
    });

    it("Should copy the episode's IMDB link as an HTML anchor when Copy is clicked", async () => {
      const writeText = vi.fn().mockResolvedValue(undefined);
      Object.defineProperty(navigator, "clipboard", {
        value: { writeText },
        configurable: true,
      });

      await searchAndSelectExampleEpisode();
      const panel = await screen.findByTestId("tv-episode-panel");

      fireEvent.click(
        within(panel).getByRole("button", { name: "copy-imdb-link" }),
      );
      expect(writeText).toHaveBeenCalledWith(
        '<a href="https://www.imdb.com/title/tt20000001">Link</a>',
      );
      expect(await within(panel).findByText("Copied!")).toBeInTheDocument();
    });

    it("Should not deselect the card when clicking inside the panel", async () => {
      const { episodeCard } = await searchAndSelectExampleEpisode();
      const panel = await screen.findByTestId("tv-episode-panel");

      fireEvent.click(panel);
      expect(episodeCard.classList.contains("selected")).toBe(true);
      expect(episodeCard.classList.contains("expanded")).toBe(true);
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
  ])("When %s while the TV episode panel is expanded", (_, goBack) => {
    it("Should shrink the panel, then deselect the episode card and bring the whole season back", async () => {
      const { router, currentUrl, episodeCard, seasonCard } =
        await searchAndSelectExampleEpisode();
      await screen.findByTestId("tv-episode-panel");
      const tvSeasonPanel = screen.getByTestId("tv-season-panel");

      await goBack(router);
      await waitFor(() => {
        expect(episodeCard.classList.contains("expanded")).toBe(false);
      });
      expect(currentUrl()).toBe(SEASON_URL);
      expect(episodeCard.classList.contains("selected")).toBe(true);
      expect(screen.queryByTestId("tv-episode-panel")).not.toBeInTheDocument();
      // The season stays hidden until the card has shrunk back to card size
      expect(tvSeasonPanel).toHaveClass("has-child-selection");

      await waitFor(() => {
        expect(episodeCard.classList.contains("selected")).toBe(false);
      });
      expect(episodeCard.classList.contains("deselecting")).toBe(true);
      expect(tvSeasonPanel).not.toHaveClass("has-child-selection");
      expect(screen.getByText("Episodes").closest("details")).not.toHaveClass(
        "has-child-selection",
      );
      // The series is still handed over to the season
      expect(screen.getByTestId("tv-series-panel")).toHaveClass(
        "has-child-selection",
      );
      expect(screen.getAllByTestId("episode-card")).toHaveLength(3);
      expect(screen.getByTestId("tv-season-panel")).toBeInTheDocument();
      expect(seasonCard.classList.contains("expanded")).toBe(true);
    });
  });

  describe("When a TV episode URL is loaded directly", () => {
    it("Should show the episode panel inside the season and series panels at once", async () => {
      renderApp(EPISODE_URL);
      const panel = await screen.findByTestId("tv-episode-panel");
      const episodeCard = panel.closest("[data-testid='episode-card']");
      expect(episodeCard?.classList.contains("selected")).toBe(true);
      expect(episodeCard?.classList.contains("expanded")).toBe(true);
      expect(screen.getByTestId("tv-season-panel")).toContainElement(panel);
      expect(screen.getByTestId("tv-series-panel")).toContainElement(panel);
      expect(screen.getByTestId("tv-season-panel")).toHaveClass(
        "has-child-selection",
      );
      expect(screen.getByTestId("tv-series-panel")).toHaveClass(
        "has-child-selection",
      );
      expect(screen.getByText("Episodes").closest("details")?.open).toBe(true);
      expect(panel.textContent).toContain("Season 1, Episode 1");
    });
  });

  describe("When the TV episode has missing data", () => {
    it("Should show placeholders instead of the missing values", async () => {
      server.use(
        http.get("/api/tvepisode", () =>
          HttpResponse.json({
            ...tvEpisodeDataJson1,
            stillPath: null,
            imdbRating: "N/A",
            imdbVotes: "N/A",
            airDate: null,
            year: 0,
            omdbAverageEpisodeRuntimeNumber: 0,
            runtime: 0,
            rated: "N/A",
            episodeType: "",
            knownForActors: "N/A",
            omdbGenres: "N/A",
            omdbOverview: "",
            cast: [],
            guestStars: [],
          }),
        ),
      );

      render(
        <MemoryRouter>
          <TvEpisodePanel
            tmdbTvSeriesId={90002}
            seasonNumber={1}
            episodeNumber={1}
            tvSeriesImdbId="tt10000002"
          />
        </MemoryRouter>,
      );
      const panel = await screen.findByTestId("tv-episode-panel");
      const text = panel.textContent;

      for (const expected of [
        "No imageExample PilotSeason 1, Episode 1IMDB: —LinkCopyTV Series IMDB:Link",
        "Air Date—",
        "Year—",
        "Runtime——",
        "Rated—",
        "Type—",
        "Known For—",
        "Genres—",
        "Cast(0)None",
        "Guest Stars(0)None",
        "Overview (OMDB)—",
      ]) {
        expect(text).toContain(expected);
      }
    });
  });

  describe("When the TV episode API returns an error", () => {
    it("Should show an error message", async () => {
      render(
        <MemoryRouter>
          <TvEpisodePanel
            tmdbTvSeriesId={90002}
            seasonNumber={1}
            episodeNumber={9}
            tvSeriesImdbId="tt10000002"
          />
        </MemoryRouter>,
      );

      expect(
        await screen.findByText(
          "Loading TV episode failed with status 404: Not a valid TV episode for mock. Please try another search.",
        ),
      ).toBeInTheDocument();
    });
  });
});
