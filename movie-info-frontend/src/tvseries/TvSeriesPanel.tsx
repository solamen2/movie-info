import { type ReactNode } from "react";
import { useLocation, useNavigate, useSearchParams } from "react-router-dom";
import { imdbTitleUrl } from "../movie/movieTypes";
import Collapsible from "../shared/Collapsible";
import HorizontalList from "../shared/HorizontalList";
import ImdbRow, { ImdbRowSeparator } from "../shared/ImdbRow";
import TitleStatus from "../shared/TitleStatus";
import WatchProviderSection from "../shared/WatchProviderSection";
import { useCardSelection } from "../shared/useCardSelection";
import { useDetailData } from "../shared/useDetailData";
import { type PanelCards, usePanelCards } from "../shared/usePanelCards";
import {
  detailIdQuery,
  displayDate,
  displayGenres,
  displayRuntime,
  displayText,
  getIntParam,
  withQueryParams,
} from "../utilities/utilities";
import CreatorCard from "./CreatorCard";
import CrewCard from "./CrewCard";
import NetworkCard from "./NetworkCard";
import SeasonCard from "./SeasonCard";
import TvSeriesCastCollapsible, {
  TV_SERIES_CAST_CARD_TYPE,
} from "./TvSeriesCastCollapsible";
import { type TvSeriesCrew, sortSeasons } from "./tvSeriesTypes";
import "../shared/shared.css";
import "./tvseries.css";

// TMDB lists every distinct episode runtime a series has had
function displayEpisodeRunTimes(minutes: number[]): string {
  const runtimes = minutes.filter((m) => m > 0).map(displayRuntime);
  return runtimes.length > 0 ? runtimes.join(", ") : "—";
}

function CrewSection({
  title,
  cardType,
  crew,
  cards,
}: {
  title: string;
  // The section's card type in the URL (see usePanelCards)
  cardType: string;
  crew: TvSeriesCrew[];
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

interface TvSeriesPanelProps {
  // The TV series' IMDB or TMDB id (see isTmdbId)
  itemId: string;
}

// Shows a TV series. A season within it is selected via the URL's
// `?tmdbTvSeriesId=…&seasonNumber=…` query parameters (the same ones the
// /api/tvseason call takes), so browser back / forward and directly loaded
// URLs work for seasons too. Its cast, creator and crew cards open the
// person's own panel in place of this one (see usePanelCards).
function TvSeriesPanel({ itemId }: TvSeriesPanelProps) {
  const { data: tvSeries, error } = useDetailData(
    `/api/tvseries?${detailIdQuery(itemId)}`,
    "TV series",
  );
  const [searchParams] = useSearchParams();
  const location = useLocation();
  const navigate = useNavigate();
  const selectedSeasonNumber = getIntParam(searchParams, "seasonNumber");
  const tmdbTvSeriesIdParam = getIntParam(searchParams, "tmdbTvSeriesId");
  const seasonSelection = useCardSelection({
    targetId:
      selectedSeasonNumber === null ? null : String(selectedSeasonNumber),
    opensPanel: true,
    hasCardFor: (id) =>
      tvSeries?.seasons.some((s) => String(s.seasonNumber) === id) ?? false,
  });
  const cards = usePanelCards({
    cardsIn: {
      [TV_SERIES_CAST_CARD_TYPE]: tvSeries?.cast,
      creators: tvSeries?.creators,
      directors: tvSeries?.directors,
      writers: tvSeries?.writers,
    },
    // With a season open, the URL's card is one of the season's / episode's
    enabled: selectedSeasonNumber === null,
  });

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
  const tmdbTvSeriesId = tmdbTvSeriesIdParam ?? tvSeries.tmdbId;
  const imdbRating = displayText(tvSeries.imdbRating);
  const imdbVotes = displayText(tvSeries.imdbVotes);
  const sortedSeasons = sortSeasons(tvSeries.seasons);
  const seasonSelected = seasonSelection.selectedId !== null;
  const creatorSelected = cards.isSelectedIn("creators");

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
    <div
      className={`detail-panel fly-origin${seasonSelected || cards.hasSelection ? " has-child-selection" : ""}`}
      data-testid="tv-series-panel"
    >
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
          <div className="detail-title detail-title-row">
            <h2>{tvSeries.name}</h2>
            <TitleStatus status={tvSeries.status} usualStatus="Ended" />
          </div>
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
        <Collapsible
          title="Seasons"
          count={tvSeries.seasons.length}
          forceOpen={seasonSelected}
          childSelected={seasonSelected}
        >
          <HorizontalList hasSelection={seasonSelected}>
            {sortedSeasons.map((s) => {
              const seasonId = String(s.seasonNumber);
              return (
                <SeasonCard
                  key={s.id}
                  season={s}
                  tmdbTvSeriesId={tmdbTvSeriesId}
                  tvSeriesImdbId={tvSeries.imdbId}
                  selected={seasonSelection.selectedId === seasonId}
                  expanded={seasonSelection.expandedId === seasonId}
                  deselecting={
                    seasonSelection.previouslySelectedId === seasonId &&
                    seasonSelection.selectedId !== seasonId
                  }
                  onExpand={() => {
                    void navigate(
                      withQueryParams(location.pathname, location.search, {
                        tmdbTvSeriesId: String(tvSeries.tmdbId),
                        seasonNumber: seasonId,
                        episodeNumber: null,
                      }),
                    );
                  }}
                />
              );
            })}
          </HorizontalList>
        </Collapsible>
        <TvSeriesCastCollapsible cast={tvSeries.cast} cards={cards} />
        <Collapsible
          title="Creators"
          count={tvSeries.creators.length}
          {...cards.sectionProps("creators")}
        >
          <HorizontalList hasSelection={creatorSelected}>
            {tvSeries.creators.map((c) => (
              <CreatorCard
                key={c.id}
                creator={c}
                {...cards.cardProps("creators", "person", c.tmdbId)}
              />
            ))}
          </HorizontalList>
        </Collapsible>
        <CrewSection
          title="Directors"
          cardType="directors"
          crew={tvSeries.directors}
          cards={cards}
        />
        <CrewSection
          title="Writers"
          cardType="writers"
          crew={tvSeries.writers}
          cards={cards}
        />
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
