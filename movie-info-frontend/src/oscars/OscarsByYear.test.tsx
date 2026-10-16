import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { http, HttpResponse } from "msw";
import { server } from "../../tests/mocks/node.ts";
import { renderApp } from "../../tests/renderApp";

// jsdom doesn't implement scrolling; the "Jump To" dropdown scrolls to a
// year's section.
const scrollIntoView = vi.fn();

// Renders the page with every year expanded (they start collapsed)
async function renderOscarsPage() {
  const utils = renderApp("/oscars");
  const years = await screen.findAllByTestId<HTMLDetailsElement>("oscar-year");
  fireEvent.click(screen.getByRole("button", { name: "Expand All" }));
  return { ...utils, years };
}

function categoriesOf(year: HTMLElement) {
  return within(year).getAllByTestId("oscar-category");
}

function nomineesSection(category: HTMLElement) {
  const details = within(category).getByText("Nominees").closest("details");
  if (!details) {
    throw new Error("Nominees section not found");
  }
  return details;
}

describe("OscarsByYear", () => {
  beforeEach(() => {
    Element.prototype.scrollIntoView = scrollIntoView;
  });

  afterEach(() => {
    vi.restoreAllMocks();
    scrollIntoView.mockReset();
  });

  it("Should list every ceremony, most recent first", async () => {
    const { years } = await renderOscarsPage();

    expect(screen.getByRole("heading", { level: 1 }).textContent).toBe(
      "Oscars By Year",
    );
    expect(years.map((y) => y.querySelector("h2")?.textContent)).toEqual([
      "201689th Academy Awards",
      "1927/281st Academy Awards",
    ]);
  });

  it("Should start with every year collapsed and empty, and expand or collapse them all from the top", async () => {
    renderApp("/oscars");
    const years =
      await screen.findAllByTestId<HTMLDetailsElement>("oscar-year");
    const isOpen = () => years.map((y) => y.open);

    expect(isOpen()).toEqual([false, false]);
    expect(screen.queryByTestId("oscar-category")).toBeNull();

    fireEvent.click(screen.getByRole("button", { name: "Expand All" }));
    expect(isOpen()).toEqual([true, true]);
    expect(screen.getAllByTestId("oscar-category")).toHaveLength(6);

    // A year closed by hand is opened again by "Expand All"
    fireEvent.click(within(years[0]).getByText("2016"));
    await waitFor(() => {
      expect(years[0].open).toBe(false);
    });
    expect(screen.getAllByTestId("oscar-category")).toHaveLength(3);
    fireEvent.click(screen.getByRole("button", { name: "Expand All" }));
    expect(isOpen()).toEqual([true, true]);

    fireEvent.click(screen.getByRole("button", { name: "Collapse All" }));
    expect(isOpen()).toEqual([false, false]);
    expect(screen.queryByTestId("oscar-category")).toBeNull();
  });

  it("Should show a year's categories in order, with their winners", async () => {
    const { years } = await renderOscarsPage();
    const categories = categoriesOf(years[0]);

    expect(categories.map((c) => c.querySelector("h3")?.textContent)).toEqual([
      "Best Picture",
      "Best Actress*",
      "Best Original Song",
    ]);
    expect(
      categories.map((c) => within(c).getByTestId("oscar-winners").textContent),
    ).toEqual([
      "Example Movie (Example Producer and Example Smith, Producers)",
      "Example Smith as Example Hero (Example Film)",
      "Example Movie (“Example Song” — Music and Lyric by Example Composer and Example Lyricist, Jr.)",
    ]);
  });

  it("Should keep the other nominees collapsed, and only render them once opened", async () => {
    const { years } = await renderOscarsPage();
    const bestPicture = categoriesOf(years[0])[0];
    const nominees = nomineesSection(bestPicture);

    expect(nominees.open).toBe(false);
    expect(within(bestPicture).getByText("Nominees").textContent).toBe(
      "Nominees(2)",
    );
    expect(within(bestPicture).queryByTestId("oscar-nominees")).toBeNull();

    fireEvent.click(within(nominees).getByText("Nominees"));

    await waitFor(() => {
      expect(nominees.open).toBe(true);
    });
    const list = await within(bestPicture).findByTestId("oscar-nominees");
    expect(
      [...list.querySelectorAll("li")].map((li) => li.textContent),
    ).toEqual([
      "Example Film (Another Producer, Producer)",
      "Example Old Film (Old Producer, Producer)",
    ]);
  });

  it("Should set a winner's entry a little larger than its detail, but a nominee's should not be increased this way", async () => {
    const { years } = await renderOscarsPage();
    const bestPicture = categoriesOf(years[0])[0];
    const winner = within(bestPicture).getByTestId("oscar-winners");

    expect(winner.querySelector(".oscar-award-primary")?.textContent).toBe(
      "Example Movie",
    );
    expect(winner.querySelector(".oscar-award-primary")).toHaveClass(
      "oscar-award-primary-winner",
    );
    expect(winner.querySelector(".oscar-award-secondary")?.textContent).toBe(
      "(Example Producer and Example Smith, Producers)",
    );

    fireEvent.click(within(bestPicture).getByText("Nominees"));
    const nominees = await within(bestPicture).findByTestId("oscar-nominees");
    expect(nominees.querySelector(".oscar-award-primary")).not.toHaveClass(
      "oscar-award-primary-winner",
    );
  });

  it("Should name the director before the film, like actors", async () => {
    const { years } = await renderOscarsPage();
    const bestDirector = categoriesOf(years[1])[1];

    expect(bestDirector.querySelector("h3")?.textContent).toBe(
      "Best Director*",
    );
    expect(within(bestDirector).getByTestId("oscar-winners").textContent).toBe(
      "Example Director (Example Silent)",
    );
  });

  it("Should not have a nominees section when every nominee won", async () => {
    const { years } = await renderOscarsPage();
    const originalSong = categoriesOf(years[0])[2];

    expect(within(originalSong).queryByText("Nominees")).toBeNull();
  });

  it("Should show what a category was called that year when its asterisk is tapped", async () => {
    const { years } = await renderOscarsPage();
    const bestActress = categoriesOf(years[0])[1];
    const asterisk = within(bestActress).getByRole("button", {
      name: "original category name",
    });

    expect(within(bestActress).queryByRole("tooltip")).toBeNull();
    fireEvent.click(asterisk);
    expect(within(bestActress).getByRole("tooltip").textContent).toBe(
      "Original Category Name: Actress",
    );

    fireEvent.keyDown(document, { key: "Escape" });
    expect(within(bestActress).queryByRole("tooltip")).toBeNull();

    // Tapping anywhere else closes it too
    fireEvent.click(asterisk);
    expect(within(bestActress).getByRole("tooltip")).toBeInTheDocument();
    fireEvent.pointerDown(document.body);
    expect(within(bestActress).queryByRole("tooltip")).toBeNull();
  });

  it("Should show a nomination's note when its mark is tapped", async () => {
    const { years } = await renderOscarsPage();
    const bestPicture1928 = categoriesOf(years[1])[0];
    expect(bestPicture1928.querySelector("h3")?.textContent).toBe(
      "Best Picture*",
    );
    fireEvent.click(within(bestPicture1928).getByText("Nominees"));
    const list = await within(bestPicture1928).findByTestId("oscar-nominees");

    fireEvent.click(within(list).getByRole("button", { name: "note" }));
    expect(within(list).getByRole("tooltip").textContent).toBe(
      "NOTE: An example note.",
    );
  });

  it("Should list the special awards after the categories, with their citations", async () => {
    const { years } = await renderOscarsPage();

    const special2016 = within(years[0]).getByTestId("oscar-special-awards");
    expect(special2016.querySelector("h3")?.textContent).toBe("Special Awards");
    expect(within(special2016).getByTestId("oscar-winners").textContent).toBe(
      "Honorary Award: Example LegendTo Example Legend, for a lifetime of example work.",
    );
    expect(
      special2016.compareDocumentPosition(categoriesOf(years[0])[2]) &
        Node.DOCUMENT_POSITION_PRECEDING,
    ).toBeTruthy();

    const special1928 = within(years[1]).getByTestId("oscar-special-awards");
    expect(within(special1928).getByTestId("oscar-winners").textContent).toBe(
      "Unique and Artistic Picture: Example Art Film (Example Studios)",
    );
  });

  it("Should open a movie's panel when its title is clicked", async () => {
    const { years, currentUrl } = await renderOscarsPage();
    const bestPicture = categoriesOf(years[0])[0];

    fireEvent.click(
      within(bestPicture).getByRole("link", { name: "Example Movie" }),
    );

    expect(currentUrl()).toBe("/movie/tt0000001");
    expect(await screen.findByTestId("movie-panel")).toBeInTheDocument();
  });

  it("Should open a person's panel when their name is clicked", async () => {
    const { years, currentUrl } = await renderOscarsPage();
    const originalSong = categoriesOf(years[0])[2];

    // Each nominee in the credit text is a link, under the credit's spelling
    expect(
      within(originalSong)
        .getAllByRole("link")
        .map((link) => [link.textContent, link.getAttribute("href")]),
    ).toEqual([
      ["Example Movie", "/movie/tt0000001"],
      ["Example Composer", "/person/nm9000005"],
      ["Example Lyricist, Jr.", "/person/nm9000006"],
    ]);
    const bestPicture = categoriesOf(years[0])[0];
    fireEvent.click(
      within(bestPicture).getByRole("link", { name: "Example Smith" }),
    );

    expect(currentUrl()).toBe("/person/nm9000000");
    expect(await screen.findByTestId("person-panel")).toBeInTheDocument();
  });

  it("Should not link a nominee the app can't show, like a studio", async () => {
    const { years } = await renderOscarsPage();
    const bestPicture1928 = categoriesOf(years[1])[0];

    expect(
      within(bestPicture1928).getByTestId("oscar-winners").textContent,
    ).toBe("Example Silent (Example Studios)");
    expect(
      within(bestPicture1928)
        .getAllByRole("link")
        .map((link) => link.textContent),
    ).toEqual(["Example Silent"]);
  });

  it("Should show a category split that year as subsections, like black-and-white and color", async () => {
    const { years } = await renderOscarsPage();
    const cinematography1928 = categoriesOf(years[1])[2];

    expect(cinematography1928.querySelector("h3")?.textContent).toBe(
      "Best Cinematography",
    );
    const subsections = within(cinematography1928).getAllByTestId(
      "oscar-category-subsection",
    );
    expect(
      subsections.map(
        (subsection) => subsection.querySelector("h4")?.textContent,
      ),
    ).toEqual(["Black-and-White", "Color*"]);
    expect(
      subsections.map(
        (subsection) =>
          within(subsection).getByTestId("oscar-winners").textContent,
      ),
    ).toEqual([
      "Example Silent (Example Cinematographer)",
      "Example Talkie (Example Colorist)",
    ]);
    // Each subsection has its own nominees
    expect(within(subsections[0]).queryByText("Nominees")).toBeNull();
    fireEvent.click(within(subsections[1]).getByText("Nominees"));
    expect(
      (await within(subsections[1]).findByTestId("oscar-nominees")).textContent,
    ).toBe("Example Art Film (Example Tinter)");
    // A subsection under an older name has its own asterisk
    fireEvent.click(
      within(subsections[1]).getByRole("button", {
        name: "original category name",
      }),
    );
    expect(within(subsections[1]).getByRole("tooltip").textContent).toBe(
      "Original Category Name: Cinematography (Color Photography)",
    );
    // Other categories have no subsections
    expect(
      within(categoriesOf(years[1])[0]).queryByTestId(
        "oscar-category-subsection",
      ),
    ).toBeNull();
  });

  it("Should jump to the chosen year and open it", async () => {
    renderApp("/oscars");
    const years =
      await screen.findAllByTestId<HTMLDetailsElement>("oscar-year");
    const jumpTo = screen.getByRole("combobox", { name: "jump-to-year" });

    expect(
      [...jumpTo.querySelectorAll("option")].map((o) => o.textContent),
    ).toEqual(["Select a year", "2016", "1927/28"]);
    fireEvent.change(jumpTo, { target: { value: "1" } });

    expect(scrollIntoView).toHaveBeenCalledTimes(1);
    expect(scrollIntoView.mock.contexts[0]).toBe(
      document.getElementById("oscars-year-1"),
    );
    expect(years.map((y) => y.open)).toEqual([false, true]);
    expect(within(years[1]).getAllByTestId("oscar-category")).toHaveLength(3);
  });

  it("Should show the error when the Oscars can't be loaded", async () => {
    server.use(
      http.get("/api/oscars", () =>
        HttpResponse.json(
          { detail: "The Academy Awards data could not be loaded." },
          { status: 503 },
        ),
      ),
    );
    renderApp("/oscars");

    expect((await screen.findByText(/Loading Oscars failed/)).textContent).toBe(
      "Loading Oscars failed with status 503: The Academy Awards data could not be loaded. Please try again.",
    );
    expect(screen.queryByTestId("oscar-year")).toBeNull();
  });
});
