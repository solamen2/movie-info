import { expect, type Locator, type Page } from "@playwright/test";
import { test } from "./e2ebase.ts";
import process from "node:process";

test.beforeEach(async ({ page }) => {
  console.log("Logging in...");
  await page.goto("");
  await expect(page).toHaveTitle("Movie Search");
  const email = page.getByLabel("Email");
  const emailText = process.env.CI
    ? process.env.E2E_TEST_USERNAME
    : process.env.E2E_TEST_LOCAL_USERNAME;
  await email.fill(emailText ?? "");
  const password = page.getByLabel("Password");
  const passwordText = process.env.CI
    ? process.env.E2E_TEST_PASSWORD
    : process.env.E2E_TEST_LOCAL_PASSWORD;
  await password.fill(passwordText ?? "");
  const loginButton = page.getByRole("button", { name: "Login" });
  await loginButton.click();
  console.log("Login finished.");
});

test.afterEach(async ({ page }) => {
  console.log("Logging out...");
  const logoutButton = page.getByRole("button", { name: "Logout" });
  await logoutButton.click();
  console.log("Logout finished.");
});

const useMockHttpCalls = process.env.VITE_USE_MOCK_HTTP_CALLS === "true";

// Whether a computed border color is (all but) the blue highlight of a
// selected card. "All but", since the fade towards the highlight can be cut
// short a frame early by the next phase of the animation.
function isHighlighted(color: string): boolean {
  const match = /^rgba?\((\d+), (\d+), (\d+)(?:, ([\d.]+))?\)$/.exec(color);
  if (!match) return false;
  const [red, green, blue] = match.slice(1, 4).map(Number);
  const alpha = Number((match[4] as string | undefined) ?? 1);
  return red <= 25 && green <= 25 && blue >= 230 && alpha >= 0.9;
}

async function getBorderColor(card: Locator): Promise<string> {
  return card.evaluate((element) => getComputedStyle(element).borderTopColor);
}

// Long enough for any border color transition to have finished.
const HIGHLIGHT_SETTLE_MS = 700;

// Called before a card is selected, i.e. at the beginning of the animation
// that grows it into a panel. Returns the border color to expect once the
// animation has ended.
async function getUnselectedColor(card: Locator): Promise<string> {
  const unselectedColor = await getBorderColor(card);
  expect(isHighlighted(unselectedColor)).toBe(false);
  return unselectedColor;
}

// Every panel has the same frame, whichever kind of card it grew from.
const PANEL_FRAME = {
  "border-top-color": "rgb(68, 68, 68)",
  "border-top-style": "solid",
  "border-top-width": "2px",
  "border-top-left-radius": "8px",
  "padding-top": "12px",
  "padding-left": "12px",
};

// At the end of the animation that grows a card into a panel, the highlight
// has faded out again, leaving the panel's own frame.
async function expectHighlightFadedOut(panelCard: Locator) {
  for (const [property, value] of Object.entries(PANEL_FRAME)) {
    await expect(panelCard).toHaveCSS(property, value);
  }
}

// At the end of the animation that takes a panel back to being a card, the
// card has settled back among the other cards without a highlight. The mouse
// is moved out of the way first, since result cards change color on hover.
async function expectHighlightGone(
  page: Page,
  card: Locator,
  unselectedColor: string,
) {
  await page.mouse.move(0, 0);
  await expect(card).toHaveCSS("border-top-color", unselectedColor);
}

// A panel's border never changes color, no matter where the user clicks.
async function expectNeverHighlighted(
  page: Page,
  panelCard: Locator,
  panel: Locator,
) {
  const panelColor = await getBorderColor(panelCard);
  expect(isHighlighted(panelColor)).toBe(false);
  for (const target of [
    panel.getByRole("heading").first(),
    page.locator(".results-toolbar"),
  ]) {
    await target.click();
    await page.waitForTimeout(HIGHLIGHT_SETTLE_MS);
    expect(await getBorderColor(panelCard)).toBe(panelColor);
  }
}

// Real data is limited to values that are unlikely to change over time (so no
// rating, vote count, rank, revenue, or watch providers).
const expectedMovie = useMockHttpCalls
  ? {
      searchText: "1",
      title: "Example Movie",
      imdbId: "tt0000001",
      // A cast member whose person panel is opened from the movie panel, and
      // one of their movie cast credits to open from there in turn
      castMember: "Example Jones",
      castMemberMovieCredit: "Example Film",
      cardText: [
        "Example MovieSearch Type: MediaMedia Type: MovieRank: ",
        "4444Known For: Example Jones, Example BrownYear: 2016",
      ],
      tagline: "An example tagline.",
      imdbRow: /IMDB: 7\.9 \(123,456\)Rank: 4444Link/,
      imdbUrl: "https://www.imdb.com/title/tt0000001",
      wikipediaUrl: "https://en.wikipedia.org/wiki/Example_Movie",
      facts: [
        "Original TitleExample Movie Original",
        "Release DateSep 23, 2016",
        "Runtime2h 22m",
        "RatedPG-13",
        "Known ForExample Jones, Example Brown",
        "GenresMystery, Thriller, Drama, Science Fiction, Horror",
        "Budget$25,000,000",
        "Revenue$58,000,000",
        "Homepagehttps://example.com/example-movie",
        "Origin CountriesUnited States of America",
        "Production CountriesUnited States of America",
        "Origin LanguageEnglish",
        "Spoken LanguagesEnglish, Spanish",
      ],
      sections: [
        ["Cast", "Example Jones"],
        ["Directors", "Example Director"],
        ["Writers", "Example Writer"],
        [
          "Oscars",
          "Best Picture: Example Producer and Example Smith, Producers",
        ],
        ["Plot (TMDB)", "An example plot from TMDB."],
        ["Plot (OMDB)", "An example plot from OMDB."],
        ["Where to Stream", "Example Stream"],
        ["Where to Rent", "None"],
        ["Where to Buy", "Example Buy Store"],
      ] as [string, string | RegExp][],
    }
  : {
      searchText: "The Shawshank Redemption",
      title: "The Shawshank Redemption",
      imdbId: "tt0111161",
      castMember: "Tim Robbins",
      castMemberMovieCredit: "The Shawshank Redemption",
      cardText: [
        "The Shawshank RedemptionSearch Type: MediaMedia Type: MovieRank: ",
        "Known For: Tim Robbins, Morgan FreemanYear: 1994", // remove rank from Shawshank because it changes over time
      ],
      tagline: "Fear can hold you prisoner. Hope can set you free.",
      imdbRow: /IMDB: \d\.\d \([\d,]+\)Rank: \d+Link/,
      imdbUrl: "https://www.imdb.com/title/tt0111161",
      wikipediaUrl:
        "https://en.wikipedia.org/wiki/The_Shawshank_Redemption_(1994_film)",
      facts: [
        "Original TitleThe Shawshank Redemption",
        "Release DateSep 23, 1994",
        "Runtime2h 22m",
        "RatedR",
        "Known ForTim Robbins, Morgan Freeman",
        "GenresDrama, Crime",
        "Budget$25,000,000",
        "Origin CountriesUnited States of America",
        "Production CountriesUnited States of America",
        "Origin LanguageEnglish",
        "Spoken LanguagesEnglish",
      ],
      sections: [
        ["Cast", "Tim Robbins"],
        ["Directors", "Frank Darabont"],
        ["Writers", "Stephen King"],
        // Seven nominations, no wins
        [
          "Oscars",
          /Wins\s*None[\s\S]*Nominations[\s\S]*Best Actor: Morgan Freeman as Red/,
        ],
        ["Plot (TMDB)", "Shawshank"],
        // OMDB's plot text changes over time, so only check that there is one
        ["Plot (OMDB)", /\w/],
        ["Where to Stream", ""],
        ["Where to Rent", ""],
        ["Where to Buy", ""],
      ] as [string, string | RegExp][],
    };

