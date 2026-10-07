import { type ReactNode } from "react";
import { hasImdbId, imdbTitleUrl } from "../movie/movieTypes";
import Collapsible from "../shared/Collapsible";
import HorizontalList from "../shared/HorizontalList";
import ExternalLinkRow, {
  ExternalLinkRowSeparator,
} from "../shared/ExternalLinkRow";
import { useDetailData } from "../shared/useDetailData";
import { type PanelCards, usePanelCards } from "../shared/usePanelCards";
import {
  displayEpisodeType,
  displaySeasonEpisode,
} from "../tvseason/tvSeasonTypes";
import {
  displayDate,
  displayGenres,
  displayRuntime,
  displayText,
  getTmdbImageUrl,
} from "../utilities/utilities";
import CastCard from "./CastCard";
import CrewCard from "./CrewCard";
import GuestStarCard from "./GuestStarCard";
import { type TvEpisodeCrew, tvEpisodeGoogleSearchUrl } from "./tvEpisodeTypes";
import "../shared/shared.css";

function CrewSection({
  title,
  cardType,
  crew,
  cards,
}: {
  title: string;
  // The section's card type in the URL (see usePanelCards)
  cardType: string;
  crew: TvEpisodeCrew[];
  cards: PanelCards;
}) {
  const selected = cards.isSelectedIn(cardType);
  return (
    <Collapsible
      title={title}
      count={crew.length}
      {...cards.sectionProps(cardType)}
    >
      <HorizontalList hasSelection={selected}>
        {crew.map((c) => (
          <CrewCard
            key={c.id}
            crew={c}
            {...cards.cardProps(cardType, "person", c.tmdbId)}
          />
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
  // For the Google search offered when the episode has no IMDB page
  tvSeriesName: string;
}

// Shows one episode of a TV series, loaded from /api/tvepisode with the same
// query parameters that select it in the URL. Its cast, crew and guest star
// cards open the person's own panel in place of the whole TV series panel
// (see usePanelCards).
function TvEpisodePanel({
  tmdbTvSeriesId,
  seasonNumber,
  episodeNumber,
  tvSeriesImdbId,
  tvSeriesName,
}: TvEpisodePanelProps) {
  const params = new URLSearchParams({
    tmdbTvSeriesId: String(tmdbTvSeriesId),
    seasonNumber: String(seasonNumber),
    episodeNumber: String(episodeNumber),
  });
  const { data: tvEpisode, error } = useDetailData(
    `/api/tvepisode?${params.toString()}`,
    "TV episode",
  );
  const cards = usePanelCards({
    cardsIn: {
      cast: tvEpisode?.cast,
      directors: tvEpisode?.directors,
      writers: tvEpisode?.writers,
      "guest-stars": tvEpisode?.guestStars,
    },
  });

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
  const castSelected = cards.isSelectedIn("cast");
  const guestStarSelected = cards.isSelectedIn("guest-stars");

  const facts: [string, ReactNode][] = [
    ["Air Date", displayDate(tvEpisode.airDate)],
    [
      "Runtime",
      <>
        {displayRuntime(tvEpisode.omdbAverageEpisodeRuntimeNumber)}
        <ExternalLinkRowSeparator />
        {displayRuntime(tvEpisode.runtime)}
      </>,
    ],
    ["Rated", displayText(tvEpisode.rated)],
    ["Type", displayEpisodeType(tvEpisode.episodeType)],
    ["Known For", displayText(tvEpisode.knownForActors)],
    ["Genres", displayGenres("", tvEpisode.omdbGenres)],
  ];

  return (
    <div
      className={`detail-panel fly-origin${cards.hasSelection ? " has-child-selection" : ""}`}
      data-testid="tv-episode-panel"
    >
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
          {hasImdbId(tvEpisode.imdbId) ? (
            <ExternalLinkRow
              url={imdbTitleUrl(tvEpisode.imdbId)}
              copyAriaLabel="copy-imdb-link"
            >
              <span className="external-link-row-label">IMDB:</span>{" "}
              {imdbRating}
              {imdbRating !== "—" && imdbVotes !== "—" && (
                <>
                  {" "}
                  <span className="external-link-row-votes">({imdbVotes})</span>
                </>
              )}
            </ExternalLinkRow>
          ) : (
            // TMDB knows of no IMDB page for the episode (so there is no OMDB
            // rating either); offer a Google search for it instead
            <ExternalLinkRow
              url=""
              searchUrl={tvEpisodeGoogleSearchUrl(
                tvSeriesName,
                tvEpisode.seasonNumber,
                tvEpisode.episodeNumber,
                tvEpisode.title,
              )}
              label="IMDB:"
            />
          )}
          <ExternalLinkRow
            url={imdbTitleUrl(tvSeriesImdbId)}
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
        <Collapsible
          title="Cast"
          count={tvEpisode.cast.length}
          {...cards.sectionProps("cast")}
        >
          <HorizontalList hasSelection={castSelected}>
            {sortedCast.map((c) => (
              <CastCard
                key={c.id}
                cast={c}
                {...cards.cardProps("cast", "person", c.tmdbId)}
              />
            ))}
          </HorizontalList>
        </Collapsible>
        <CrewSection
          title="Directors"
          cardType="directors"
          crew={tvEpisode.directors}
          cards={cards}
        />
        <CrewSection
          title="Writers"
          cardType="writers"
          crew={tvEpisode.writers}
          cards={cards}
        />
        <Collapsible
          title="Guest Stars"
          count={tvEpisode.guestStars.length}
          {...cards.sectionProps("guest-stars")}
        >
          <HorizontalList hasSelection={guestStarSelected}>
            {sortedGuestStars.map((g) => (
              <GuestStarCard
                key={g.id}
                guestStar={g}
                {...cards.cardProps("guest-stars", "person", g.tmdbId)}
              />
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
