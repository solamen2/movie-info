import { type ReactNode, useEffect, useState } from "react";
import type { TmdbGender } from "../shared/sharedTypes";
import Collapsible from "../shared/Collapsible";
import HorizontalList from "../shared/HorizontalList";
import ImdbRow from "../shared/ImdbRow";
import { useOpenDetail } from "../shared/useOpenDetail";
import {
  describeFailedLoad,
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
import { type Person, imdbNameUrl, sortByDateDescending } from "./personTypes";
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
// TV series' own panel in place of this one (see useOpenDetail).
function PersonPanel({ itemId }: PersonPanelProps) {
  const [person, setPerson] = useState<Person | null>(null);
  const [error, setError] = useState("");
  const openDetail = useOpenDetail();

  useEffect(() => {
    const controller = new AbortController();

    async function loadPerson() {
      try {
        const response = await fetch(`/api/person?${detailIdQuery(itemId)}`, {
          signal: controller.signal,
        });
        if (!response.ok) {
          setError(await describeFailedLoad("Loading person", response));
          return;
        }
        const data = (await response.json()) as Person | null; // TODO: Maybe someday make this validation more robust
        if (data == null) {
          setError("No person details were found.");
          return;
        }
        setPerson(data);
      } catch {
        if (!controller.signal.aborted) {
          setError("An unexpected error occurred while loading the person.");
        }
      }
    }

    void loadPerson();
    return () => {
      controller.abort();
    };
  }, [itemId]);

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
    <div className="detail-panel" data-testid="person-panel">
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
          <ImdbRow imdbUrl={imdbNameUrl(person.imdbId)}>
            <span className="imdb-row-label">IMDB:</span>{" "}
            <span className="imdb-row-label">Rank:</span>{" "}
            {displayText(person.imdbRank)}
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
        <Collapsible title="Biography">
          <p className="detail-prose person-biography">
            {displayText(person.biography)}
          </p>
        </Collapsible>
        <Collapsible title="Movie Cast Credits" count={movieCastCredits.length}>
          <HorizontalList>
            {movieCastCredits.map((c) => (
              <MovieCastCard
                key={c.id}
                credit={c}
                onClick={() => {
                  openDetail("movie", c.tmdbId);
                }}
              />
            ))}
          </HorizontalList>
        </Collapsible>
        <Collapsible title="Movie Crew Credits" count={movieCrewCredits.length}>
          <HorizontalList>
            {movieCrewCredits.map((c) => (
              <MovieCrewCard
                key={c.id}
                credit={c}
                onClick={() => {
                  openDetail("movie", c.tmdbId);
                }}
              />
            ))}
          </HorizontalList>
        </Collapsible>
        <Collapsible
          title="TV Series Cast Credits"
          count={tvSeriesCastCredits.length}
        >
          <HorizontalList>
            {tvSeriesCastCredits.map((c) => (
              <TvSeriesCastCard
                key={c.id}
                credit={c}
                onClick={() => {
                  openDetail("tvseries", c.tmdbId);
                }}
              />
            ))}
          </HorizontalList>
        </Collapsible>
        <Collapsible
          title="TV Series Crew Credits"
          count={tvSeriesCrewCredits.length}
        >
          <HorizontalList>
            {tvSeriesCrewCredits.map((c) => (
              <TvSeriesCrewCard
                key={c.id}
                credit={c}
                onClick={() => {
                  openDetail("tvseries", c.tmdbId);
                }}
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
