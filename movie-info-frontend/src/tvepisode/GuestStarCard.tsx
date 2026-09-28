import { getTmdbImageUrl } from "../utilities/utilities";
import { type TvEpisodeGuestStar } from "./tvEpisodeTypes";

interface GuestStarCardProps {
  guestStar: TvEpisodeGuestStar;
}

function GuestStarCard({ guestStar }: GuestStarCardProps) {
  const imageUrl = getTmdbImageUrl(guestStar.profilePath, "w185");

  return (
    <div className="person-card" data-testid="guest-star-card">
      {imageUrl ? (
        <img
          src={imageUrl}
          alt={guestStar.name}
          loading="lazy"
          className="person-card-image"
        />
      ) : (
        <div className="person-card-image person-card-image-placeholder">
          No image
        </div>
      )}
      <p className="person-card-name">{guestStar.name}</p>
      {guestStar.originalName && guestStar.originalName !== guestStar.name && (
        <p className="person-card-secondary">({guestStar.originalName})</p>
      )}
      {guestStar.character && (
        <p className="person-card-secondary">{guestStar.character}</p>
      )}
    </div>
  );
}

export default GuestStarCard;
