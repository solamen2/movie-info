import {
  type ReactNode,
  useEffect,
  useLayoutEffect,
  useRef,
  useState,
} from "react";
import "./shared.css";

interface InfoPopupProps {
  // What is shown in the text, e.g. "*"
  trigger: ReactNode;
  // Describes the trigger for screen readers and tests
  label: string;
  // Whether hovering the mark with a mouse also shows the explanation, for as
  // long as the mouse stays over the mark or the explanation (default false)
  showOnHover?: boolean;
  children: ReactNode;
}

// The least room left between the popup and the edge of the page
const PAGE_MARGIN_PX = 8;

// A small mark in the text (an asterisk, say) that pops up an explanation
// when tapped, and closes on the next tap anywhere or on Escape.
function InfoPopup({
  trigger,
  label,
  showOnHover = false,
  children,
}: InfoPopupProps) {
  // Opened by a tap or click, which keeps it open until the next one
  const [open, setOpen] = useState(false);
  // Only ever set by a mouse: a touch screen also reports a tap as the
  // pointer entering, but never as leaving, which would leave the popup stuck
  const [hovered, setHovered] = useState(false);
  const shown = open || hovered;
  const rootRef = useRef<HTMLSpanElement>(null);
  const contentRef = useRef<HTMLSpanElement>(null);

  // The popup opens rightwards from its mark (see the CSS), so a mark near
  // the right edge of the page would push it off the page: slide it back
  // left by as much as it sticks out, though never past the left edge.
  useLayoutEffect(() => {
    const content = contentRef.current;
    if (!shown || !content) return;
    const box = content.getBoundingClientRect();
    const overflow =
      box.right - (document.documentElement.clientWidth - PAGE_MARGIN_PX);
    if (overflow > 0) {
      const shift = Math.min(overflow, Math.max(box.left - PAGE_MARGIN_PX, 0));
      content.style.left = `${String(-shift)}px`;
    }
  }, [shown]);

  useEffect(() => {
    if (!open) return;
    function onPointerDown(e: PointerEvent) {
      if (!rootRef.current?.contains(e.target as Node)) {
        setOpen(false);
      }
    }
    function onKeyDown(e: KeyboardEvent) {
      if (e.key === "Escape") {
        setOpen(false);
      }
    }
    document.addEventListener("pointerdown", onPointerDown);
    document.addEventListener("keydown", onKeyDown);
    return () => {
      document.removeEventListener("pointerdown", onPointerDown);
      document.removeEventListener("keydown", onKeyDown);
    };
  }, [open]);

  return (
    <span
      className="info-popup"
      ref={rootRef}
      onPointerEnter={(e) => {
        if (showOnHover && e.pointerType === "mouse") {
          setHovered(true);
        }
      }}
      onPointerLeave={() => {
        setHovered(false);
      }}
    >
      <button
        type="button"
        className="info-popup-trigger"
        aria-label={label}
        aria-expanded={shown}
        onClick={() => {
          setOpen((o) => !o);
        }}
      >
        {trigger}
      </button>
      {shown && (
        <span role="tooltip" className="info-popup-content" ref={contentRef}>
          {children}
        </span>
      )}
    </span>
  );
}

export default InfoPopup;
