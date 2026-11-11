import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import AspectRatios from "./AspectRatios";
import {
  aspectRatioGoogleSearchUrl,
  aspectRatiosLabel,
} from "./aspectRatioUtils";
import type { AspectRatio } from "./sharedTypes";

const SEARCH_URL = "https://www.google.com/search?q=example";

const FULLSCREEN: AspectRatio = {
  ratio: "4:3",
  decimal: "1.33",
  name: "Fullscreen",
  description: "The shape of television before HD.",
  wikipediaLink: "https://en.wikipedia.org/wiki/Fullscreen_(aspect_ratio)",
};
const SCOPE: AspectRatio = {
  ratio: "2.39:1",
  decimal: "2.39",
  name: "Scope",
  description: "The modern anamorphic widescreen standard.",
  wikipediaLink: "https://en.wikipedia.org/wiki/Anamorphic_format",
};
const UNNAMED: AspectRatio = {
  ratio: "3:2",
  decimal: "1.50",
  name: null,
  description: "The shape of a 35 mm still photograph.",
  wikipediaLink: null,
};
const UNNAMED_DECIMAL: AspectRatio = {
  ratio: "1.75:1",
  decimal: "1.75",
  name: null,
  description: "An early widescreen shape.",
  wikipediaLink: null,
};
const UNNAMED_WITH_PAGE: AspectRatio = {
  ratio: "14:9",
  decimal: "1.56",
  name: null,
  description: "A compromise between 4:3 and 16:9.",
  wikipediaLink: "https://en.wikipedia.org/wiki/14:9_aspect_ratio",
};
const NAMED_WITHOUT_PAGE: AspectRatio = {
  ratio: "1.66:1",
  decimal: "1.66",
  name: "European widescreen",
  description: "An early widescreen shape.",
  wikipediaLink: null,
};

function renderAspectRatios(aspectRatios: AspectRatio[] | null) {
  return render(
    <AspectRatios aspectRatios={aspectRatios} searchUrl={SEARCH_URL} />,
  );
}

