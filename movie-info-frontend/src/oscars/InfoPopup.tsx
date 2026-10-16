import { type ReactNode, useEffect, useRef, useState } from "react";
import "./oscars.css";

interface InfoPopupProps {
  // What is shown in the text, e.g. "*"
  trigger: ReactNode;
  // Describes the trigger for screen readers and tests
  label: string;
  children: ReactNode;
}

// A small mark in the text (an asterisk, say) that pops up an explanation
// when tapped, and closes on the next tap anywhere or on Escape.
function InfoPopup({ trigger, label, children }: InfoPopupProps) {
  const [open, setOpen] = useState(false);
  const rootRef = useRef<HTMLSpanElement>(null);

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
    <span className="info-popup" ref={rootRef}>
      <button
        type="button"
        className="info-popup-trigger"
        aria-label={label}
        aria-expanded={open}
        onClick={() => {
          setOpen((o) => !o);
        }}
      >
        {trigger}
      </button>
      {open && (
        <span role="tooltip" className="info-popup-content">
          {children}
        </span>
      )}
    </span>
  );
}

export default InfoPopup;
