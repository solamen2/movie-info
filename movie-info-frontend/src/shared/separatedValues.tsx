import { Fragment, type ReactNode } from "react";
import { ExternalLinkRowSeparator } from "./ExternalLinkRow";

// Several values for one fact (e.g. OMDB's and TMDB's runtime), with a
// separator between them. Missing ones ("—") are left out; "—" when none
// remain, so the fact itself is left out (see DetailFacts).
export function separatedValues(values: string[]): ReactNode {
  const present = values.filter((value) => value !== "—");
  if (present.length === 0) return "—";
  return present.map((value, i) => (
    <Fragment key={i}>
      {i > 0 && <ExternalLinkRowSeparator />}
      {value}
    </Fragment>
  ));
}
