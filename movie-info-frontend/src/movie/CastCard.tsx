import { type MovieCast, tmdbImageUrl } from "./movieTypes";

interface CastCardProps {
  cast: MovieCast;
}

function CastCard({ cast }: CastCardProps) {
  const imageUrl = tmdbImageUrl(cast.profilePath, "w185");

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
      {cast.character && (
        <p className="person-card-secondary">{cast.character}</p>
      )}
    </div>
  );
}

export default CastCard;
