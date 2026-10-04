import { useNavigate, useSearchParams } from "react-router-dom";
import { detailPath, type PanelKind } from "../utilities/utilities";

// Opens the detail panel of an item shown as a card inside another panel: a
// person from a movie's / TV series' / TV episode's cast or crew, or a movie
// or TV series from a person's credits. Such cards only know the item's TMDB
// id. Like a click on a search result card, this only navigates to the item's
// own detail URL (`/person/287?q=…` etc.), keeping the search query so that the
// results grid stays behind the new panel; the search page then shows the new
// panel in place of the old one. Browser back returns to the old panel —
// including any season / episode it had open, since those live in its URL too.
export function useOpenDetail(): (
  panelKind: PanelKind,
  tmdbId: number,
) => void {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const query = searchParams.get("q") ?? "";

  return (panelKind, tmdbId) => {
    void navigate(detailPath(panelKind, String(tmdbId), query));
    // The card was likely clicked well down the old panel; the new panel
    // starts at the top of the page.
    window.scrollTo({ top: 0 });
  };
}
