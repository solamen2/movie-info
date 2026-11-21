import { type ReactNode, useState } from "react";
import { fuzzyMatches } from "./fuzzyMatch";
import HorizontalList from "./HorizontalList";
import { type PanelCardProps, type PanelCards } from "./usePanelCards";
import "./shared.css";

interface FilterablePerson {
  tmdbId: number;
  name: string;
  originalName: string;
}

interface FilterablePeopleListProps<T extends FilterablePerson> {
  // What the list holds, for the filter's accessible name: "cast" makes it
  // "Filter cast"
  label: string;
  // In the order to show them
  people: T[];
  charactersOf: (person: T) => string[];
  // The section's card type in the URL (see usePanelCards)
  cardType: string;
  cards: PanelCards;
  renderCard: (person: T, cardProps: PanelCardProps) => ReactNode;
  // Shown between the filter and the cards (a section's own options)
  children?: ReactNode;
}

// A section's row of person cards under a free-text box that filters them as
// the user types, by actor name and by character name (see fuzzyMatches).
// The filter is remembered for as long as the list stays mounted, and is
// deliberately not part of the URL. The URL's own card is shown whatever the
// filter says, so a card opened from elsewhere in the panel (see DetailLink)
// is always there to be flown and highlighted.
function FilterablePeopleList<T extends FilterablePerson>({
  label,
  people,
  charactersOf,
  cardType,
  cards,
  renderCard,
  children,
}: FilterablePeopleListProps<T>) {
  const [filter, setFilter] = useState("");
  const shown = people
    .map((person) => ({
      person,
      cardProps: cards.cardProps(cardType, "person", person.tmdbId),
    }))
    .filter(
      ({ person, cardProps }) =>
        cardProps.selected ||
        cardProps.deselecting ||
        cardProps.highlighted ||
        fuzzyMatches(filter, [
          person.name,
          person.originalName,
          ...charactersOf(person),
        ]),
    );

  return (
    <>
      <input
        type="search"
        className="list-filter"
        aria-label={`Filter ${label}`}
        placeholder="Filter by actor or character"
        value={filter}
        onChange={(e) => {
          setFilter(e.target.value);
        }}
      />
      {children}
      {shown.length === 0 && people.length > 0 ? (
        <p className="detail-empty" role="status">
          No matches
        </p>
      ) : (
        <HorizontalList hasSelection={cards.isSelectedIn(cardType)}>
          {shown.map(({ person, cardProps }) => renderCard(person, cardProps))}
        </HorizontalList>
      )}
    </>
  );
}

export default FilterablePeopleList;
