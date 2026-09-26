import { type ReactNode, useEffect, useState } from "react";
import { imdbTitleUrl } from "../movie/movieTypes";
import Collapsible from "../shared/Collapsible";
import HorizontalList from "../shared/HorizontalList";
import ImdbRow, { ImdbRowSeparator } from "../shared/ImdbRow";
import WatchProviderSection from "../shared/WatchProviderSection";
import {
  displayDate,
  displayGenres,
  displayRuntime,
  displayText,
} from "../utilities/utilities";
import CastCard from "./CastCard";
import CreatorCard from "./CreatorCard";
import CrewCard from "./CrewCard";
import NetworkCard from "./NetworkCard";
import SeasonCard from "./SeasonCard";
import { type TvSeries, type TvSeriesCrew, sortSeasons } from "./tvSeriesTypes";
import "../shared/shared.css";
import "./tvseries.css";

// TMDB lists every distinct episode runtime a series has had
function displayEpisodeRunTimes(minutes: number[]): string {
  const runtimes = minutes.filter((m) => m > 0).map(displayRuntime);
  return runtimes.length > 0 ? runtimes.join(", ") : "—";
}

function CrewSection({ title, crew }: { title: string; crew: TvSeriesCrew[] }) {
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

interface TvSeriesPanelProps {
  imdbId: string;
}

function TvSeriesPanel({ imdbId }: TvSeriesPanelProps) {
  const [tvSeries, setTvSeries] = useState<TvSeries | null>(null);
  const [error, setError] = useState("");
  useEffect(() => {
    const controller = new AbortController();

    async function loadTvSeries() {
      try {
        const response = await fetch(
          `/api/tvseries?imdbId=${encodeURIComponent(imdbId)}`,
          { signal: controller.signal },
        );
        if (!response.ok) {
          setError(
            `Loading TV series failed with status ${String(response.status)}. Please try again.`,
          );
          return;
        }
        const data = (await response.json()) as TvSeries | null; // TODO: Maybe someday make this validation more robust
        if (data == null) {
          setError("No TV series details were found.");
          return;
        }
        setTvSeries(data);
      } catch {
        if (!controller.signal.aborted) {
          setError("An unexpected error occurred while loading the TV series.");
        }
      }
    }

    void loadTvSeries();
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

  if (!tvSeries) {
    return (
      <div className="detail-panel">
        <p className="detail-loading">Loading TV series details…</p>
      </div>
    );
  }

  const imdbUrl = imdbTitleUrl(tvSeries.imdbId);
  const imdbRating = displayText(tvSeries.imdbRating);
  const imdbVotes = displayText(tvSeries.imdbVotes);
  const sortedSeasons = sortSeasons(tvSeries.seasons);
  const sortedCast = [...tvSeries.cast].sort(
    (a, b) => a.billedOrder - b.billedOrder,
  );

  const facts: [string, ReactNode][] = [
    ["Original Name", displayText(tvSeries.originalName)],
    ["Years", displayText(tvSeries.years)],
    ["First Air Date", displayDate(tvSeries.firstAirDate)],
    ["Last Air Date", displayDate(tvSeries.lastAirDate)],
    ["Next Air Date", displayDate(tvSeries.nextAirDate)],
    [
      "Average Runtime",
      <>
        {displayRuntime(tvSeries.omdbAverageEpisodeRuntimeNumber)}
        <ImdbRowSeparator />
        {displayEpisodeRunTimes(tvSeries.tmdbEpisodeRunTimes)}
      </>,
    ],
    ["Rated", displayText(tvSeries.rated)],
    ["Status", displayText(tvSeries.status)],
    ["In Production", tvSeries.isInProduction ? "Yes" : "No"],
    ["Type", displayText(tvSeries.tvSeriesType)],
    ["Number of Seasons", displayText(tvSeries.numberOfSeasons)],
    ["Number of Episodes", displayText(tvSeries.numberOfEpisodes)],
    ["Known For", displayText(tvSeries.knownForActors)],
    ["Genres", displayGenres(tvSeries.tmdbGenres, tvSeries.omdbGenres)],
    [
      "Homepage",
      tvSeries.homepage ? (
        <a href={tvSeries.homepage} target="_blank" rel="noopener">
          {tvSeries.homepage}
        </a>
      ) : (
        "—"
      ),
    ],
    ["Origin Countries", displayText(tvSeries.originCountries)],
    ["Production Countries", displayText(tvSeries.productionCountries)],
    ["Origin Language", displayText(tvSeries.originLanguage)],
    ["Spoken Languages", displayText(tvSeries.spokenLanguages)],
  ];

  return (
    <div className="detail-panel" data-testid="tv-series-panel">
      <div className="detail-panel-top">
        {tvSeries.image ? (
          <img
            src={tvSeries.image.imageURL}
            alt={tvSeries.name}
            className="detail-poster"
          />
        ) : (
          <div className="detail-poster card-image-placeholder">No image</div>
        )}
        <div className="detail-panel-heading">
          <h2 className="detail-title">{tvSeries.name}</h2>
          {tvSeries.tagline && (
            <p className="detail-tagline">{tvSeries.tagline}</p>
          )}
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
            {displayText(tvSeries.imdbRank)}
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
        <Collapsible title="Seasons" count={tvSeries.seasons.length}>
          <HorizontalList>
            {sortedSeasons.map((s) => (
              <SeasonCard key={s.id} season={s} />
            ))}
          </HorizontalList>
        </Collapsible>
        <Collapsible title="Cast" count={tvSeries.cast.length}>
          <HorizontalList>
            {sortedCast.map((c) => (
              <CastCard key={c.id} cast={c} />
            ))}
          </HorizontalList>
        </Collapsible>
        <Collapsible title="Creators" count={tvSeries.creators.length}>
          <HorizontalList>
            {tvSeries.creators.map((c) => (
              <CreatorCard key={c.id} creator={c} />
            ))}
          </HorizontalList>
        </Collapsible>
        <CrewSection title="Directors" crew={tvSeries.directors} />
        <CrewSection title="Writers" crew={tvSeries.writers} />
        <Collapsible title="Overview (TMDB)">
          <p className="detail-prose">{displayText(tvSeries.tmdbOverview)}</p>
        </Collapsible>
        <Collapsible title="Overview (OMDB)">
          <p className="detail-prose">{displayText(tvSeries.omdbOverview)}</p>
        </Collapsible>
        <Collapsible title="Networks" count={tvSeries.networks.length}>
          <HorizontalList>
            {tvSeries.networks.map((n) => (
              <NetworkCard key={n.id} network={n} />
            ))}
          </HorizontalList>
        </Collapsible>
        <WatchProviderSection
          title="Where to Stream"
          providers={tvSeries.watchProvidersFlatrate}
        />
        <WatchProviderSection
          title="Where to Rent"
          providers={tvSeries.watchProvidersRent}
        />
        <WatchProviderSection
          title="Where to Buy"
          providers={tvSeries.watchProvidersBuy}
        />
      </div>
    </div>
  );
}

export default TvSeriesPanel;
