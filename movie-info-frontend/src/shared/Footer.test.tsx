import { act, fireEvent, screen, waitFor } from "@testing-library/react";
import { describe, expect, it } from "vitest";
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

describe("Footer", () => {
  it("Should show the links and the logout button on the search page, after a separator", () => {
    renderApp();

    expect(footer().querySelector("hr")).toBeInTheDocument();
    expect(
      screen.getByRole("link", { name: "Oscars By Year" }),
    ).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Search" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "logout" })).toBeInTheDocument();
    // The logout button is no longer in the search header
    expect(
      document.querySelector(".search-header button.logout-button"),
    ).toBeNull();
  });

  it("Should not show at all on the login and register pages, which are seen while logged out", async () => {
    const { router } = renderApp("/login");

    expect(document.querySelector("footer")).toBeNull();
    expect(screen.queryByRole("link", { name: "Oscars By Year" })).toBeNull();
    expect(screen.queryByRole("link", { name: "Search" })).toBeNull();
    expect(screen.queryByRole("button", { name: "logout" })).toBeNull();

    await act(() => router.navigate("/register"));
    expect(document.querySelector("footer")).toBeNull();

    await act(() => router.navigate("/search"));
    expect(document.querySelector("footer")).not.toBeNull();
  });

  it("Should open the Oscars By Year page, and go back to the search", async () => {
    const { currentUrl } = renderApp();

    fireEvent.click(screen.getByRole("link", { name: "Oscars By Year" }));
    expect(currentUrl()).toBe("/oscars");
    expect(await screen.findByTestId("oscars-page")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("link", { name: "Search" }));
    expect(currentUrl()).toBe("/search");
    expect(
      screen.getByRole("textbox", { name: "search-query-input" }),
    ).toBeInTheDocument();
  });

  it("Should log the user out and go to the login page", async () => {
    const { currentUrl } = renderApp();

    fireEvent.click(screen.getByRole("button", { name: "logout" }));

    await waitFor(() => {
      expect(currentUrl()).toBe("/login");
    });
  });

  it("Should say when logging out failed", async () => {
    server.use(
      http.post("/api/logout", () => new HttpResponse(null, { status: 500 })),
    );
    renderApp();

    fireEvent.click(screen.getByRole("button", { name: "logout" }));

    expect((await screen.findByText(/Logout failed/)).textContent).toBe(
      "Logout failed. Redirecting to login page...",
    );
  });
});
