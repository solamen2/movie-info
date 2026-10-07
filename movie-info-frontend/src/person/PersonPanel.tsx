import { type ReactNode } from "react";
import type { TmdbGender } from "../shared/sharedTypes";
import Collapsible from "../shared/Collapsible";
import HorizontalList from "../shared/HorizontalList";
import ExternalLinkRow from "../shared/ExternalLinkRow";
import { useDetailData } from "../shared/useDetailData";
import { usePanelCards } from "../shared/usePanelCards";
import {
  detailIdQuery,
  displayAge,
  displayDate,
  displayText,
  getAge,
} from "../utilities/utilities";
import MovieCastCard from "./MovieCastCard";
import MovieCrewCard from "./MovieCrewCard";
import ProfileImagesList from "./ProfileImagesList";
import TvSeriesCastCard from "./TvSeriesCastCard";
import TvSeriesCrewCard from "./TvSeriesCrewCard";
import { imdbNameUrl, sortByDateDescending } from "./personTypes";
import "../shared/shared.css";
import "./person.css";

const GENDER_LABELS: Record<TmdbGender, string> = {
  NotSetNotSpecified: "Not specified",
  Female: "Female",
  Male: "Male",
  NonBinary: "Non-binary",
};

interface PersonPanelProps {
  // The person's IMDB or TMDB id (see isTmdbId)
  itemId: string;
}

// Shows a person. Their movie and TV series credit cards open the movie's /
// TV series' own panel in place of this one (see usePanelCards).
function PersonPanel({ itemId }: PersonPanelProps) {
  const { data: person, error } = useDetailData(
    `/api/person?${detailIdQuery(itemId)}`,
    "person",
  );
  const cards = usePanelCards({
    cardsIn: {
      "movie-cast-credits": person?.movieCastCredits,
      "movie-crew-credits": person?.movieCrewCredits,
      "tv-series-cast-credits": person?.tvSeriesCastCredits,
      "tv-series-crew-credits": person?.tvSeriesCrewCredits,
    },
  });

  if (error) {
    return (
      <div className="detail-panel">
        <p className="error-message">{error}</p>
      </div>
    );
  }

  if (!person) {
    return (
      <div className="detail-panel">
        <p className="detail-loading">Loading person details…</p>
      </div>
    );
  }

  const movieCastCredits = sortByDateDescending(
    person.movieCastCredits,
    (c) => c.releaseDate,
  );
  const movieCrewCredits = sortByDateDescending(
    person.movieCrewCredits,
    (c) => c.releaseDate,
  );
  const tvSeriesCastCredits = sortByDateDescending(
    person.tvSeriesCastCredits,
    (c) => c.firstCreditAirDate ?? c.firstAirDate,
  );
  const tvSeriesCrewCredits = sortByDateDescending(
    person.tvSeriesCrewCredits,
    (c) => c.firstCreditAirDate ?? c.firstAirDate,
  );
  const movieCastSelected = cards.isSelectedIn("movie-cast-credits");
  const movieCrewSelected = cards.isSelectedIn("movie-crew-credits");
  const tvSeriesCastSelected = cards.isSelectedIn("tv-series-cast-credits");
  const tvSeriesCrewSelected = cards.isSelectedIn("tv-series-crew-credits");

  const age = getAge(person.birthday, person.deathday);

  const facts: [string, ReactNode][] = [
    ["Known For", displayText(person.knownForMovies)],
    ["Known For Department", displayText(person.knownForDepartment)],
    ["Also Known As", displayText(person.alsoKnownAs.join(", "))],
    [
      "Birthday",
      <>
        {displayDate(person.birthday)}
        {age !== null && (
          <>
            {" "}
            <i data-testid="person-age">{displayAge(age)}</i>
          </>
        )}
      </>,
    ],
    ["Deathday", displayDate(person.deathday)],
    ["Place of Birth", displayText(person.placeOfBirth)],
    ["Gender", GENDER_LABELS[person.gender]],
    [
      "Homepage",
      person.homepage ? (
        <a href={person.homepage} target="_blank" rel="noopener">
          {person.homepage}
        </a>
      ) : (
        "—"
      ),
    ],
  ];

  return (
    <div
      className={`detail-panel fly-origin${cards.hasSelection ? " has-child-selection" : ""}`}
      data-testid="person-panel"
    >
      <div className="detail-panel-top">
        {person.image ? (
          <img
            src={person.image.imageURL}
            alt={person.name}
            className="detail-poster"
          />
        ) : (
          <div className="detail-poster card-image-placeholder">No image</div>
        )}
        <div className="detail-panel-heading">
          <h2 className="detail-title">{person.name}</h2>
          <ExternalLinkRow
            url={imdbNameUrl(person.imdbId)}
            copyAriaLabel="copy-imdb-link"
          >
            <span className="external-link-row-label">IMDB:</span>{" "}
            <span className="external-link-row-label">Rank:</span>{" "}
            {displayText(person.imdbRank)}
          </ExternalLinkRow>
          <ExternalLinkRow
            url={person.wikipediaLink ?? ""}
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
        <Collapsible title="Biography">
          <p className="detail-prose person-biography">
            {displayText(person.biography)}
          </p>
        </Collapsible>
        <Collapsible
          title="Movie Cast Credits"
          count={movieCastCredits.length}
          {...cards.sectionProps("movie-cast-credits")}
        >
          <HorizontalList hasSelection={movieCastSelected}>
            {movieCastCredits.map((c) => (
              <MovieCastCard
                key={c.id}
                credit={c}
                {...cards.cardProps("movie-cast-credits", "movie", c.tmdbId)}
              />
            ))}
          </HorizontalList>
        </Collapsible>
        <Collapsible
          title="Movie Crew Credits"
          count={movieCrewCredits.length}
          {...cards.sectionProps("movie-crew-credits")}
        >
          <HorizontalList hasSelection={movieCrewSelected}>
            {movieCrewCredits.map((c) => (
              <MovieCrewCard
                key={c.id}
                credit={c}
                {...cards.cardProps("movie-crew-credits", "movie", c.tmdbId)}
              />
            ))}
          </HorizontalList>
        </Collapsible>
        <Collapsible
          title="TV Series Cast Credits"
          count={tvSeriesCastCredits.length}
          {...cards.sectionProps("tv-series-cast-credits")}
        >
          <HorizontalList hasSelection={tvSeriesCastSelected}>
            {tvSeriesCastCredits.map((c) => (
              <TvSeriesCastCard
                key={c.id}
                credit={c}
                {...cards.cardProps(
                  "tv-series-cast-credits",
                  "tvseries",
                  c.tmdbId,
                )}
              />
            ))}
          </HorizontalList>
        </Collapsible>
        <Collapsible
          title="TV Series Crew Credits"
          count={tvSeriesCrewCredits.length}
          {...cards.sectionProps("tv-series-crew-credits")}
        >
          <HorizontalList hasSelection={tvSeriesCrewSelected}>
            {tvSeriesCrewCredits.map((c) => (
              <TvSeriesCrewCard
                key={c.id}
                credit={c}
                {...cards.cardProps(
                  "tv-series-crew-credits",
                  "tvseries",
                  c.tmdbId,
                )}
              />
            ))}
          </HorizontalList>
        </Collapsible>
        <Collapsible title="Profile Images" count={person.profileImages.length}>
          <ProfileImagesList name={person.name} images={person.profileImages} />
        </Collapsible>
      </div>
    </div>
  );
}

export default PersonPanel;
