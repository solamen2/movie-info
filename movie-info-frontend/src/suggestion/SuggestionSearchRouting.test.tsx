import { act, fireEvent, screen, waitFor } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { renderApp } from "../../tests/renderApp";

function search(text: string) {
  const searchQueryInput = screen.getByRole("textbox", {
    name: "search-query-input",
  });
  fireEvent.change(searchQueryInput, { target: { value: text } });
  fireEvent.click(screen.getByRole("button", { name: "search" }));
  return searchQueryInput;
}

async function findCard(name: string) {
  const text = await screen.findByText(name);
  const card = text.closest<HTMLDivElement>("#search-card");
  if (!card) {
    throw new Error(`Search card for ${name} not found`);
  }
  return card;
}

describe("SuggestionSearch routing", () => {
  describe("When a search is submitted", () => {
    it("Should put the query in the URL", async () => {
      const { currentUrl } = renderApp();
      search("1");
      expect(currentUrl()).toBe("/search?q=1");
      await screen.findByText("8 results.");
    });

    it("Should URL-encode the query", () => {
      const { currentUrl } = renderApp();
      search("a b&c");
      expect(currentUrl()).toBe("/search?q=a+b%26c");
    });
  });

  describe("When a card with a detail panel is clicked", () => {
    it.each([
      ["Example Movie", "/movie/tt0000001?q=1", "movie-panel"],
      ["Example TV Series", "/tvseries/tt10000002?q=1", "tv-series-panel"],
      ["Example Smith", "/person/nm9000000?q=1", "person-panel"],
    ])(
      "Should navigate %s to %s and show the panel",
      async (name, expectedUrl, panelTestId) => {
        const { currentUrl } = renderApp();
        search("1");
        const card = await findCard(name);
        fireEvent.click(card);
        expect(currentUrl()).toBe(expectedUrl);
        expect(card.classList.contains("selected")).toBe(true);
        await screen.findByTestId(panelTestId);
        expect(card.classList.contains("expanded")).toBe(true);
      },
    );
  });

  describe("When a card without a detail panel is clicked", () => {
    it("Should add the card to the URL and remove it again on a second click", async () => {
      const { currentUrl } = renderApp();
      search("2");
      const card = await findCard("Example Video Game");
      fireEvent.click(card);
      expect(currentUrl()).toBe("/search?q=2&selected=tt0100008");
      expect(card.classList.contains("selected")).toBe(true);
      expect(card.classList.contains("expanded")).toBe(false);
      expect(screen.queryByText("6 results.")).not.toBeInTheDocument();

      fireEvent.click(card);
      await waitFor(() => {
        expect(currentUrl()).toBe("/search?q=2");
      });
      expect(card.classList.contains("selected")).toBe(false);
      expect(card.classList.contains("deselecting")).toBe(true);
      expect(screen.getByText("6 results.")).toBeInTheDocument();
    });

    it("Should highlight the card when its URL is loaded directly", async () => {
      renderApp("/search?q=2&selected=tt0100008");
      const card = await findCard("Example Video Game");
      expect(card.classList.contains("selected")).toBe(true);
      expect(card.classList.contains("expanded")).toBe(false);
    });
  });

  describe("When ESC is pressed while a detail panel is open", () => {
    it("Should go back in history to the search results URL", async () => {
      const { currentUrl, router, goForward } = renderApp();
      search("1");
      const card = await findCard("Example Movie");
      fireEvent.click(card);
      await screen.findByTestId("movie-panel");
      const movieLocationKey = router.state.location.key;

      fireEvent.keyDown(window, { key: "Escape" });
      await waitFor(() => {
        expect(currentUrl()).toBe("/search?q=1");
      });
      await waitFor(() => {
        expect(card.classList.contains("selected")).toBe(false);
      });
      expect(card.classList.contains("deselecting")).toBe(true);
      expect(screen.queryByTestId("movie-panel")).not.toBeInTheDocument();
      await screen.findByText("8 results.");

      // It popped history rather than pushing, so forward returns to the
      // very same movie history entry.
      await goForward();
      expect(currentUrl()).toBe("/movie/tt0000001?q=1");
      expect(router.state.location.key).toBe(movieLocationKey);
    });
  });

  describe("When the browser back and forward buttons are used", () => {
    it("Should close and reopen the detail panel", async () => {
      const { currentUrl, goBack, goForward } = renderApp();
      search("1");
      const card = await findCard("Example Movie");
      fireEvent.click(card);
      const panel = await screen.findByTestId("movie-panel");
      expect(card.classList.contains("expanded")).toBe(true);

      await goBack();
      expect(currentUrl()).toBe("/search?q=1");
      // The panel shrinks first, then the card flies home.
      expect(card.classList.contains("expanded")).toBe(false);
      expect(panel).not.toBeInTheDocument();
      expect(card.classList.contains("selected")).toBe(true);
      await waitFor(() => {
        expect(card.classList.contains("selected")).toBe(false);
      });
      expect(card.classList.contains("deselecting")).toBe(true);
      expect(screen.getByText("8 results.")).toBeInTheDocument();

      await goForward();
      expect(currentUrl()).toBe("/movie/tt0000001?q=1");
      expect(card.classList.contains("selected")).toBe(true);
      expect(card.classList.contains("deselecting")).toBe(false);
      expect(card.classList.contains("expanded")).toBe(false);
      await screen.findByTestId("movie-panel");
      expect(card.classList.contains("expanded")).toBe(true);
      expect(screen.queryByText("8 results.")).not.toBeInTheDocument();
    });

    it("Should restore an earlier search, including the input text", async () => {
      const { currentUrl, goBack, goForward } = renderApp();
      const input = search("1");
      await screen.findByText("8 results.");
      search("2");
      await screen.findByText("6 results.");
      expect(currentUrl()).toBe("/search?q=2");

      await goBack();
      expect(currentUrl()).toBe("/search?q=1");
      expect(input).toHaveValue("1");
      await screen.findByText("8 results.");
      expect(screen.getByText("Example Movie")).toBeInTheDocument();

      await goForward();
      expect(currentUrl()).toBe("/search?q=2");
      expect(input).toHaveValue("2");
      await screen.findByText("6 results.");
      expect(screen.getByText("Example Video Game")).toBeInTheDocument();
    });

    it("Should drop an open panel when going back to a different search", async () => {
      const { currentUrl, goBack, container } = renderApp();
      search("2");
      await screen.findByText("6 results.");
      search("1");
      const card = await findCard("Example Movie");
      fireEvent.click(card);
      await screen.findByTestId("movie-panel");

      await goBack();
      await goBack();
      expect(currentUrl()).toBe("/search?q=2");
      await screen.findByText("6 results.");
      expect(screen.queryByTestId("movie-panel")).not.toBeInTheDocument();
      expect(container.querySelectorAll(".result-card.selected")).toHaveLength(
        0,
      );
      expect(
        container.querySelector(".results-container.has-selection"),
      ).toBeNull();
    });
  });

  describe("When a search URL is loaded directly", () => {
    it("Should run the search and fill in the input", async () => {
      renderApp("/search?q=1");
      expect(
        screen.getByRole("textbox", { name: "search-query-input" }),
      ).toHaveValue("1");
      await screen.findByText("8 results.");
      expect(screen.getByText("Example Movie")).toBeInTheDocument();
    });

    it("Should show an empty search page without a query", () => {
      renderApp("/search");
      expect(
        screen.getByRole("textbox", { name: "search-query-input" }),
      ).toHaveValue("");
      expect(screen.queryByText(/results\./)).not.toBeInTheDocument();
    });
  });

  describe("When a detail URL with a query is loaded directly", () => {
    it.each([
      ["/movie/tt0000001?q=1", "movie-panel", "Example Movie"],
      ["/tvseries/tt10000002?q=1", "tv-series-panel", "Example TV Series"],
      ["/person/nm9000000?q=1", "person-panel", "Example Smith"],
    ])(
      "Should show the panel for %s at once and load the results behind it",
      async (url, panelTestId, name) => {
        const { container, currentUrl, router } = renderApp(url);
        const panel = await screen.findByTestId(panelTestId);
        const card = panel.closest("#search-card");
        expect(card?.classList.contains("selected")).toBe(true);
        expect(card?.classList.contains("expanded")).toBe(true);
        expect(screen.queryByText("8 results.")).not.toBeInTheDocument();

        // Once the results arrive, the same panel is hosted by the real card,
        // and there is exactly one card for the item.
        await waitFor(() => {
          expect(container.querySelectorAll("#search-card")).toHaveLength(8);
        });
        expect(screen.getByTestId(panelTestId)).toBe(panel);
        expect(
          container.querySelectorAll("#search-card.selected.expanded"),
        ).toHaveLength(1);

        // Editing the URL back to the results shows the grid.
        await act(() => router.navigate("/search?q=1"));
        expect(currentUrl()).toBe("/search?q=1");
        await waitFor(() => {
          expect(
            container.querySelectorAll("#search-card.selected"),
          ).toHaveLength(0);
        });
        expect(screen.getByText("8 results.")).toBeInTheDocument();
        expect(await findCard(name)).not.toHaveClass("expanded");
      },
    );
  });

  describe("When a detail URL without a query is loaded directly", () => {
    it("Should show the panel by itself", async () => {
      const { container } = renderApp("/movie/tt0000001");
      await screen.findByTestId("movie-panel");
      expect(container.querySelectorAll("#search-card")).toHaveLength(1);
      expect(
        screen.getByRole("textbox", { name: "search-query-input" }),
      ).toHaveValue("");
    });

    it("Should use the panel named by the URL for the item", async () => {
      renderApp("/person/nm9000000");
      await screen.findByTestId("person-panel");
      expect(screen.queryByTestId("movie-panel")).not.toBeInTheDocument();
    });
  });

  describe("When an unknown URL is loaded", () => {
    it("Should redirect to the login page", () => {
      const { currentUrl } = renderApp("/nonsense");
      expect(currentUrl()).toBe("/login");
      expect(screen.getByRole("button", { name: "login" })).toBeInTheDocument();
    });
  });
});
