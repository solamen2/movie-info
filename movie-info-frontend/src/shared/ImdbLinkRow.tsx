import ExternalLinkRow from "./ExternalLinkRow";
import { displayText } from "../utilities/utilities";
import "./shared.css";

interface ImdbLinkRowProps {
  url: string;
  // As OMDB sends them; missing ones ("N/A", "") are left out
  rating?: string;
  votes?: string;
  rank?: number | null;
}

// A detail panel's IMDB row: the rating and number of votes, when known, and
// the link to the IMDB page; then the IMDB rank, when known, on a line of its
// own as an indented bullet.
function ImdbLinkRow({ url, rating, votes, rank }: ImdbLinkRowProps) {
  const shownRating = displayText(rating);
  const shownVotes = displayText(votes);

  return (
    <>
      <ExternalLinkRow url={url} label="IMDB:" copyAriaLabel="Copy IMDB link">
        {shownRating === "—" ? null : (
          <>
            {" "}
            {shownRating}
            {shownVotes !== "—" && (
              <>
                {" "}
                <span className="external-link-row-votes">({shownVotes})</span>
              </>
            )}
          </>
        )}
      </ExternalLinkRow>
      {rank != null && (
        <ul className="external-link-row-details">
          <li>
            <span className="external-link-row-label">Rank:</span> {rank}
          </li>
        </ul>
      )}
    </>
  );
}

export default ImdbLinkRow;
