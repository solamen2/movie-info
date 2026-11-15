// In the app opened from the iOS Home Screen (iOS 26), going back or forward
// sometimes leaves the top of the page blank: the content is all there, and
// nothing is still animating, but WebKit hasn't drawn it and doesn't until
// the page is next scrolled. Nothing short of a real scroll makes it catch up
// (asking for a frame, restyling the page and switching off every animation
// were all tried on a phone), so after such a move the page is scrolled down
// by a pixel and straight back, ending where it was. A browser tab doesn't
// have the problem, so nothing is set up there.

// When to nudge after the move: once the new page is up, and again once its
// animations (0.5s, see shared.css) are over, since either can be the draw
// that goes missing.
const NUDGE_DELAYS_MS = [100, 650];

export function repaintAfterHistoryMoves() {
  if (!window.matchMedia("(display-mode: standalone)").matches) return;

  let fingerIsDown = false;
  let timeoutIds: number[] = [];

  function nudge() {
    // Scrolling by hand redraws the page anyway, and must not be fought.
    if (fingerIsDown) return;
    window.scrollBy(0, 1);
    // A frame later, so that the two can't cancel out before either is drawn
    requestAnimationFrame(() => {
      window.scrollBy(0, -1);
    });
  }

  window.addEventListener("popstate", () => {
    timeoutIds.forEach((id) => {
      window.clearTimeout(id);
    });
    timeoutIds = NUDGE_DELAYS_MS.map((ms) => window.setTimeout(nudge, ms));
  });
  window.addEventListener(
    "touchstart",
    () => {
      fingerIsDown = true;
    },
    { passive: true },
  );
  for (const liftEvent of ["touchend", "touchcancel"]) {
    window.addEventListener(
      liftEvent,
      () => {
        fingerIsDown = false;
      },
      { passive: true },
    );
  }
}
