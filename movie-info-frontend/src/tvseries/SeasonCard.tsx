import Collapsible from "../shared/Collapsible";
import {
  displayDate,
  displayText,
  getTmdbImageUrl,
} from "../utilities/utilities";
import { type TvSeriesSeason, displayEpisodeCount } from "./tvSeriesTypes";

interface SeasonCardProps {
  season: TvSeriesSeason;
}

function SeasonCard({ season }: SeasonCardProps) {
  const posterUrl = getTmdbImageUrl(season.posterPath, "w185");

  return (
    <div className="season-card" data-testid="season-card">
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
      {/* TODO: Load the season's episodes from /api/tvseason when clicked */}
      <button type="button" className="season-card-expand">
        Expand
      </button>
    </div>
  );
}

export default SeasonCard;
