import MoviePanel from "../movie/MoviePanel";
import PersonPanel from "../person/PersonPanel";
import ExpandableCard from "../shared/ExpandableCard";
import TvSeriesPanel from "../tvseries/TvSeriesPanel";
import { type MediaResultType, type MediaType } from "../utilities/constants";
import {
  canHaveTvSeriesPanel,
  getSearchTypeLabel,
  type PanelKind,
} from "../utilities/utilities";

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
  mediaType: MediaResultType | null;
  rank: number | null;
  knownFor: string;
  year: number | null;
  years: string | null;
}

interface SuggestionSearchCardProps {
  item: Suggestion;
  selected: boolean;
  deselecting?: boolean;
  // When true the card has grown into a full detail panel (currently movies,
  // TV series, and people).
  expanded?: boolean;
  mediaType: MediaType | null;
  // Which detail panel this card grows into, or null for cards that only
  // highlight when selected.
  panelKind: PanelKind | null;
  onClick: () => void;
}

function SuggestionSearchCard({
  item,
  selected,
  deselecting = false,
  expanded = false,
  mediaType,
  panelKind,
  onClick,
}: SuggestionSearchCardProps) {
  const searchTypeLabel = getSearchTypeLabel(item.searchType);
  const isPerson = searchTypeLabel === "Person";
  const showYear = !isPerson && !canHaveTvSeriesPanel(mediaType);
  const showYears = !isPerson && canHaveTvSeriesPanel(mediaType);

  return (
    <ExpandableCard
      id="search-card"
      className="result-card"
      selected={selected}
      deselecting={deselecting}
      expanded={expanded}
      onClick={onClick}
    >
      {expanded ? (
        <>
          {panelKind === "movie" && <MoviePanel itemId={item.itemID} />}
          {panelKind === "tvseries" && <TvSeriesPanel itemId={item.itemID} />}
          {panelKind === "person" && <PersonPanel itemId={item.itemID} />}
        </>
      ) : (
        <>
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
                <strong>Media Type:</strong> {mediaType ?? "—"}
              </p>
            )}
            <p>
              <strong>Rank:</strong> {item.rank ?? "—"}
            </p>
            <p>
              <strong>Known For:</strong> {item.knownFor}
            </p>
            {showYear && (
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
        </>
      )}
    </ExpandableCard>
  );
}

export default SuggestionSearchCard;