test("Basic happy path: search, check results are valid, select a movie search card, check the movie panel is valid, and unselect the card", async ({
  page,
}) => {
  console.log("Starting basic happy path test...");
  const searchQueryInput = page.getByRole("textbox", {
    name: "search-query-input",
  });
  await searchQueryInput.fill(expectedMovie.searchText);
  const searchButton = page.getByRole("button", { name: "search" });
  await searchButton.click();

  const movieTextElement = page.getByText(expectedMovie.title, {
    exact: true,
  });
  const movieCard = movieTextElement.locator("ancestor=#search-card");
  for (const cardText of expectedMovie.cardText) {
    expect(await movieCard.textContent()).toContain(cardText);
  }
  const resultsMessage = page.getByText(/^\d+ results\.$/);
  await expect(resultsMessage).toBeVisible();

  console.log("Selecting the movie search card...");
  const unselectedColor = await getUnselectedColor(movieCard);
  await movieCard.click();
  await expect(resultsMessage).toBeHidden();
  const detailUrl = new RegExp(
    `/movie/${expectedMovie.imdbId}\\?q=${encodeURIComponent(expectedMovie.searchText).replace(/%20/g, "\\+")}$`,
  );
  await expect(page).toHaveURL(detailUrl);

  const moviePanel = page.getByTestId("movie-panel");
  await expect(moviePanel).toBeVisible();
  const selectedCard = page.locator("#search-card.selected.expanded");
  await expect(selectedCard).toHaveCount(1);
  await expect(
    moviePanel.getByRole("heading", { name: expectedMovie.title, exact: true }),
  ).toBeVisible();

  console.log("Checking the highlight faded out and stays away...");
  await expectHighlightFadedOut(selectedCard);
  await expectNeverHighlighted(page, selectedCard, moviePanel);
  await expect(moviePanel).toContainText(expectedMovie.tagline);
  await expect(moviePanel).toContainText(expectedMovie.imdbRow);
  // The IMDB link comes first, then the Wikipedia link
  const movieLinks = moviePanel.getByRole("link", {
    name: "Link",
    exact: true,
  });
  await expect(movieLinks).toHaveCount(2);
  const imdbLink = movieLinks.first();
  await expect(imdbLink).toHaveAttribute("href", expectedMovie.imdbUrl);
  await expect(imdbLink).toHaveAttribute("target", "_blank");
  await expect(
    moviePanel.getByRole("button", { name: "copy-imdb-link" }),
  ).toBeVisible();
  await expect(moviePanel).toContainText("Wikipedia:Link");
  const wikipediaLink = movieLinks.last();
  await expect(wikipediaLink).toHaveAttribute(
    "href",
    expectedMovie.wikipediaUrl,
  );
  await expect(wikipediaLink).toHaveAttribute("target", "_blank");
  await expect(
    moviePanel.getByRole("button", { name: "copy-wikipedia-link" }),
  ).toBeVisible();
  for (const fact of expectedMovie.facts) {
    await expect(moviePanel).toContainText(fact);
  }

  console.log("Checking the collapsible sections...");
  const sections = moviePanel.locator("details");
  await expect(sections.locator("summary")).toHaveText(
    expectedMovie.sections.map(
      ([title]) => new RegExp(`^${escapeRegExp(title)}`),
    ),
  );
  for (const [index, [, content]] of expectedMovie.sections.entries()) {
    const section = sections.nth(index);
    const sectionBody = section.locator(".collapsible-body");
    await expect(sectionBody).toBeHidden();
    await section.locator("summary").click();
    await expect(sectionBody).toBeVisible();
    await expect(sectionBody).toContainText(content);
  }

  console.log("Unselecting the movie search card...");
  await page.goBack();
  await expect(moviePanel).toBeHidden();
  await expect(page.locator("#search-card.selected")).toHaveCount(0);
  await expectHighlightGone(page, movieCard, unselectedColor);
  await expect(resultsMessage).toBeVisible();
  for (const cardText of expectedMovie.cardText) {
    expect(await movieCard.textContent()).toContain(cardText);
  }
  const searchUrl = /\/search\?q=[^/]+$/;
  await expect(page).toHaveURL(searchUrl);

  console.log("Reselecting the movie and using the browser back button...");
  await movieCard.click();
  await expect(moviePanel).toBeVisible();
  await expect(page).toHaveURL(detailUrl);
  await page.goBack();
  await expect(page).toHaveURL(searchUrl);
  await expect(moviePanel).toBeHidden();
  await expect(page.locator("#search-card.selected")).toHaveCount(0);
  await expect(resultsMessage).toBeVisible();

  console.log("Using the browser forward button...");
  await page.goForward();
  await expect(page).toHaveURL(detailUrl);
  await expect(moviePanel).toBeVisible();
  await expect(page.locator("#search-card.selected.expanded")).toHaveCount(1);
  await expect(
    moviePanel.getByRole("heading", { name: expectedMovie.title, exact: true }),
  ).toBeVisible();

  console.log("Reloading the movie URL directly...");
  await page.reload();
  await expect(page).toHaveURL(detailUrl);
  await expect(moviePanel).toBeVisible();
  await expect(page.locator("#search-card.selected.expanded")).toHaveCount(1);
  await expect(
    moviePanel.getByRole("heading", { name: expectedMovie.title, exact: true }),
  ).toBeVisible();
  await expect(
    page.getByRole("textbox", { name: "search-query-input" }),
  ).toHaveValue(expectedMovie.searchText);
  console.log("Checking the panel shown from a URL is not highlighted...");
  await expectNeverHighlighted(
    page,
    page.locator("#search-card.selected.expanded"),
    moviePanel,
  );
  await page.goBack();
  await expect(page).toHaveURL(searchUrl);
  await expect(moviePanel).toBeHidden();
  await expect(resultsMessage).toBeVisible();
  for (const cardText of expectedMovie.cardText) {
    expect(await movieCard.textContent()).toContain(cardText);
  }
  console.log("Basic happy path test finished.");
});

// Real data is limited to values that are unlikely to change over time (so no
// rank, "known for" text, or credit counts).
const expectedPerson = useMockHttpCalls
  ? {
      searchText: "1",
      name: "Example Smith",
      cardText: [
        "Example SmithSearch Type: PersonRank: ",
        "3Known For: Actress, Example Film",
      ],
      imdbRow: /IMDB: Rank: 3Link/,
      imdbUrl: "https://www.imdb.com/name/nm9000000",
      wikipediaUrl: "https://en.wikipedia.org/wiki/Example_Smith",
      facts: [
        "Known ForActress, Example Film",
        "Known For DepartmentActing",
        "Also Known AsExample Smithee, Betsy Smith",
        "BirthdayApr 14, 1977",
        "Deathday—",
        "Place of BirthNew York City, New York, USA",
        "GenderFemale",
        "Homepagehttps://example.com/example-smith",
      ],
      sections: [
        ["Biography", "She is especially known for Example Film."],
        ["Oscars", "Best Actress* (2016): Example Film as Example Hero"],
        ["Movie Cast Credits", "Example Hero"],
        ["Movie Crew Credits", "Executive Producer"],
        ["TV Series Cast Credits", "Example Slayer"],
        ["TV Series Crew Credits", "None"],
        ["Profile Images", ""],
      ],
    }
  : {
      searchText: "Sarah Michelle Gellar",
      name: "Sarah Michelle Gellar",
      cardText: [
        "Sarah Michelle GellarSearch Type: PersonRank: ",
        "Known For: ",
      ],
      imdbRow: /IMDB: Rank: \d+Link/,
      imdbUrl: "https://www.imdb.com/name/nm0001264",
      wikipediaUrl: "https://en.wikipedia.org/wiki/Sarah_Michelle_Gellar",
      facts: [
        "Known For DepartmentActing",
        "BirthdayApr 14, 1977",
        "Deathday—",
        "Place of BirthNew York City, New York, USA",
        "GenderFemale",
      ],
      sections: [
        ["Biography", "Gellar"],
        // No wins and no nominations
        ["Oscars", "WinsNoneNominationsNone"],
        ["Movie Cast Credits", "Cruel Intentions"],
        ["Movie Crew Credits", ""],
        ["TV Series Cast Credits", "Buffy the Vampire Slayer"],
        ["TV Series Crew Credits", "Ringer"],
        ["Profile Images", ""],
      ],
    };

test("Person happy path: search, check results are valid, select a person search card, check the person panel is valid, and unselect the card", async ({
  page,
}) => {
  console.log("Starting person happy path test...");
  const searchQueryInput = page.getByRole("textbox", {
    name: "search-query-input",
  });
  await searchQueryInput.fill(expectedPerson.searchText);
  const searchButton = page.getByRole("button", { name: "search" });
  await searchButton.click();

  const personTextElement = page
    .getByText(expectedPerson.name, { exact: true })
    .first();
  const personCard = personTextElement.locator("ancestor=#search-card");
  for (const cardText of expectedPerson.cardText) {
    expect(await personCard.textContent()).toContain(cardText);
  }
  const resultsMessage = page.getByText(/^\d+ results\.$/);
  await expect(resultsMessage).toBeVisible();

  console.log("Selecting the person search card...");
  const unselectedColor = await getUnselectedColor(personCard);
  await personCard.click();
  await expect(resultsMessage).toBeHidden();

  const personPanel = page.getByTestId("person-panel");
  await expect(personPanel).toBeVisible();
  const selectedCard = page.locator("#search-card.selected.expanded");
  await expect(selectedCard).toHaveCount(1);
  await expect(
    personPanel.getByRole("heading", {
      name: expectedPerson.name,
      exact: true,
    }),
  ).toBeVisible();

  console.log("Checking the highlight faded out and stays away...");
  await expectHighlightFadedOut(selectedCard);
  await expectNeverHighlighted(page, selectedCard, personPanel);

  await expect(personPanel).toContainText(expectedPerson.imdbRow);
  // The IMDB link comes first, then the Wikipedia link
  const personLinks = personPanel.getByRole("link", {
    name: "Link",
    exact: true,
  });
  await expect(personLinks).toHaveCount(2);
  const imdbLink = personLinks.first();
  await expect(imdbLink).toHaveAttribute("href", expectedPerson.imdbUrl);
  await expect(imdbLink).toHaveAttribute("target", "_blank");
  await expect(
    personPanel.getByRole("button", { name: "copy-imdb-link" }),
  ).toBeVisible();
  await expect(personPanel).toContainText("Wikipedia:Link");
  const wikipediaLink = personLinks.last();
  await expect(wikipediaLink).toHaveAttribute(
    "href",
    expectedPerson.wikipediaUrl,
  );
  await expect(wikipediaLink).toHaveAttribute("target", "_blank");
  await expect(
    personPanel.getByRole("button", { name: "copy-wikipedia-link" }),
  ).toBeVisible();
  for (const fact of expectedPerson.facts) {
    await expect(personPanel).toContainText(fact);
  }
  // The age depends on today's date, so only its form is checked
  await expect(personPanel).toContainText(
    /BirthdayApr 14, 1977 \(\d+\)Deathday—/,
  );
  await expect(personPanel.getByTestId("person-age")).toHaveCSS(
    "font-style",
    "italic",
  );

  console.log("Checking the collapsible sections...");
  const sections = personPanel.locator("details");
  await expect(sections.locator("summary")).toHaveText(
    expectedPerson.sections.map(
      ([title]) => new RegExp(`^${escapeRegExp(title)}`),
    ),
  );
  for (const [index, [, content]] of expectedPerson.sections.entries()) {
    const section = sections.nth(index);
    const sectionBody = section.locator(".collapsible-body");
    await expect(sectionBody).toBeHidden();
    await section.locator("summary").click();
    await expect(sectionBody).toBeVisible();
    await expect(sectionBody).toContainText(content);
  }

  console.log("Checking the page does not scroll horizontally...");
  const hasHorizontalScrollbar = await page.evaluate(
    () =>
      document.documentElement.scrollWidth >
      document.documentElement.clientWidth,
  );
  expect(hasHorizontalScrollbar).toBe(false);

  console.log("Reloading the person URL directly...");
  await page.reload();
  await expect(
    personPanel.getByRole("heading", {
      name: expectedPerson.name,
      exact: true,
    }),
  ).toBeVisible();
  await expectNeverHighlighted(page, selectedCard, personPanel);

  console.log("Unselecting the person search card...");
  await page.keyboard.press("Escape");
  await expect(personPanel).toBeHidden();
  await expect(page.locator("#search-card.selected")).toHaveCount(0);
  await expectHighlightGone(page, personCard, unselectedColor);
  await expect(resultsMessage).toBeVisible();
  for (const cardText of expectedPerson.cardText) {
    expect(await personCard.textContent()).toContain(cardText);
  }
  console.log("Person happy path test finished.");
});

