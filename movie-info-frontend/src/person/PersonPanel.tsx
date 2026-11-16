import { useState } from "react";
import type { TmdbGender } from "../shared/sharedTypes";
import Collapsible from "../shared/Collapsible";
import DetailFacts, { type DetailFact } from "../shared/DetailFacts";
import HorizontalList from "../shared/HorizontalList";
import ExternalLinkRow from "../shared/ExternalLinkRow";
import ImdbLinkRow from "../shared/ImdbLinkRow";
import { knownForCreditFact } from "../shared/linkFacts";
import OptionToggle from "../shared/OptionToggle";
import { useDetailData } from "../shared/useDetailData";
import { type PanelCardProps, usePanelCards } from "../shared/usePanelCards";
import {
  detailIdQuery,
  displayAge,
  displayDate,
  displayText,
  getAge,
} from "../utilities/utilities";
import MovieCastCard from "./MovieCastCard";
import MovieCrewCard from "./MovieCrewCard";
import PersonOscars from "./PersonOscars";
import ProfileImagesList from "./ProfileImagesList";
import TvSeriesCastCard from "./TvSeriesCastCard";
import TvSeriesCrewCard from "./TvSeriesCrewCard";
import {
  crewFilterLabel,
  matchesCrewFilter,
  MOVIE_CREW_FILTERS,
  NO_CREW_FILTER,
  TV_SERIES_CREW_FILTERS,
} from "./crewFilters";
import { imdbNameUrl, sortByDateDescending } from "./personTypes";
import "../shared/shared.css";
import "./person.css";

const GENDER_LABELS: Record<TmdbGender, string> = {
  NotSetNotSpecified: "Not specified",
  Female: "Female",
  Male: "Male",
  NonBinary: "Non-binary",
};

// Whether a crew credit's card is shown under the section's filter. The URL's
// own card is shown whatever the filter says, so a card opened from elsewhere
// in the panel (see DetailLink) is always there to be flown and highlighted.
function isShown(
  filter: string,
  credit: { department: string; job: string },
  cardProps: PanelCardProps,
): boolean {
  return (
    cardProps.selected ||
    cardProps.deselecting ||
    cardProps.highlighted ||
    matchesCrewFilter(filter, credit)
  );
}

interface PersonPanelProps {
  // The person's IMDB or TMDB id (see isTmdbId)
  itemId: string;
}

// Shows a person. Their movie and TV series credit cards open the movie's /
// TV series' own panel in place of this one (see usePanelCards). Each crew
// credits section can be narrowed down to one role (see crewFilters); the
// choice lasts as long as the panel stays mounted and is not part of the URL.
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
  const [movieCrewFilter, setMovieCrewFilter] = useState(NO_CREW_FILTER);
  const [tvSeriesCrewFilter, setTvSeriesCrewFilter] = useState(NO_CREW_FILTER);

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

  const facts: DetailFact[] = [
    knownForCreditFact(
      person.knownForMovies,
      [
        ...movieCastCredits.map((c) => ({
          cardType: "movie-cast-credits",
          panelKind: "movie" as const,
          tmdbId: c.tmdbId,
          title: c.title,
          date: c.releaseDate,
        })),
        ...movieCrewCredits.map((c) => ({
          cardType: "movie-crew-credits",
          panelKind: "movie" as const,
          tmdbId: c.tmdbId,
          title: c.title,
          date: c.releaseDate,
        })),
        ...tvSeriesCastCredits.map((c) => ({
          cardType: "tv-series-cast-credits",
          panelKind: "tvseries" as const,
          tmdbId: c.tmdbId,
          title: c.name,
          date: c.firstAirDate,
        })),
        ...tvSeriesCrewCredits.map((c) => ({
          cardType: "tv-series-crew-credits",
          panelKind: "tvseries" as const,
          tmdbId: c.tmdbId,
          title: c.name,
          date: c.firstAirDate,
        })),
      ],
      cards,
    ),
    ["Known For Department", displayText(person.knownForDepartment)],
    ["Also Known As", displayText(person.alsoKnownAs.join(", "))],
    [
      "Birthday",
      displayDate(person.birthday) === "—" ? (
        "—"
      ) : (
        <>
          {displayDate(person.birthday)}
          {age !== null && (
            <>
              {" "}
              <i data-testid="person-age">{displayAge(age)}</i>
            </>
          )}
        </>
      ),
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
          <ImdbLinkRow
            url={imdbNameUrl(person.imdbId)}
            rank={person.imdbRank}
          />
          <ExternalLinkRow
            url={person.wikipediaLink ?? ""}
            label="Wikipedia:"
            copyAriaLabel="copy-wikipedia-link"
          />
          <DetailFacts facts={facts} />
        </div>
      </div>

      <div className="detail-sections">
        {displayText(person.biography) !== "—" && (
          <Collapsible title="Biography">
            <p className="detail-prose person-biography">{person.biography}</p>
          </Collapsible>
        )}
        <PersonOscars oscars={person.oscars} />
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
          <OptionToggle
            label="movie crew credits filter"
            options={MOVIE_CREW_FILTERS}
            labelOf={(filter) => crewFilterLabel(filter, movieCrewCredits)}
            selected={movieCrewFilter}
            onSelect={setMovieCrewFilter}
          />
          <HorizontalList hasSelection={movieCrewSelected}>
            {movieCrewCredits.flatMap((c) => {
              const cardProps = cards.cardProps(
                "movie-crew-credits",
                "movie",
                c.tmdbId,
              );
              return isShown(movieCrewFilter, c, cardProps)
                ? [<MovieCrewCard key={c.id} credit={c} {...cardProps} />]
                : [];
            })}
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
          <OptionToggle
            label="TV series crew credits filter"
            options={TV_SERIES_CREW_FILTERS}
            labelOf={(filter) => crewFilterLabel(filter, tvSeriesCrewCredits)}
            selected={tvSeriesCrewFilter}
            onSelect={setTvSeriesCrewFilter}
          />
          <HorizontalList hasSelection={tvSeriesCrewSelected}>
            {tvSeriesCrewCredits.flatMap((c) => {
              const cardProps = cards.cardProps(
                "tv-series-crew-credits",
                "tvseries",
                c.tmdbId,
              );
              return isShown(tvSeriesCrewFilter, c, cardProps)
                ? [<TvSeriesCrewCard key={c.id} credit={c} {...cardProps} />]
                : [];
            })}
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
