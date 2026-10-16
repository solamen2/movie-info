import Collapsible from "../shared/Collapsible";
import OscarWinsAndNominations from "../oscars/OscarWinsAndNominations";
import type { OscarAward } from "../oscars/oscarTypes";

interface PersonOscarsProps {
  // The person's Academy Award wins and nominations: empty when there are
  // none, null only when the Oscars data could not be loaded
  oscars: OscarAward[] | null;
}

// A person's "Oscars" section: their wins, then their nominations, each as
// the category, the year, and the movie.
function PersonOscars({ oscars }: PersonOscarsProps) {
  return (
    <Collapsible title="Oscars" count={oscars?.length}>
      <OscarWinsAndNominations awards={oscars} context="person" />
    </Collapsible>
  );
}

export default PersonOscars;