describe("AspectRatios", () => {
  it("Should show the ratio, its decimal form, and its name", () => {
    const { container } = renderAspectRatios([FULLSCREEN]);
    expect(container.textContent).toBe("4:3 / 1.33 - Fullscreen ⓘ");
  });

  it("Should not repeat a ratio that is already written as a decimal", () => {
    const { container } = renderAspectRatios([SCOPE]);
    expect(container.textContent).toBe("2.39:1 - Scope ⓘ");
  });

  it("Should show a ratio without a name on its own, still with its info mark", () => {
    expect(renderAspectRatios([UNNAMED]).container.textContent).toBe(
      "3:2 / 1.50 ⓘ",
    );
    expect(renderAspectRatios([UNNAMED_DECIMAL]).container.textContent).toBe(
      "1.75:1 ⓘ",
    );
  });

  it("Should separate several ratios with commas", () => {
    const { container } = renderAspectRatios([FULLSCREEN, UNNAMED, SCOPE]);
    expect(container.textContent).toBe(
      "4:3 / 1.33 - Fullscreen ⓘ, 3:2 / 1.50 ⓘ, 2.39:1 - Scope ⓘ",
    );
  });

  it("Should link a ratio's name to its Wikipedia page, in a new tab", () => {
    renderAspectRatios([FULLSCREEN, SCOPE]);

    const links = screen.getAllByRole("link");
    expect(links.map((link) => link.textContent)).toEqual([
      "Fullscreen",
      "Scope",
    ]);
    expect(links[0]).toHaveAttribute(
      "href",
      "https://en.wikipedia.org/wiki/Fullscreen_(aspect_ratio)",
    );
    expect(links[0]).toHaveAttribute("target", "_blank");
    expect(links[1]).toHaveAttribute(
      "href",
      "https://en.wikipedia.org/wiki/Anamorphic_format",
    );
  });

  it("Should link the numbers of a ratio without a name instead", () => {
    const { container } = renderAspectRatios([UNNAMED_WITH_PAGE]);

    expect(container.textContent).toBe("14:9 / 1.56 ⓘ");
    const link = screen.getByRole("link", { name: "14:9 / 1.56" });
    expect(link).toHaveAttribute(
      "href",
      "https://en.wikipedia.org/wiki/14:9_aspect_ratio",
    );
  });

  it("Should not link a ratio without a Wikipedia page", () => {
    const { container } = renderAspectRatios([
      UNNAMED,
      UNNAMED_DECIMAL,
      NAMED_WITHOUT_PAGE,
    ]);

    expect(container.textContent).toBe(
      "3:2 / 1.50 ⓘ, 1.75:1 ⓘ, 1.66:1 - European widescreen ⓘ",
    );
    expect(screen.queryByRole("link")).toBeNull();
  });

  it("Should pop up each ratio's description from its own info mark", () => {
    renderAspectRatios([FULLSCREEN, UNNAMED]);
    expect(screen.queryByRole("tooltip")).toBeNull();

    fireEvent.click(screen.getByRole("button", { name: "about 4:3" }));
    expect(screen.getByRole("tooltip").textContent).toBe(
      "The shape of television before HD.",
    );

    fireEvent.click(screen.getByRole("button", { name: "about 4:3" }));
    expect(screen.queryByRole("tooltip")).toBeNull();

    fireEvent.click(screen.getByRole("button", { name: "about 3:2" }));
    expect(screen.getByRole("tooltip").textContent).toBe(
      "The shape of a 35 mm still photograph.",
    );
  });

  it("Should show the description while the mouse is over the info mark", () => {
    renderAspectRatios([FULLSCREEN, UNNAMED]);
    const mark = screen.getByRole("button", { name: "about 3:2" });

    fireEvent.pointerEnter(mark, { pointerType: "mouse" });
    expect(screen.getByRole("tooltip").textContent).toBe(
      "The shape of a 35 mm still photograph.",
    );
    expect(mark).toHaveAttribute("aria-expanded", "true");

    fireEvent.pointerLeave(mark, { pointerType: "mouse" });
    expect(screen.queryByRole("tooltip")).toBeNull();
  });

  it("Should keep a clicked description open after the mouse leaves", () => {
    renderAspectRatios([FULLSCREEN]);
    const mark = screen.getByRole("button", { name: "about 4:3" });

    fireEvent.pointerEnter(mark, { pointerType: "mouse" });
    fireEvent.click(mark);
    fireEvent.pointerLeave(mark, { pointerType: "mouse" });
    expect(screen.getByRole("tooltip")).toBeInTheDocument();

    fireEvent.click(mark);
    expect(screen.queryByRole("tooltip")).toBeNull();
  });

  it("Should not treat a touch as a hover, which would never end", () => {
    renderAspectRatios([FULLSCREEN]);
    const mark = screen.getByRole("button", { name: "about 4:3" });

    fireEvent.pointerEnter(mark, { pointerType: "touch" });
    expect(screen.queryByRole("tooltip")).toBeNull();

    fireEvent.click(mark);
    expect(screen.getByRole("tooltip")).toBeInTheDocument();
    fireEvent.click(mark);
    expect(screen.queryByRole("tooltip")).toBeNull();
  });

  it.each([
    ["no aspect ratios", []],
    ["a failed lookup", null],
  ])("Should link to a Google search instead for %s", (_, aspectRatios) => {
    const { container } = renderAspectRatios(aspectRatios);

    expect(container.textContent).toBe("Search Google");
    const link = screen.getByRole("link", { name: "Search Google" });
    expect(link).toHaveAttribute("href", SEARCH_URL);
    expect(link).toHaveAttribute("target", "_blank");
    expect(screen.queryByRole("button")).toBeNull();
  });
});

describe("aspectRatiosLabel", () => {
  it("Should be plural only for several ratios", () => {
    expect(aspectRatiosLabel([FULLSCREEN, SCOPE])).toBe("Aspect Ratios");
    expect(aspectRatiosLabel([FULLSCREEN])).toBe("Aspect Ratio");
    expect(aspectRatiosLabel([])).toBe("Aspect Ratio");
    expect(aspectRatiosLabel(null)).toBe("Aspect Ratio");
  });
});

describe("aspectRatioGoogleSearchUrl", () => {
  it("Should search for the quoted title, its qualifier, and 'aspect ratio'", () => {
    expect(aspectRatioGoogleSearchUrl("Example Movie", "(2016 film)")).toBe(
      "https://www.google.com/search?q=%22Example+Movie%22+%282016+film%29+aspect+ratio",
    );
    expect(aspectRatioGoogleSearchUrl("Tom & Jerry", "TV series")).toBe(
      "https://www.google.com/search?q=%22Tom+%26+Jerry%22+TV+series+aspect+ratio",
    );
  });
});
