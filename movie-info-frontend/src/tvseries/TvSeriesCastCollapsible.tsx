import { useId, useState } from "react";
import Collapsible from "../shared/Collapsible";
import HorizontalList from "../shared/HorizontalList";
import { type PanelCards } from "../shared/usePanelCards";
import CastCard from "./CastCard";
import { type TvSeriesCast } from "./tvSeriesTypes";
import "./tvseries.css";

// The section's card type in the URL (see usePanelCards)
export const TV_SERIES_CAST_CARD_TYPE = "cast";

interface TvSeriesCastCollapsibleProps {
  cast: TvSeriesCast[];
  cards: PanelCards;
}

// The TV series Cast section: a Collapsible whose cards can hide how many
// episodes each cast member appears in, since that can be a spoiler. The
// checkbox defaults to hiding them and remembers the user's choice for as long
// as this component stays mounted (it is deliberately not part of the URL).
function TvSeriesCastCollapsible({
  cast,
  cards,
}: TvSeriesCastCollapsibleProps) {
  const [hideEpisodeCount, setHideEpisodeCount] = useState(true);
  const checkboxId = useId();
  const sortedCast = [...cast].sort((a, b) => a.billedOrder - b.billedOrder);
  const selected = cards.isSelectedIn(TV_SERIES_CAST_CARD_TYPE);

  return (
    <Collapsible
      title="Cast"
      count={cast.length}
      {...cards.sectionProps(TV_SERIES_CAST_CARD_TYPE)}
    >
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
      <HorizontalList hasSelection={selected}>
        {sortedCast.map((c) => (
          <CastCard
            key={c.id}
            cast={c}
            showEpisodeCount={!hideEpisodeCount}
            {...cards.cardProps(TV_SERIES_CAST_CARD_TYPE, "person", c.tmdbId)}
          />
        ))}
      </HorizontalList>
    </Collapsible>
  );
}

export default TvSeriesCastCollapsible;
