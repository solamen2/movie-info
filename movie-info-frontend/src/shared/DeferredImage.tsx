import { useEffect, useRef, useState } from "react";

// NOTE: This class was created because large numbers of credit images were loading
// slowly on my iPhone when it was running content blockers. If those are ever
// removed, pages with large numbers of credits (Grey's Anatomy, ER, Law & Order, etc.)
// should be retested without this fix / PR.

// How close to the screen an image has to come before it is created. The
// scroll margin is what reaches sideways into a row of cards that scrolls
// (browsers without it create the image as it scrolls into view instead).
const OBSERVER_OPTIONS = {
  rootMargin: "300px",
  scrollMargin: "600px",
} as IntersectionObserverInit;

// One observer for every image waiting to be shown, with what to do for each
// once it comes near the screen
const onNear = new WeakMap<Element, () => void>();
let observer: IntersectionObserver | null = null;

function observe(element: Element, callback: () => void): () => void {
  observer ??= new IntersectionObserver((entries) => {
    for (const entry of entries) {
      if (entry.isIntersecting) {
        observer?.unobserve(entry.target);
        onNear.get(entry.target)?.();
        onNear.delete(entry.target);
      }
    }
  }, OBSERVER_OPTIONS);
  onNear.set(element, callback);
  observer.observe(element);
  return () => {
    observer?.unobserve(element);
    onNear.delete(element);
  };
}

interface DeferredImageProps {
  src: string;
  alt: string;
  className: string;
  // Added to the box that holds the image's place until it is created
  placeholderClassName: string;
}

// An image in a long list of cards (a TV series' cast, a person's credits)
// that is only created once its card comes near the screen; until then an
// empty box of the same size holds its place. This is instead of
// `loading="lazy"`, which defers the download but not the element: on iOS
// merely giving an <img> a URL costs a millisecond or two (with content
// blockers installed), so a few thousand of them held up the whole panel for
// seconds, even inside a closed section. Once created, the image stays.
function DeferredImage({
  src,
  alt,
  className,
  placeholderClassName,
}: DeferredImageProps) {
  // Without an IntersectionObserver there is no telling, so show it at once
  const [near, setNear] = useState(typeof IntersectionObserver === "undefined");
  const placeholderRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (near || !placeholderRef.current) return;
    return observe(placeholderRef.current, () => {
      setNear(true);
    });
  }, [near]);

  if (near) {
    return <img src={src} alt={alt} className={className} />;
  }
  return (
    <div
      ref={placeholderRef}
      className={`${className} ${placeholderClassName}`}
    />
  );
}

export default DeferredImage;
