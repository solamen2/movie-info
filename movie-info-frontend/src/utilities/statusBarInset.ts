// Opened from the iOS Home Screen (iOS 26), the page first rests at a negative
// scroll position (-54 on an iPhone with a notch), which keeps it clear of the
// status bar. The first scroll, or the keyboard opening, removes that offset
// for good: the page then rests at 0, under the status bar, where iOS blurs
// it, and it cannot be scrolled back. Once that has happened, tell the
// stylesheet how much room was lost (--lost-top-inset, see index.css), so it
// can pad the page by the same amount instead.
//
// Only an app opened from the Home Screen gets any of this: nothing is set up
// in a browser tab. Even there, only a negative resting position (which
// other platforms' installed apps don't have) leads to any padding.
//
// On a page outside the app's scope (see manifest.json; any page but the one
// it was added from, for an app added before the manifest existed), iOS shows
// a bar with the site's domain, which keeps the page clear of the status bar
// by itself, so no padding is wanted while that bar is up.

// The page is a status bar shorter than the screen (62px of 874) without the
// domain bar, and far shorter with it (192px).
const DOMAIN_BAR_MIN_HEIGHT_LOSS_PX = 120;

export function keepClearOfStatusBar() {
  if (!window.matchMedia("(display-mode: standalone)").matches) return;

  let restingY: number | null = null;
  let offsetIsLost = false;

  // The position before the very first touch, i.e. before anything can have
  // scrolled the page.
  function noteRestingPosition() {
    restingY = window.scrollY;
  }

  function updatePadding() {
    if (restingY === null || restingY >= 0) return;
    offsetIsLost ||= window.scrollY >= 0;
    const domainBarIsUp =
      window.screen.height - window.innerHeight > DOMAIN_BAR_MIN_HEIGHT_LOSS_PX;
    document.documentElement.style.setProperty(
      "--lost-top-inset",
      offsetIsLost && !domainBarIsUp ? `${String(-restingY)}px` : "0px",
    );
  }

  // These last as long as the page does: this runs once, at startup, and the
  // padding has to keep following the domain bar as pages come and go.
  window.addEventListener("touchstart", noteRestingPosition, {
    capture: true,
    passive: true,
    once: true,
  });
  window.addEventListener("scroll", updatePadding, { passive: true });
  window.addEventListener("resize", updatePadding);
  window.visualViewport?.addEventListener("scroll", updatePadding);
}
