import Collapsible from "../shared/Collapsible";

interface RtCriticsConsensusProps {
  // Rotten Tomatoes' critics consensus, as quoted on the movie's English
  // Wikipedia page: "" when there is no page or it quotes none, null when the
  // lookup failed
  consensus: string | null;
}

// A movie's "RT Critics Consensus" section: the one or two sentences Rotten
// Tomatoes sums the reviews up with. Left out when Wikipedia quotes none.
function RtCriticsConsensus({ consensus }: RtCriticsConsensusProps) {
  if (consensus === "") return null;

  return (
    <Collapsible title="RT Critics Consensus">
      {consensus === null ? (
        <p className="detail-empty">
          Critics consensus data from Wikipedia not available
        </p>
      ) : (
        <p className="detail-prose">{consensus}</p>
      )}
    </Collapsible>
  );
}

export default RtCriticsConsensus;
