import { type ReactNode } from "react";
import "./shared.css";

interface HorizontalListProps {
  // Set while one of the list's ExpandableCards is selected, so the others
  // fade out and the selected one can take the full width.
  hasSelection?: boolean;
  children: ReactNode[];
}

function HorizontalList({ hasSelection, children }: HorizontalListProps) {
  if (children.length === 0) {
    return <p className="detail-empty">None</p>;
  }
  return (
    <div className={`horizontal-list${hasSelection ? " has-selection" : ""}`}>
      {children}
    </div>
  );
}

export default HorizontalList;
