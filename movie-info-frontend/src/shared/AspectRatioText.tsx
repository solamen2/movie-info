import type { AspectRatio } from "./sharedTypes";

// A ratio written as "N:1" (e.g. "2.39:1") already is its decimal form, so
// the decimal is only shown beside the others (e.g. "4:3 / 1.33").
function showsDecimal(aspectRatio: AspectRatio): boolean {
  return !aspectRatio.ratio.endsWith(":1");
}

// An aspect ratio as the app writes it, e.g. "4:3 / 1.33 - Fullscreen": the
// ratio, its decimal form, and its common name when it has one. The name (or
// the numbers, for a ratio without a name) links to the ratio's Wikipedia
// page when it has one.
function AspectRatioText({ aspectRatio }: { aspectRatio: AspectRatio }) {
  const numbers = `${aspectRatio.ratio}${showsDecimal(aspectRatio) ? ` / ${aspectRatio.decimal}` : ""}`;
  const linked = aspectRatio.name ?? numbers;
  const link =
    aspectRatio.wikipediaLink !== null ? (
      <a href={aspectRatio.wikipediaLink} target="_blank" rel="noopener">
        {linked}
      </a>
    ) : (
      linked
    );
  return aspectRatio.name !== null ? (
    <>
      {numbers} - {link}
    </>
  ) : (
    <>{link}</>
  );
}

export default AspectRatioText;