// Real data is limited to values that are unlikely to change over time (so no
// rating, vote count, rank, "known for" text, credit counts, or watch
// providers).
const expectedTvSeries = useMockHttpCalls
  ? {
      searchText: "1",
      name: "Example TV Series",
      imdbId: "tt10000002",
      tmdbId: 90002,
      cardText: [
        "Example TV SeriesSearch Type: MediaMedia Type: TV SeriesRank: ",
        "4444Known For: John Smith, James JohnsonYears: 2001-2003",
      ],
      tagline: "An example TV series tagline.",
      imdbRow: /IMDB: 8\.3 \(172,659\)Rank: 4444Link/,
      imdbUrl: "https://www.imdb.com/title/tt10000002",
      wikipediaUrl: "https://en.wikipedia.org/wiki/Example_TV_Series",
      facts: [
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
      ],
      sections: [
        ["Seasons", "Season 1"],
        ["Cast", "Example Jones"],
        ["Creators", "Example Creator"],
        ["Directors", "Example Director"],
        ["Writers", "Example Writer"],
        ["Overview (TMDB)", "An example overview from TMDB."],
        ["Overview (OMDB)", "An example overview from OMDB."],
        ["Networks", "Example Network"],
        ["Where to Stream", "Example Stream"],
        ["Where to Rent", "None"],
        ["Where to Buy", "Example Buy Store"],
      ] as [string, string | RegExp][],
      firstSeason: {
        name: "Season 1",
        overview: "An example overview for season 1.",
      },
    }
  : {
      searchText: "Buffy the Vampire Slayer",
      name: "Buffy the Vampire Slayer",
      imdbId: "tt0118276",
      tmdbId: 95,
      cardText: [
        "Buffy the Vampire SlayerSearch Type: MediaMedia Type: TV SeriesRank: ",
        "Years: 1997-2003",
      ],
      tagline: "",
      imdbRow: /IMDB: \d\.\d \([\d,]+\)Rank: \d+Link/,
      imdbUrl: "https://www.imdb.com/title/tt0118276",
      wikipediaUrl:
        "https://en.wikipedia.org/wiki/Buffy_the_Vampire_Slayer_(TV_series)",
      facts: [
        "Original NameBuffy the Vampire Slayer",
        "Years1997-2003",
        "First Air DateMar 10, 1997",
        "Last Air DateMay 20, 2003",
        "Next Air Date—",
        "RatedTV-14",
        "In ProductionNo",
        "TypeScripted",
        "Number of Seasons7",
        "Number of Episodes144",
        "Origin CountriesUnited States of America",
        "Origin LanguageEnglish",
      ],
      sections: [
        ["Seasons", "Season 1"],
        ["Cast", "Sarah Michelle Gellar"],
        ["Creators", "Joss Whedon"],
        ["Directors", "Joss Whedon"],
        ["Writers", "Joss Whedon"],
        ["Overview (TMDB)", "vampire"],
        // OMDB's overview text changes over time, so only check that there is one
        ["Overview (OMDB)", /\w/],
        ["Networks", "The WB"],
        ["Where to Stream", ""],
        ["Where to Rent", ""],
        ["Where to Buy", ""],
      ] as [string, string | RegExp][],
      firstSeason: {
        name: "Season 1",
        overview: "",
      },
    };

test("TV series happy path: search, check results are valid, select a TV series search card, check the TV series panel is valid, open a season overview, and unselect the card", async ({
  page,
}) => {
  console.log("Starting TV series happy path test...");
  const searchQueryInput = page.getByRole("textbox", {
    name: "search-query-input",
  });
  await searchQueryInput.fill(expectedTvSeries.searchText);
  const searchButton = page.getByRole("button", { name: "search" });
  await searchButton.click();

  const tvSeriesTextElement = page
    .getByText(expectedTvSeries.name, { exact: true })
    .first();
  const tvSeriesCard = tvSeriesTextElement.locator("ancestor=#search-card");
  for (const cardText of expectedTvSeries.cardText) {
    expect(await tvSeriesCard.textContent()).toContain(cardText);
  }
  const resultsMessage = page.getByText(/^\d+ results\.$/);
  await expect(resultsMessage).toBeVisible();

  console.log("Selecting the TV series search card...");
  const unselectedColor = await getUnselectedColor(tvSeriesCard);
  await tvSeriesCard.click();
  await expect(resultsMessage).toBeHidden();

  const tvSeriesPanel = page.getByTestId("tv-series-panel");
  await expect(tvSeriesPanel).toBeVisible();
  const selectedCard = page.locator("#search-card.selected.expanded");
  await expect(selectedCard).toHaveCount(1);
  await expect(
    tvSeriesPanel.getByRole("heading", {
      name: expectedTvSeries.name,
      exact: true,
    }),
  ).toBeVisible();

  console.log("Checking the highlight faded out and stays away...");
  await expectHighlightFadedOut(selectedCard);
  await expectNeverHighlighted(page, selectedCard, tvSeriesPanel);

  if (expectedTvSeries.tagline) {
    await expect(tvSeriesPanel).toContainText(expectedTvSeries.tagline);
  }
  await expect(tvSeriesPanel).toContainText(expectedTvSeries.imdbRow);
  // The IMDB link comes first, then the Wikipedia link
  const tvSeriesLinks = tvSeriesPanel.getByRole("link", {
    name: "Link",
    exact: true,
  });
  await expect(tvSeriesLinks).toHaveCount(2);
  const imdbLink = tvSeriesLinks.first();
  await expect(imdbLink).toHaveAttribute("href", expectedTvSeries.imdbUrl);
  await expect(imdbLink).toHaveAttribute("target", "_blank");
  await expect(
    tvSeriesPanel.getByRole("button", { name: "copy-imdb-link" }),
  ).toBeVisible();
  await expect(tvSeriesPanel).toContainText("Wikipedia:Link");
  const wikipediaLink = tvSeriesLinks.last();
  await expect(wikipediaLink).toHaveAttribute(
    "href",
    expectedTvSeries.wikipediaUrl,
  );
  await expect(wikipediaLink).toHaveAttribute("target", "_blank");
  await expect(
    tvSeriesPanel.getByRole("button", { name: "copy-wikipedia-link" }),
  ).toBeVisible();
  for (const fact of expectedTvSeries.facts) {
    await expect(tvSeriesPanel).toContainText(fact);
  }

  console.log("Checking the collapsible sections...");
  // Season cards have their own (nested) overview collapsible, so only look at
  // the panel's top-level sections here
  const sections = tvSeriesPanel.locator(".detail-sections > details");
  await expect(sections.locator("> summary")).toHaveText(
    expectedTvSeries.sections.map(
      ([title]) => new RegExp(`^${escapeRegExp(title)}`),
    ),
  );
  for (const [index, [, content]] of expectedTvSeries.sections.entries()) {
    const section = sections.nth(index);
    const sectionBody = section.locator("> .collapsible-body");
    await expect(sectionBody).toBeHidden();
    await section.locator("> summary").click();
    await expect(sectionBody).toBeVisible();
    await expect(sectionBody).toContainText(content);
  }

  console.log("Checking the cast episode count checkbox...");
  const castSection = tvSeriesPanel
    .locator(".detail-sections > details")
    .filter({ has: page.locator("> summary", { hasText: /^Cast/ }) });
  const hideEpisodesCheckbox = castSection.getByRole("checkbox", {
    name: "Hide number of episodes",
  });
  const firstCastCard = castSection.getByTestId("cast-card").first();
  await expect(hideEpisodesCheckbox).toBeChecked();
  await expect(firstCastCard).not.toContainText(/\d+ episodes?/);
  await hideEpisodesCheckbox.uncheck();
  await expect(firstCastCard).toContainText(/\d+ episodes?/);
  await hideEpisodesCheckbox.check();
  await expect(firstCastCard).not.toContainText(/\d+ episodes?/);

  console.log("Checking a season card...");
  const firstSeasonCard = page
    .getByTestId("season-card")
    .filter({ hasText: expectedTvSeries.firstSeason.name })
    .first();
  await expect(firstSeasonCard).toBeVisible();
  const seasonOverview = firstSeasonCard.locator(".collapsible-body");
  await expect(seasonOverview).toBeHidden();
  await firstSeasonCard.locator("summary").click();
  await expect(seasonOverview).toBeVisible();
  await expect(seasonOverview).toContainText(
    expectedTvSeries.firstSeason.overview,
  );
  await expect(
    firstSeasonCard.getByRole("button", { name: "Expand" }),
  ).toBeVisible();

  console.log("Checking the page does not scroll horizontally...");
  const hasHorizontalScrollbar = await page.evaluate(
    () =>
      document.documentElement.scrollWidth >
      document.documentElement.clientWidth,
  );
  expect(hasHorizontalScrollbar).toBe(false);

  console.log("Unselecting the TV series search card...");
  await page.goBack();
  await expect(tvSeriesPanel).toBeHidden();
  await expect(page.locator("#search-card.selected")).toHaveCount(0);
  await expectHighlightGone(page, tvSeriesCard, unselectedColor);
  await expect(resultsMessage).toBeVisible();
  for (const cardText of expectedTvSeries.cardText) {
    expect(await tvSeriesCard.textContent()).toContain(cardText);
  }
  console.log("TV series happy path test finished.");
});

