import ExpandableCard from "../shared/ExpandableCard";
import { type PanelCardProps } from "../shared/usePanelCards";
import { displayDate, getTmdbImageUrl } from "../utilities/utilities";

interface CreditCardProps extends PanelCardProps {
  testId: string;
  posterPath: string | null;
  title: string;
  originalTitle: string;
  // The person's character or job in the credit
  role: string;
  date: string | null;
  dateLabel?: string;
  episodeCount?: number;
}

// Shared layout for the movie / TV series cast / crew credit cards. The card
// can be selected (highlighted and flown to its panel's corner) on the way to
// the movie's / TV series' own panel, or highlighted in its place; see
// usePanelCards.
function CreditCard({
  testId,
  posterPath,
  title,
  originalTitle,
  role,
  date,
  dateLabel,
  episodeCount,
  selected,
  deselecting,
  highlighted,
  onClick,
}: CreditCardProps) {
  const imageUrl = getTmdbImageUrl(posterPath, "w185");

  return (
    <ExpandableCard
      className="credit-card"
      testId={testId}
      selected={selected}
      deselecting={deselecting}
      highlighted={highlighted}
      expanded={false}
      onClick={onClick}
    >
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
      {date && dateLabel && (
        <p className="credit-card-secondary">{dateLabel}:</p>
      )}
      {date && <p className="credit-card-secondary">{displayDate(date)}</p>}
    </ExpandableCard>
  );
}

export default CreditCard;
