import { type ReactNode, useLayoutEffect, useRef } from "react";
import "./shared.css";

interface ExpandableCardProps {
  id?: string;
  className: string;
  testId?: string;
  selected: boolean;
  deselecting: boolean;
  // When true the card has grown into a full detail panel, and card clicks are
  // ignored so interacting with the panel doesn't deselect it — the browser's
  // back button / ESC key handle that instead.
  expanded: boolean;
  onClick?: () => void;
  children: ReactNode;
}

// The card's layout position relative to `origin`, as seen on screen: walks
// the offsetParent chain (so it ignores any in-flight transform, making this
// safe mid-animation) and subtracts the scroll of every scrolled ancestor on
// the way (e.g. a horizontally scrolled card row). Without an origin, or if
// the origin isn't an ancestor, the position is relative to the offsetParent.
function offsetWithin(
  card: HTMLElement,
  origin: HTMLElement | null,
): { x: number; y: number } {
  let x = 0;
  let y = 0;
  let element: HTMLElement | null = card;
  while (element && element !== origin) {
    x += element.offsetLeft;
    y += element.offsetTop;
    const offsetParent = element.offsetParent as HTMLElement | null;
    for (
      let scroller: HTMLElement | null = element.parentElement;
      scroller && scroller !== offsetParent?.parentElement;
      scroller = scroller.parentElement
    ) {
      x -= scroller.scrollLeft;
      y -= scroller.scrollTop;
    }
    element = offsetParent;
    if (!origin) break;
  }
  return { x, y };
}

// A card in a list that can be selected (flying to the list's upper-left while
// the other cards fade out) and then expanded to fill the list with a detail
// panel. The animation itself is in CSS, keyed off the classes set here; see
// `.expandable-card` in shared.css.
function ExpandableCard({
  id,
  className,
  testId,
  selected,
  deselecting,
  expanded,
  onClick,
  children,
}: ExpandableCardProps) {
  const cardRef = useRef<HTMLDivElement>(null);

  // On becoming selected, capture the card's layout offset from its
  // `.fly-origin` (the results grid, or the enclosing detail panel) so CSS can
  // translate it to that element's upper-left corner. This runs before the
  // browser paints the `.selected` class, and the other cards / panel content
  // are still in layout at that point (their `display: none` is deferred until
  // their fade-out ends), so the offset is still the card's slot. Selection
  // can be triggered by the browser's back / forward buttons as well as by a
  // click, which is why this lives here rather than in a click handler.
  useLayoutEffect(() => {
    const card = cardRef.current;
    if (selected && card) {
      const origin =
        card.parentElement?.closest<HTMLElement>(".fly-origin") ?? null;
      const { x, y } = offsetWithin(card, origin);
      card.style.setProperty("--orig-x", `${String(x)}px`);
      card.style.setProperty("--orig-y", `${String(y)}px`);
    }
  }, [selected]);

  const classes =
    `${className} expandable-card` +
    (selected ? " selected" : "") +
    (deselecting ? " deselecting" : "") +
    (expanded ? " expanded" : "");

  return (
    <div
      ref={cardRef}
      id={id}
      data-testid={testId}
      className={classes}
      onClick={expanded ? undefined : onClick}
    >
      {children}
    </div>
  );
}

export default ExpandableCard;
