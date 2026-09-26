import { expect } from "@playwright/test";
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

// Real data is limited to values that are unlikely to change over time (so no
// rating, vote count, rank, revenue, or watch providers).
const expectedMovie = useMockHttpCalls
  ? {
      searchText: "1",
      title: "Example Movie",
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
        "StatusReleased",
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
        "StatusReleased",
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
  await movieCard.click();
  const backButton = page.getByRole("button", { name: "back" });
  await expect(backButton).toBeVisible();
  await expect(resultsMessage).toBeHidden();

  const moviePanel = page.getByTestId("movie-panel");
  await expect(moviePanel).toBeVisible();
  const selectedCard = page.locator("#search-card.selected.expanded");
  await expect(selectedCard).toHaveCount(1);
  await expect(
    moviePanel.getByRole("heading", { name: expectedMovie.title, exact: true }),
  ).toBeVisible();
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
  await backButton.click();
  await expect(moviePanel).toBeHidden();
  await expect(page.locator("#search-card.selected")).toHaveCount(0);
  await expect(backButton).toBeHidden();
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
  await personCard.click();
  const backButton = page.getByRole("button", { name: "back" });
  await expect(backButton).toBeVisible();
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

  console.log("Unselecting the person search card...");
  await page.keyboard.press("Escape");
  await expect(personPanel).toBeHidden();
  await expect(page.locator("#search-card.selected")).toHaveCount(0);
  await expect(backButton).toBeHidden();
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
        "StatusEnded",
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
  await tvSeriesCard.click();
  const backButton = page.getByRole("button", { name: "back" });
  await expect(backButton).toBeVisible();
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
  await backButton.click();
  await expect(tvSeriesPanel).toBeHidden();
  await expect(page.locator("#search-card.selected")).toHaveCount(0);
  await expect(backButton).toBeHidden();
  await expect(resultsMessage).toBeVisible();
  for (const cardText of expectedTvSeries.cardText) {
    expect(await tvSeriesCard.textContent()).toContain(cardText);
  }
  console.log("TV series happy path test finished.");
});

function escapeRegExp(text: string): string {
  return text.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}
