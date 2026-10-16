import Collapsible from "../shared/Collapsible";
import OscarWinsAndNominations from "../oscars/OscarWinsAndNominations";
import type { OscarAward } from "../oscars/oscarTypes";

interface MovieOscarsProps {
  // The movie's Academy Award wins and nominations: empty when there are
  // none, null only when the Oscars data could not be loaded
  oscars: OscarAward[] | null;
}

// A movie's "Oscars" section: its wins, then its nominations, each as the
// category and the people honored.
function MovieOscars({ oscars }: MovieOscarsProps) {
  return (
    <Collapsible title="Oscars" count={oscars?.length}>
      <OscarWinsAndNominations awards={oscars} context="movie" />
    </Collapsible>
  );
}

export default MovieOscars;
