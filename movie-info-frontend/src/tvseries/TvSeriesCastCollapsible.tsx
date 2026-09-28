import { useId, useState } from "react";
import Collapsible from "../shared/Collapsible";
import HorizontalList from "../shared/HorizontalList";
import CastCard from "./CastCard";
import { type TvSeriesCast } from "./tvSeriesTypes";
import "./tvseries.css";

interface TvSeriesCastCollapsibleProps {
  cast: TvSeriesCast[];
}

// The TV series Cast section: a Collapsible whose cards can hide how many
// episodes each cast member appears in, since that can be a spoiler. The
// checkbox defaults to hiding them and remembers the user's choice for as long
// as this component stays mounted (it is deliberately not part of the URL).
function TvSeriesCastCollapsible({ cast }: TvSeriesCastCollapsibleProps) {
  const [hideEpisodeCount, setHideEpisodeCount] = useState(true);
  const checkboxId = useId();
  const sortedCast = [...cast].sort((a, b) => a.billedOrder - b.billedOrder);

  return (
    <Collapsible title="Cast" count={cast.length}>
      <div className="collapsible-option">
        <input
          id={checkboxId}
          type="checkbox"
          checked={hideEpisodeCount}
          onChange={(e) => {
            setHideEpisodeCount(e.target.checked);
          }}
        />
        <label htmlFor={checkboxId}>Hide number of episodes</label>
      </div>
      <HorizontalList>
        {sortedCast.map((c) => (
          <CastCard key={c.id} cast={c} showEpisodeCount={!hideEpisodeCount} />
        ))}
      </HorizontalList>
    </Collapsible>
  );
}

export default TvSeriesCastCollapsible;
