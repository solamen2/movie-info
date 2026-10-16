import { type ReactNode } from "react";
import { Link, useLocation } from "react-router-dom";
import { detailPath, type PanelKind } from "../utilities/utilities";
import InfoPopup from "./InfoPopup";
import { splitNameIntoNominees } from "./oscarUtils";
import type { OscarAward } from "./oscarTypes";
import "./oscars.css";

interface OscarItemLinkProps {
  text: string;
  panelKind: PanelKind;
  // The dataset's IMDB id for the movie or person; null when it has none
  // (or when the "person" is a studio or a country)
  imdbId: string | null;
}

// A link to a movie's or a person's panel in the app, by its IMDB id. Entries
// the dataset has no IMDB id for, or that are a studio or a country rather
// than a person, are plain text, since the app can't show them. Like every
// in-app link, it keeps the current search query (`?q=`) so the results grid
// stays behind the panel.
export function OscarItemLink({ text, panelKind, imdbId }: OscarItemLinkProps) {
  const location = useLocation();
  if (imdbId === null) return <>{text}</>;
  const query = new URLSearchParams(location.search).get("q") ?? "";
  return (
    <Link to={detailPath(panelKind, imdbId, query)} className="oscar-item-link">
      {text}
    </Link>
  );
}

// The "*" after a category shown under its modern name, which pops up the
// category's name at the time
export function CategoryAsterisk({
  originalCategory,
}: {
  originalCategory: string;
}) {
  return (
    <InfoPopup trigger="*" label="original category name">
      Original Category Name: {originalCategory}
    </InfoPopup>
  );
}

function Credit({ award }: { award: OscarAward }) {
  const parts = splitNameIntoNominees(
    award.name,
    award.nominees.map((n) => n.name),
  );
  const imdbIdOf = (nominee: string) =>
    award.nominees.find((n) => n.name === nominee)?.imdbId ?? null;
  return (
    <>
      {parts.map((part, i) =>
        part.nominee === null ? (
          <span key={i}>{part.text}</span>
        ) : (
          <OscarItemLink
            key={i}
            text={part.text}
            panelKind="person"
            imdbId={imdbIdOf(part.nominee)}
          />
        ),
      )}
    </>
  );
}

function Films({ award }: { award: OscarAward }) {
  return (
    <>
      {award.films.map((film, i) => (
        <span key={i}>
          {i > 0 && ", "}
          <OscarItemLink
            text={film.title}
            panelKind="movie"
            imdbId={film.imdbId}
          />
        </span>
      ))}
    </>
  );
}

const SEPARATOR = " — ";

interface OscarAwardLineProps {
  award: OscarAward;
  // Where the line is shown, which decides what it needs to say:
  // - "year": the Oscars By Year listing, under the category's heading
  // - "movie": a movie's panel, so the film is known
  // - "person": a person's panel, so the year matters
  context: "year" | "movie" | "person";
}

// One award (a nomination or win) as a line of text: the people as "<actor>
// as <character>" for acting awards, the song for song awards, the film(s),
// and the citation of special awards. People and films link to their panels.
function OscarAwardLine({ award, context }: OscarAwardLineProps) {
  const hasCredit = award.name !== "" || award.nominees.length > 0;
  const showFilms = context !== "movie" && award.films.length > 0;
  const showCategory = context !== "year" || award.isSpecial;
  const isActing = award.characters.length > 0;
  // Acting and directing awards name the person before the film
  const personFirst = isActing || award.category.startsWith("Best Director");

  const pieces: ReactNode[] = [];
  if (personFirst) {
    // "Cillian Murphy as J. Robert Oppenheimer — Oppenheimer",
    // "Christopher Nolan — Oppenheimer"
    pieces.push(
      <span key="credit">
        <Credit award={award} />
        {isActing && ` as ${award.characters.join(", ")}`}
      </span>,
    );
    if (showFilms) pieces.push(<Films key="films" award={award} />);
  } else {
    // "Barbie — “What Was I Made For?” — Music and Lyric by Billie Eilish…"
    if (showFilms) pieces.push(<Films key="films" award={award} />);
    if (award.song !== null) {
      pieces.push(<span key="song">“{award.song}”</span>);
    }
    if (hasCredit) {
      pieces.push(
        <span key="credit">
          <Credit award={award} />
        </span>,
      );
    }
  }

  return (
    <span className="oscar-award-line">
      {showCategory && (
        <span className="oscar-award-category">
          {award.category}
          {award.originalCategory !== null && (
            <CategoryAsterisk originalCategory={award.originalCategory} />
          )}
          {context === "person" && ` (${award.year})`}
          {pieces.length > 0 && ": "}
        </span>
      )}
      {context === "year" ? (
        // On the Oscars By Year page the first piece (the film, or the person
        // for acting and directing awards) is the entry itself, and the rest
        // is its detail in parentheses; a winner's entry is a little larger
        <>
          <span
            className={`oscar-award-primary${award.winner ? " oscar-award-primary-winner" : ""}`}
          >
            {pieces[0]}
          </span>
          {pieces.length > 1 && " "}
          {pieces.length > 1 && (
            <span className="oscar-award-secondary">
              (
              {pieces.slice(1).map((piece, i) => (
                <span key={i}>
                  {i > 0 && SEPARATOR}
                  {piece}
                </span>
              ))}
              )
            </span>
          )}
        </>
      ) : (
        pieces.map((piece, i) => (
          <span key={i}>
            {i > 0 && SEPARATOR}
            {piece}
          </span>
        ))
      )}
      {award.note !== null && (
        <>
          {" "}
          <InfoPopup trigger="ⓘ" label="note">
            {award.note}
          </InfoPopup>
        </>
      )}
      {award.citation !== null && (
        <span className="oscar-award-citation">{award.citation}</span>
      )}
    </span>
  );
}

export default OscarAwardLine;
