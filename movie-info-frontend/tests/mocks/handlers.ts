import { http, HttpResponse } from "msw";
import searchDataJson1 from "./data/searchData1.json" with { type: "json" };
import searchDataJson2 from "./data/searchData2.json" with { type: "json" };
import movieDataJson1 from "./data/movieData1.json" with { type: "json" };
import personDataJson1 from "./data/personData1.json" with { type: "json" };
import tvSeriesDataJson1 from "./data/tvSeriesData1.json" with { type: "json" };
import tvSeasonDataJson1 from "./data/tvSeasonData1.json" with { type: "json" };
import tvEpisodeDataJson1 from "./data/tvEpisodeData1.json" with { type: "json" };

type LoginPathParams = object;

interface LoginRequestBody {
  email: string;
  password: string;
}

type LoginResponseBody = object;

type RegisterPathParams = object;

interface RegisterRequestBody {
  email: string;
  password: string;
}

// A tiny poster-shaped image served for every mock image URL, so pages render
// real (not broken) images in mock mode. Browsers size a broken image
// differently from a loaded one, which shifts the layout under the user (and
// under the end-to-end tests) when the failed request comes back.
const PLACEHOLDER_IMAGE_SVG = `<svg xmlns="http://www.w3.org/2000/svg" width="200" height="300" viewBox="0 0 200 300"><rect width="200" height="300" fill="#556"/><text x="100" y="160" fill="#ccd" font-family="sans-serif" font-size="28" text-anchor="middle">mock</text></svg>`;

function placeholderImage() {
  return new HttpResponse(PLACEHOLDER_IMAGE_SVG, {
    status: 200,
    headers: { "Content-Type": "image/svg+xml" },
  });
}

// The people shown as cards in the movie / TV series / TV episode fixtures,
// and the movies and TV series shown as credit cards in the person fixture, by
// TMDB id (the only id those cards carry). Opening one of them serves the
// matching fixture under the clicked card's own name and TMDB id, so a panel
// opened from a card is recognizably that card's.
function namesByTmdbId(
  groups: { tmdbId: number; name: string }[][],
): Map<number, string> {
  return new Map(groups.flat().map((item) => [item.tmdbId, item.name]));
}

const peopleByTmdbId = namesByTmdbId([
  movieDataJson1.cast,
  movieDataJson1.directors,
  movieDataJson1.writers,
  tvSeriesDataJson1.cast,
  tvSeriesDataJson1.creators,
  tvSeriesDataJson1.directors,
  tvSeriesDataJson1.writers,
  tvEpisodeDataJson1.cast,
  tvEpisodeDataJson1.directors,
  tvEpisodeDataJson1.writers,
  tvEpisodeDataJson1.guestStars,
]);
const movieTitlesByTmdbId = namesByTmdbId(
  [personDataJson1.movieCastCredits, personDataJson1.movieCrewCredits].map(
    (credits) => credits.map((c) => ({ tmdbId: c.tmdbId, name: c.title })),
  ),
);
const tvSeriesNamesByTmdbId = namesByTmdbId([
  personDataJson1.tvSeriesCastCredits,
  personDataJson1.tvSeriesCrewCredits,
]);

function invalidIdResponse(tmdbId: string | null) {
  return HttpResponse.json(
    {
      error:
        tmdbId === null
          ? "Not a valid IMDB ID for mock"
          : "Not a valid TMDB ID for mock",
    },
    { status: 404 },
  );
}

