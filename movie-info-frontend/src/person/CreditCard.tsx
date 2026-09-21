import { displayDate, getTmdbImageUrl } from "../utilities/utilities";

interface CreditCardProps {
  testId: string;
  posterPath: string | null;
  title: string;
  originalTitle: string;
  // The person's character or job in the credit
  role: string;
  date: string | null;
  episodeCount?: number;
}

// Shared layout for the movie / TV series cast / crew credit cards.
function CreditCard({
  testId,
  posterPath,
  title,
  originalTitle,
  role,
  date,
  episodeCount,
}: CreditCardProps) {
  const imageUrl = getTmdbImageUrl(posterPath, "w185");

  return (
    <div className="credit-card" data-testid={testId}>
      {imageUrl ? (
        <img
          src={imageUrl}
          alt={title}
          loading="lazy"
          className="credit-card-image"
        />
      ) : (
        <div className="credit-card-image credit-card-image-placeholder">
          No image
        </div>
      )}
      <p className="credit-card-name">{title}</p>
      {originalTitle && originalTitle !== title && (
        <p className="credit-card-secondary">({originalTitle})</p>
      )}
      {role && <p className="credit-card-secondary">{role}</p>}
      {episodeCount != null && episodeCount > 0 && (
        <p className="credit-card-secondary">
          {episodeCount} {episodeCount === 1 ? "episode" : "episodes"}
        </p>
      )}
      {date && <p className="credit-card-secondary">{displayDate(date)}</p>}
    </div>
  );
}

export default CreditCard;
