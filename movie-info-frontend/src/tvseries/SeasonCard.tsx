import Collapsible from "../shared/Collapsible";
import ExpandableCard from "../shared/ExpandableCard";
import TvSeasonPanel from "../tvseason/TvSeasonPanel";
import {
  displayDate,
  displayText,
  getTmdbImageUrl,
} from "../utilities/utilities";
import { type TvSeriesSeason, displayEpisodeCount } from "./tvSeriesTypes";

interface SeasonCardProps {
  season: TvSeriesSeason;
  // The TMDB id of the series and its IMDB id, needed by the season panel the
  // card expands into.
  tmdbTvSeriesId: number;
  tvSeriesImdbId: string;
  selected: boolean;
  deselecting: boolean;
  expanded: boolean;
  // Called when the Expand button is clicked; the caller navigates to the
  // season's URL, which is what actually expands the card.
  onExpand: () => void;
}

function SeasonCard({
  season,
  tmdbTvSeriesId,
  tvSeriesImdbId,
  selected,
  deselecting,
  expanded,
  onExpand,
}: SeasonCardProps) {
  const posterUrl = getTmdbImageUrl(season.posterPath, "w185");

  return (
    <ExpandableCard
      className="season-card"
      testId="season-card"
      selected={selected}
      deselecting={deselecting}
      expanded={expanded}
    >
      {expanded ? (
        <TvSeasonPanel
          tmdbTvSeriesId={tmdbTvSeriesId}
          seasonNumber={season.seasonNumber}
          tvSeriesImdbId={tvSeriesImdbId}
        />
      ) : (
        <>
          <div className="season-card-top">
            {posterUrl ? (
              <img
                src={posterUrl}
                alt={season.name}
                loading="lazy"
                className="season-card-image"
              />
            ) : (
              <div className="season-card-image season-card-image-placeholder">
                No image
              </div>
            )}
            <div className="season-card-details">
              <p className="season-card-name">{season.name}</p>
              <p className="season-card-secondary">
                {displayEpisodeCount(season.episodeCount)}
              </p>
              <p className="season-card-secondary">
                {displayDate(season.firstAirDate)}
              </p>
            </div>
          </div>
          <Collapsible title="Overview">
            <p className="detail-prose">{displayText(season.overview)}</p>
          </Collapsible>
          <button
            type="button"
            className="season-card-expand"
            onClick={onExpand}
          >
            Expand
          </button>
        </>
      )}
    </ExpandableCard>
  );
}

export default SeasonCard;
