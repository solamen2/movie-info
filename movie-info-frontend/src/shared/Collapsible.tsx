import { type ReactNode, useEffect, useRef, useState } from "react";
import "./shared.css";

interface CollapsibleProps {
  title: string;
  count?: number;
  // While true the section is kept open (e.g. because a card inside it is
  // selected via the URL). It is never closed automatically, so the user can
  // still collapse it, and it stays open for the deselect animation.
  forceOpen?: boolean;
  // While true, a card inside this section is selected and the enclosing
  // panel has handed itself over to it (see .has-child-selection in
  // shared.css): the section's own heading and frame get out of the way.
  childSelected?: boolean;
  children: ReactNode;
}

function Collapsible({
  title,
  count,
  forceOpen,
  childSelected,
  children,
}: CollapsibleProps) {
  const detailsRef = useRef<HTMLDetailsElement>(null);
  // Open from the very first paint when forced open from the start (e.g. a
  // directly loaded URL selecting a card inside); later changes go through
  // the effect below, so the attribute itself never changes afterwards.
  const [initiallyOpen] = useState(forceOpen ?? false);

  useEffect(() => {
    if (forceOpen && detailsRef.current) {
      detailsRef.current.open = true;
    }
  }, [forceOpen]);

  return (
    <details
      ref={detailsRef}
      open={initiallyOpen}
      className={`collapsible${childSelected ? " has-child-selection" : ""}`}
    >
      <summary>
        <span className="chevron" aria-hidden="true" />
        {title}
        {count != null && <span className="collapsible-count">({count})</span>}
      </summary>
      <div className="collapsible-body">{children}</div>
    </details>
  );
}

export default Collapsible;
