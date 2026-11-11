import { screen, within } from "@testing-library/react";
import { afterEach, describe, expect, it } from "vitest";
import { http, HttpResponse } from "msw";
import { server } from "../../tests/mocks/node.ts";
import { renderApp } from "../../tests/renderApp";
import { clearDetailDataCache } from "../shared/useDetailData";
import aspectRatiosDataJson1 from "../../tests/mocks/data/aspectRatiosData1.json" with { type: "json" };

async function renderPage() {
  renderApp("/aspect-ratios");
  const page = screen.getByTestId("aspect-ratio-reference-page");
  const items = await within(page).findAllByTestId("aspect-ratio");
  return { page, items };
}

describe("AspectRatioReference", () => {
  afterEach(() => {
    clearDetailDataCache();
  });

  it("Should list every aspect ratio as a bullet, from the smallest decimal to the largest", async () => {
    const { page, items } = await renderPage();

    expect(
      within(page).getByRole("heading", {
        level: 1,
        name: "Aspect Ratio Reference",
      }),
    ).toBeInTheDocument();
    expect(items).toHaveLength(aspectRatiosDataJson1.length);
    expect(items.every((item) => item.tagName === "LI")).toBe(true);
    const decimals = aspectRatiosDataJson1.map((a) => Number(a.decimal));
    expect(decimals).toEqual([...decimals].sort((a, b) => a - b));
    expect(items[0].textContent).toContain("9:16 / 0.56 - Vertical");
    expect(items[items.length - 1].textContent).toContain("4:1 - Polyvision");
  });

  it("Should write each ratio as the panels do, with its description on the page instead of in a popup", async () => {
    const { page, items } = await renderPage();

    const fullscreen = items.find((item) => item.textContent.startsWith("4:3"));
    expect(fullscreen?.textContent).toBe(
      "4:3 / 1.33 - FullscreenThe shape of silent films and of television before HD. A modern widescreen TV shows it with black bars on the left and right.",
    );
    const scope = items.find((item) => item.textContent.startsWith("2.39:1"));
    expect(scope?.textContent).toMatch(/^2\.39:1 - ScopeThe modern/);
    const unnamed = items.find((item) => item.textContent.startsWith("3:2"));
    expect(unnamed?.textContent).toMatch(/^3:2 \/ 1\.50The shape of/);
    expect(within(page).queryByRole("button")).toBeNull();
    expect(within(page).queryByRole("tooltip")).toBeNull();
  });

  it("Should link the name, or the numbers of a ratio without one, to its Wikipedia page when it has one", async () => {
    const { items } = await renderPage();
    const linkIn = (start: string) => {
      const item = items.find((i) => i.textContent.startsWith(start));
      if (!item) throw new Error(`${start} not found`);
      return within(item).queryByRole("link");
    };

    const fullscreen = linkIn("4:3");
    expect(fullscreen?.textContent).toBe("Fullscreen");
    expect(fullscreen).toHaveAttribute(
      "href",
      "https://en.wikipedia.org/wiki/Fullscreen_(aspect_ratio)",
    );
    expect(fullscreen).toHaveAttribute("target", "_blank");

    const unnamedWithPage = linkIn("14:9");
    expect(unnamedWithPage?.textContent).toBe("14:9 / 1.56");
    expect(unnamedWithPage).toHaveAttribute(
      "href",
      "https://en.wikipedia.org/wiki/14:9_aspect_ratio",
    );

    expect(linkIn("3:2")).toBeNull();
    expect(linkIn("1.66:1")).toBeNull();
  });

  it("Should say when the aspect ratios cannot be loaded", async () => {
    server.use(
      http.get(
        "/api/aspectratios",
        () => new HttpResponse(null, { status: 500 }),
      ),
    );
    renderApp("/aspect-ratios");

    expect(
      (await screen.findByText(/Loading aspect ratios failed/)).className,
    ).toBe("error-message");
    expect(
      screen.getByRole("heading", { name: "Aspect Ratio Reference" }),
    ).toBeInTheDocument();
  });
});