// A collapsible section's title and the content (one string, or several that
// must all appear) expected inside it once opened.
type ExpectedSection = [title: string, content: string | string[]];

interface ExpectedTvSeason {
  name: string;
  seasonNumber: number;
  facts: string[];
  sections: ExpectedSection[];
  episode: {
    episodeNumber: number;
    cardText: string;
    title: string;
    seasonEpisode: string;
    imdbRow: RegExp;
    facts: string[];
    sections: ExpectedSection[];
  };
}

// Real data is limited to values that are unlikely to change over time (so no
// watch providers or overviews).
const expectedTvSeason: ExpectedTvSeason = useMockHttpCalls
  ? {
      name: "Season 1",
      seasonNumber: 1,
      facts: [
        "Season Number1",
        "Number of Episodes3",
        "First Air DateMar 10, 2001",
      ],
      sections: [
        ["Episodes", "Example Pilot"],
        ["Overview (TMDB)", "An example overview for season 1."],
        ["Networks", "Example Network"],
        ["Where to Stream", "Example Stream"],
        ["Where to Rent", "None"],
        ["Where to Buy", "Example Buy Store"],
      ],
      episode: {
        episodeNumber: 1,
        cardText: "1. Example PilotMar 10, 200144m · Standard",
        title: "Example Pilot",
        seasonEpisode: "Season 1, Episode 1",
        imdbRow: /IMDB: 8\.1 \(3,456\)Link/,
        facts: [
          "Air DateMar 10, 2001",
          "Runtime44m43m",
          "RatedTV-14",
          "TypeStandard",
          "Known ForExample Jones, Example Brown",
          "GenresAction, Drama, Science Fiction",
        ],
        sections: [
          ["Cast", "Example Jones"],
          ["Directors", "Example Director"],
          ["Writers", "Example Writer"],
          ["Guest Stars", "Example Guest"],
          ["Overview (OMDB)", "An example episode overview from OMDB."],
          ["Overview (TMDB)", "An example episode overview from TMDB."],
        ],
      },
    }
  : {
      name: "Season 6",
      seasonNumber: 6,
      facts: [
        "Season Number6",
        "Number of Episodes22",
        "First Air DateOct 2, 2001",
      ],
      sections: [
        ["Episodes", "Once More, with Feeling"],
        ["Overview (TMDB)", ""],
        ["Networks", ""],
        ["Where to Stream", ""],
        ["Where to Rent", ""],
        ["Where to Buy", ""],
      ],
      episode: {
        episodeNumber: 7,
        cardText: "7. Once More, with FeelingNov 6, 2001",
        title: "Once More, with Feeling",
        seasonEpisode: "Season 6, Episode 7",
        imdbRow: /IMDB: \d\.\d \([\d,]+\)Link/,
        facts: [
          "Air DateNov 6, 2001",
          // The musical episode runs long (OMDB runtime, then TMDB's)
          "Runtime50m50m",
          "Rated13+",
          "TypeStandard",
        ],
        sections: [
          ["Cast", ["Sarah Michelle Gellar", "James Marsters"]],
          ["Directors", "Joss Whedon"],
          ["Writers", "Joss Whedon"],
          ["Guest Stars", ["Hinton Battle", "Amber Benson"]],
          ["Overview (OMDB)", ""],
          ["Overview (TMDB)", ""],
        ],
      },
    };

// Search for the TV series, open its panel, and expand the expected season.
async function openSeason(page: Page) {
  const searchQueryInput = page.getByRole("textbox", {
    name: "search-query-input",
  });
  await searchQueryInput.fill(expectedTvSeries.searchText);
  await page.getByRole("button", { name: "search" }).click();

  const tvSeriesCard = page
    .getByText(expectedTvSeries.name, { exact: true })
    .first()
    .locator("ancestor=#search-card");
  await tvSeriesCard.click();
  const tvSeriesPanel = page.getByTestId("tv-series-panel");
  await expect(tvSeriesPanel).toBeVisible();

  await tvSeriesPanel
    .locator(".detail-sections > details")
    .filter({ has: page.locator("> summary", { hasText: /^Seasons/ }) })
    .locator("> summary")
    .click();
  const seasonCard = page
    .getByTestId("season-card")
    .filter({ hasText: expectedTvSeason.name })
    .first();
  await expect(seasonCard).toBeVisible();
  const seasonUnselectedColor = await getUnselectedColor(seasonCard);
  await seasonCard.getByRole("button", { name: "Expand" }).click();

  const seasonUrl = new RegExp(
    `/tvseries/${expectedTvSeries.imdbId}\\?q=[^&]+&tmdbTvSeriesId=${String(expectedTvSeries.tmdbId)}&seasonNumber=${String(expectedTvSeason.seasonNumber)}$`,
  );
  await expect(page).toHaveURL(seasonUrl);
  const tvSeasonPanel = page.getByTestId("tv-season-panel");
  await expect(tvSeasonPanel).toBeVisible();
  await expect(seasonCard).toHaveClass(/selected/);
  await expect(seasonCard).toHaveClass(/expanded/);

  console.log("Checking the highlight faded out and stays away...");
  await expect(
    tvSeasonPanel.getByRole("heading", {
      name: expectedTvSeason.name,
      exact: true,
    }),
  ).toBeVisible();
  await expectHighlightFadedOut(seasonCard);
  await expectNeverHighlighted(page, seasonCard, tvSeasonPanel);

  console.log("Checking the season has taken over from the TV series...");
  const tvSeriesTop = tvSeriesPanel.locator("> .detail-panel-top");
  await expect(tvSeriesTop).toBeHidden();
  await expect(
    tvSeriesPanel.getByRole("heading", {
      name: expectedTvSeries.name,
      exact: true,
    }),
  ).toBeHidden();
  await expect(
    page.locator("[data-testid='season-card']:not(.selected)").first(),
  ).toBeHidden();
  return {
    tvSeriesPanel,
    tvSeriesTop,
    seasonCard,
    tvSeasonPanel,
    seasonUrl,
    seasonUnselectedColor,
  };
}

// Each section is [title, expected content], where the content can be a
// single string or several strings that must all appear.
async function checkCollapsibleSections(
  panel: Locator,
  expected: ExpectedSection[],
) {
  const sections = panel.locator(".detail-sections > details");
  await expect(sections.locator("> summary")).toHaveText(
    expected.map(([title]) => new RegExp(`^${escapeRegExp(title)}`)),
  );
  for (const [index, [, content]] of expected.entries()) {
    const section = sections.nth(index);
    const sectionBody = section.locator("> .collapsible-body");
    await expect(sectionBody).toBeHidden();
    await section.locator("> summary").click();
    await expect(sectionBody).toBeVisible();
    for (const text of Array.isArray(content) ? content : [content]) {
      await expect(sectionBody).toContainText(text);
    }
  }
}

test("TV season happy path: open a TV series, expand a season card, check the season panel is valid, and go back with the browser", async ({
  page,
}) => {
  console.log("Starting TV season happy path test...");
  const {
    tvSeriesPanel,
    tvSeriesTop,
    seasonCard,
    tvSeasonPanel,
    seasonUnselectedColor,
  } = await openSeason(page);

  await expect(
    tvSeasonPanel.getByRole("heading", {
      name: expectedTvSeason.name,
      exact: true,
    }),
  ).toBeVisible();
  const imdbLink = tvSeasonPanel.getByRole("link", {
    name: "Link",
    exact: true,
  });
  await expect(imdbLink).toHaveAttribute("href", expectedTvSeries.imdbUrl);
  await expect(imdbLink).toHaveAttribute("target", "_blank");
  await expect(
    tvSeasonPanel.getByRole("button", { name: "copy-imdb-link" }),
  ).toHaveCount(0);
  for (const fact of expectedTvSeason.facts) {
    await expect(tvSeasonPanel).toContainText(fact);
  }

  console.log("Checking the collapsible sections...");
  await checkCollapsibleSections(tvSeasonPanel, expectedTvSeason.sections);

  console.log("Checking an episode card...");
  const episodeCard = tvSeasonPanel
    .getByTestId("episode-card")
    .filter({ hasText: expectedTvSeason.episode.title })
    .first();
  await expect(episodeCard).toBeVisible();
  expect(await episodeCard.textContent()).toContain(
    expectedTvSeason.episode.cardText,
  );
  const episodeOverview = episodeCard.locator(".collapsible-body");
  await expect(episodeOverview).toBeHidden();
  await episodeCard.locator("summary").click();
  await expect(episodeOverview).toBeVisible();
  await expect(
    episodeCard.getByRole("button", { name: "Expand" }),
  ).toBeVisible();

  console.log("Checking the page does not scroll horizontally...");
  const hasHorizontalScrollbar = await page.evaluate(
    () =>
      document.documentElement.scrollWidth >
      document.documentElement.clientWidth,
  );
  expect(hasHorizontalScrollbar).toBe(false);

  console.log("Going back to the TV series with the browser back button...");
  await page.goBack();
  await expect(page).toHaveURL(
    new RegExp(`/tvseries/${expectedTvSeries.imdbId}\\?q=[^&]+$`),
  );
  await expect(tvSeasonPanel).toBeHidden();
  await expect(seasonCard).not.toHaveClass(/selected/);
  await expectHighlightGone(page, seasonCard, seasonUnselectedColor);
  await expect(tvSeriesPanel).toBeVisible();
  await expect(tvSeriesTop).toBeVisible();
  await expect(
    tvSeriesPanel.getByRole("heading", {
      name: expectedTvSeries.name,
      exact: true,
    }),
  ).toBeVisible();
  await expect(
    page.locator("[data-testid='season-card']:not(.selected)").first(),
  ).toBeVisible();
  await expect(seasonCard).toContainText(expectedTvSeason.name);
  console.log("TV season happy path test finished.");
});

