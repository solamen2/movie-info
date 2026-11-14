import { type ReactNode } from "react";
import "./shared.css";

// A label and its value. A value of "—" (what displayText and its relatives
// return for missing data), "" or null means there is nothing to show.
export type DetailFact = [label: string, value: ReactNode];

interface DetailFactsProps {
  facts: DetailFact[];
}

// A detail panel's grid of facts. Facts with nothing to show are left out,
// and so is the whole grid when none remain.
function DetailFacts({ facts }: DetailFactsProps) {
  const shown = facts.filter(
    ([, value]) => value != null && value !== "" && value !== "—",
  );
  if (shown.length === 0) return null;

  return (
    <dl className="detail-facts">
      {shown.map(([label, value]) => (
        <div className="detail-fact" key={label}>
          <dt>{label}</dt>
          <dd>{value}</dd>
        </div>
      ))}
    </dl>
  );
}

export default DetailFacts;
