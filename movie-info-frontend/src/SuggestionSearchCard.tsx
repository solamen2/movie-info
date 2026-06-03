import { useRef } from "react";

export interface SuggestionImage {
  height: number;
  imageURL: string;
  width: number;
}

export interface Suggestion {
  id: string;
  image: SuggestionImage | null;
  itemID: string;
  name: string;
  searchType: number | null;
  mediaType: { value: string } | null;
  rank: number | null;
  knownFor: string;
  year: number | null;
  years: string | null;
}

const SEARCH_TYPE_LABELS: Record<number, string> = {
  0: "Person",
  1: "Media",
};

interface SuggestionSearchCardProps {
  item: Suggestion;
  selected: boolean;
  deselecting?: boolean;
  onClick: () => void;
}

function SuggestionSearchCard({
  item,
  selected,
  deselecting,
  onClick,
}: SuggestionSearchCardProps) {
  const cardRef = useRef<HTMLDivElement>(null);
  const searchTypeLabel =
    item.searchType != null
      ? (SEARCH_TYPE_LABELS[item.searchType] ?? String(item.searchType))
      : "—";
  const isPerson = searchTypeLabel === "Person";
  const mediaTypeValue = item.mediaType?.value;
  const showYears =
    !isPerson &&
    (mediaTypeValue === "TV Series" || mediaTypeValue === "TV Mini Series");

  function handleClick() {
    // When transitioning into the selected state, capture the card's layout
    // offset so CSS can translate it to the upper-left corner. The results
    // container is position:relative, so it is the card's offsetParent and
    // offsetLeft / offsetTop are already measured from its inner edges.
    // (offsetLeft/offsetTop reflect layout position and ignore any in-flight
    // transform, so this is safe even mid-animation.)
    if (!selected) {
      const card = cardRef.current;
      if (card) {
        card.style.setProperty("--orig-x", `${String(card.offsetLeft)}px`);
        card.style.setProperty("--orig-y", `${String(card.offsetTop)}px`);
      }
    }
    onClick();
  }

  const className =
    "result-card" +
    (selected ? " selected" : "") +
    (deselecting ? " deselecting" : "");

  return (
    <div
      ref={cardRef}
      id="search-card"
      className={className}
      onClick={handleClick}
    >
      {item.image ? (
        <img
          src={item.image.imageURL}
          alt={item.name}
          width={item.image.width}
          height={item.image.height}
          className="card-image"
        />
      ) : (
        <div className="card-image-placeholder">No image</div>
      )}
      <div className="card-details">
        <h3 className="card-name">{item.name}</h3>
        <p>
          <strong>Search Type:</strong> {searchTypeLabel}
        </p>
        {!isPerson && (
          <p>
            <strong>Media Type:</strong> {mediaTypeValue ?? "—"}
          </p>
        )}
        <p>
          <strong>Rank:</strong> {item.rank ?? "—"}
        </p>
        <p>
          <strong>Known For:</strong> {item.knownFor}
        </p>
        {!isPerson && (
          <p>
            <strong>Year:</strong> {item.year ?? "—"}
          </p>
        )}
        {showYears && (
          <p>
            <strong>Years:</strong> {item.years ?? "—"}
          </p>
        )}
      </div>
    </div>
  );
}

export default SuggestionSearchCard;
