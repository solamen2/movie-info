import { act, render, screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import DeferredImage from "./DeferredImage";

describe("DeferredImage", () => {
  afterEach(() => {
    vi.unstubAllGlobals();
  });

  it("shows the image at once where there is no IntersectionObserver", () => {
    render(
      <DeferredImage
        src="/a.jpg"
        alt="A"
        className="image"
        placeholderClassName="placeholder"
      />,
    );

    expect(screen.getByAltText("A")).toHaveAttribute("src", "/a.jpg");
  });

  it("holds the image's place until it comes near the screen", () => {
    let notify: IntersectionObserverCallback = () => undefined;
    const unobserve = vi.fn();
    vi.stubGlobal(
      "IntersectionObserver",
      class {
        constructor(callback: IntersectionObserverCallback) {
          notify = callback;
        }
        observe() {
          return undefined;
        }
        unobserve = unobserve;
      },
    );

    const { container } = render(
      <DeferredImage
        src="/a.jpg"
        alt="A"
        className="image"
        placeholderClassName="placeholder"
      />,
    );

    expect(screen.queryByAltText("A")).not.toBeInTheDocument();
    const placeholder = container.querySelector(".image.placeholder");
    expect(placeholder).toBeInTheDocument();

    act(() => {
      notify(
        [
          { target: placeholder, isIntersecting: false },
        ] as IntersectionObserverEntry[],
        {} as IntersectionObserver,
      );
    });
    expect(screen.queryByAltText("A")).not.toBeInTheDocument();

    act(() => {
      notify(
        [
          { target: placeholder, isIntersecting: true },
        ] as IntersectionObserverEntry[],
        {} as IntersectionObserver,
      );
    });
    expect(screen.getByAltText("A")).toHaveClass("image");
    expect(container.querySelector(".placeholder")).not.toBeInTheDocument();
    expect(unobserve).toHaveBeenCalledWith(placeholder);
  });
});
