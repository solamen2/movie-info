import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { MemoryRouter } from "react-router-dom";
import { http, HttpResponse } from "msw";
import { server } from "../../tests/mocks/node.ts";
import personDataJson1 from "../../tests/mocks/data/personData1.json" with { type: "json" };
import SuggestionSearch from "../suggestion/SuggestionSearch";
import PersonPanel from "./PersonPanel";

async function searchAndSelectExamplePerson() {
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

  const personText = await screen.findByText("Example Smith");
  const personCard = personText.closest<HTMLDivElement>("#search-card");
  if (!personCard) {
    throw new Error("Person search card not found");
  }
  fireEvent.click(personCard);
  return { ...utils, personCard };
}

describe("PersonPanel", () => {
  afterEach(() => {
    vi.restoreAllMocks();
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
      await searchAndSelectExamplePerson();
      const panel = await screen.findByTestId("person-panel");
      const text = panel.textContent;

      for (const expected of [
        "Example SmithIMDB: 3LinkCopy",
        "Known ForActress, Example Film",
        "Known For DepartmentActing",
        "Also Known AsExample Smithee, Betsy Smith",
        "BirthdayApr 14, 1977",
        "Deathday—",
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
        "No imageExample Talk ShowSelf1 episodeSep 13, 2011",
        "Example ShowExample Slayer144 episodesMar 10, 1997",
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

    it("Should replace the results message with a back arrow", async () => {
      await searchAndSelectExamplePerson();

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
  ])("When %s while the person panel is expanded", (_, goBack) => {
    it("Should shrink the panel, then deselect the card and restore the results message", async () => {
      const { container, personCard } = await searchAndSelectExamplePerson();
      await screen.findByTestId("person-panel");

      goBack();
      expect(personCard.classList.contains("expanded")).toBe(false);
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
      expect(
        screen.queryByRole("button", { name: "back" }),
      ).not.toBeInTheDocument();
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

      render(<PersonPanel imdbId="nm9000000" />);
      const panel = await screen.findByTestId("person-panel");
      const text = panel.textContent;

      for (const expected of [
        "No imageExample SmithIMDB: —LinkCopy",
        "Known For—",
        "Also Known As—",
        "DeathdayJan 2, 2020",
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
      render(<PersonPanel imdbId="nm9999999" />);

      expect(
        await screen.findByText(
          "Loading person failed with status 404. Please try again.",
        ),
      ).toBeInTheDocument();
    });
  });
});
