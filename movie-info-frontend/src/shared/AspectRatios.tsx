import AspectRatioText from "./AspectRatioText";
import InfoPopup from "./InfoPopup";
import type { AspectRatio } from "./sharedTypes";
import "./shared.css";

interface AspectRatiosProps {
  // Empty when Wikidata lists none for the title, null when the lookup failed
  aspectRatios: AspectRatio[] | null;
  // Where "Search Google" goes when there are no aspect ratios to show
  searchUrl: string;
}

// A detail panel's aspect ratios, e.g. "4:3 / 1.33 - Fullscreen ⓘ, 16:9 /
// 1.78 - Widescreen ⓘ": each ratio as AspectRatioText writes it, and a mark
// that pops up a short description of it (on hover as well as on a tap).
// Without any ratios, a link to search Google for them instead.
function AspectRatios({ aspectRatios, searchUrl }: AspectRatiosProps) {
  if (aspectRatios === null || aspectRatios.length === 0) {
    return (
      <a href={searchUrl} target="_blank" rel="noopener">
        Search Google
      </a>
    );
  }

  return (
    <>
      {aspectRatios.map((aspectRatio, i) => (
        <span key={aspectRatio.ratio}>
          {i > 0 && " "}
          {/* The comma belongs to the ratio before it, so that it never
              starts a line */}
          <span className="aspect-ratio">
            <AspectRatioText aspectRatio={aspectRatio} />{" "}
            <InfoPopup
              trigger="ⓘ"
              label={`about ${aspectRatio.ratio}`}
              showOnHover
            >
              {aspectRatio.description}
            </InfoPopup>
            {i < aspectRatios.length - 1 && ","}
          </span>
        </span>
      ))}
    </>
  );
}

export default AspectRatios;
