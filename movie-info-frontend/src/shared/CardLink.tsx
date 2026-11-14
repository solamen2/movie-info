import { type ReactNode } from "react";
import { useSearchParams } from "react-router-dom";
import { type PanelCards } from "./usePanelCards";
import { detailPath, type PanelKind } from "../utilities/utilities";

interface CardLinkProps {
  // The panel's cards, the card's section, and the panel and item the card
  // opens (see usePanelCards)
  cards: PanelCards;
  cardType: string;
  panelKind: PanelKind;
  tmdbId: number;
  children: ReactNode;
}

// A link in a detail panel's text to an item that also has a card in one of
// the panel's sections (e.g. a director's name among the facts). Clicking it
// does what clicking that card does: the card flies to the corner and the
// item's own panel opens, and browser back returns to the card. It is a real
// link to the item's panel, so it can also be opened in a new tab.
function CardLink({
  cards,
  cardType,
  panelKind,
  tmdbId,
  children,
}: CardLinkProps) {
  const [searchParams] = useSearchParams();
  const query = searchParams.get("q") ?? "";

  return (
    <a
      href={detailPath(panelKind, String(tmdbId), query)}
      onClick={(e) => {
        // Leave opening in a new tab or window to the browser
        if (
          e.button !== 0 ||
          e.metaKey ||
          e.ctrlKey ||
          e.shiftKey ||
          e.altKey
        ) {
          return;
        }
        e.preventDefault();
        cards.openCard(cardType, panelKind, tmdbId);
      }}
    >
      {children}
    </a>
  );
}

export default CardLink;
