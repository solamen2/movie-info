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
        ["Plot (TMDB)", "An example plot from TMDB."],
        ["Plot (OMDB)", "An example plot from OMDB."],
        ["Where to Stream", "Example Stream"],
        ["Where to Rent", "None"],
        ["Where to Buy", "Example Buy Store"],
      ],
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
        ["Plot (TMDB)", "Shawshank"],
        ["Plot (OMDB)", "Shawshank"],
        ["Where to Stream", ""],
        ["Where to Rent", ""],
        ["Where to Buy", ""],
      ],
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
  const imdbLink = moviePanel.getByRole("link", { name: "Link", exact: true });
  await expect(imdbLink).toHaveAttribute("href", expectedMovie.imdbUrl);
  await expect(imdbLink).toHaveAttribute("target", "_blank");
  await expect(
    moviePanel.getByRole("button", { name: "copy-imdb-link" }),
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
      facts: [
        "Known For DepartmentActing",
        "BirthdayApr 14, 1977",
        "Deathday—",
        "Place of BirthNew York City, New York, USA",
        "GenderFemale",
      ],
      sections: [
        ["Biography", "Gellar"],
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
  const imdbLink = personPanel.getByRole("link", { name: "Link", exact: true });
  await expect(imdbLink).toHaveAttribute("href", expectedPerson.imdbUrl);
  await expect(imdbLink).toHaveAttribute("target", "_blank");
  await expect(
    personPanel.getByRole("button", { name: "copy-imdb-link" }),
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
      ],
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
        ["Overview (OMDB)", "vampire"],
        ["Networks", "The WB"],
        ["Where to Stream", ""],
        ["Where to Rent", ""],
        ["Where to Buy", ""],
      ],
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
  const imdbLink = tvSeriesPanel.getByRole("link", {
    name: "Link",
    exact: true,
  });
  await expect(imdbLink).toHaveAttribute("href", expectedTvSeries.imdbUrl);
  await expect(imdbLink).toHaveAttribute("target", "_blank");
  await expect(
    tvSeriesPanel.getByRole("button", { name: "copy-imdb-link" }),
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
          "Year2001",
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
          "Year2001",
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

function escapeRegExp(text: string): string {
  return text.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}
