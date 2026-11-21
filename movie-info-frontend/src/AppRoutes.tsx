import { useEffect, useRef } from "react";
import { Routes, Route, Navigate, useLocation } from "react-router-dom";
import LoginUser from "./user/LoginUser";
import RegisterUser from "./user/RegisterUser";
import SuggestionSearch from "./suggestion/SuggestionSearch";
import OscarsByYear from "./oscars/OscarsByYear";
import ImdbLists from "./imdb/ImdbLists";
import AspectRatioReference from "./aspectratio/AspectRatioReference";
import Footer from "./shared/Footer";
import HistoryNav from "./shared/HistoryNav";

// Which of the app's pages a URL shows. The search page's panels have URLs
// of their own, but open within the search page.
function pageOf(pathname: string): string {
  return /^\/(search|movie|tvseries|person)(\/|$)/.test(pathname)
    ? "/search"
    : pathname;
}

function AppRoutes() {
  const page = pageOf(useLocation().pathname);
  const mainRef = useRef<HTMLElement>(null);
  const shownPage = useRef(page);

  // Going to another page moves the keyboard focus to the start of that
  // page's content: otherwise it stays where it was (on a link in the footer,
  // say, below everything new), or is lost along with what had it, and a
  // screen reader never learns that the page changed. Within a page, focus
  // is left alone (see ExpandableCard for the panels).
  useEffect(() => {
    if (shownPage.current !== page) {
      shownPage.current = page;
      mainRef.current?.focus({ preventScroll: true });
    }
  }, [page]);

  return (
    <>
      <HistoryNav />
      <main ref={mainRef} tabIndex={-1}>
        <Routes>
          <Route path="/login" element={<LoginUser />} />
          <Route path="/register" element={<RegisterUser />} />
          <Route path="/search" element={<SuggestionSearch />} />
          <Route
            path="/movie/:itemId"
            element={<SuggestionSearch panelKind="movie" />}
          />
          <Route
            path="/tvseries/:itemId"
            element={<SuggestionSearch panelKind="tvseries" />}
          />
          <Route
            path="/person/:itemId"
            element={<SuggestionSearch panelKind="person" />}
          />
          <Route path="/oscars" element={<OscarsByYear />} />
          <Route path="/imdb-lists" element={<ImdbLists />} />
          <Route path="/aspect-ratios" element={<AspectRatioReference />} />
          <Route path="*" element={<Navigate to="/login" replace />} />
        </Routes>
      </main>
      <Footer />
    </>
  );
}

export default AppRoutes;
