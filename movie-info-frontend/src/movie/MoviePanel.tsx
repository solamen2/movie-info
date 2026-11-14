import { type ReactNode } from "react";
import AspectRatios from "../shared/AspectRatios";
import {
  aspectRatioGoogleSearchUrl,
  aspectRatiosLabel,
} from "../shared/aspectRatioUtils";
import Collapsible from "../shared/Collapsible";
import DetailFacts, { type DetailFact } from "../shared/DetailFacts";
import HorizontalList from "../shared/HorizontalList";
import ExternalLinkRow from "../shared/ExternalLinkRow";
import ImdbLinkRow from "../shared/ImdbLinkRow";
import { knownForActorsFact, peopleFact } from "../shared/linkFacts";
import PlotSection from "../shared/PlotSection";
import TitleStatus from "../shared/TitleStatus";
import WatchProviderSection from "../shared/WatchProviderSection";
import { useDetailData } from "../shared/useDetailData";
import { usePanelCards } from "../shared/usePanelCards";
import {
  detailIdQuery,
  displayDate,
  displayGenres,
  displayMoney,
  displayRuntime,
  displayText,
} from "../utilities/utilities";
import CastCard from "./CastCard";
import InflationAdjustedMoney from "./InflationAdjustedMoney";
import { parseYearMonth } from "./inflationUtils";
import MovieOscars from "./MovieOscars";
import { imdbTitleUrl } from "./movieTypes";
import "../shared/shared.css";

interface MoviePanelProps {
  // The movie's IMDB or TMDB id (see isTmdbId)
  itemId: string;
}

// Shows a movie. Its cast cards open the person's own panel in place of this
// one (see usePanelCards).
function MoviePanel({ itemId }: MoviePanelProps) {
  const { data: movie, error } = useDetailData(
    `/api/movie?${detailIdQuery(itemId)}`,
    "movie",
  );
  const cards = usePanelCards({
    cardsIn: {
      cast: movie?.cast,
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
  const sortedCast = [...movie.cast].sort(
    (a, b) => a.billedOrder - b.billedOrder,
  );
  const castSelected = cards.isSelectedIn("cast");

  // A budget or revenue with its inflation adjustment, which needs the
  // release month and year; just the amount (or "—") without them. Keyed by
  // the movie so the adjustment's month and year start over with a new one.
  const releaseMonth = parseYearMonth(movie.releaseDate);
  const moneyFact = (amount: number, name: string): ReactNode =>
    amount <= 0 || releaseMonth === null ? (
      displayMoney(amount)
    ) : (
      <InflationAdjustedMoney
        key={movie.imdbId}
        amount={amount}
        releaseDate={releaseMonth}
        name={name}
      />
    );

  const facts: DetailFact[] = [
    ["Original Title", displayText(movie.originalTitle)],
    ["Release Date", displayDate(movie.releaseDate)],
    ["Runtime", displayRuntime(movie.runtime)],
    ["Rated", displayText(movie.rated)],
    [
      aspectRatiosLabel(movie.aspectRatios),
      <AspectRatios
        aspectRatios={movie.aspectRatios}
        searchUrl={aspectRatioGoogleSearchUrl(
          movie.title,
          movie.year === null ? "film" : `(${String(movie.year)} film)`,
        )}
      />,
    ],
    peopleFact("Director", movie.directors),
    peopleFact("Writer", movie.writers),
    knownForActorsFact(
      movie.knownForActors,
      [{ cardType: "cast", people: movie.cast }],
      cards,
    ),
    ["Genres", displayGenres(movie.tmdbGenres, movie.omdbGenres)],
    ["Budget", moneyFact(movie.budget, "budget")],
    ["Revenue", moneyFact(movie.revenue, "revenue")],
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
          <ImdbLinkRow
            url={imdbUrl}
            rating={movie.imdbRating}
            votes={movie.imdbVotes}
            rank={movie.imdbRank}
          />
          <ExternalLinkRow
            url={movie.wikipediaLink ?? ""}
            label="Wikipedia:"
            copyAriaLabel="copy-wikipedia-link"
          />
          <DetailFacts facts={facts} />
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
        <MovieOscars oscars={movie.oscars} />
        <PlotSection
          title="Plot"
          sources={[
            { name: "TMDB", text: movie.tmdbPlot },
            { name: "OMDB", text: movie.omdbPlot },
          ]}
        />
        <WatchProviderSection
          flatrate={movie.watchProvidersFlatrate}
          rent={movie.watchProvidersRent}
          buy={movie.watchProvidersBuy}
        />
      </div>
    </div>
  );
}

export default MoviePanel;
