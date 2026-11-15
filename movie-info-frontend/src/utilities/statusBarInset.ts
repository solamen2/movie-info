// Opened from the iOS Home Screen (iOS 26), the page first rests at a negative
// scroll position (-54 on an iPhone with a notch), which keeps it clear of the
// status bar. The first scroll, or the keyboard opening, removes that offset
// for good: the page then rests at 0, under the status bar, where iOS blurs
// it, and it cannot be scrolled back. Once that has happened, tell the
// stylesheet how much room was lost (--lost-top-inset, see index.css), so it
// can pad the page by the same amount instead. Nothing happens in a browser
// tab, where the resting position is never negative.
export function keepClearOfStatusBar() {
  let restingY: number | null = null;

  // The position before the very first touch, i.e. before anything can have
  // scrolled the page.
  function noteRestingPosition() {
    restingY ??= window.scrollY;
  }

  function padOnceOffsetIsLost() {
    if (restingY === null || restingY >= 0 || window.scrollY < 0) return;
    document.documentElement.style.setProperty(
      "--lost-top-inset",
      `${String(-restingY)}px`,
    );
    window.removeEventListener("touchstart", noteRestingPosition, true);
    window.removeEventListener("scroll", padOnceOffsetIsLost);
    window.visualViewport?.removeEventListener("scroll", padOnceOffsetIsLost);
  }

  window.addEventListener("touchstart", noteRestingPosition, {
    capture: true,
    passive: true,
  });
  window.addEventListener("scroll", padOnceOffsetIsLost, { passive: true });
  window.visualViewport?.addEventListener("scroll", padOnceOffsetIsLost);
}
