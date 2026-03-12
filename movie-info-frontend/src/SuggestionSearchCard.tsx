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
  onClick: () => void;
}

function SuggestionSearchCard({ item, selected, onClick }: SuggestionSearchCardProps) {
  const searchTypeLabel =
    item.searchType != null
      ? SEARCH_TYPE_LABELS[item.searchType] ?? String(item.searchType)
      : "—";
  const isPerson = searchTypeLabel === "Person";
  const mediaTypeValue = item.mediaType?.value;
  const showYears =
    !isPerson &&
    (mediaTypeValue === "TV Series" || mediaTypeValue === "TV Mini Series");

  return (
    <div
      className={`result-card${selected ? " selected" : ""}`}
      onClick={onClick}
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
            <strong>Media Type:</strong>{" "}
            {mediaTypeValue ? mediaTypeValue : "—"}
          </p>
        )}
        <p><strong>Rank:</strong> {item.rank ?? "—"}</p>
        <p><strong>Known For:</strong> {item.knownFor}</p>
        {!isPerson && <p><strong>Year:</strong> {item.year ?? "—"}</p>}
        {showYears && <p><strong>Years:</strong> {item.years ?? "—"}</p>}
      </div>
    </div>
  );
}

export default SuggestionSearchCard;
