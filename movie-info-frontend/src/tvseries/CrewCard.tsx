import { getTmdbImageUrl } from "../utilities/utilities";
import { type TvSeriesCrew, displayEpisodeCount } from "./tvSeriesTypes";

interface CrewCardProps {
  crew: TvSeriesCrew;
}

function CrewCard({ crew }: CrewCardProps) {
  const imageUrl = getTmdbImageUrl(crew.profilePath, "w185");
  const jobs = crew.jobs.filter((j) => j !== "").join(", ");

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
      {jobs && <p className="person-card-secondary">{jobs}</p>}
      {crew.totalEpisodeCount > 0 && (
        <p className="person-card-secondary">
          {displayEpisodeCount(crew.totalEpisodeCount)}
        </p>
      )}
    </div>
  );
}

export default CrewCard;
