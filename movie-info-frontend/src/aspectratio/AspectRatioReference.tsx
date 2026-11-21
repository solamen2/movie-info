import AspectRatioText from "../shared/AspectRatioText";
import { useDetailData } from "../shared/useDetailData";
import "../shared/shared.css";
import { usePageTitle } from "../utilities/pageTitle";

// Every aspect ratio the app knows by name or description, narrowest first
// (the backend sends them in that order), each with the description that a
// movie's or TV series' panel shows in a popup.
function AspectRatioReference() {
  const { data: aspectRatios, error } = useDetailData(
    "/api/aspectratios",
    "aspect ratios",
  );

  usePageTitle("Aspect Ratio Reference");

  return (
    <div className="reference-page" data-testid="aspect-ratio-reference-page">
      <h1>Aspect Ratio Reference</h1>
      {error ? (
        <p className="error-message" role="alert">
          {error}
        </p>
      ) : !aspectRatios ? (
        <p className="detail-loading" role="status">
          Loading aspect ratios…
        </p>
      ) : (
        <ul className="reference-list">
          {aspectRatios.map((aspectRatio) => (
            <li key={aspectRatio.ratio} data-testid="aspect-ratio">
              <span className="reference-list-title">
                <AspectRatioText aspectRatio={aspectRatio} />
              </span>
              <p className="reference-list-description">
                {aspectRatio.description}
              </p>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

export default AspectRatioReference;
