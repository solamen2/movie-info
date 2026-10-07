import { type ReactNode } from "react";
import Collapsible from "../shared/Collapsible";
import HorizontalList from "../shared/HorizontalList";
import ExternalLinkRow, {
  ExternalLinkRowSeparator,
} from "../shared/ExternalLinkRow";
import TitleStatus from "../shared/TitleStatus";
import WatchProviderSection from "../shared/WatchProviderSection";
import { useDetailData } from "../shared/useDetailData";
import { type PanelCards, usePanelCards } from "../shared/usePanelCards";
import {
  detailIdQuery,
  displayDate,
  displayGenres,
  displayRuntime,
  displayText,
} from "../utilities/utilities";
import CastCard from "./CastCard";
import CrewCard from "./CrewCard";
import { type MovieCrew, imdbTitleUrl } from "./movieTypes";
import "../shared/shared.css";

function displayMoney(value: number): string {
  return value > 0
    ? new Intl.NumberFormat("en-US", {
        style: "currency",
        currency: "USD",
        maximumFractionDigits: 0,
      }).format(value)
    : "—";
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
  crew: MovieCrew[];
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

interface MoviePanelProps {
  // The movie's IMDB or TMDB id (see isTmdbId)
  itemId: string;
}

// Shows a movie. Its cast and crew cards open the person's own panel in place
// of this one (see usePanelCards).
function MoviePanel({ itemId }: MoviePanelProps) {
  const { data: movie, error } = useDetailData(
    `/api/movie?${detailIdQuery(itemId)}`,
    "movie",
  );
  const cards = usePanelCards({
    cardsIn: {
      cast: movie?.cast,
      directors: movie?.directors,
      writers: movie?.writers,
    },
  });

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
  const castSelected = cards.isSelectedIn("cast");

  const facts: [string, ReactNode][] = [
    ["Original Title", displayText(movie.originalTitle)],
    ["Release Date", displayDate(movie.releaseDate)],
    ["Runtime", displayRuntime(movie.runtime)],
    ["Rated", displayText(movie.rated)],
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
    <div
      className={`detail-panel fly-origin${cards.hasSelection ? " has-child-selection" : ""}`}
      data-testid="movie-panel"
    >
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
          <div className="detail-title detail-title-row">
            <h2>{movie.title}</h2>
            <TitleStatus status={movie.status} usualStatus="Released" />
          </div>
          {movie.tagline && <p className="detail-tagline">{movie.tagline}</p>}
          <ExternalLinkRow url={imdbUrl} copyAriaLabel="copy-imdb-link">
            <span className="external-link-row-label">IMDB:</span> {imdbRating}
            {imdbRating !== "—" && imdbVotes !== "—" && (
              <>
                {" "}
                <span className="external-link-row-votes">({imdbVotes})</span>
              </>
            )}
            <ExternalLinkRowSeparator />
            <span className="external-link-row-label">Rank:</span>{" "}
            {displayText(movie.imdbRank)}
          </ExternalLinkRow>
          <ExternalLinkRow
            url={movie.wikipediaLink ?? ""}
            label="Wikipedia:"
            copyAriaLabel="copy-wikipedia-link"
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
          count={movie.cast.length}
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
          crew={movie.directors}
          cards={cards}
        />
        <CrewSection
          title="Writers"
          cardType="writers"
          crew={movie.writers}
          cards={cards}
        />
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
