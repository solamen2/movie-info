import { getTmdbImageUrl } from "../utilities/utilities";
import { type TvEpisodeCrew } from "./tvEpisodeTypes";

interface CrewCardProps {
  crew: TvEpisodeCrew;
}

function CrewCard({ crew }: CrewCardProps) {
  const imageUrl = getTmdbImageUrl(crew.profilePath, "w185");

  return (
    <div className="person-card" data-testid="crew-card">
      {imageUrl ? (
        <img
          src={imageUrl}
          alt={crew.name}
          loading="lazy"
          className="person-card-image"
        />
      ) : (
        <div className="person-card-image person-card-image-placeholder">
          No image
        </div>
      )}
      <p className="person-card-name">{crew.name}</p>
      {crew.originalName && crew.originalName !== crew.name && (
        <p className="person-card-secondary">({crew.originalName})</p>
      )}
      {crew.job && <p className="person-card-secondary">{crew.job}</p>}
    </div>
  );
}

export default CrewCard;
