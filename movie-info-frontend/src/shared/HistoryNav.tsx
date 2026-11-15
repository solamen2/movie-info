import { useEffect, useState, useSyncExternalStore } from "react";
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
function useTuckedAway() {
  const [tuckedAway, setTuckedAway] = useState(false);

  useEffect(() => {
    // Where the page was when it last changed direction, or showed or hid
    // the button.
    let turningPointY = window.scrollY;
    let isTuckedAway = false;

    function onScroll() {
      // iOS reports positions above the top while the page bounces there.
      const y = Math.max(0, window.scrollY);
      const movedAgainst = isTuckedAway ? turningPointY - y : y - turningPointY;
      if (movedAgainst <= 0) {
        // Still going the way that led to the current state
        turningPointY = y;
      }
      if (movedAgainst >= SCROLL_TOLERANCE_PX || (isTuckedAway && y === 0)) {
        isTuckedAway = !isTuckedAway;
        turningPointY = y;
        setTuckedAway(isTuckedAway);
      }
    }

    window.addEventListener("scroll", onScroll, { passive: true });
    return () => {
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
  const tuckedAway = useTuckedAway();
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
