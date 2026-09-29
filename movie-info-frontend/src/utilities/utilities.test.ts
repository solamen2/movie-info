import { describe, expect, it } from "vitest";
import {
  describeFailedLoad,
  describeFailedResponse,
  displayAge,
  getAge,
  getErrorResponseMessage,
} from "./utilities";

function response(body: string | null, status = 404) {
  return new Response(body, { status });
}

describe("getErrorResponseMessage", () => {
  it("Should return a JSON string body as-is (what Results.NotFound sends)", async () => {
    expect(
      await getErrorResponseMessage(
        response("\"Person for search 'nm1' was not found in the TMDB.\""),
      ),
    ).toBe("Person for search 'nm1' was not found in the TMDB.");
  });

  it.each([
    ['{"message":"Bad query"}', "Bad query"],
    [
      '{"error":"Not a valid IMDB ID for mock"}',
      "Not a valid IMDB ID for mock",
    ],
    ['{"title":"Not Found","status":404}', "Not Found"],
    ['{"detail":"Missing id"}', "Missing id"],
  ])(
    "Should pick the message out of the JSON object %s",
    async (body, expected) => {
      expect(await getErrorResponseMessage(response(body))).toBe(expected);
    },
  );

  it("Should return plain text bodies", async () => {
    expect(await getErrorResponseMessage(response("Service unavailable"))).toBe(
      "Service unavailable",
    );
  });

  it.each([
    ["empty", ""],
    ["whitespace", "   "],
    ["JSON without a message", '{"status":404}'],
    ["an empty JSON string", '""'],
  ])("Should return null for %s", async (_, body) => {
    expect(await getErrorResponseMessage(response(body))).toBeNull();
  });
});

describe("describeFailedResponse", () => {
  it("Should include the message and end with a single period", async () => {
    expect(
      await describeFailedResponse(
        "Loading person",
        response('"Person was not found."'),
      ),
    ).toBe("Loading person failed with status 404: Person was not found.");
    expect(
      await describeFailedResponse(
        "Search",
        response('{"message":"Not a valid search query"}'),
      ),
    ).toBe("Search failed with status 404: Not a valid search query.");
  });

  it("Should fall back to the status alone", async () => {
    expect(
      await describeFailedResponse("Loading movie", response(null, 500)),
    ).toBe("Loading movie failed with status 500.");
  });
});

describe("describeFailedLoad", () => {
  it("Should suggest another search when the response is a 404", async () => {
    expect(
      await describeFailedLoad(
        "Loading movie",
        response('{"message":"Movie was not found."}', 404),
      ),
    ).toBe(
      "Loading movie failed with status 404: Movie was not found. Please try another search.",
    );
  });

  it("Should suggest trying again for any other failed response", async () => {
    expect(await describeFailedLoad("Loading movie", response(null, 500))).toBe(
      "Loading movie failed with status 500. Please try again.",
    );
  });
});

describe("getAge", () => {
  // Local time, like the user's clock
  const today = new Date(2026, 8, 28);

  it.each([
    ["the birthday is still to come this year", "1977-12-25", 48],
    ["the birthday is tomorrow", "1977-09-29", 48],
    ["the birthday is today", "1977-09-28", 49],
    ["the birthday has passed this year", "1977-04-14", 49],
    ["the person is less than a year old", "2025-09-29", 0],
    ["the person was born today", "2026-09-28", 0],
  ])("Should round down to whole years when %s", (_, birthday, expected) => {
    expect(getAge(birthday, null, today)).toBe(expected);
  });

  it("Should use the local date, not the UTC date", () => {
    const lateEvening = new Date(2026, 8, 28, 23, 59, 59);
    const earlyMorning = new Date(2026, 8, 28, 0, 0, 0);
    expect(getAge("1977-09-28", null, lateEvening)).toBe(49);
    expect(getAge("1977-09-28", null, earlyMorning)).toBe(49);
    expect(getAge("1977-09-29", null, lateEvening)).toBe(48);
  });

  it("Should count a Feb 29 birthday from Mar 1 in years without one", () => {
    expect(getAge("2000-02-29", null, new Date(2026, 1, 28))).toBe(25);
    expect(getAge("2000-02-29", null, new Date(2026, 2, 1))).toBe(26);
    expect(getAge("2000-02-29", null, new Date(2028, 1, 29))).toBe(28);
  });

  it("Should use the age at death rather than today's age", () => {
    expect(getAge("1977-04-14", "2020-01-02", today)).toBe(42);
    expect(getAge("1977-04-14", "2020-04-14", today)).toBe(43);
    expect(getAge("2019-06-01", "2020-01-02", today)).toBe(0);
  });

  it.each([
    ["there is no birthday", null, null],
    ["there is no birthday but there is a deathday", null, "2020-01-02"],
    ["the birthday can't be read", "1977", null],
    ["the deathday can't be read", "1977-04-14", "2020"],
    ["the birthday is in the future", "2030-01-01", null],
    ["the deathday is before the birthday", "1977-04-14", "1970-01-01"],
  ])("Should return null when %s", (_, birthday, deathday) => {
    expect(getAge(birthday, deathday, today)).toBeNull();
  });
});

describe("displayAge", () => {
  it("Should show the age in parentheses", () => {
    expect(displayAge(49)).toBe("(49)");
    expect(displayAge(1)).toBe("(1)");
  });

  it("Should show '(<1)' for someone less than a year old", () => {
    expect(displayAge(0)).toBe("(<1)");
  });
});
