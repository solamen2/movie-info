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
import { CARD_FLY_MS } from "../shared/useCardSelection";
import { http, HttpResponse } from "msw";
import { server } from "../../tests/mocks/node.ts";
import personDataJson1 from "../../tests/mocks/data/personData1.json" with { type: "json" };
import PersonPanel from "./PersonPanel";

async function searchAndSelectExamplePerson() {
  const utils = renderApp();
  const searchQueryInput = screen.getByRole("textbox", {
    name: "search-query-input",
  });
  fireEvent.change(searchQueryInput, { target: { value: "1" } });
  fireEvent.click(screen.getByRole("button", { name: "search" }));

  const personText = await screen.findByText("Example Smith");
  const personCard = personText.closest<HTMLDivElement>("#search-card");
  if (!personCard) {
    throw new Error("Person search card not found");
  }
  fireEvent.click(personCard);
  return { ...utils, personCard };
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

describe("PersonPanel", () => {
  afterEach(() => {
    vi.restoreAllMocks();
    vi.useRealTimers();
  });

  describe("When a person search card is selected", () => {
    it("Should expand the card into the person panel after the fly animation", async () => {
      const { personCard } = await searchAndSelectExamplePerson();

      expect(personCard.classList.contains("selected")).toBe(true);
      expect(personCard.classList.contains("expanded")).toBe(false);

      const panel = await screen.findByTestId("person-panel");
      expect(personCard.classList.contains("expanded")).toBe(true);
      expect(personCard).toContainElement(panel);
      expect(screen.queryByTestId("movie-panel")).not.toBeInTheDocument();
    });

    it("Should render the name to the right of the profile image at the top of the panel", async () => {
      await searchAndSelectExamplePerson();
      const panel = await screen.findByTestId("person-panel");

      const top = panel.querySelector(".detail-panel-top");
      const image = screen.getByAltText("Example Smith");
      const heading = screen.getByRole("heading", { name: "Example Smith" });
      expect(image).toHaveAttribute("src", "https://example.com/example1.jpg");
      expect(top?.firstElementChild).toBe(image);
      expect(image.nextElementSibling).toContainElement(heading);
    });

    it("Should render the visible person data members", async () => {
      vi.useFakeTimers({ toFake: ["Date"], now: new Date(2026, 8, 28) });
      await searchAndSelectExamplePerson();
      const panel = await screen.findByTestId("person-panel");
      const text = panel.textContent;
      expect(screen.getByTestId("person-age").tagName).toBe("I");

      for (const expected of [
        "Example SmithIMDB: Rank: 3LinkCopy",
        "Known ForActress, Example Film",
        "Known For DepartmentActing",
        "Also Known AsExample Smithee, Betsy Smith",
        "BirthdayApr 14, 1977 (49)Deathday—",
        "Place of BirthNew York City, New York, USA",
        "GenderFemale",
        "Homepagehttps://example.com/example-smith",
      ]) {
        expect(text).toContain(expected);
      }
    });

    it("Should not render the hidden person data members", async () => {
      await searchAndSelectExamplePerson();
      const panel = await screen.findByTestId("person-panel");
      const text = panel.textContent;

      for (const hidden of [
        "7d1c2b3a-4e5f-4a6b-8c7d-9e0f1a2b3c4d",
        "nm9000000",
        "Imdb Id",
        "80001",
        "Tmdb Id",
        "Profile Path",
        "/exampleSmithProfile.jpg",
      ]) {
        expect(text).not.toContain(hidden);
      }
    });

    it("Should order the collapsed sections with the biography first and profile images last", async () => {
      await searchAndSelectExamplePerson();
      const panel = await screen.findByTestId("person-panel");

      const summaries = [...panel.querySelectorAll("summary")].map(
        (s) => s.textContent,
      );
      expect(summaries).toEqual([
        "Biography",
        "Movie Cast Credits(3)",
        "Movie Crew Credits(1)",
        "TV Series Cast Credits(2)",
        "TV Series Crew Credits(0)",
        "Profile Images(2)",
      ]);
    });

    it("Should hide the biography, credits, and profile images behind collapsed sections", async () => {
      await searchAndSelectExamplePerson();
      await screen.findByTestId("person-panel");

      for (const [summary, content] of [
        ["Biography", "She is especially known for Example Film."],
        ["Movie Cast Credits", "Example Hero"],
        ["Movie Crew Credits", "Example Produced Film"],
        ["TV Series Cast Credits", "Example Slayer"],
        ["TV Series Crew Credits", "None"],
      ]) {
        const details = screen.getByText(summary).closest("details");
        if (!details) {
          throw new Error(`Collapsible for ${summary} not found`);
        }
        expect(details.open).toBe(false);
        expect(details.textContent).toContain(content);
      }

      const profileImagesDetails = screen
        .getByText("Profile Images")
        .closest("details");
      expect(profileImagesDetails?.open).toBe(false);
      expect(profileImagesDetails).toContainElement(
        screen.getByTestId("profile-images-list"),
      );
    });

    it("Should render movie credits as cards in horizontal lists, newest first with undated credits last", async () => {
      await searchAndSelectExamplePerson();
      await screen.findByTestId("person-panel");

      const movieCastCards = screen.getAllByTestId("movie-cast-card");
      expect(movieCastCards.map((c) => c.textContent)).toEqual([
        "No imageExample Film(Film d'exemple)Example HeroNov 10, 2006",
        "Example Old FilmExample SidekickMar 5, 1999",
        "No imageExample Upcoming Film",
      ]);
      expect(screen.getByAltText("Example Old Film")).toHaveAttribute(
        "src",
        "https://image.tmdb.org/t/p/w185/exampleOldFilm.jpg",
      );

      const movieCrewCards = screen.getAllByTestId("movie-crew-card");
      expect(movieCrewCards.map((c) => c.textContent)).toEqual([
        "Example Produced FilmExecutive ProducerAug 27, 2004",
      ]);

      for (const card of [...movieCastCards, ...movieCrewCards]) {
        expect(card.parentElement?.classList.contains("horizontal-list")).toBe(
          true,
        );
      }
    });

    it("Should render TV series credits as cards in horizontal lists, newest credit first", async () => {
      await searchAndSelectExamplePerson();
      await screen.findByTestId("person-panel");

      const tvSeriesCastCards = screen.getAllByTestId("tv-series-cast-card");
      expect(tvSeriesCastCards.map((c) => c.textContent)).toEqual([
        "No imageExample Talk ShowSelf1 episodeFirst appearance:Sep 13, 2011",
        "Example ShowExample Slayer144 episodesFirst appearance:Mar 10, 1997",
      ]);
      for (const card of tvSeriesCastCards) {
        expect(card.parentElement?.classList.contains("horizontal-list")).toBe(
          true,
        );
      }
      expect(screen.queryAllByTestId("tv-series-crew-card")).toHaveLength(0);
    });

    it("Should render the profile images in a vertical list", async () => {
      await searchAndSelectExamplePerson();
      await screen.findByTestId("person-panel");

      const list = screen.getByTestId("profile-images-list");
      const images = [...list.querySelectorAll("img")];
      expect(images.map((i) => i.getAttribute("src"))).toEqual([
        "https://image.tmdb.org/t/p/w500/exampleSmithProfile.jpg",
        "https://image.tmdb.org/t/p/w500/exampleSmithProfile2.jpg",
      ]);
      expect(images.map((i) => i.alt)).toEqual([
        "Example Smith profile 1",
        "Example Smith profile 2",
      ]);
      for (const image of images) {
        expect(image.classList.contains("profile-image")).toBe(true);
      }
    });

    it("Should link to the IMDB name page in a new tab", async () => {
      await searchAndSelectExamplePerson();
      await screen.findByTestId("person-panel");

      const link = screen.getByRole("link", { name: "Link" });
      expect(link).toHaveAttribute(
        "href",
        "https://www.imdb.com/name/nm9000000",
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

      await searchAndSelectExamplePerson();
      await screen.findByTestId("person-panel");

      fireEvent.click(screen.getByRole("button", { name: "copy-imdb-link" }));
      expect(writeText).toHaveBeenCalledWith(
        '<a href="https://www.imdb.com/name/nm9000000">Link</a>',
      );
      expect(await screen.findByText("Copied!")).toBeInTheDocument();
    });

    it("Should show that the copy failed when the clipboard is unavailable", async () => {
      const writeText = vi.fn().mockRejectedValue(new Error("denied"));
      Object.defineProperty(navigator, "clipboard", {
        value: { writeText },
        configurable: true,
      });

      await searchAndSelectExamplePerson();
      await screen.findByTestId("person-panel");

      fireEvent.click(screen.getByRole("button", { name: "copy-imdb-link" }));
      expect(await screen.findByText("Failed")).toBeInTheDocument();
    });

    it("Should not deselect the card when clicking inside the panel", async () => {
      const { personCard } = await searchAndSelectExamplePerson();
      const panel = await screen.findByTestId("person-panel");

      fireEvent.click(panel);
      expect(personCard.classList.contains("selected")).toBe(true);
      expect(personCard.classList.contains("expanded")).toBe(true);
    });

    it("Should hide the results message", async () => {
      await searchAndSelectExamplePerson();

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
  ])("When %s while the person panel is expanded", (_, goBack) => {
    it("Should shrink the panel, then deselect the card and restore the results message", async () => {
      const { container, router, personCard } =
        await searchAndSelectExamplePerson();
      await screen.findByTestId("person-panel");

      await goBack(router);
      await waitFor(() => {
        expect(personCard.classList.contains("expanded")).toBe(false);
      });
      expect(personCard.classList.contains("selected")).toBe(true);
      expect(screen.queryByTestId("person-panel")).not.toBeInTheDocument();

      await waitFor(() => {
        expect(personCard.classList.contains("selected")).toBe(false);
      });
      expect(personCard.classList.contains("deselecting")).toBe(true);
      expect(
        container
          .querySelector(".results-container")
          ?.classList.contains("has-selection"),
      ).toBe(false);
      expect(screen.getByText("8 results.")).toBeInTheDocument();
    });
  });

  describe("When the person has no birthday", () => {
    it("Should not show an age", async () => {
      server.use(
        http.get("/api/person", () =>
          HttpResponse.json({ ...personDataJson1, birthday: null }),
        ),
      );

      render(
        <MemoryRouter>
          <PersonPanel itemId="nm9000000" />
        </MemoryRouter>,
      );
      const panel = await screen.findByTestId("person-panel");
      expect(panel.textContent).toContain("Birthday—Deathday—");
      expect(screen.queryByTestId("person-age")).toBeNull();
    });
  });

  describe("When the person is less than a year old", () => {
    it("Should show the age as '(<1)'", async () => {
      vi.useFakeTimers({ toFake: ["Date"], now: new Date(2026, 8, 28) });
      server.use(
        http.get("/api/person", () =>
          HttpResponse.json({ ...personDataJson1, birthday: "2026-01-15" }),
        ),
      );

      render(
        <MemoryRouter>
          <PersonPanel itemId="nm9000000" />
        </MemoryRouter>,
      );
      const panel = await screen.findByTestId("person-panel");
      expect(panel.textContent).toContain("BirthdayJan 15, 2026 (<1)");
    });
  });

  describe("When the person has missing data", () => {
    it("Should show placeholders instead of the missing values", async () => {
      server.use(
        http.get("/api/person", () =>
          HttpResponse.json({
            ...personDataJson1,
            image: null,
            imdbRank: null,
            knownForMovies: null,
            alsoKnownAs: [],
            biography: "",
            deathday: "2020-01-02",
            gender: "NotSetNotSpecified",
            homepage: null,
            placeOfBirth: null,
            profileImages: [],
          }),
        ),
      );

      render(
        <MemoryRouter>
          <PersonPanel itemId="nm9000000" />
        </MemoryRouter>,
      );
      const panel = await screen.findByTestId("person-panel");
      const text = panel.textContent;

      for (const expected of [
        "No imageExample SmithIMDB: Rank: —LinkCopy",
        "Known For—",
        "Also Known As—",
        "BirthdayApr 14, 1977 (42)DeathdayJan 2, 2020",
        "Place of Birth—",
        "GenderNot specified",
        "Homepage—",
        "Biography—",
        "Profile Images(0)None",
      ]) {
        expect(text).toContain(expected);
      }
    });
  });

  describe("When the person API returns an error", () => {
    it("Should show an error message", async () => {
      render(
        <MemoryRouter>
          <PersonPanel itemId="nm9999999" />
        </MemoryRouter>,
      );

      expect(
        await screen.findByText(
          "Loading person failed with status 404: Not a valid IMDB ID for mock. Please try another search.",
        ),
      ).toBeInTheDocument();
    });
  });

  describe("When a card inside the person panel is clicked", () => {
    it.each([
      [
        "Movie Cast Credits",
        "movie-cast-credits",
        "movie-cast-card",
        "Example Film",
        81002,
        "movie",
        "movie-panel",
      ],
      [
        "Movie Crew Credits",
        "movie-crew-credits",
        "movie-crew-card",
        "Example Produced Film",
        82001,
        "movie",
        "movie-panel",
      ],
      [
        "TV Series Cast Credits",
        "tv-series-cast-credits",
        "tv-series-cast-card",
        "Example Show",
        83001,
        "tvseries",
        "tv-series-panel",
      ],
    ])(
      "Should highlight the %s card and fly it to the corner, then replace the person panel with the card's own panel",
      async (_, cardType, testId, name, tmdbId, targetKind, targetTestId) => {
        const { currentUrl, container, personCard } =
          await searchAndSelectExamplePerson();
        const panel = await screen.findByTestId("person-panel");
        const card = findCard(panel, testId, name);
        const section = card.closest("details");

        fireEvent.click(card);
        // Step 1: the URL names the card, which is selected and flies to the
        // panel's corner while the panel hands itself over to it
        expect(currentUrl()).toBe(
          `/person/nm9000000?q=1&cardType=${cardType}&cardId=${String(tmdbId)}`,
        );
        expect(card).toHaveClass("selected");
        expect(card).not.toHaveClass("expanded");
        expect(card.parentElement).toHaveClass("has-selection");
        expect(section).toHaveClass("has-child-selection");
        expect(section?.open).toBe(true);
        expect(panel).toHaveClass("has-child-selection");
        expect(personCard).toHaveClass("expanded");
        expect(screen.queryByTestId(targetTestId)).not.toBeInTheDocument();

        // Step 2: once the card has arrived, its item's own URL replaces the
        // person panel with the item's panel, hosted by a stand-in card that
        // grows out of the flown card's spot
        const targetPanel = await screen.findByTestId(targetTestId);
        expect(currentUrl()).toBe(`/${targetKind}/${String(tmdbId)}?q=1`);
        expect(panel).not.toBeInTheDocument();
        expect(personCard).not.toHaveClass("selected");
        expect(personCard).toHaveClass("deselecting");
        const targetCard = targetPanel.closest("#search-card");
        expect(targetCard).not.toBe(personCard);
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
      await searchAndSelectExamplePerson();
      const panel = await screen.findByTestId("person-panel");

      fireEvent.click(findCard(panel, "movie-cast-card", "Example Film"));
      expect(scrollTo).not.toHaveBeenCalled();
      await screen.findByTestId("movie-panel");
      expect(scrollTo).toHaveBeenCalledWith({ top: 0 });
    });

    it("Should fly the card home instead of opening its panel when the flying card is clicked again", async () => {
      const { currentUrl } = await searchAndSelectExamplePerson();
      const panel = await screen.findByTestId("person-panel");
      const card = findCard(panel, "movie-cast-card", "Example Film");

      fireEvent.click(card);
      expect(card).toHaveClass("selected");
      fireEvent.click(card);
      await waitFor(() => {
        expect(currentUrl()).toBe("/person/nm9000000?q=1");
      });
      expect(card).not.toHaveClass("selected");
      expect(card).not.toHaveClass("highlighted");
      expect(card).toHaveClass("deselecting");
      expect(panel).not.toHaveClass("has-child-selection");

      // Waiting out the flight
      await act(
        () => new Promise((resolve) => setTimeout(resolve, CARD_FLY_MS + 100)),
      );
      expect(currentUrl()).toBe("/person/nm9000000?q=1");
      expect(screen.queryByTestId("movie-panel")).not.toBeInTheDocument();
    });

    it("Should shrink the new panel back into the card on browser back, which then flies home highlighted, and unhighlight it on a second back", async () => {
      const { currentUrl, container, personCard, goBack, goForward } =
        await searchAndSelectExamplePerson();
      const panel = await screen.findByTestId("person-panel");
      fireEvent.click(findCard(panel, "movie-cast-card", "Example Film"));
      const targetPanel = await screen.findByTestId("movie-panel");
      const targetCard = targetPanel.closest("#search-card");
      const cardUrl =
        "/person/nm9000000?q=1&cardType=movie-cast-credits&cardId=81002";

      await goBack();
      expect(currentUrl()).toBe(cardUrl);
      // The stand-in card shrinks back to card size first...
      expect(targetCard).toHaveClass("selected");
      expect(targetCard).not.toHaveClass("expanded");
      expect(targetPanel).not.toBeInTheDocument();
      expect(personCard).not.toHaveClass("selected");
      // ...then the person's own card hosts its panel again at once, with
      // the card in the corner the stand-in shrank into...
      await waitFor(() => {
        expect(personCard).toHaveClass("expanded");
      });
      expect(targetCard).not.toBeInTheDocument();
      const panelAgain = screen.getByTestId("person-panel");
      const card = findCard(panelAgain, "movie-cast-card", "Example Film");
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
      expect(currentUrl()).toBe("/person/nm9000000?q=1");
      expect(card).not.toHaveClass("highlighted");
      expect(card).not.toHaveClass("selected");
      expect(card.closest("details")?.open).toBe(true);
      expect(personCard).toHaveClass("expanded");

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
      expect(screen.queryByTestId("movie-panel")).not.toBeInTheDocument();

      await goForward();
      expect(currentUrl()).toBe("/movie/81002?q=1");
      await screen.findByTestId("movie-panel");
      expect(personCard).toHaveClass("deselecting");
      expect(container.querySelectorAll("#search-card")).toHaveLength(9);
    });

    it("Should push the URL without the card when the highlighted card is clicked, and nothing else", async () => {
      const { currentUrl, container, personCard, goBack, goForward } =
        await searchAndSelectExamplePerson();
      const panel = await screen.findByTestId("person-panel");
      fireEvent.click(findCard(panel, "movie-cast-card", "Example Film"));
      await screen.findByTestId("movie-panel");
      await goBack();
      await waitFor(() => {
        expect(personCard).toHaveClass("expanded");
      });
      const card = findCard(
        screen.getByTestId("person-panel"),
        "movie-cast-card",
        "Example Film",
      );
      await waitFor(() => {
        expect(card).toHaveClass("highlighted");
      });

      fireEvent.click(card);
      await waitFor(() => {
        expect(currentUrl()).toBe("/person/nm9000000?q=1");
      });
      expect(card).not.toHaveClass("highlighted");
      expect(card).not.toHaveClass("selected");
      expect(card.closest("details")?.open).toBe(true);
      expect(container.querySelectorAll(".highlighted")).toHaveLength(0);

      // The URL was pushed, not popped: the forward history (the panel that
      // was opened from the card) is gone, and back returns to the card
      // highlighted
      await goForward();
      expect(currentUrl()).toBe("/person/nm9000000?q=1");
      await goBack();
      expect(currentUrl()).toBe(
        "/person/nm9000000?q=1&cardType=movie-cast-credits&cardId=81002",
      );
    });
  });

  describe("When a person URL naming a card is loaded directly", () => {
    it("Should show the whole panel with the card highlighted in its place and only its section open", async () => {
      const url =
        "/person/nm9000000?q=1&cardType=movie-crew-credits&cardId=82001";
      const { currentUrl } = renderApp(url);
      const panel = await screen.findByTestId("person-panel");
      expect(panel).not.toHaveClass("has-child-selection");
      expect(
        within(panel).getByRole("heading", { level: 2 }),
      ).toBeInTheDocument();
      const card = findCard(panel, "movie-crew-card", "Example Produced Film");
      expect(card).toHaveClass("highlighted");
      expect(card).not.toHaveClass("selected");
      expect(card).not.toHaveClass("deselecting");
      expect(card.parentElement).not.toHaveClass("has-selection");
      const section = card.closest("details");
      expect(section).not.toHaveClass("has-child-selection");
      expect(section?.open).toBe(true);
      expect(
        screen.getByText("Movie Cast Credits").closest("details")?.open,
      ).toBe(false);

      await act(
        () => new Promise((resolve) => setTimeout(resolve, CARD_FLY_MS + 100)),
      );
      expect(currentUrl()).toBe(url);
      expect(screen.queryByTestId("movie-panel")).not.toBeInTheDocument();
    });
  });
});