test("TV episode happy path: open a TV season, expand an episode card, check the episode panel is valid, reload it, and go back with the browser", async ({
  page,
}) => {
  console.log("Starting TV episode happy path test...");
  const { tvSeriesTop, seasonCard, tvSeasonPanel, seasonUrl } =
    await openSeason(page);
  const expectedEpisode = expectedTvSeason.episode;

  console.log("Expanding the expected episode card...");
  await tvSeasonPanel
    .locator(".detail-sections > details")
    .filter({ has: page.locator("> summary", { hasText: /^Episodes/ }) })
    .locator("> summary")
    .click();
  const episodeCard = tvSeasonPanel
    .getByTestId("episode-card")
    .filter({ hasText: expectedEpisode.title })
    .first();
  await expect(episodeCard).toBeVisible();
  const unselectedColor = await getUnselectedColor(episodeCard);
  await episodeCard.getByRole("button", { name: "Expand" }).click();

  const episodeUrl = new RegExp(
    `/tvseries/${expectedTvSeries.imdbId}\\?q=[^&]+&tmdbTvSeriesId=${String(expectedTvSeries.tmdbId)}&seasonNumber=${String(expectedTvSeason.seasonNumber)}&episodeNumber=${String(expectedEpisode.episodeNumber)}$`,
  );
  await expect(page).toHaveURL(episodeUrl);
  const tvEpisodePanel = page.getByTestId("tv-episode-panel");
  await expect(tvEpisodePanel).toBeVisible();
  await expect(episodeCard).toHaveClass(/selected/);
  await expect(episodeCard).toHaveClass(/expanded/);
  await expect(
    tvEpisodePanel.getByRole("heading", {
      name: expectedEpisode.title,
      exact: true,
    }),
  ).toBeVisible();

  console.log("Checking the highlight faded out and stays away...");
  await expectHighlightFadedOut(episodeCard);
  await expectNeverHighlighted(page, episodeCard, tvEpisodePanel);

  console.log("Checking the episode has taken over from the season...");
  const tvSeasonTop = tvSeasonPanel.locator("> .detail-panel-top");
  await expect(tvSeasonTop).toBeHidden();
  await expect(
    tvSeasonPanel.getByRole("heading", {
      name: expectedTvSeason.name,
      exact: true,
    }),
  ).toBeHidden();
  await expect(tvSeriesTop).toBeHidden();
  await expect(
    page.locator("[data-testid='episode-card']:not(.selected)").first(),
  ).toBeHidden();
  await expect(tvEpisodePanel).toContainText(expectedEpisode.seasonEpisode);
  await expect(tvEpisodePanel).toContainText(expectedEpisode.imdbRow);
  const imdbLinks = tvEpisodePanel.getByRole("link", {
    name: "Link",
    exact: true,
  });
  await expect(imdbLinks).toHaveCount(2);
  await expect(imdbLinks.first()).toHaveAttribute(
    "href",
    /^https:\/\/www\.imdb\.com\/title\/tt\d+$/,
  );
  await expect(imdbLinks.last()).toHaveAttribute(
    "href",
    expectedTvSeries.imdbUrl,
  );
  await expect(
    tvEpisodePanel.getByRole("button", { name: "copy-imdb-link" }),
  ).toHaveCount(1);
  for (const fact of expectedEpisode.facts) {
    await expect(tvEpisodePanel).toContainText(fact);
  }

  console.log("Checking the collapsible sections...");
  await checkCollapsibleSections(tvEpisodePanel, expectedEpisode.sections);

  console.log("Checking the page does not scroll horizontally...");
  const hasHorizontalScrollbar = await page.evaluate(
    () =>
      document.documentElement.scrollWidth >
      document.documentElement.clientWidth,
  );
  expect(hasHorizontalScrollbar).toBe(false);

  console.log("Reloading the episode URL directly...");
  await page.reload();
  await expect(page).toHaveURL(episodeUrl);
  await expect(tvEpisodePanel).toBeVisible();
  await expect(
    tvEpisodePanel.getByRole("heading", {
      name: expectedEpisode.title,
      exact: true,
    }),
  ).toBeVisible();
  await expect(page.getByTestId("tv-season-panel")).toBeVisible();
  await expect(page.getByTestId("tv-series-panel")).toBeVisible();

  console.log("Checking the panels shown from a URL are not highlighted...");
  const expandedCards = page.locator(".expandable-card.expanded");
  await expect(expandedCards).toHaveCount(3);
  for (const expandedCard of await expandedCards.all()) {
    expect(isHighlighted(await getBorderColor(expandedCard))).toBe(false);
  }
  await expectNeverHighlighted(page, episodeCard, tvEpisodePanel);

  console.log("Going back to the season with the ESC key...");
  await page.keyboard.press("Escape");
  await expect(page).toHaveURL(seasonUrl);
  await expect(tvEpisodePanel).toBeHidden();
  await expect(episodeCard).not.toHaveClass(/selected/);
  await expectHighlightGone(page, episodeCard, unselectedColor);
  await expect(tvSeasonPanel).toBeVisible();
  await expect(tvSeasonTop).toBeVisible();
  await expect(
    tvSeasonPanel.getByRole("heading", {
      name: expectedTvSeason.name,
      exact: true,
    }),
  ).toBeVisible();
  await expect(
    page.locator("[data-testid='episode-card']:not(.selected)").first(),
  ).toBeVisible();
  // The season still has the series handed over to it
  await expect(tvSeriesTop).toBeHidden();
  await expect(seasonCard).toHaveClass(/expanded/);
  await expect(episodeCard).toContainText(expectedEpisode.title);
  console.log("TV episode happy path test finished.");
});

// A card inside a panel (a movie's cast, a person's credits, ...) opens its
// own panel in place of the one it was in. Finds the card with the given text
// in the panel's section with the given title, opening the section.
async function openSectionCard(
  panel: Locator,
  sectionTitle: string,
  cardTestId: string,
  cardText: string,
): Promise<Locator> {
  const section = panel
    .locator(".detail-sections > details")
    .filter({ has: panel.page().locator("summary", { hasText: sectionTitle }) })
    .first();
  await section.locator("summary").click();
  const card = section.getByTestId(cardTestId).filter({ hasText: cardText });
  await expect(card).toBeVisible();
  return card;
}

