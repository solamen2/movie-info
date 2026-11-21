import { act, fireEvent, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { findResultCards, renderApp } from "../../tests/renderApp";

function search(text: string) {
  fireEvent.change(
    screen.getByRole("textbox", {
      name: "Search movies, people, and TV shows",
    }),
    { target: { value: text } },
  );
  fireEvent.click(screen.getByRole("button", { name: "Search" }));
}

async function findCard(name: string) {
  const text = await screen.findByText(name);
  const card = text.closest<HTMLDivElement>('[data-testid="search-card"]');
  if (!card) {
    throw new Error(`Search card for ${name} not found`);
  }
  return card;
}

describe("Accessibility", () => {
  describe("Using the keyboard", () => {
    it("Should make every result card a button that the keyboard can reach", async () => {
      renderApp();
      search("1");

      const cards = await findResultCards(8);
      for (const card of cards) {
        expect(card).toHaveAttribute("role", "button");
        expect(card).toHaveAttribute("tabindex", "0");
      }
    });

    it.each([
      ["Enter", "Enter"],
      ["Space", " "],
    ])("Should open a card's panel with %s", async (_name, key) => {
      const { currentUrl } = renderApp();
      search("1");
      const card = await findCard("Example Movie");

      fireEvent.keyDown(card, { key });

      expect(currentUrl()).toBe("/movie/tt0000001?q=1");
      await screen.findByTestId("movie-panel");
    });

    it("Should leave other keys, and keys pressed inside a card, alone", async () => {
      const { currentUrl } = renderApp();
      search("1");
      const card = await findCard("Example Movie");

      fireEvent.keyDown(card, { key: "a" });
      fireEvent.keyDown(screen.getByText("Example Movie"), { key: "Enter" });

      expect(currentUrl()).toBe("/search?q=1");
    });

    it("Should move the focus into a panel as it opens, where it is no longer a button, and leave it on the card when the panel closes", async () => {
      const { goBack } = renderApp();
      search("1");
      const card = await findCard("Example Movie");
      card.focus();

      fireEvent.keyDown(card, { key: "Enter" });
      await screen.findByTestId("movie-panel");

      expect(card).toHaveFocus();
      expect(card).not.toHaveAttribute("role");
      expect(card).toHaveAttribute("tabindex", "-1");

      await goBack();
      await waitFor(() => {
        expect(card).toHaveAttribute("role", "button");
      });
      expect(card).toHaveFocus();
    });

    it("Should give a panel opened from a card inside another panel the focus", async () => {
      renderApp("/movie/tt0000001?q=1");
      fireEvent.click(await screen.findByText("Cast", { selector: "summary" }));
      const castCard = (await screen.findAllByTestId("cast-card"))[0];
      expect(castCard).toHaveAttribute("role", "button");

      fireEvent.keyDown(castCard, { key: "Enter" });

      const personPanel = await screen.findByTestId("person-panel");
      expect(personPanel.closest(".expandable-card")).toHaveFocus();
    });

    it("Should not take the focus from someone typing as a panel opens", async () => {
      renderApp();
      search("1");
      const card = await findCard("Example Movie");
      const searchBox = screen.getByRole("textbox", {
        name: "Search movies, people, and TV shows",
      });

      fireEvent.click(card);
      searchBox.focus();
      await screen.findByTestId("movie-panel");

      expect(searchBox).toHaveFocus();
    });
  });

  describe("Going from page to page", () => {
    it("Should move the focus to the new page's content, but leave it alone within the search page", async () => {
      const { router } = renderApp();
      const main = screen.getByRole("main");
      const oscarsLink = screen.getByRole("link", { name: "Oscars" });
      oscarsLink.focus();

      fireEvent.click(oscarsLink);
      expect(main).toHaveFocus();

      await act(() => router.navigate("/search"));
      const searchBox = screen.getByRole("textbox", {
        name: "Search movies, people, and TV shows",
      });
      searchBox.focus();
      search("1");
      await findResultCards(8);
      expect(searchBox).toHaveFocus();
    });

    it("Should name each page in the browser tab's title", async () => {
      const { router } = renderApp("/login");
      expect(document.title).toBe("Login – Movie Search");

      await act(() => router.navigate("/search"));
      expect(document.title).toBe("Movie Search");

      await act(() => router.navigate("/search?q=1"));
      expect(document.title).toBe("1 – Movie Search");

      await act(() => router.navigate("/oscars"));
      expect(document.title).toBe("Oscars By Year – Movie Search");

      await act(() => router.navigate("/imdb-lists"));
      expect(document.title).toBe("IMDB Lists – Movie Search");

      await act(() => router.navigate("/aspect-ratios"));
      expect(document.title).toBe("Aspect Ratio Reference – Movie Search");
    });

    it("Should name an open panel in the title, and go back to the search's as it closes", async () => {
      const { goBack } = renderApp();
      search("1");
      fireEvent.click(await findCard("Example Movie"));

      await waitFor(() => {
        expect(document.title).toBe("Example Movie – Movie Search");
      });

      await goBack();
      await waitFor(() => {
        expect(document.title).toBe("1 – Movie Search");
      });
    });
  });

  describe("For screen readers", () => {
    it("Should keep the search box's name the same on a phone, where its hint is a shorter one", () => {
      const { unmount } = renderApp();
      const name = "Search movies, people, and TV shows";
      expect(screen.getByRole("textbox", { name })).toHaveAttribute(
        "placeholder",
        name,
      );
      unmount();

      const matchMedia = vi.spyOn(window, "matchMedia").mockImplementation(
        (query) =>
          ({
            matches: query === "(max-width: 640px)",
            media: query,
            addEventListener: () => undefined,
            removeEventListener: () => undefined,
          }) as unknown as MediaQueryList,
      );
      renderApp();
      expect(screen.getByRole("textbox", { name })).toHaveAttribute(
        "placeholder",
        "Movies, people, TV",
      );
      matchMedia.mockRestore();
    });

    it("Should give the search page a main landmark, a search landmark and a level one heading", () => {
      renderApp();

      expect(screen.getByRole("main")).toBeInTheDocument();
      expect(screen.getByRole("search")).toBeInTheDocument();
      expect(screen.getByRole("heading", { level: 1 }).textContent).toBe(
        "Movie Search",
      );
    });

    it("Should announce a search that finds nothing", async () => {
      renderApp();
      search("empty");

      expect((await screen.findByRole("status")).textContent).toBe(
        "No results. Please try another search.",
      );
    });
  });
});
