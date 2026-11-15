import { useEffect, useRef, useState, useSyncExternalStore } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import "./history-nav.css";

// The part of the browser's Navigation API used here (not yet in
// TypeScript's own DOM types). Browsers without it have no way to tell
// whether there is anywhere to go back to.
interface HistoryEntries extends EventTarget {
  canGoBack: boolean;
}
const historyEntries = (window as { navigation?: HistoryEntries }).navigation;

function subscribeToHistory(onChange: () => void) {
  historyEntries?.addEventListener("currententrychange", onChange);
  return () => {
    historyEntries?.removeEventListener("currententrychange", onChange);
  };
}

// Going back counts as possible when the browser can't tell.
function canGoBack() {
  return historyEntries?.canGoBack ?? true;
}

// How far the page has to be scrolled one way before the button reacts, so
// that a wobbling finger doesn't make it flicker.
const SCROLL_TOLERANCE_PX = 20;

// Whether scrolling has tucked the back button away: scrolling down the page
// does, and scrolling back up, or reaching the top, brings it back. This is
// the one part of the button's behaviour its stylesheet can't work out by
// itself: iOS has no CSS feature yet that knows which way the page was last
// scrolled. Everything visible about it is in history-nav.css.
//
// Only the user's own scrolling counts, i.e. a finger dragged across the
// screen and the glide that follows it. The app scrolls the page too (to
// bring a panel into view, or back to where a page was left), which must not
// hide the button: a tap never drags, and a new page (routeKey changes)
// starts from scratch.
function useTuckedAway(routeKey: string) {
  const [tuckedAway, setTuckedAway] = useState(false);
  const scrolledByUser = useRef(false);

  useEffect(() => {
    scrolledByUser.current = false;
  }, [routeKey]);

  useEffect(() => {
    // Where the page was when it last changed direction, or showed or hid
    // the button.
    let turningPointY = window.scrollY;
    let isTuckedAway = false;

    function onTouchStart() {
      scrolledByUser.current = false;
    }

    function onTouchMove() {
      scrolledByUser.current = true;
    }

    function onScroll() {
      // iOS reports positions above the top while the page bounces there.
      const y = Math.max(0, window.scrollY);
      const movedAgainst = isTuckedAway ? turningPointY - y : y - turningPointY;
      const atTopAgain = isTuckedAway && y === 0;
      if (movedAgainst <= 0 || !scrolledByUser.current) {
        // Still going the way that led to the current state, or moved by the
        // app, which is where the user's next scroll starts from.
        turningPointY = y;
      }
      if (
        (scrolledByUser.current && movedAgainst >= SCROLL_TOLERANCE_PX) ||
        atTopAgain
      ) {
        isTuckedAway = !isTuckedAway;
        turningPointY = y;
        setTuckedAway(isTuckedAway);
      }
    }

    window.addEventListener("touchstart", onTouchStart, { passive: true });
    window.addEventListener("touchmove", onTouchMove, { passive: true });
    window.addEventListener("scroll", onScroll, { passive: true });
    return () => {
      window.removeEventListener("touchstart", onTouchStart);
      window.removeEventListener("touchmove", onTouchMove);
      window.removeEventListener("scroll", onScroll);
    };
  }, []);

  return tuckedAway;
}

// A back button floating over the top left corner of the page. Only the app
// opened from a phone's Home Screen shows it (see history-nav.css): there the
// browser's own back button is missing. Logged out, there is nowhere worth
// going back to, so the login and register pages have none.
function HistoryNav() {
  const navigate = useNavigate();
  const location = useLocation();
  const backIsPossible = useSyncExternalStore(subscribeToHistory, canGoBack);
  const tuckedAway = useTuckedAway(location.key);
  const onAuthPage =
    location.pathname === "/login" || location.pathname === "/register";
  if (onAuthPage) {
    return null;
  }

  return (
    <button
      aria-label="back"
      type="button"
      className={`back-button${tuckedAway ? " is-tucked-away" : ""}`}
      disabled={!backIsPossible}
      onClick={() => {
        void navigate(-1);
      }}
    >
      <span aria-hidden="true">‹</span>
    </button>
  );
}

export default HistoryNav;
