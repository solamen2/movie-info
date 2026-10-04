import {
  act,
  fireEvent,
  render,
  screen,
  waitFor,
  within,
} from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { type AppRouter, renderApp } from "../../tests/renderApp";
import { CARD_FLY_MS } from "../shared/useCardSelection";
import { MemoryRouter } from "react-router-dom";
import { http, HttpResponse } from "msw";
import { server } from "../../tests/mocks/node.ts";
import tvSeriesDataJson1 from "../../tests/mocks/data/tvSeriesData1.json" with { type: "json" };
import TvSeriesPanel from "./TvSeriesPanel";

async function searchAndSelectExampleTvSeries() {
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
  return { ...utils, tvSeriesCard };
}

// Finds the card with the given text (a person or a credit) inside a panel.
function findCard(panel: HTMLElement, testId: string, text: string) {
  const card = within(panel)
    .getAllByTestId(testId)
    .find((c) => c.textContent.includes(text));
  if (!card) {
    throw new Error(`${testId} containing ${text} not found`);
  }
  return card;
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
        "Example JonesLead, Lead's Twin",
        "No imageExample BrownSecond Lead",
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

    it("Should hide the results message", async () => {
      await searchAndSelectExampleTvSeries();

      expect(screen.queryByText("8 results.")).not.toBeInTheDocument();
    });
  });

  describe("When the Cast section's 'Hide number of episodes' checkbox is used", () => {
    function castCardTexts() {
      return screen.getAllByTestId("cast-card").map((c) => c.textContent);
    }

    it("Should be checked by default and hide every cast member's episode count", async () => {
      await searchAndSelectExampleTvSeries();
      await screen.findByTestId("tv-series-panel");

      const checkbox = screen.getByRole("checkbox", {
        name: "Hide number of episodes",
      });
      expect(screen.getByText("Cast").closest("details")).toContainElement(
        checkbox,
      );
      expect(checkbox).toBeChecked();
      expect(castCardTexts()).toEqual([
        "Example JonesLead, Lead's Twin",
        "No imageExample BrownSecond Lead",
      ]);
    });

    it("Should show the episode counts while unchecked and hide them again when re-checked", async () => {
      await searchAndSelectExampleTvSeries();
      await screen.findByTestId("tv-series-panel");
      const checkbox = screen.getByRole("checkbox", {
        name: "Hide number of episodes",
      });

      fireEvent.click(checkbox);
      expect(checkbox).not.toBeChecked();
      expect(castCardTexts()).toEqual([
        "Example JonesLead, Lead's Twin37 episodes",
        "No imageExample BrownSecond Lead1 episode",
      ]);

      fireEvent.click(checkbox);
      expect(checkbox).toBeChecked();
      expect(castCardTexts()).toEqual([
        "Example JonesLead, Lead's Twin",
        "No imageExample BrownSecond Lead",
      ]);
    });

    it("Should keep the user's choice while the Cast section is collapsed and re-expanded", async () => {
      await searchAndSelectExampleTvSeries();
      await screen.findByTestId("tv-series-panel");
      const checkbox = screen.getByRole("checkbox", {
        name: "Hide number of episodes",
      });
      const castSection = screen.getByText("Cast").closest("details");
      if (!castSection) {
        throw new Error("Cast section not found");
      }

      fireEvent.click(checkbox);
      castSection.open = true;
      castSection.open = false;
      castSection.open = true;
      expect(checkbox).not.toBeChecked();
      expect(castCardTexts()[0]).toBe(
        "Example JonesLead, Lead's Twin37 episodes",
      );
    });

    it("Should not put the choice in the URL", async () => {
      const { currentUrl } = await searchAndSelectExampleTvSeries();
      await screen.findByTestId("tv-series-panel");
      const urlBefore = currentUrl();

      fireEvent.click(
        screen.getByRole("checkbox", { name: "Hide number of episodes" }),
      );
      expect(currentUrl()).toBe(urlBefore);
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
  ])("When %s while the TV series panel is expanded", (_, goBack) => {
    it("Should shrink the panel, then deselect the card and restore the results message", async () => {
      const { container, router, tvSeriesCard } =
        await searchAndSelectExampleTvSeries();
      await screen.findByTestId("tv-series-panel");

      await goBack(router);
      await waitFor(() => {
        expect(tvSeriesCard.classList.contains("expanded")).toBe(false);
      });
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

      render(
        <MemoryRouter>
          <TvSeriesPanel itemId="tt10000002" />
        </MemoryRouter>,
      );
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

  describe("When the TV series has ended", () => {
    it("Should not show the status", async () => {
      render(
        <MemoryRouter>
          <TvSeriesPanel itemId="tt10000002" />
        </MemoryRouter>,
      );
      const panel = await screen.findByTestId("tv-series-panel");

      expect(screen.getByRole("heading", { level: 2 }).textContent).toBe(
        "Example TV Series",
      );
      expect(screen.queryByTestId("title-status")).toBeNull();
      expect(panel.textContent).not.toContain("Status");
    });
  });

  describe("When the TV series has not ended", () => {
    it("Should show the status beside the name's heading instead of as a fact", async () => {
      server.use(
        http.get("/api/tvseries", () =>
          HttpResponse.json({
            ...tvSeriesDataJson1,
            status: "Returning Series",
          }),
        ),
      );

      render(
        <MemoryRouter>
          <TvSeriesPanel itemId="tt10000002" />
        </MemoryRouter>,
      );
      const panel = await screen.findByTestId("tv-series-panel");

      const heading = screen.getByRole("heading", { level: 2 });
      expect(heading.textContent).toBe("Example TV Series");
      const status = screen.getByTestId("title-status");
      expect(heading.nextElementSibling).toBe(status);
      expect(status.tagName).toBe("I");
      expect(status.textContent).toBe("(returning series)");
      expect(panel.textContent).not.toContain("Status");
    });
  });

  describe("When the TV series API returns an error", () => {
    it("Should show an error message", async () => {
      render(
        <MemoryRouter>
          <TvSeriesPanel itemId="tt9999999" />
        </MemoryRouter>,
      );

      expect(
        await screen.findByText(
          "Loading TV series failed with status 404: Not a valid IMDB ID for mock. Please try another search.",
        ),
      ).toBeInTheDocument();
    });
  });

  describe("When a card inside the TV series panel is clicked", () => {
    it.each([
      [
        "Cast",
        "cast",
        "cast-card",
        "Example Brown",
        2,
        "person",
        "person-panel",
      ],
      [
        "Creators",
        "creators",
        "creator-card",
        "Example Creator",
        12891,
        "person",
        "person-panel",
      ],
      [
        "Directors",
        "directors",
        "crew-card",
        "Example Director",
        3,
        "person",
        "person-panel",
      ],
      [
        "Writers",
        "writers",
        "crew-card",
        "Example Writer",
        4,
        "person",
        "person-panel",
      ],
    ])(
      "Should highlight the %s card and fly it to the corner, then replace the TV series panel with the card's own panel",
      async (_, cardType, testId, name, tmdbId, targetKind, targetTestId) => {
        const { currentUrl, container, tvSeriesCard } =
          await searchAndSelectExampleTvSeries();
        const panel = await screen.findByTestId("tv-series-panel");
        const card = findCard(panel, testId, name);
        const section = card.closest("details");

        fireEvent.click(card);
        // Step 1: the URL names the card, which is selected and flies to the
        // panel's corner while the panel hands itself over to it
        expect(currentUrl()).toBe(
          `/tvseries/tt10000002?q=1&cardType=${cardType}&cardId=${String(tmdbId)}`,
        );
        expect(card).toHaveClass("selected");
        expect(card).not.toHaveClass("expanded");
        expect(card.parentElement).toHaveClass("has-selection");
        expect(section).toHaveClass("has-child-selection");
        expect(section?.open).toBe(true);
        expect(panel).toHaveClass("has-child-selection");
        expect(tvSeriesCard).toHaveClass("expanded");
        expect(screen.queryByTestId(targetTestId)).not.toBeInTheDocument();

        // Step 2: once the card has arrived, its item's own URL replaces the
        // TV series panel with the item's panel, hosted by a stand-in card that
        // grows out of the flown card's spot
        const targetPanel = await screen.findByTestId(targetTestId);
        expect(currentUrl()).toBe(`/${targetKind}/${String(tmdbId)}?q=1`);
        expect(panel).not.toBeInTheDocument();
        expect(tvSeriesCard).not.toHaveClass("selected");
        expect(tvSeriesCard).toHaveClass("deselecting");
        const targetCard = targetPanel.closest("#search-card");
        expect(targetCard).not.toBe(tvSeriesCard);
        expect(targetCard).toHaveClass("selected");
        expect(targetCard).toHaveClass("expanded");
        expect(targetCard).toHaveClass("grow-in");
        expect(targetCard).toHaveClass("stand-in");
        expect(
          container.querySelectorAll("#search-card.selected"),
        ).toHaveLength(1);
        expect(
          within(targetPanel).getByRole("heading", { name }),
        ).toBeInTheDocument();
        expect(screen.queryByText("8 results.")).not.toBeInTheDocument();
      },
    );

    it("Should scroll to the top of the page once the new panel opens", async () => {
      const scrollTo = vi
        .spyOn(window, "scrollTo")
        .mockImplementation(() => undefined);
      await searchAndSelectExampleTvSeries();
      const panel = await screen.findByTestId("tv-series-panel");

      fireEvent.click(findCard(panel, "cast-card", "Example Brown"));
      expect(scrollTo).not.toHaveBeenCalled();
      await screen.findByTestId("person-panel");
      expect(scrollTo).toHaveBeenCalledWith({ top: 0 });
    });

    it("Should fly the card home instead of opening its panel when the flying card is clicked again", async () => {
      const { currentUrl } = await searchAndSelectExampleTvSeries();
      const panel = await screen.findByTestId("tv-series-panel");
      const card = findCard(panel, "cast-card", "Example Brown");

      fireEvent.click(card);
      expect(card).toHaveClass("selected");
      fireEvent.click(card);
      await waitFor(() => {
        expect(currentUrl()).toBe("/tvseries/tt10000002?q=1");
      });
      expect(card).not.toHaveClass("selected");
      expect(card).not.toHaveClass("highlighted");
      expect(card).toHaveClass("deselecting");
      expect(panel).not.toHaveClass("has-child-selection");

      // Waiting out the flight
      await act(
        () => new Promise((resolve) => setTimeout(resolve, CARD_FLY_MS + 100)),
      );
      expect(currentUrl()).toBe("/tvseries/tt10000002?q=1");
      expect(screen.queryByTestId("person-panel")).not.toBeInTheDocument();
    });

    it("Should shrink the new panel back into the card on browser back, which then flies home highlighted, and unhighlight it on a second back", async () => {
      const { currentUrl, container, tvSeriesCard, goBack, goForward } =
        await searchAndSelectExampleTvSeries();
      const panel = await screen.findByTestId("tv-series-panel");
      fireEvent.click(findCard(panel, "cast-card", "Example Brown"));
      const targetPanel = await screen.findByTestId("person-panel");
      const targetCard = targetPanel.closest("#search-card");
      const cardUrl = "/tvseries/tt10000002?q=1&cardType=cast&cardId=2";

      await goBack();
      expect(currentUrl()).toBe(cardUrl);
      // The stand-in card shrinks back to card size first...
      expect(targetCard).toHaveClass("selected");
      expect(targetCard).not.toHaveClass("expanded");
      expect(targetPanel).not.toBeInTheDocument();
      expect(tvSeriesCard).not.toHaveClass("selected");
      // ...then the TV series's own card hosts its panel again at once, with
      // the card in the corner the stand-in shrank into...
      await waitFor(() => {
        expect(tvSeriesCard).toHaveClass("expanded");
      });
      expect(targetCard).not.toBeInTheDocument();
      const panelAgain = screen.getByTestId("tv-series-panel");
      const card = findCard(panelAgain, "cast-card", "Example Brown");
      // ...from where it flies home, highlighted, while the rest of the panel
      // comes back into view around it
      await waitFor(() => {
        expect(card).toHaveClass("highlighted");
      });
      expect(card).toHaveClass("deselecting");
      expect(card).not.toHaveClass("selected");
      expect(panelAgain).not.toHaveClass("has-child-selection");
      expect(card.parentElement).not.toHaveClass("has-selection");
      expect(card.closest("details")).not.toHaveClass("has-child-selection");
      expect(card.closest("details")?.open).toBe(true);
      expect(container.querySelectorAll("#search-card")).toHaveLength(8);

      await goBack();
      expect(currentUrl()).toBe("/tvseries/tt10000002?q=1");
      expect(card).not.toHaveClass("highlighted");
      expect(card).not.toHaveClass("selected");
      expect(card.closest("details")?.open).toBe(true);
      expect(tvSeriesCard).toHaveClass("expanded");

      // Forward highlights the card where it is, with no flight (it's not
      // selected, which is what flies), and without opening its panel by
      // itself: that only ever follows a click
      await goForward();
      expect(currentUrl()).toBe(cardUrl);
      expect(card).toHaveClass("highlighted");
      expect(card).not.toHaveClass("selected");
      expect(panelAgain).not.toHaveClass("has-child-selection");
      await act(
        () => new Promise((resolve) => setTimeout(resolve, CARD_FLY_MS + 100)),
      );
      expect(currentUrl()).toBe(cardUrl);
      expect(screen.queryByTestId("person-panel")).not.toBeInTheDocument();

      await goForward();
      expect(currentUrl()).toBe("/person/2?q=1");
      await screen.findByTestId("person-panel");
      expect(tvSeriesCard).toHaveClass("deselecting");
      expect(container.querySelectorAll("#search-card")).toHaveLength(9);
    });

    it("Should push the URL without the card when the highlighted card is clicked, and nothing else", async () => {
      const { currentUrl, container, tvSeriesCard, goBack, goForward } =
        await searchAndSelectExampleTvSeries();
      const panel = await screen.findByTestId("tv-series-panel");
      fireEvent.click(findCard(panel, "cast-card", "Example Brown"));
      await screen.findByTestId("person-panel");
      await goBack();
      await waitFor(() => {
        expect(tvSeriesCard).toHaveClass("expanded");
      });
      const card = findCard(
        screen.getByTestId("tv-series-panel"),
        "cast-card",
        "Example Brown",
      );
      await waitFor(() => {
        expect(card).toHaveClass("highlighted");
      });

      fireEvent.click(card);
      await waitFor(() => {
        expect(currentUrl()).toBe("/tvseries/tt10000002?q=1");
      });
      expect(card).not.toHaveClass("highlighted");
      expect(card).not.toHaveClass("selected");
      expect(card.closest("details")?.open).toBe(true);
      expect(container.querySelectorAll(".highlighted")).toHaveLength(0);

      // The URL was pushed, not popped: the forward history (the panel that
      // was opened from the card) is gone, and back returns to the card
      // highlighted
      await goForward();
      expect(currentUrl()).toBe("/tvseries/tt10000002?q=1");
      await goBack();
      expect(currentUrl()).toBe(
        "/tvseries/tt10000002?q=1&cardType=cast&cardId=2",
      );
    });
  });

  describe("When a TV series URL naming a card is loaded directly", () => {
    it("Should show the whole panel with the card highlighted in its place and only its section open", async () => {
      const url = "/tvseries/tt10000002?q=1&cardType=creators&cardId=12891";
      const { currentUrl } = renderApp(url);
      const panel = await screen.findByTestId("tv-series-panel");
      expect(panel).not.toHaveClass("has-child-selection");
      expect(
        within(panel).getByRole("heading", { level: 2 }),
      ).toBeInTheDocument();
      const card = findCard(panel, "creator-card", "Example Creator");
      expect(card).toHaveClass("highlighted");
      expect(card).not.toHaveClass("selected");
      expect(card).not.toHaveClass("deselecting");
      expect(card.parentElement).not.toHaveClass("has-selection");
      const section = card.closest("details");
      expect(section).not.toHaveClass("has-child-selection");
      expect(section?.open).toBe(true);
      expect(screen.getByText("Directors").closest("details")?.open).toBe(
        false,
      );

      await act(
        () => new Promise((resolve) => setTimeout(resolve, CARD_FLY_MS + 100)),
      );
      expect(currentUrl()).toBe(url);
      expect(screen.queryByTestId("person-panel")).not.toBeInTheDocument();
    });
  });
});
