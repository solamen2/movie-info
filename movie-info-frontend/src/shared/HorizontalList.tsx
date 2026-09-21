import { type ReactNode } from "react";
import "./shared.css";

function HorizontalList({ children }: { children: ReactNode[] }) {
  if (children.length === 0) {
    return <p className="detail-empty">None</p>;
  }
  return <div className="horizontal-list">{children}</div>;
}

export default HorizontalList;
