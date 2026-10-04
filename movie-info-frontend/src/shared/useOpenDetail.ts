import { useEffect, useRef } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import {
  detailPath,
  type PanelKind,
  withQueryParams,
} from "../utilities/utilities";
import { CARD_FLY_MS } from "./useCardSelection";

// Opens the detail panel of an item shown as a card inside another panel: a
// person from a movie's / TV series' / TV episode's cast or crew, or a movie
// or TV series from a person's credits. Such cards only know the item's TMDB
// id. Like a click on a search result card, this only navigates, in two steps
// that leave two history entries:
// 1. The current URL gains `cardType` / `cardId` query parameters naming the
//    clicked card. That highlights the card and flies it to its panel's corner
//    (see usePanelCards), the way a season card flies before its panel opens.
// 2. Once the card has arrived, the item's own detail URL (`/person/287?q=…`
//    etc.) is pushed, without the card parameters but keeping the search
//    query so the results grid stays behind the new panel; the search page
//    then grows the new panel out of the card's spot (see ReplaceMode in
//    useCardSelection). It is skipped if the URL changes in the meantime,
//    e.g. by the browser's back button or a second click on the flying card.
// Browser back from the new panel so lands on step 1's URL, which shows the
// card the panel was opened from highlighted again (and any season / episode
// that panel had open, since those live in its URL too); one more back flies
// the card home.
export function useOpenDetail(): (
  panelKind: PanelKind,
  tmdbId: number,
  cardType: string,
  cardId: string,
) => void {
  const navigate = useNavigate();
  const location = useLocation();
  const currentUrl = location.pathname + location.search;
  const query = new URLSearchParams(location.search).get("q") ?? "";
  // Step 2, waiting for step 1's URL to have been reached.
  const pending = useRef<{ fromUrl: string; toUrl: string } | null>(null);

  useEffect(() => {
    const step = pending.current;
    if (!step) return;
    if (step.fromUrl !== currentUrl) {
      pending.current = null;
      return;
    }
    const timeoutId = window.setTimeout(() => {
      pending.current = null;
      void navigate(step.toUrl);
      // The card was likely clicked well down the old panel; the new panel
      // starts at the top of the page.
      window.scrollTo({ top: 0 });
    }, CARD_FLY_MS);
    return () => {
      window.clearTimeout(timeoutId);
    };
  }, [currentUrl, navigate]);

  return (panelKind, tmdbId, cardType, cardId) => {
    const fromUrl = withQueryParams(location.pathname, location.search, {
      cardType,
      cardId,
    });
    pending.current = {
      fromUrl,
      toUrl: detailPath(panelKind, String(tmdbId), query),
    };
    void navigate(fromUrl);
  };
}
