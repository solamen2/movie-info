import { type ReactNode, useEffect, useState } from "react";
import Collapsible from "../shared/Collapsible";
import HorizontalList from "../shared/HorizontalList";
import ImdbRow, { ImdbRowSeparator } from "../shared/ImdbRow";
import { displayDate, displayText } from "../utilities/utilities";
import CastCard from "./CastCard";
import CrewCard from "./CrewCard";
import WatchProviderCard from "./WatchProviderCard";
import {
  type Movie,
  type MovieCrew,
  type WatchProvider,
  imdbTitleUrl,
} from "./movieTypes";
import "../shared/shared.css";
import "./movie.css";

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

  if (error) {
    return (
      <div className="detail-panel">
        <p className="error-message">{error}</p>
      </div>
    );
  }

  if (!movie) {
    return (
      <div className="detail-panel">
        <p className="detail-loading">Loading movie details…</p>
      </div>
    );
  }

  const imdbUrl = imdbTitleUrl(movie.imdbId);
  const imdbRating = displayText(movie.imdbRating);
  const imdbVotes = displayText(movie.imdbVotes);
  const sortedCast = [...movie.cast].sort(
    (a, b) => a.billedOrder - b.billedOrder,
  );

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
    <div className="detail-panel" data-testid="movie-panel">
      <div className="detail-panel-top">
        {movie.image ? (
          <img
            src={movie.image.imageURL}
            alt={movie.title}
            className="detail-poster"
          />
        ) : (
          <div className="detail-poster card-image-placeholder">No image</div>
        )}
        <div className="detail-panel-heading">
          <h2 className="detail-title">{movie.title}</h2>
          {movie.tagline && <p className="movie-tagline">{movie.tagline}</p>}
          <ImdbRow imdbUrl={imdbUrl}>
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
          </ImdbRow>
          <dl className="detail-facts">
            {facts.map(([label, value]) => (
              <div className="detail-fact" key={label}>
                <dt>{label}</dt>
                <dd>{value}</dd>
              </div>
            ))}
          </dl>
        </div>
      </div>

      <div className="detail-sections">
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
          <p className="detail-prose">{displayText(movie.tmdbPlot)}</p>
        </Collapsible>
        <Collapsible title="Plot (OMDB)">
          <p className="detail-prose">{displayText(movie.omdbPlot)}</p>
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
