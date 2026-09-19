import { type ReactNode, useEffect, useRef, useState } from "react";
import CastCard from "./CastCard";
import CrewCard from "./CrewCard";
import WatchProviderCard from "./WatchProviderCard";
import {
  type Movie,
  type MovieCrew,
  type WatchProvider,
  imdbTitleUrl,
} from "./movieTypes";
import "./movie.css";

const COPY_FEEDBACK_MS = 1500;

// OMDB uses "N/A" for missing values, and the backend substitutes empty
// strings when OMDB returns nothing at all.
function displayText(value: string | number | null | undefined): string {
  if (value == null) return "—";
  const text = String(value).trim();
  return text === "" || text === "N/A" ? "—" : text;
}

// OMDB genre names (lowercased) that differ from TMDB's name for the same genre.
const OMDB_GENRE_ALIASES: Record<string, string> = {
  "sci-fi": "Science Fiction",
};

// OMDB and TMDB each supply a comma-separated genre list; merge them,
// preferring TMDB's spelling of each genre.
function displayGenres(tmdbGenres: string, omdbGenres: string): string {
  const genres = new Map<string, string>();
  for (const genreList of [tmdbGenres, omdbGenres]) {
    if (displayText(genreList) === "—") continue;
    for (const genre of genreList.split(",")) {
      const trimmed = genre.trim();
      const name = OMDB_GENRE_ALIASES[trimmed.toLowerCase()] ?? trimmed;
      const key = name.toLowerCase();
      if (name !== "" && !genres.has(key)) {
        genres.set(key, name);
      }
    }
  }
  return displayText([...genres.values()].join(", "));
}

function displayMoney(value: number): string {
  return value > 0
    ? new Intl.NumberFormat("en-US", {
        style: "currency",
        currency: "USD",
        maximumFractionDigits: 0,
      }).format(value)
    : "—";
}

function displayRuntime(minutes: number): string {
  if (minutes <= 0) return "—";
  const hours = Math.floor(minutes / 60);
  const mins = minutes % 60;
  return hours > 0 ? `${String(hours)}h ${String(mins)}m` : `${String(mins)}m`;
}

function displayDate(isoDate: string): string {
  if (!/^\d{4}-\d{2}-\d{2}$/.test(isoDate)) return displayText(isoDate);
  return new Date(`${isoDate}T00:00:00Z`).toLocaleDateString("en-US", {
    timeZone: "UTC",
    dateStyle: "medium",
  });
}

function ImdbRowSeparator() {
  return (
    <span
      className="imdb-row-separator"
      role="separator"
      aria-orientation="vertical"
    />
  );
}

interface CollapsibleProps {
  title: string;
  count?: number;
  children: ReactNode;
}

function Collapsible({ title, count, children }: CollapsibleProps) {
  return (
    <details className="movie-collapsible">
      <summary>
        <span className="chevron" aria-hidden="true" />
        {title}
        {count != null && (
          <span className="movie-collapsible-count">({count})</span>
        )}
      </summary>
      <div className="movie-collapsible-body">{children}</div>
    </details>
  );
}

function HorizontalList({ children }: { children: ReactNode[] }) {
  if (children.length === 0) {
    return <p className="movie-empty">None</p>;
  }
  return <div className="horizontal-list">{children}</div>;
}

function CrewSection({ title, crew }: { title: string; crew: MovieCrew[] }) {
  return (
    <Collapsible title={title} count={crew.length}>
      <HorizontalList>
        {crew.map((c) => (
          <CrewCard key={c.id} crew={c} />
        ))}
      </HorizontalList>
    </Collapsible>
  );
}

function WatchProviderSection({
  title,
  providers,
}: {
  title: string;
  providers: WatchProvider[];
}) {
  const sorted = [...providers].sort(
    (a, b) => a.displayPriority - b.displayPriority,
  );
  return (
    <Collapsible title={title} count={providers.length}>
      <HorizontalList>
        {sorted.map((p) => (
          <WatchProviderCard key={p.id} provider={p} />
        ))}
      </HorizontalList>
    </Collapsible>
  );
}

interface MoviePanelProps {
  imdbId: string;
}

