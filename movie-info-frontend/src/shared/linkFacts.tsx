import { Fragment, type ReactNode } from "react";
import { Link } from "react-router-dom";
import DetailLink from "./DetailLink";
import { type DetailFact } from "./DetailFacts";
import { type PanelCards } from "./usePanelCards";
import {
  displayText,
  type PanelKind,
  searchPath,
} from "../utilities/utilities";

interface Person {
  tmdbId: number;
  name: string;
}

function commaSeparated(items: ReactNode[]): ReactNode {
  return items.map((item, i) => (
    <Fragment key={i}>
      {i > 0 && ", "}
      {item}
    </Fragment>
  ));
}

// A fact naming the people in a role, each a link to the person's panel (see
// DetailLink), e.g. "Director: Frank Darabont" or "Writers: Stephen King,
// Frank Darabont". The label is plural only for several people, and without
// anyone the fact is left out (see DetailFacts). Someone credited more than
// once in the role (e.g. for both the story and the screenplay) is named
// once.
export function peopleFact(role: string, people: Person[]): DetailFact {
  const unique = people.filter(
    (person, i) => people.findIndex((p) => p.tmdbId === person.tmdbId) === i,
  );
  if (unique.length === 0) return [role, "—"];
  return [
    unique.length > 1 ? `${role}s` : role,
    commaSeparated(
      unique.map((person) => (
        <DetailLink panelKind="person" tmdbId={person.tmdbId}>
          {person.name}
        </DetailLink>
      )),
    ),
  ];
}

// The "Known For" fact of a movie, TV series or TV episode: IMDB's comma
// separated names of its best known actors, each a link. IMDB gives only the
// names, so each is looked up among the panel's own people (its sections, by
// card type, in order of preference) and opens that person's panel by way of
// their card (see DetailLink); a name that isn't found searches for it instead.
export function knownForActorsFact(
  knownFor: string | null | undefined,
  sections: { cardType: string; people: Person[] }[],
  cards: PanelCards,
): DetailFact {
  const text = displayText(knownFor);
  if (text === "—") return ["Known For", "—"];
  return [
    "Known For",
    commaSeparated(
      text.split(/,\s*/).map((name) => {
        for (const { cardType, people } of sections) {
          const person = people.find(
            (p) => p.name.toLowerCase() === name.toLowerCase(),
          );
          if (person) {
            return (
              <DetailLink
                panelKind="person"
                tmdbId={person.tmdbId}
                card={{ cards, cardType }}
              >
                {name}
              </DetailLink>
            );
          }
        }
        return <Link to={searchPath(name)}>{name}</Link>;
      }),
    ),
  ];
}

// A movie or TV series a person is credited in, and the section of the
// person's panel that holds its card
export interface KnownForCredit {
  cardType: string;
  panelKind: PanelKind;
  tmdbId: number;
  title: string;
  // Release / first air date, if known (ISO, so it starts with the year)
  date: string | null;
}

// The "Known For" fact of a person: IMDB's text for what they are best known
// for, a job and a title, e.g. "Actress, Buffy the Vampire Slayer
// (1997-2003)". The title is a link that opens the movie's or TV series'
// panel by way of its card among the person's credits (see DetailLink), when
// it is found there (in order of preference, and by its year too if IMDB
// names one); otherwise the text is shown as it is.
export function knownForCreditFact(
  knownFor: string | null | undefined,
  credits: KnownForCredit[],
  cards: PanelCards,
): DetailFact {
  const text = displayText(knownFor);
  if (text === "—") return ["Known For", "—"];

  // The job never has a comma, but the title may
  const jobEnd = text.indexOf(", ");
  const job = jobEnd < 0 ? "" : text.slice(0, jobEnd + 2);
  const titleAndYear = text.slice(job.length);
  const match = /^(.*?)(?: \((\d{4})[^)]*\))?$/.exec(titleAndYear);
  const title = (match?.[1] ?? titleAndYear).toLowerCase();
  const year = match?.[2];
  const sameTitle = credits.filter((c) => c.title.toLowerCase() === title);
  const credit =
    sameTitle.find((c) => year !== undefined && c.date?.startsWith(year)) ??
    sameTitle.at(0);
  if (!credit) return ["Known For", text];

  return [
    "Known For",
    <>
      {job}
      <DetailLink
        panelKind={credit.panelKind}
        tmdbId={credit.tmdbId}
        card={{ cards, cardType: credit.cardType }}
      >
        {titleAndYear}
      </DetailLink>
    </>,
  ];
}
