import type { AspectRatio } from "./sharedTypes";

// The fact's label: plural only when there are several ratios to show
export function aspectRatiosLabel(aspectRatios: AspectRatio[] | null): string {
  return aspectRatios !== null && aspectRatios.length > 1
    ? "Aspect Ratios"
    : "Aspect Ratio";
}

// A Google search for a title's aspect ratio, for when Wikidata has none.
// The query is `"<title>" <qualifier> aspect ratio` (spaces become "+"),
// where the qualifier tells the title apart from others of the same name,
// e.g. "(1994 film)" or "TV series".
export function aspectRatioGoogleSearchUrl(
  title: string,
  qualifier: string,
): string {
  const params = new URLSearchParams({
    q: `"${title}" ${qualifier} aspect ratio`,
  });
  return `https://www.google.com/search?${params.toString()}`;
}