test("Panel to panel: open a movie, open a cast member's person panel from it, open one of their movie credits from that, and go back with the browser", async ({
  page,
}) => {
  console.log("Starting panel to panel test...");
  const searchQueryInput = page.getByRole("textbox", {
    name: "search-query-input",
  });
  await searchQueryInput.fill(expectedMovie.searchText);
  await page.getByRole("button", { name: "search" }).click();
  const movieCard = page
    .getByText(expectedMovie.title, { exact: true })
    .locator("ancestor=#search-card");
  const resultsMessage = page.getByText(/^\d+ results\.$/);
  await expect(resultsMessage).toBeVisible();
  await movieCard.click();
  const moviePanel = page.getByTestId("movie-panel");
  await expect(moviePanel).toBeVisible();
  const movieUrl = new RegExp(`/movie/${expectedMovie.imdbId}\\?q=[^/&]+$`);
  await expect(page).toHaveURL(movieUrl);
  const expandedCard = page.locator("#search-card.selected.expanded");

  console.log("Clicking a cast card in the movie panel...");
  const castCard = await openSectionCard(
    moviePanel,
    "Cast",
    "cast-card",
    expectedMovie.castMember,
  );
  const castUnselectedColor = await getUnselectedColor(castCard);
  await castCard.click();
  // First the URL names the card, which is highlighted and flies to the
  // panel's corner while the panel hands itself over to it...
  const castCardUrl = new RegExp(
    `/movie/${expectedMovie.imdbId}\\?q=[^/&]+&cardType=cast&cardId=\\d+$`,
  );
  await expect(page).toHaveURL(castCardUrl);
  await expect(castCard).toHaveClass(/selected/);
  await expect(moviePanel).toHaveClass(/has-child-selection/);
  // ...then the person's own URL (the person is only known by TMDB id)
  // replaces the movie panel with their panel
  const personUrl = /\/person\/\d+\?q=[^/&]+$/;
  await expect(page).toHaveURL(personUrl);
  await expect(moviePanel).toBeHidden();
  const personPanel = page.getByTestId("person-panel");
  await expect(personPanel).toBeVisible();
  await expect(expandedCard).toHaveCount(1);
  await expect(
    personPanel.getByRole("heading", {
      name: expectedMovie.castMember,
      exact: true,
    }),
  ).toBeVisible();
  await expect(resultsMessage).toBeHidden();
  expect(await page.evaluate(() => window.scrollY)).toBe(0);
  console.log(
    "Checking the person panel's highlight faded out and stays away...",
  );
  await expectHighlightFadedOut(expandedCard);
  await expectNeverHighlighted(page, expandedCard, personPanel);

  console.log("Clicking one of the person's movie credits in their panel...");
  const creditCard = await openSectionCard(
    personPanel,
    "Movie Cast Credits",
    "movie-cast-card",
    expectedMovie.castMemberMovieCredit,
  );
  const creditUnselectedColor = await getUnselectedColor(creditCard);
  await creditCard.click();
  const creditCardUrl =
    /\/person\/\d+\?q=[^/&]+&cardType=movie-cast-credits&cardId=\d+$/;
  await expect(page).toHaveURL(creditCardUrl);
  await expect(creditCard).toHaveClass(/selected/);
  const creditUrl = /\/movie\/\d+\?q=[^/&]+$/;
  await expect(page).toHaveURL(creditUrl);
  await expect(personPanel).toBeHidden();
  await expect(moviePanel).toBeVisible();
  await expect(expandedCard).toHaveCount(1);
  await expect(
    moviePanel.getByRole("heading", {
      name: expectedMovie.castMemberMovieCredit,
      exact: true,
    }),
  ).toBeVisible();

  console.log("Going back, which shows the credit card highlighted...");
  await page.goBack();
  await expect(page).toHaveURL(creditCardUrl);
  await expect(moviePanel).toBeHidden();
  await expect(personPanel).toBeVisible();
  // The card flies home from the corner and rests highlighted in its row,
  // with the rest of the panel back in view around it
  const highlightedCredit = personPanel.locator(
    "[data-testid='movie-cast-card'].highlighted",
  );
  await expect(personPanel).not.toHaveClass(/has-child-selection/);
  await expect(
    personPanel.getByRole("heading", {
      name: expectedMovie.castMember,
      exact: true,
    }),
  ).toBeVisible();
  await expect(personPanel.locator(".detail-panel-top")).toBeVisible();
  await expect(highlightedCredit).toHaveCount(1);
  await expect(highlightedCredit).toContainText(
    expectedMovie.castMemberMovieCredit,
  );
  await page.waitForTimeout(HIGHLIGHT_SETTLE_MS);
  expect(isHighlighted(await getBorderColor(highlightedCredit))).toBe(true);

  console.log("Going back again, which unhighlights the credit card...");
  await page.goBack();
  await expect(page).toHaveURL(personUrl);
  await expect(highlightedCredit).toHaveCount(0);
  await expectHighlightGone(page, creditCard, creditUnselectedColor);
  await expect(
    personPanel.getByRole("heading", {
      name: expectedMovie.castMember,
      exact: true,
    }),
  ).toBeVisible();

  console.log("Reloading the person URL directly...");
  await page.reload();
  await expect(page).toHaveURL(personUrl);
  await expect(personPanel).toBeVisible();
  await expect(expandedCard).toHaveCount(1);
  await expect(
    personPanel.getByRole("heading", {
      name: expectedMovie.castMember,
      exact: true,
    }),
  ).toBeVisible();

  console.log("Going back, which shows the cast card highlighted...");
  await page.goBack();
  await expect(page).toHaveURL(castCardUrl);
  await expect(personPanel).toBeHidden();
  await expect(moviePanel).toBeVisible();
  const highlightedCast = moviePanel.locator(
    "[data-testid='cast-card'].highlighted",
  );
  await expect(moviePanel).not.toHaveClass(/has-child-selection/);
  await expect(
    moviePanel.getByRole("heading", { name: expectedMovie.title, exact: true }),
  ).toBeVisible();
  await expect(moviePanel.locator(".detail-panel-top")).toBeVisible();
  await expect(highlightedCast).toHaveCount(1);
  await expect(highlightedCast).toContainText(expectedMovie.castMember);
  await page.waitForTimeout(HIGHLIGHT_SETTLE_MS);
  expect(isHighlighted(await getBorderColor(highlightedCast))).toBe(true);

  console.log("Going back again, which unhighlights the cast card...");
  await page.goBack();
  await expect(page).toHaveURL(movieUrl);
  await expect(highlightedCast).toHaveCount(0);
  await expectHighlightGone(page, castCard, castUnselectedColor);
  await expect(
    moviePanel.getByRole("heading", { name: expectedMovie.title, exact: true }),
  ).toBeVisible();
  await expect(resultsMessage).toBeHidden();

  console.log("Going back to the search results with the browser...");
  await page.goBack();
  await expect(page).toHaveURL(/\/search\?q=[^/]+$/);
  await expect(moviePanel).toBeHidden();
  await expect(page.locator("#search-card.selected")).toHaveCount(0);
  await expect(resultsMessage).toBeVisible();
  console.log("Panel to panel test finished.");
});

// ---------------------------------------------------------------------------
// Oscars By Year

interface ExpectedOscarSubsection {
  // The subsection's title, or null when the category is undivided that year
  title: string | null;
  // The category's name at the time, in the popup behind the asterisk on the
  // title, when it differed from the modern one
  originalName?: string;
  // The winners' lines, in order, as "<entry> (<detail>)"
  winners: string[];
  // The count on the collapsed "Nominees" section, or null when every entry
  // in the subsection won
  nominees: number | null;
}

interface ExpectedOscarCategory {
  title: string;
  originalName?: string;
  subsections: ExpectedOscarSubsection[];
}

interface ExpectedOscarYear {
  ceremony: number;
  // The year and the ceremony's ordinal, as the section's heading reads
  heading: string;
  // The categories to spot check, not every category of the year
  categories: ExpectedOscarCategory[];
  // Substrings of each special award's line, in order, when checked
  specialAwards?: string[][];
}

interface ExpectedOscarsPage {
  // The most recent year's section is first; the dataset grows by a year
  // after each ceremony, so only lower bounds are checked
  latest: { minYear: number; minCeremony: number; everyCeremony: boolean };
  years: ExpectedOscarYear[];
  // The year "Jump To" scrolls to
  jumpTo: number;
  // A winner whose film and one of whose nominees link to their panels, and a
  // winner whose nominee is a studio or a country, which doesn't
  movieLink: {
    ceremony: number;
    category: string;
    title: string;
    imdbId: string;
  };
  personLink: {
    ceremony: number;
    category: string;
    name: string;
    imdbId: string;
  };
  plainText: { ceremony: number; category: string; text: string };
  // An entry with a note behind its ⓘ mark
  note: {
    ceremony: number;
    category: string;
    subsection: string | null;
    inNominees: boolean;
    line: string;
    text: string;
  };
}

