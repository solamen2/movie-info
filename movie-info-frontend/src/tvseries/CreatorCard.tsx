import { getTmdbImageUrl } from "../utilities/utilities";
import { type TvSeriesCreator } from "./tvSeriesTypes";

interface CreatorCardProps {
  creator: TvSeriesCreator;
}

function CreatorCard({ creator }: CreatorCardProps) {
  const imageUrl = getTmdbImageUrl(creator.profilePath, "w185");

  return (
    <div className="person-card" data-testid="creator-card">
      {imageUrl ? (
        <img
          src={imageUrl}
          alt={creator.name}
          loading="lazy"
          className="person-card-image"
        />
      ) : (
        <div className="person-card-image person-card-image-placeholder">
          No image
        </div>
      )}
      <p className="person-card-name">{creator.name}</p>
      {creator.originalName && creator.originalName !== creator.name && (
        <p className="person-card-secondary">({creator.originalName})</p>
      )}
    </div>
  );
}

export default CreatorCard;
