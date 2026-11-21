import { act, fireEvent, screen, waitFor } from "@testing-library/react";
import { afterEach, describe, expect, it } from "vitest";
import { http, HttpResponse } from "msw";
import { server } from "../../tests/mocks/node.ts";
import { renderApp } from "../../tests/renderApp";

function footer() {
  const element = document.querySelector("footer");
  if (!element) {
    throw new Error("Footer not found");
  }
  return element;
}

function themeButton(name: string) {
  return screen.getByRole("button", { name });
}

afterEach(() => {
  localStorage.clear();
  delete document.documentElement.dataset.theme;
});

describe("Footer", () => {
  it("Should show the links and the logout button on the search page, after a separator", () => {
    renderApp();

    expect(footer().querySelector("hr")).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Oscars" })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Search" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Logout" })).toBeInTheDocument();
    // The logout button is no longer in the search header
    expect(
      document.querySelector(".search-header button.logout-button"),
    ).toBeNull();
  });

  it("Should open the IMDB Lists and Aspect Ratio Reference pages", async () => {
    const { currentUrl } = renderApp();

    fireEvent.click(screen.getByRole("link", { name: "IMDB Lists" }));
    expect(currentUrl()).toBe("/imdb-lists");
    expect(
      screen.getByRole("heading", { name: "IMDB Lists" }),
    ).toBeInTheDocument();

    fireEvent.click(screen.getByRole("link", { name: "Aspect Ratios" }));
    expect(currentUrl()).toBe("/aspect-ratios");
    expect(
      screen.getByRole("heading", { name: "Aspect Ratio Reference" }),
    ).toBeInTheDocument();
    expect(await screen.findAllByTestId("aspect-ratio")).not.toHaveLength(0);
  });

  it("Should list its links in order, all inside the app", () => {
    renderApp();

    const links = [...footer().querySelectorAll("a")];
    expect(links.map((link) => link.textContent)).toEqual([
      "Search",
      "Oscars",
      "IMDB Lists",
      "Aspect Ratios",
    ]);
    expect(links.map((link) => link.getAttribute("href"))).toEqual([
      "/search",
      "/oscars",
      "/imdb-lists",
      "/aspect-ratios",
    ]);
  });

  it("Should not show at all on the login and register pages, which are seen while logged out", async () => {
    const { router } = renderApp("/login");

    expect(document.querySelector("footer")).toBeNull();
    expect(screen.queryByRole("link", { name: "Oscars" })).toBeNull();
    expect(screen.queryByRole("link", { name: "Search" })).toBeNull();
    expect(screen.queryByRole("button", { name: "Logout" })).toBeNull();

    await act(() => router.navigate("/register"));
    expect(document.querySelector("footer")).toBeNull();

    await act(() => router.navigate("/search"));
    expect(document.querySelector("footer")).not.toBeNull();
  });

  it("Should open the Oscars By Year page, and go back to the search", async () => {
    const { currentUrl } = renderApp();

    fireEvent.click(screen.getByRole("link", { name: "Oscars" }));
    expect(currentUrl()).toBe("/oscars");
    expect(await screen.findByTestId("oscars-page")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("link", { name: "Search" }));
    expect(currentUrl()).toBe("/search");
    expect(
      screen.getByRole("textbox", {
        name: "Search movies, people, and TV shows",
      }),
    ).toBeInTheDocument();
  });

  it("Should log the user out and go to the login page", async () => {
    const { currentUrl } = renderApp();

    fireEvent.click(screen.getByRole("button", { name: "Logout" }));

    await waitFor(() => {
      expect(currentUrl()).toBe("/login");
    });
  });

  it("Should say when logging out failed", async () => {
    server.use(
      http.post("/api/logout", () => new HttpResponse(null, { status: 500 })),
    );
    renderApp();

    fireEvent.click(screen.getByRole("button", { name: "Logout" }));

    expect((await screen.findByText(/Logout failed/)).textContent).toBe(
      "Logout failed. Redirecting to login page...",
    );
  });

  it('Should offer the themes after a "Theme:" label, with Auto picked until another is', () => {
    renderApp();

    const toggle = footer().querySelector(".theme-toggle");
    expect(toggle?.textContent).toBe("Theme:AutoLightDark");
    expect(screen.getByRole("group", { name: "Theme" })).toBeInTheDocument();
    expect(themeButton("Auto")).toHaveAttribute("aria-pressed", "true");
    expect(themeButton("Light")).toHaveAttribute("aria-pressed", "false");
    expect(themeButton("Dark")).toHaveAttribute("aria-pressed", "false");
    // Nothing picked: the CSS follows the OS setting
    expect(document.documentElement.dataset.theme).toBeUndefined();
  });

  it("Should apply a picked theme at once and save it, and go back to following the OS setting with Auto", () => {
    renderApp();

    fireEvent.click(themeButton("Light"));
    expect(themeButton("Light")).toHaveAttribute("aria-pressed", "true");
    expect(themeButton("Auto")).toHaveAttribute("aria-pressed", "false");
    expect(document.documentElement.dataset.theme).toBe("light");
    expect(localStorage.getItem("theme")).toBe("light");

    fireEvent.click(themeButton("Dark"));
    expect(document.documentElement.dataset.theme).toBe("dark");
    expect(localStorage.getItem("theme")).toBe("dark");

    fireEvent.click(themeButton("Auto"));
    expect(themeButton("Auto")).toHaveAttribute("aria-pressed", "true");
    expect(document.documentElement.dataset.theme).toBeUndefined();
    expect(localStorage.getItem("theme")).toBeNull();
  });

  it("Should start with the theme saved on an earlier visit, and ignore a saved value that is not a theme", () => {
    localStorage.setItem("theme", "dark");
    const { unmount } = renderApp();

    expect(themeButton("Dark")).toHaveAttribute("aria-pressed", "true");
    expect(document.documentElement.dataset.theme).toBe("dark");
    unmount();

    localStorage.setItem("theme", "sepia");
    renderApp();
    expect(themeButton("Auto")).toHaveAttribute("aria-pressed", "true");
    expect(document.documentElement.dataset.theme).toBeUndefined();
  });

  it("Should follow a theme picked in another tab", () => {
    renderApp();

    localStorage.setItem("theme", "light");
    act(() => {
      window.dispatchEvent(new StorageEvent("storage", { key: "theme" }));
    });

    expect(themeButton("Light")).toHaveAttribute("aria-pressed", "true");
    expect(document.documentElement.dataset.theme).toBe("light");
  });

  it("Should point the browser's theme colour at the picked theme, and back at the OS setting with Auto", () => {
    for (const scheme of ["light", "dark"]) {
      const meta = document.createElement("meta");
      meta.name = "theme-color";
      meta.dataset.themeColor = scheme;
      meta.media = `(prefers-color-scheme: ${scheme})`;
      document.head.append(meta);
    }
    const mediaOf = (scheme: string) =>
      document.querySelector<HTMLMetaElement>(
        `meta[data-theme-color="${scheme}"]`,
      )?.media;
    renderApp();

    fireEvent.click(themeButton("Light"));
    expect(mediaOf("light")).toBe("all");
    expect(mediaOf("dark")).toBe("not all");

    fireEvent.click(themeButton("Auto"));
    expect(mediaOf("light")).toBe("(prefers-color-scheme: light)");
    expect(mediaOf("dark")).toBe("(prefers-color-scheme: dark)");

    document.querySelectorAll("meta[data-theme-color]").forEach((meta) => {
      meta.remove();
    });
  });
});
