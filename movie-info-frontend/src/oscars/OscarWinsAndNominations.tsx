import OscarAwardLine from "./OscarAwardLine";
import type { OscarAward } from "./oscarTypes";
import "./oscars.css";

interface OscarWinsAndNominationsProps {
  // Empty when there are none; null only when the Oscars data could not be
  // loaded
  awards: OscarAward[] | null;
  context: "movie" | "person";
}

function AwardList({
  title,
  awards,
  context,
}: {
  title: string;
  awards: OscarAward[];
  context: "movie" | "person";
}) {
  return (
    <div className="oscar-award-group" data-testid={`oscar-${title}`}>
      <h4 className="oscar-award-group-title">{title}</h4>
      {awards.length === 0 ? (
        <p className="detail-empty">None</p>
      ) : (
        <ul className="oscar-award-list">
          {awards.map((award, i) => (
            <li key={i}>
              <OscarAwardLine award={award} context={context} />
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

// The body of a movie's or person's "Oscars" section: their wins, then
// their nominations.
function OscarWinsAndNominations({
  awards,
  context,
}: OscarWinsAndNominationsProps) {
  if (awards === null) {
    return <p className="detail-empty">Oscars data unavailable</p>;
  }
  return (
    <>
      <AwardList
        title="Wins"
        awards={awards.filter((a) => a.winner)}
        context={context}
      />
      <AwardList
        title="Nominations"
        awards={awards.filter((a) => !a.winner)}
        context={context}
      />
    </>
  );
}

export default OscarWinsAndNominations;
