import ExpandableCard from "./ExpandableCard";
import { type PanelCardProps } from "./usePanelCards";
import { getTmdbImageUrl } from "../utilities/utilities";
import "./shared.css";

interface PersonCardProps extends PanelCardProps {
  testId: string;
  name: string;
  originalName: string;
  profilePath: string | null;
  // Secondary lines shown under the name (character, job, episode count, ...);
  // empty ones are skipped.
  details: string[];
}

// Shared layout for the cast / crew / creator / guest star cards shown in the
// horizontally scrolling rows of the movie, TV series and TV episode panels.
// The card can be selected (highlighted and flown to its panel's corner) on
// the way to the person's own panel, or highlighted in its place; see
// usePanelCards.
function PersonCard({
  testId,
  name,
  originalName,
  profilePath,
  details,
  selected,
  deselecting,
  highlighted,
  onClick,
}: PersonCardProps) {
  const imageUrl = getTmdbImageUrl(profilePath, "w185");

  return (
    <ExpandableCard
      className="person-card"
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
          alt={name}
          loading="lazy"
          className="person-card-image"
        />
      ) : (
        <div className="person-card-image person-card-image-placeholder">
          No image
        </div>
      )}
      <p className="person-card-name">{name}</p>
      {originalName && originalName !== name && (
        <p className="person-card-secondary">({originalName})</p>
      )}
      {details
        .filter((detail) => detail !== "")
        .map((detail, i) => (
          <p className="person-card-secondary" key={i}>
            {detail}
          </p>
        ))}
    </ExpandableCard>
  );
}

export default PersonCard;
