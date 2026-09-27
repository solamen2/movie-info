import { useLayoutEffect, useRef } from "react";
import MoviePanel from "../movie/MoviePanel";
import PersonPanel from "../person/PersonPanel";
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
  // TV series, and people), and card clicks are ignored so interacting with
  // the panel doesn't deselect it — the back arrow / ESC key handle that
  // instead.
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
  deselecting,
  expanded,
  mediaType,
  panelKind,
  onClick,
}: SuggestionSearchCardProps) {
  const cardRef = useRef<HTMLDivElement>(null);
  const searchTypeLabel = getSearchTypeLabel(item.searchType);
  const isPerson = searchTypeLabel === "Person";
  const showYear = !isPerson && !canHaveTvSeriesPanel(mediaType);
  const showYears = !isPerson && canHaveTvSeriesPanel(mediaType);

  // On becoming selected, capture the card's layout offset so CSS can translate
  // it to the upper-left corner. This runs before the browser paints the
  // `.selected` class, and the other cards are still in layout at that point
  // (their `display: none` is deferred until their fade-out ends), so the
  // offset is still the card's slot in the grid. Selection can be triggered
  // by the browser's back / forward buttons as well as by a click, which is
  // why this lives here rather than in the click handler. The results
  // container is position:relative, so it is the card's offsetParent and
  // offsetLeft / offsetTop are already measured from its inner edges.
  // (offsetLeft/offsetTop reflect layout position and ignore any in-flight
  // transform, so this is safe even mid-animation.)
  useLayoutEffect(() => {
    const card = cardRef.current;
    if (selected && card) {
      card.style.setProperty("--orig-x", `${String(card.offsetLeft)}px`);
      card.style.setProperty("--orig-y", `${String(card.offsetTop)}px`);
    }
  }, [selected]);

  function handleClick() {
    if (expanded) return;
    onClick();
  }

  const className =
    "result-card" +
    (selected ? " selected" : "") +
    (deselecting ? " deselecting" : "") +
    (expanded ? " expanded" : "");

  if (expanded) {
    return (
      <div ref={cardRef} id="search-card" className={className}>
        {panelKind === "movie" && <MoviePanel imdbId={item.itemID} />}
        {panelKind === "tvseries" && <TvSeriesPanel imdbId={item.itemID} />}
        {panelKind === "person" && <PersonPanel imdbId={item.itemID} />}
      </div>
    );
  }

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
    </div>
  );
}

export default SuggestionSearchCard;
