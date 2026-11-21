import { useEffect, useId } from "react";

// The browser tab's title while nothing more specific is known
export const APP_TITLE = "Movie Search";

// What the components on the page each want in the title. The deepest one
// wins (an episode's panel over its season's, a panel over the search behind
// it), and among equals the latest (a panel that replaces another).
const requests = new Map<
  string,
  { title: string; level: number; order: number }
>();
let requestCount = 0;

function updateTitle() {
  let top: { title: string; level: number; order: number } | null = null;
  for (const request of requests.values()) {
    if (
      !top ||
      request.level > top.level ||
      (request.level === top.level && request.order > top.order)
    ) {
      top = request;
    }
  }
  document.title = top ? `${top.title} – ${APP_TITLE}` : APP_TITLE;
}

// Names the page in the browser tab's title for as long as the component is
// shown, e.g. "Oscars By Year – Movie Search", so that tabs, history entries
// and screen readers can tell the app's pages apart. Nothing is added while
// the title is still unknown (data still loading). `level` is how deep the
// component is nested among others that set a title.
export function usePageTitle(title: string | null | undefined, level = 0) {
  const id = useId();
  useEffect(() => {
    if (!title) return;
    requests.set(id, { title, level, order: ++requestCount });
    updateTitle();
    return () => {
      requests.delete(id);
      updateTitle();
    };
  }, [id, title, level]);
}