const expectedOscars: ExpectedOscarsPage = useMockHttpCalls
  ? {
      latest: { minYear: 2016, minCeremony: 89, everyCeremony: false },
      years: [
        {
          ceremony: 89,
          heading: "201689th Academy Awards",
          categories: [
            {
              title: "Best Picture",
              subsections: [
                {
                  title: null,
                  winners: [
                    "Example Movie (Example Producer and Example Smith, Producers)",
                  ],
                  nominees: 2,
                },
              ],
            },
            {
              title: "Best Actress",
              originalName: "Actress",
              subsections: [
                {
                  title: null,
                  originalName: "Actress",
                  winners: ["Example Smith as Example Hero (Example Film)"],
                  nominees: 1,
                },
              ],
            },
            {
              title: "Best Original Song",
              subsections: [
                {
                  title: null,
                  winners: [
                    "Example Movie (“Example Song” — Music and Lyric by Example Composer and Example Lyricist, Jr.)",
                  ],
                  nominees: null,
                },
              ],
            },
          ],
          specialAwards: [
            [
              "Honorary Award",
              "To Example Legend, for a lifetime of example work.",
            ],
          ],
        },
        {
          ceremony: 1,
          heading: "1927/281st Academy Awards",
          categories: [
            {
              title: "Best Picture",
              originalName: "Outstanding Picture",
              subsections: [
                {
                  title: null,
                  originalName: "Outstanding Picture",
                  winners: ["Example Silent (Example Studios)"],
                  nominees: 1,
                },
              ],
            },
            {
              title: "Best Cinematography",
              subsections: [
                {
                  title: "Black-and-White",
                  winners: ["Example Silent (Example Cinematographer)"],
                  nominees: null,
                },
                {
                  title: "Color",
                  originalName: "Cinematography (Color Photography)",
                  winners: ["Example Talkie (Example Colorist)"],
                  nominees: 1,
                },
              ],
            },
          ],
          specialAwards: [
            ["Unique and Artistic Picture: Example Art Film (Example Studios)"],
          ],
        },
      ],
      jumpTo: 1,
      movieLink: {
        ceremony: 89,
        category: "Best Picture",
        title: "Example Movie",
        imdbId: "tt0000001",
      },
      personLink: {
        ceremony: 89,
        category: "Best Picture",
        name: "Example Smith",
        imdbId: "nm9000000",
      },
      plainText: {
        ceremony: 1,
        category: "Best Picture",
        text: "Example Studios",
      },
      note: {
        ceremony: 1,
        category: "Best Picture",
        subsection: null,
        inNominees: true,
        line: "Example Talkie",
        text: "NOTE: An example note.",
      },
    }
  : {
      latest: { minYear: 2025, minCeremony: 98, everyCeremony: true },
      years: [
        // The first ceremony: studios as Best Picture nominees, a director
        // award split in two, an actress nominated for three films at once,
        // and the awards that were given only that year
        {
          ceremony: 1,
          heading: "1927/281st Academy Awards",
          categories: [
            {
              title: "Best Picture",
              originalName: "Outstanding Picture",
              subsections: [
                {
                  title: null,
                  originalName: "Outstanding Picture",
                  winners: ["Wings (Paramount Famous Lasky)"],
                  nominees: 2,
                },
              ],
            },
            {
              title: "Best Director",
              subsections: [
                {
                  title: "Dramatic Picture",
                  winners: ["Frank Borzage (7th Heaven)"],
                  nominees: 2,
                },
                {
                  title: "Comedy Picture",
                  winners: ["Lewis Milestone (Two Arabian Knights)"],
                  nominees: 1,
                },
              ],
            },
            {
              title: "Best Actress",
              subsections: [
                {
                  title: null,
                  winners: [
                    "Janet Gaynor as Diane, Angela, The Wife (7th Heaven, Street Angel, Sunrise)",
                  ],
                  nominees: 2,
                },
              ],
            },
            {
              title: "Best Visual Effects",
              originalName: "Engineering Effects",
              subsections: [
                {
                  title: null,
                  originalName: "Engineering Effects",
                  winners: ["Wings (Roy Pomeroy)"],
                  nominees: 2,
                },
              ],
            },
          ],
          specialAwards: [
            ["Unique and Artistic Picture: Sunrise (Fox)"],
            ["Unique and Artistic Picture: The Crowd (Metro-Goldwyn-Mayer)"],
            ["Unique and Artistic Picture: Chang (Paramount Famous Lasky)"],
            ["Special Award: The Jazz Singer", "To Warner Bros."],
            ["Special Award: The Circus", "To Charles Chaplin"],
          ],
        },
        // 1942: three categories split in two, two of them with the names
        // of the time behind asterisks on the subsections
        {
          ceremony: 15,
          heading: "194215th Academy Awards",
          categories: [
            {
              title: "Best Cinematography",
              subsections: [
                {
                  title: "Black-and-White",
                  winners: ["Mrs. Miniver (Joseph Ruttenberg)"],
                  nominees: 9,
                },
                {
                  title: "Color",
                  winners: ["The Black Swan (Leon Shamroy)"],
                  nominees: 5,
                },
              ],
            },
            {
              title: "Best Original Screenplay",
              subsections: [
                {
                  title: "Best Original Screenplay",
                  winners: [
                    "Woman of the Year (Ring Lardner, Jr., Michael Kanin)",
                  ],
                  nominees: 4,
                },
                {
                  title: "Best Story",
                  originalName: "Writing (Original Motion Picture Story)",
                  winners: ["The Invaders (Emeric Pressburger)"],
                  nominees: 4,
                },
              ],
            },
            {
              title: "Best Original Score",
              subsections: [
                {
                  title: "Best Original Non-Musical Score",
                  originalName:
                    "Music (Music Score of a Dramatic or Comedy Picture)",
                  winners: ["Now, Voyager (Max Steiner)"],
                  nominees: 17,
                },
                {
                  title: "Best Original Musical Score",
                  originalName: "Music (Scoring of a Musical Picture)",
                  winners: [
                    "Yankee Doodle Dandy (Ray Heindorf, Heinz Roemheld)",
                  ],
                  nominees: 7,
                },
              ],
            },
          ],
        },
        // 1976: no competitive visual effects award, so the two Special
        // Achievement Awards stand in, both winners and nobody else
        {
          ceremony: 49,
          heading: "197649th Academy Awards",
          categories: [
            {
              title: "Best Visual Effects",
              originalName: "Special Achievement Award (Visual Effects)",
              subsections: [
                {
                  title: null,
                  originalName: "Special Achievement Award (Visual Effects)",
                  winners: [
                    "King Kong (Carlo Rambaldi, Glen Robinson, Frank Van der Veer)",
                    "Logan's Run (L. B. Abbott, Glen Robinson, Matthew Yuricich)",
                  ],
                  nominees: null,
                },
              ],
            },
          ],
        },
        // 1994: sound split into mixing and editing, under older names
        {
          ceremony: 67,
          heading: "199467th Academy Awards",
          categories: [
            {
              title: "Best Picture",
              subsections: [
                {
                  title: null,
                  winners: [
                    "Forrest Gump (Wendy Finerman, Steve Tisch and Steve Starkey, Producers)",
                  ],
                  nominees: 4,
                },
              ],
            },
            {
              title: "Best Sound",
              subsections: [
                {
                  title: "Best Sound Mixing",
                  originalName: "Sound",
                  winners: [
                    "Speed (Gregg Landaker, Steve Maslow, Bob Beemer, David R. B. MacMillan)",
                  ],
                  nominees: 4,
                },
                {
                  title: "Best Sound Editing",
                  originalName: "Sound Effects Editing",
                  winners: ["Speed (Stephen Hunter Flick)"],
                  nominees: 2,
                },
              ],
            },
          ],
        },
        // 2012: a tie, so two winners and three nominees
        {
          ceremony: 85,
          heading: "201285th Academy Awards",
          categories: [
            {
              title: "Best Sound",
              subsections: [
                {
                  title: "Best Sound Mixing",
                  winners: [
                    "Les Misérables (Andy Nelson, Mark Paterson and Simon Hayes)",
                  ],
                  nominees: 4,
                },
                {
                  title: "Best Sound Editing",
                  winners: [
                    "Skyfall (Per Hallberg and Karen Baker Landers)",
                    "Zero Dark Thirty (Paul N.J. Ottosson)",
                  ],
                  nominees: 3,
                },
              ],
            },
          ],
        },
        // 2023: a recent year with an acting award's character, a song, a
        // country as the nominee, and honorary awards
        {
          ceremony: 96,
          heading: "202396th Academy Awards",
          categories: [
            {
              title: "Best Picture",
              subsections: [
                {
                  title: null,
                  winners: [
                    "Oppenheimer (Emma Thomas, Charles Roven and Christopher Nolan, Producers)",
                  ],
                  nominees: 9,
                },
              ],
            },
            {
              title: "Best Actor",
              subsections: [
                {
                  title: null,
                  winners: [
                    "Cillian Murphy as J. Robert Oppenheimer (Oppenheimer)",
                  ],
                  nominees: 4,
                },
              ],
            },
            {
              title: "Best International Feature Film",
              subsections: [
                {
                  title: null,
                  winners: ["The Zone of Interest (United Kingdom)"],
                  nominees: 4,
                },
              ],
            },
            {
              title: "Best Original Song",
              subsections: [
                {
                  title: null,
                  winners: [
                    "Barbie (“What Was I Made For?” — Music and Lyric by Billie Eilish and Finneas O'Connell)",
                  ],
                  nominees: 4,
                },
              ],
            },
          ],
          specialAwards: [
            ["Jean Hersholt Humanitarian Award: Michelle Satter"],
            ["Honorary Award", "To Angela Bassett"],
            ["Honorary Award", "To Mel Brooks"],
            ["Honorary Award", "To Carol Littleton"],
          ],
        },
      ],
      jumpTo: 67,
      movieLink: {
        ceremony: 96,
        category: "Best Picture",
        title: "Oppenheimer",
        imdbId: "tt15398776",
      },
      personLink: {
        ceremony: 96,
        category: "Best Picture",
        name: "Christopher Nolan",
        imdbId: "nm0634240",
      },
      plainText: {
        ceremony: 96,
        category: "Best International Feature Film",
        text: "United Kingdom",
      },
      note: {
        ceremony: 85,
        category: "Best Sound",
        subsection: "Best Sound Editing",
        inNominees: false,
        line: "Skyfall",
        text: "A tie.",
      },
    };

async function openOscarsPage(page: Page): Promise<Locator> {
  await page.getByRole("link", { name: "Oscars By Year" }).click();
  await expect(page).toHaveURL(/\/oscars$/);
  const oscarsPage = page.getByTestId("oscars-page");
  await expect(
    oscarsPage.getByRole("heading", { name: "Oscars By Year" }),
  ).toBeVisible();
  // Every year starts collapsed
  await expect(page.locator("details.oscar-year").first()).toBeVisible();
  await expect(page.locator("details.oscar-year[open]")).toHaveCount(0);
  return oscarsPage;
}

function oscarYear(page: Page, ceremony: number): Locator {
  return page.locator(`#oscars-year-${String(ceremony)}`);
}

async function openOscarYear(page: Page, ceremony: number): Promise<Locator> {
  const year = oscarYear(page, ceremony);
  await year.locator("> summary").click();
  await expect(year).toHaveAttribute("open", "");
  await expect(year.getByTestId("oscar-category").first()).toBeVisible();
  return year;
}

// A title reads "<name>*" when the name at the time is behind an asterisk,
// and "<name>*Original Category Name: <name at the time>" while its popup
// is open
type Asterisk = "yes" | "no" | "either";

function oscarTitle(name: string, asterisk: Asterisk): RegExp {
  const popup = "\\*(Original Category Name: .*)?";
  const suffix = { yes: popup, no: "", either: `(${popup})?` }[asterisk];
  return new RegExp(`^${escapeRegExp(name)}${suffix}$`);
}

function asteriskFor(originalName: string | undefined): Asterisk {
  return originalName === undefined ? "no" : "yes";
}

function oscarCategory(year: Locator, title: string, asterisk: Asterisk) {
  return year.getByTestId("oscar-category").filter({
    has: year.page().locator("h3.oscar-category-title", {
      hasText: oscarTitle(title, asterisk),
    }),
  });
}

// The element holding a subsection's winners and nominees: the subsection
// itself, or the category when it is undivided that year
function oscarSubsection(
  category: Locator,
  subsection: ExpectedOscarSubsection,
): Locator {
  if (subsection.title === null) return category;
  return category.getByTestId("oscar-category-subsection").filter({
    has: category.page().locator("h4.oscar-category-subsection-title", {
      hasText: oscarTitle(
        subsection.title,
        asteriskFor(subsection.originalName),
      ),
    }),
  });
}

