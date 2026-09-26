import { getTmdbImageUrl } from "../utilities/utilities";
import { type TvSeriesCast, displayEpisodeCount } from "./tvSeriesTypes";

interface CastCardProps {
  cast: TvSeriesCast;
}

function CastCard({ cast }: CastCardProps) {
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
      {cast.totalEpisodeCount > 0 && (
        <p className="person-card-secondary">
          {displayEpisodeCount(cast.totalEpisodeCount)}
        </p>
      )}
    </div>
  );
}

export default CastCard;
