import { useEffect, useState } from "react";
import { useLocation, useNavigate, useSearchParams } from "react-router-dom";
import { type PanelKind, withQueryParams } from "../utilities/utilities";
import { useCardSelection } from "./useCardSelection";
import { useOpenDetail } from "./useOpenDetail";

// The URL names a highlighted card inside a detail panel with the query
// parameters `cardType` — the panel section the card is in, e.g. "cast",
// "writers" or "movie-cast-credits" — and `cardId`, the TMDB id of the card's
// person / movie / TV series. (The TMDB id is what's stable about a card: the
// view models' own ids are fresh with every response.)
function cardKey(cardType: string, cardId: string): string {
  return `${cardType}:${cardId}`;
}

function cardTypeOf(key: string): string {
  return key.slice(0, key.indexOf(":"));
}

// The props a card gets from usePanelCards; see ExpandableCard for the first
// three.
export interface PanelCardProps {
  selected: boolean;
  deselecting: boolean;
  highlighted: boolean;
  onClick: () => void;
}

export interface PanelCards {
  // Whether a card of the panel is on its way to the panel's corner, in which
  // case the panel hands itself over to it (see .has-child-selection in
  // shared.css).
  hasSelection: boolean;
  // Whether the card on its way to the corner is in the given section, which
  // then hands itself over to it too (its HorizontalList's `hasSelection`).
  isSelectedIn: (cardType: string) => boolean;
  // The section's Collapsible props: it stays open while it holds the URL's
  // card, and hands itself over to the card while that flies.
  sectionProps: (cardType: string) => {
    forceOpen: boolean;
    childSelected: boolean;
  };
  // Props for the card with the given TMDB id in the given section; clicking
  // it opens the given kind of panel for the id (see useOpenDetail), or, if
  // the card is already selected or highlighted, deselects it: the URL
  // without the card is pushed, and nothing else happens (unlike a second
  // click on a selected search result card, which goes back). Pushing drops
  // the forward history, so forward can't jump to a panel that the card no
  // longer leads to.
  cardProps: (
    cardType: string,
    panelKind: PanelKind,
    tmdbId: number,
  ) => PanelCardProps;
}

interface PanelCardsOptions {
  // The cards of each section, by card type, once the panel has loaded.
  cardsIn: Record<string, { tmdbId: number }[] | undefined>;
  // False while a panel nested in this one is open (a TV series with a season
  // open), since the URL's card then belongs to the innermost panel.
  enabled?: boolean;
}

// Drives the cards inside a detail panel from the URL's `cardType` / `cardId`
// and gives each card its props. The URL's card is shown in one of two ways:
// - Right after being clicked here, it is selected: highlighted and flown to
//   the panel's corner while the panel hands itself over to it (see
//   useCardSelection), on its way to becoming the item's own panel (see
//   useOpenDetail). A card never expands inside its panel.
// - Reached any other way — the browser's back / forward buttons, a directly
//   loaded URL — it is merely highlighted, in its place in its row, with its
//   section open and the rest of the panel in view around it. Where the panel
//   had just been reached by the item's panel shrinking back to card size
//   (browser back; the panel's cards are already loaded, see useDetailData),
//   the card first flies home from the corner the panel shrank into while the
//   rest of the panel fades back in, reversing the flight it made when clicked.
export function usePanelCards({
  cardsIn,
  enabled = true,
}: PanelCardsOptions): PanelCards {
  const [searchParams] = useSearchParams();
  const location = useLocation();
  const navigate = useNavigate();
  const openDetail = useOpenDetail();
  const cardType = searchParams.get("cardType");
  const cardId = searchParams.get("cardId");
  const targetKey =
    enabled && cardType !== null && cardId !== null
      ? cardKey(cardType, cardId)
      : null;
  const hasCardFor = (key: string) => {
    const id = key.slice(key.indexOf(":") + 1);
    return (
      cardsIn[cardTypeOf(key)]?.some((c) => String(c.tmdbId) === id) ?? false
    );
  };

  // The card last clicked here, which the URL is about to name.
  const [clickedKey, setClickedKey] = useState<string | null>(null);
  // The URL's card once it rests highlighted in its row (as opposed to being
  // on its way to the corner). Without its cards loaded yet, the panel has
  // nothing to fly, so the card settles at once.
  const [settledKey, setSettledKey] = useState<string | null>(() =>
    targetKey !== null && !hasCardFor(targetKey) ? targetKey : null,
  );
  // The target the state above was last synced to (see "Adjusting some state
  // when a prop changes" in the React docs).
  const [syncedKey, setSyncedKey] = useState(targetKey);
  if (targetKey !== syncedKey) {
    setSyncedKey(targetKey);
    if (targetKey === null || targetKey === clickedKey) {
      setSettledKey(null);
    } else {
      // Not clicked here (e.g. the browser's forward button), so there is no
      // flight to reverse: highlight the card where it is.
      setSettledKey(targetKey);
    }
    if (targetKey === null) {
      setClickedKey(null);
    }
  }

  // Reached with the cards already loaded (e.g. browser back from the item's
  // panel): the card is first shown in the corner, for one painted frame, and
  // then flies home.
  useEffect(() => {
    if (
      targetKey === null ||
      targetKey === clickedKey ||
      settledKey === targetKey
    ) {
      return;
    }
    const frameId = window.requestAnimationFrame(() => {
      setSettledKey(targetKey);
    });
    return () => {
      window.cancelAnimationFrame(frameId);
    };
  }, [targetKey, clickedKey, settledKey]);

  const selection = useCardSelection({
    targetId: targetKey !== null && settledKey !== targetKey ? targetKey : null,
    opensPanel: false,
    hasCardFor,
  });
  const selectedType =
    selection.selectedId === null ? null : cardTypeOf(selection.selectedId);
  const settledType = settledKey === null ? null : cardTypeOf(settledKey);

  return {
    hasSelection: selection.selectedId !== null,
    isSelectedIn: (type) => selectedType === type,
    sectionProps: (type) => ({
      forceOpen: selectedType === type || settledType === type,
      childSelected: selectedType === type,
    }),
    cardProps: (type, panelKind, tmdbId) => {
      const key = cardKey(type, String(tmdbId));
      const selected = selection.selectedId === key;
      return {
        selected,
        deselecting: selection.previouslySelectedId === key && !selected,
        highlighted: settledKey === key,
        onClick: () => {
          if (selected || settledKey === key) {
            void navigate(
              withQueryParams(location.pathname, location.search, {
                cardType: null,
                cardId: null,
              }),
            );
          } else {
            setClickedKey(key);
            openDetail(panelKind, tmdbId, type, String(tmdbId));
          }
        },
      };
    },
  };
}
