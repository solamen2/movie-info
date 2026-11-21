import { type ReactNode } from "react";
import "./shared.css";

interface HorizontalListProps {
  // Set while one of the list's ExpandableCards is selected, so the others
  // fade out and the selected one can take the full width.
  hasSelection?: boolean;
  // Names a list whose cards can't be given the keyboard focus (they do
  // nothing when clicked): the list itself then can, so that the keyboard
  // can scroll it sideways.
  label?: string;
  children: ReactNode[];
}

function HorizontalList({
  hasSelection,
  label,
  children,
}: HorizontalListProps) {
  if (children.length === 0) {
    return <p className="detail-empty">None</p>;
  }
  return (
    <div
      className={`horizontal-list${hasSelection ? " has-selection" : ""}`}
      role={label ? "group" : undefined}
      aria-label={label}
      tabIndex={label ? 0 : undefined}
    >
      {children}
    </div>
  );
}

export default HorizontalList;