async function checkOscarAsterisk(
  page: Page,
  container: Locator,
  originalName: string | undefined,
) {
  const asterisk = container
    .locator("> h3.oscar-category-title, > h4.oscar-category-subsection-title")
    .getByRole("button", { name: "original category name" });
  if (originalName === undefined) {
    await expect(asterisk).toHaveCount(0);
    return;
  }
  await asterisk.click();
  await expect(container.getByRole("tooltip")).toHaveText(
    `Original Category Name: ${originalName}`,
  );
  await page.keyboard.press("Escape");
  await expect(container.getByRole("tooltip")).toHaveCount(0);
}

async function checkOscarYear(page: Page, expected: ExpectedOscarYear) {
  console.log(`Checking the ${expected.heading.slice(0, 7)} section...`);
  const year = await openOscarYear(page, expected.ceremony);
  await expect(year.locator("> summary h2")).toHaveText(expected.heading);
  for (const expectedCategory of expected.categories) {
    const category = oscarCategory(
      year,
      expectedCategory.title,
      asteriskFor(expectedCategory.originalName),
    );
    await expect(category).toHaveCount(1);
    // Subsections only when the category was split that year
    const split = expectedCategory.subsections[0].title !== null;
    await expect(category.getByTestId("oscar-category-subsection")).toHaveCount(
      split ? expectedCategory.subsections.length : 0,
    );
    await checkOscarAsterisk(
      page,
      category,
      split ? undefined : expectedCategory.originalName,
    );
    for (const expectedSubsection of expectedCategory.subsections) {
      const subsection = oscarSubsection(category, expectedSubsection);
      await expect(subsection).toHaveCount(1);
      if (split) {
        await checkOscarAsterisk(
          page,
          subsection,
          expectedSubsection.originalName,
        );
      }
      const winners = subsection.getByTestId("oscar-winners").locator("> li");
      await expect(winners).toHaveCount(expectedSubsection.winners.length);
      await expect(winners).toContainText(expectedSubsection.winners);
      const nominees = subsection.locator("details.collapsible > summary");
      if (expectedSubsection.nominees === null) {
        await expect(nominees).toHaveCount(0);
      } else {
        await expect(nominees).toHaveText(
          `Nominees(${String(expectedSubsection.nominees)})`,
        );
      }
    }
  }
  if (expected.specialAwards !== undefined) {
    const specialAwards = year
      .getByTestId("oscar-special-awards")
      .locator("li");
    await expect(specialAwards).toHaveCount(expected.specialAwards.length);
    for (const [index, substrings] of expected.specialAwards.entries()) {
      for (const substring of substrings) {
        await expect(specialAwards.nth(index)).toContainText(substring);
      }
    }
  }
  await year.locator("> summary").click();
  await expect(year).not.toHaveAttribute("open", "");
}

test("Oscars By Year spot checks: open the page from the footer, check the most recent year, then the winners, nominees, subsections, asterisks, and special awards of a few years", async ({
  page,
}) => {
  test.slow(); // Many small checks over several years
  console.log("Starting Oscars By Year spot checks test...");
  await openOscarsPage(page);

  console.log("Checking the most recent year...");
  const years = page.locator("details.oscar-year");
  const yearCount = await years.count();
  expect(yearCount).toBeGreaterThanOrEqual(expectedOscars.years.length);
  const latestHeading = (await years.first().locator("h2").textContent()) ?? "";
  const latestMatch = /^(\d{4})(\d+)(?:st|nd|rd|th) Academy Awards$/.exec(
    latestHeading,
  );
  expect(latestMatch).not.toBeNull();
  const [, latestYear, latestCeremony] = latestMatch ?? [];
  expect(Number(latestYear)).toBeGreaterThanOrEqual(
    expectedOscars.latest.minYear,
  );
  expect(Number(latestCeremony)).toBeGreaterThanOrEqual(
    expectedOscars.latest.minCeremony,
  );
  if (expectedOscars.latest.everyCeremony) {
    expect(Number(latestCeremony)).toBe(yearCount);
  }
  // The first ceremony is last, and "Jump To" lists every year in order
  await expect(years.last().locator("h2")).toHaveText(
    "1927/281st Academy Awards",
  );
  const jumpToOptions = page
    .getByRole("combobox", { name: "jump-to-year" })
    .locator("option:not([disabled])");
  await expect(jumpToOptions).toHaveCount(yearCount);
  await expect(jumpToOptions.first()).toHaveText(latestYear);
  await expect(jumpToOptions.last()).toHaveText("1927/28");
  // The most recent year has a single Best Picture winner, first of all
  const latest = await openOscarYear(page, Number(latestCeremony));
  const latestCategories = latest.getByTestId("oscar-category");
  await expect(latestCategories.first().locator("h3")).toHaveText(
    "Best Picture",
  );
  await expect(
    latestCategories.first().getByTestId("oscar-winners").locator("> li"),
  ).toHaveCount(1);
  await expect(
    latestCategories.first().locator("details.collapsible > summary"),
  ).toHaveText(/^Nominees\(\d+\)$/);
  await latest.locator("> summary").click();

  for (const expectedYear of expectedOscars.years) {
    await checkOscarYear(page, expectedYear);
  }
  console.log("Oscars By Year spot checks test finished.");
});

test("Oscars By Year interactions: expand and collapse all years, jump to a year, read a note, follow a link to a movie panel, and come back", async ({
  page,
}) => {
  console.log("Starting Oscars By Year interactions test...");
  await openOscarsPage(page);
  const years = page.locator("details.oscar-year");
  const yearCount = await years.count();

  console.log("Expanding and collapsing all years...");
  await page.getByRole("button", { name: "Expand All" }).click();
  await expect(page.locator("details.oscar-year[open]")).toHaveCount(yearCount);
  await expect(
    years.last().getByTestId("oscar-category").first(),
  ).toBeVisible();
  await page.getByRole("button", { name: "Collapse All" }).click();
  await expect(page.locator("details.oscar-year[open]")).toHaveCount(0);

  console.log("Jumping to a year...");
  await page
    .getByRole("combobox", { name: "jump-to-year" })
    .selectOption(String(expectedOscars.jumpTo));
  const jumpedTo = oscarYear(page, expectedOscars.jumpTo);
  await expect(jumpedTo).toHaveAttribute("open", "");
  await expect(jumpedTo.locator("> summary")).toBeInViewport();
  await expect(page.locator("details.oscar-year[open]")).toHaveCount(1);
  await jumpedTo.locator("> summary").click();

  console.log("Reading a note...");
  const { note } = expectedOscars;
  const noteYear = await openOscarYear(page, note.ceremony);
  const noteCategory = oscarCategory(noteYear, note.category, "either");
  let noteContainer =
    note.subsection === null
      ? noteCategory
      : noteCategory.getByTestId("oscar-category-subsection").filter({
          has: page.locator("h4", {
            hasText: oscarTitle(note.subsection, "either"),
          }),
        });
  if (note.inNominees) {
    await noteContainer.locator("details.collapsible > summary").click();
    noteContainer = noteContainer.getByTestId("oscar-nominees");
  } else {
    noteContainer = noteContainer.getByTestId("oscar-winners");
  }
  const noteLine = noteContainer.locator("> li").filter({ hasText: note.line });
  await expect(noteLine).toHaveCount(1);
  await noteLine.getByRole("button", { name: "note" }).click();
  await expect(noteLine.getByRole("tooltip")).toContainText(note.text);
  await page.keyboard.press("Escape");
  await expect(noteLine.getByRole("tooltip")).toHaveCount(0);
  await noteYear.locator("> summary").click();

  console.log("Checking what links and what doesn't...");
  const { plainText } = expectedOscars;
  const plainTextYear = await openOscarYear(page, plainText.ceremony);
  const plainTextWinners = oscarCategory(
    plainTextYear,
    plainText.category,
    "either",
  ).getByTestId("oscar-winners");
  await expect(plainTextWinners).toContainText(plainText.text);
  await expect(
    plainTextWinners.getByRole("link", { name: plainText.text }),
  ).toHaveCount(0);
  await plainTextYear.locator("> summary").click();
  const { personLink, movieLink } = expectedOscars;
  const linkYear = await openOscarYear(page, personLink.ceremony);
  await expect(
    oscarCategory(linkYear, personLink.category, "either")
      .getByTestId("oscar-winners")
      .getByRole("link", { name: personLink.name, exact: true }),
  ).toHaveAttribute("href", `/person/${personLink.imdbId}`);
  if (movieLink.ceremony !== personLink.ceremony) {
    await linkYear.locator("> summary").click();
    await openOscarYear(page, movieLink.ceremony);
  }
  const movieLinkElement = oscarCategory(
    oscarYear(page, movieLink.ceremony),
    movieLink.category,
    "either",
  )
    .getByTestId("oscar-winners")
    .getByRole("link", { name: movieLink.title, exact: true });
  await expect(movieLinkElement).toHaveAttribute(
    "href",
    `/movie/${movieLink.imdbId}`,
  );

  console.log("Following the movie link and coming back...");
  await movieLinkElement.click();
  await expect(page).toHaveURL(new RegExp(`/movie/${movieLink.imdbId}$`));
  const moviePanel = page.getByTestId("movie-panel");
  await expect(moviePanel).toBeVisible();
  await expect(
    moviePanel.getByRole("heading", { name: movieLink.title, exact: true }),
  ).toBeVisible();
  await page.goBack();
  await expect(page).toHaveURL(/\/oscars$/);
  await expect(page.getByTestId("oscars-page")).toBeVisible();
  console.log("Oscars By Year interactions test finished.");
});

function escapeRegExp(text: string): string {
  return text.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}
