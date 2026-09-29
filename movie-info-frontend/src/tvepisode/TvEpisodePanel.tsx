import { type ReactNode, useEffect, useState } from "react";
import { imdbTitleUrl } from "../movie/movieTypes";
import Collapsible from "../shared/Collapsible";
import HorizontalList from "../shared/HorizontalList";
import ImdbRow, { ImdbRowSeparator } from "../shared/ImdbRow";
import {
  displayEpisodeType,
  displaySeasonEpisode,
} from "../tvseason/tvSeasonTypes";
import {
  describeFailedLoad,
  displayDate,
  displayGenres,
  displayRuntime,
  displayText,
  getTmdbImageUrl,
} from "../utilities/utilities";
import CastCard from "./CastCard";
import CrewCard from "./CrewCard";
import GuestStarCard from "./GuestStarCard";
import { type TvEpisode, type TvEpisodeCrew } from "./tvEpisodeTypes";
import "../shared/shared.css";

function CrewSection({
  title,
  crew,
}: {
  title: string;
  crew: TvEpisodeCrew[];
}) {
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

interface TvEpisodePanelProps {
  tmdbTvSeriesId: number;
  seasonNumber: number;
  episodeNumber: number;
  // For the link to the series' own IMDB page
  tvSeriesImdbId: string;
}

// Shows one episode of a TV series, loaded from /api/tvepisode with the same
// query parameters that select it in the URL.
function TvEpisodePanel({
  tmdbTvSeriesId,
  seasonNumber,
  episodeNumber,
  tvSeriesImdbId,
}: TvEpisodePanelProps) {
  const [tvEpisode, setTvEpisode] = useState<TvEpisode | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    const controller = new AbortController();

    async function loadTvEpisode() {
      try {
        const params = new URLSearchParams({
          tmdbTvSeriesId: String(tmdbTvSeriesId),
          seasonNumber: String(seasonNumber),
          episodeNumber: String(episodeNumber),
        });
        const response = await fetch(`/api/tvepisode?${params.toString()}`, {
          signal: controller.signal,
        });
        if (!response.ok) {
          setError(await describeFailedLoad("Loading TV episode", response));
          return;
        }
        const data = (await response.json()) as TvEpisode | null; // TODO: Maybe someday make this validation more robust
        if (data == null) {
          setError("No TV episode details were found.");
          return;
        }
        setTvEpisode(data);
      } catch {
        if (!controller.signal.aborted) {
          setError(
            "An unexpected error occurred while loading the TV episode.",
          );
        }
      }
    }

    void loadTvEpisode();
    return () => {
      controller.abort();
    };
  }, [tmdbTvSeriesId, seasonNumber, episodeNumber]);

  if (error) {
    return (
      <div className="detail-panel">
        <p className="error-message">{error}</p>
      </div>
    );
  }

  if (!tvEpisode) {
    return (
      <div className="detail-panel">
        <p className="detail-loading">Loading TV episode details…</p>
      </div>
    );
  }

  const stillUrl = getTmdbImageUrl(tvEpisode.stillPath, "w300");
  const imdbRating = displayText(tvEpisode.imdbRating);
  const imdbVotes = displayText(tvEpisode.imdbVotes);
  const sortedCast = [...tvEpisode.cast].sort(
    (a, b) => a.billedOrder - b.billedOrder,
  );
  const sortedGuestStars = [...tvEpisode.guestStars].sort(
    (a, b) => a.billedOrder - b.billedOrder,
  );

  const facts: [string, ReactNode][] = [
    ["Air Date", displayDate(tvEpisode.airDate)],
    ["Year", tvEpisode.year > 0 ? String(tvEpisode.year) : "—"],
    [
      "Runtime",
      <>
        {displayRuntime(tvEpisode.omdbAverageEpisodeRuntimeNumber)}
        <ImdbRowSeparator />
        {displayRuntime(tvEpisode.runtime)}
      </>,
    ],
    ["Rated", displayText(tvEpisode.rated)],
    ["Type", displayEpisodeType(tvEpisode.episodeType)],
    ["Known For", displayText(tvEpisode.knownForActors)],
    ["Genres", displayGenres("", tvEpisode.omdbGenres)],
  ];

  return (
    <div className="detail-panel" data-testid="tv-episode-panel">
      <div className="detail-panel-top">
        {stillUrl ? (
          <img
            src={stillUrl}
            alt={tvEpisode.title}
            className="detail-poster detail-still"
          />
        ) : (
          <div className="detail-poster detail-still card-image-placeholder">
            No image
          </div>
        )}
        <div className="detail-panel-heading">
          <h2 className="detail-title">{tvEpisode.title}</h2>
          <p className="detail-subtitle">
            {displaySeasonEpisode(
              tvEpisode.seasonNumber,
              tvEpisode.episodeNumber,
            )}
          </p>
          <ImdbRow imdbUrl={imdbTitleUrl(tvEpisode.imdbId)}>
            <span className="imdb-row-label">IMDB:</span> {imdbRating}
            {imdbRating !== "—" && imdbVotes !== "—" && (
              <>
                {" "}
                <span className="imdb-row-votes">({imdbVotes})</span>
              </>
            )}
          </ImdbRow>
          <ImdbRow
            imdbUrl={imdbTitleUrl(tvSeriesImdbId)}
            label="TV Series IMDB:"
            copyButton={false}
          />
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
        <Collapsible title="Cast" count={tvEpisode.cast.length}>
          <HorizontalList>
            {sortedCast.map((c) => (
              <CastCard key={c.id} cast={c} />
            ))}
          </HorizontalList>
        </Collapsible>
        <CrewSection title="Directors" crew={tvEpisode.directors} />
        <CrewSection title="Writers" crew={tvEpisode.writers} />
        <Collapsible title="Guest Stars" count={tvEpisode.guestStars.length}>
          <HorizontalList>
            {sortedGuestStars.map((g) => (
              <GuestStarCard key={g.id} guestStar={g} />
            ))}
          </HorizontalList>
        </Collapsible>
        <Collapsible title="Overview (OMDB)">
          <p className="detail-prose">{displayText(tvEpisode.omdbOverview)}</p>
        </Collapsible>
        <Collapsible title="Overview (TMDB)">
          <p className="detail-prose">{displayText(tvEpisode.tmdbOverview)}</p>
        </Collapsible>
      </div>
    </div>
  );
}

export default TvEpisodePanel;
