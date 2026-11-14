import { type ReactNode } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { type PanelCards } from "./usePanelCards";
import { detailPath, type PanelKind } from "../utilities/utilities";

interface DetailLinkProps {
  // The panel and item the link opens
  panelKind: PanelKind;
  tmdbId: number;
  // For an item that also has a card in one of the panel's sections: the
  // panel's cards and the card's section (see usePanelCards)
  card?: { cards: PanelCards; cardType: string };
  children: ReactNode;
}

// A link in a detail panel's text to another item's panel (e.g. a director's
// name among the facts). When the item also has a card in the panel, clicking
// the link does what clicking that card does: the card flies to the corner
// and the item's panel opens, and browser back returns to the card. Otherwise
// the item's panel simply replaces this one. Either way it is a real link to
// the item's panel, so it can also be opened in a new tab.
function DetailLink({ panelKind, tmdbId, card, children }: DetailLinkProps) {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const href = detailPath(
    panelKind,
    String(tmdbId),
    searchParams.get("q") ?? "",
  );

  return (
    <a
      href={href}
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
        if (card) {
          card.cards.openCard(card.cardType, panelKind, tmdbId);
        } else {
          void navigate(href);
          // The new panel starts at the top of the page
          window.scrollTo({ top: 0 });
        }
      }}
    >
      {children}
    </a>
  );
}

export default DetailLink;
