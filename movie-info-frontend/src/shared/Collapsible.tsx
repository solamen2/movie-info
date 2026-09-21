import { type ReactNode } from "react";
import "./shared.css";

interface CollapsibleProps {
  title: string;
  count?: number;
  children: ReactNode;
}

function Collapsible({ title, count, children }: CollapsibleProps) {
  return (
    <details className="collapsible">
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
