import { getTmdbImageUrl } from "../utilities/utilities";
import { type TvSeriesCast, displayEpisodeCount } from "./tvSeriesTypes";

interface CastCardProps {
  cast: TvSeriesCast;
  // Whether to show how many episodes the cast member appears in (default
  // true); see TvSeriesCastCollapsible for why it can be hidden.
  showEpisodeCount?: boolean;
}

function CastCard({ cast, showEpisodeCount = true }: CastCardProps) {
  const imageUrl = getTmdbImageUrl(cast.profilePath, "w185");
  const characters = cast.characters.filter((c) => c !== "").join(", ");

  return (
    <div className="person-card" data-testid="cast-card">
      {imageUrl ? (
        <img
          src={imageUrl}
          alt={cast.name}
          loading="lazy"
          className="person-card-image"
        />
      ) : (
        <div className="person-card-image person-card-image-placeholder">
          No image
        </div>
      )}
      <p className="person-card-name">{cast.name}</p>
      {cast.originalName && cast.originalName !== cast.name && (
        <p className="person-card-secondary">({cast.originalName})</p>
      )}
      {characters && <p className="person-card-secondary">{characters}</p>}
      {showEpisodeCount && cast.totalEpisodeCount > 0 && (
        <p className="person-card-secondary">
          {displayEpisodeCount(cast.totalEpisodeCount)}
        </p>
      )}
    </div>
  );
}

export default CastCard;
