import { describe, expect, it } from "vitest";
import {
  describeFailedLoad,
  describeFailedResponse,
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