export const handlers = [
  http.get("https://image.tmdb.org/t/p/*", placeholderImage),
  http.get("https://example.com/*", placeholderImage),

  http.post<LoginPathParams, LoginRequestBody, LoginResponseBody>(
    "/api/login",
    async ({ request }) => {
      const url = new URL(request.url);
      const useCookies = url.searchParams.get("useCookies");

      if (useCookies === "true") {
        const { email, password } = await request.clone().json();
        if (email === "error" && password === "error") {
          return HttpResponse.json({}, { status: 400 });
        }

        return HttpResponse.json({}, { status: 200 });
      }

      return new HttpResponse(null, { status: 404 }); // TODO: Maybe make this more of an error in future if needed
    },
  ),

  http.post<RegisterPathParams, RegisterRequestBody>(
    "/api/register",
    async ({ request }) => {
      const { email, password } = await request.clone().json();
      if (email === "error" && password === "error") {
        return HttpResponse.json(
          {
            errors: {
              InvalidEmail: ["Email 'error' is invalid."],
            },
          },
          { status: 400 },
        );
      }

      return HttpResponse.json({}, { status: 200 });
    },
  ),

  http.post("/api/logout", () => {
    return new HttpResponse(null, { status: 200 });
  }),

  http.get("/api/search", ({ request }) => {
    const url = new URL(request.url);
    const searchQuery = url.searchParams.get("searchQuery");

    switch (searchQuery) {
      case "1":
        return HttpResponse.json(searchDataJson1, { status: 200 });
      case "2":
        return HttpResponse.json(searchDataJson2, { status: 200 });
      case "empty":
        return HttpResponse.json([], { status: 200 });
      default:
        return HttpResponse.json(
          { message: "Not a valid search query for mock" },
          { status: 404 },
        );
    }
  }),

  http.get("/api/movie", ({ request }) => {
    const url = new URL(request.url);
    const imdbId = url.searchParams.get("imdbId");
    const tmdbId = url.searchParams.get("tmdbId");

    if (
      imdbId === movieDataJson1.imdbId ||
      tmdbId === String(movieDataJson1.tmdbId)
    ) {
      return HttpResponse.json(movieDataJson1, { status: 200 });
    }
    const title =
      tmdbId === null ? undefined : movieTitlesByTmdbId.get(Number(tmdbId));
    if (title !== undefined) {
      return HttpResponse.json(
        { ...movieDataJson1, tmdbId: Number(tmdbId), title },
        { status: 200 },
      );
    }
    return invalidIdResponse(tmdbId);
  }),

  http.get("/api/tvseries", ({ request }) => {
    const url = new URL(request.url);
    const imdbId = url.searchParams.get("imdbId");
    const tmdbId = url.searchParams.get("tmdbId");

    if (
      imdbId === tvSeriesDataJson1.imdbId ||
      tmdbId === String(tvSeriesDataJson1.tmdbId)
    ) {
      return HttpResponse.json(tvSeriesDataJson1, { status: 200 });
    }
    const name =
      tmdbId === null ? undefined : tvSeriesNamesByTmdbId.get(Number(tmdbId));
    if (name !== undefined) {
      return HttpResponse.json(
        { ...tvSeriesDataJson1, tmdbId: Number(tmdbId), name },
        { status: 200 },
      );
    }
    return invalidIdResponse(tmdbId);
  }),

  http.get("/api/tvseason", ({ request }) => {
    const url = new URL(request.url);
    const tmdbTvSeriesId = url.searchParams.get("tmdbTvSeriesId");
    const seasonNumber = url.searchParams.get("seasonNumber");

    if (
      tmdbTvSeriesId === String(tvSeriesDataJson1.tmdbId) &&
      seasonNumber === String(tvSeasonDataJson1.seasonNumber)
    ) {
      return HttpResponse.json(tvSeasonDataJson1, { status: 200 });
    }
    return HttpResponse.json(
      { error: "Not a valid TV season for mock" },
      { status: 404 },
    );
  }),

  http.get("/api/tvepisode", ({ request }) => {
    const url = new URL(request.url);
    const tmdbTvSeriesId = url.searchParams.get("tmdbTvSeriesId");
    const seasonNumber = url.searchParams.get("seasonNumber");
    const episodeNumber = url.searchParams.get("episodeNumber");

    if (
      tmdbTvSeriesId === String(tvSeriesDataJson1.tmdbId) &&
      seasonNumber === String(tvEpisodeDataJson1.seasonNumber) &&
      episodeNumber === String(tvEpisodeDataJson1.episodeNumber)
    ) {
      return HttpResponse.json(tvEpisodeDataJson1, { status: 200 });
    }
    return HttpResponse.json(
      { error: "Not a valid TV episode for mock" },
      { status: 404 },
    );
  }),

  http.get("/api/person", ({ request }) => {
    const url = new URL(request.url);
    const imdbId = url.searchParams.get("imdbId");
    const tmdbId = url.searchParams.get("tmdbId");

    if (
      imdbId === personDataJson1.imdbId ||
      tmdbId === String(personDataJson1.tmdbId)
    ) {
      return HttpResponse.json(personDataJson1, { status: 200 });
    }
    const name =
      tmdbId === null ? undefined : peopleByTmdbId.get(Number(tmdbId));
    if (name !== undefined) {
      return HttpResponse.json(
        { ...personDataJson1, tmdbId: Number(tmdbId), name },
        { status: 200 },
      );
    }
    return invalidIdResponse(tmdbId);
  }),
];