function MoviePanel({ imdbId }: MoviePanelProps) {
  const [movie, setMovie] = useState<Movie | null>(null);
  const [error, setError] = useState("");
  const [copyStatus, setCopyStatus] = useState<"idle" | "copied" | "failed">(
    "idle",
  );
  const copyTimeoutRef = useRef<number | undefined>(undefined);

  useEffect(() => {
    const controller = new AbortController();

    async function loadMovie() {
      try {
        const response = await fetch(
          `/api/movie?imdbId=${encodeURIComponent(imdbId)}`,
          { signal: controller.signal },
        );
        if (!response.ok) {
          setError(
            `Loading movie failed with status ${String(response.status)}. Please try again.`,
          );
          return;
        }
        const data = (await response.json()) as Movie | null; // TODO: Maybe someday make this validation more robust
        if (data == null) {
          setError("No movie details were found.");
          return;
        }
        setMovie(data);
      } catch {
        if (!controller.signal.aborted) {
          setError("An unexpected error occurred while loading the movie.");
        }
      }
    }

    void loadMovie();
    return () => {
      controller.abort();
    };
  }, [imdbId]);

  useEffect(() => {
    return () => {
      window.clearTimeout(copyTimeoutRef.current);
    };
  }, []);

  if (error) {
    return (
      <div className="movie-panel">
        <p className="error-message">{error}</p>
      </div>
    );
  }

  if (!movie) {
    return (
      <div className="movie-panel">
        <p className="movie-loading">Loading movie details…</p>
      </div>
    );
  }

  const imdbUrl = imdbTitleUrl(movie.imdbId);
  const imdbRating = displayText(movie.imdbRating);
  const imdbVotes = displayText(movie.imdbVotes);
  const sortedCast = [...movie.cast].sort(
    (a, b) => a.billedOrder - b.billedOrder,
  );

  async function handleCopy() {
    window.clearTimeout(copyTimeoutRef.current);
    try {
      await navigator.clipboard.writeText(`<a href="${imdbUrl}">Link</a>`);
      setCopyStatus("copied");
    } catch {
      setCopyStatus("failed");
    }
    copyTimeoutRef.current = window.setTimeout(() => {
      setCopyStatus("idle");
    }, COPY_FEEDBACK_MS);
  }

  const facts: [string, ReactNode][] = [
    ["Original Title", displayText(movie.originalTitle)],
    ["Release Date", displayDate(movie.releaseDate)],
    ["Runtime", displayRuntime(movie.runtime)],
    ["Rated", displayText(movie.rated)],
    ["Status", displayText(movie.status)],
    ["Known For", displayText(movie.knownForActors)],
    ["Genres", displayGenres(movie.tmdbGenres, movie.omdbGenres)],
    ["Budget", displayMoney(movie.budget)],
    ["Revenue", displayMoney(movie.revenue)],
    [
      "Homepage",
      movie.homepage ? (
        <a href={movie.homepage} target="_blank" rel="noopener">
          {movie.homepage}
        </a>
      ) : (
        "—"
      ),
    ],
    ["Origin Countries", displayText(movie.originCountries)],
    ["Production Countries", displayText(movie.productionCountries)],
    ["Origin Language", displayText(movie.originLanguage)],
    ["Spoken Languages", displayText(movie.spokenLanguages)],
  ];

  return (
    <div className="movie-panel" data-testid="movie-panel">
      <div className="movie-panel-top">
        {movie.image ? (
          <img
            src={movie.image.imageURL}
            alt={movie.title}
            className="movie-poster"
          />
        ) : (
          <div className="movie-poster card-image-placeholder">No image</div>
        )}
        <div className="movie-panel-heading">
          <h2 className="movie-title">{movie.title}</h2>
          {movie.tagline && <p className="movie-tagline">{movie.tagline}</p>}
          <p className="imdb-row">
            <span>
              <span className="imdb-row-label">IMDB:</span> {imdbRating}
              {imdbRating !== "—" && imdbVotes !== "—" && (
                <>
                  {" "}
                  <span className="imdb-row-votes">({imdbVotes})</span>
                </>
              )}
              <ImdbRowSeparator />
              <span className="imdb-row-label">Rank:</span>{" "}
              {displayText(movie.imdbRank)}
              <ImdbRowSeparator />
              <a href={imdbUrl} target="_blank" rel="noopener">
                Link
              </a>
            </span>
            <button
              type="button"
              aria-label="copy-imdb-link"
              className="copy-button"
              onClick={handleCopy}
            >
              {copyStatus === "copied"
                ? "Copied!"
                : copyStatus === "failed"
                  ? "Failed"
                  : "Copy"}
            </button>
          </p>
          <dl className="movie-facts">
            {facts.map(([label, value]) => (
              <div className="movie-fact" key={label}>
                <dt>{label}</dt>
                <dd>{value}</dd>
              </div>
            ))}
          </dl>
        </div>
      </div>

      <div className="movie-sections">
        <Collapsible title="Cast" count={movie.cast.length}>
          <HorizontalList>
            {sortedCast.map((c) => (
              <CastCard key={c.id} cast={c} />
            ))}
          </HorizontalList>
        </Collapsible>
        <CrewSection title="Directors" crew={movie.directors} />
        <CrewSection title="Writers" crew={movie.writers} />
        <Collapsible title="Plot (TMDB)">
          <p className="movie-prose">{displayText(movie.tmdbPlot)}</p>
        </Collapsible>
        <Collapsible title="Plot (OMDB)">
          <p className="movie-prose">{displayText(movie.omdbPlot)}</p>
        </Collapsible>
        <WatchProviderSection
          title="Where to Stream"
          providers={movie.watchProvidersFlatrate}
        />
        <WatchProviderSection
          title="Where to Rent"
          providers={movie.watchProvidersRent}
        />
        <WatchProviderSection
          title="Where to Buy"
          providers={movie.watchProvidersBuy}
        />
      </div>
    </div>
  );
}

export default MoviePanel;
