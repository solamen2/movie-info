import { screen, within } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { renderApp } from "../../tests/renderApp";

describe("ImdbLists", () => {
  it("Should link to IMDB's own three lists, in new tabs, in a bulleted list", () => {
    renderApp("/imdb-lists");
    const page = screen.getByTestId("imdb-lists-page");

    expect(
      within(page).getByRole("heading", { level: 1, name: "IMDB Lists" }),
    ).toBeInTheDocument();
    const links = within(within(page).getByRole("list")).getAllByRole("link");
    expect(
      links.map((link) => [link.textContent, link.getAttribute("href")]),
    ).toEqual([
      [
        "IMDB Top 1000 Movies",
        "https://www.imdb.com/search/title/?groups=top_1000&sort=user_rating,desc",
      ],
      [
        "IMDB Bottom 1000 Movies",
        "https://www.imdb.com/search/title/?groups=bottom_1000&sort=user_rating,asc",
      ],
      ["IMDB Top 250 TV Shows", "https://www.imdb.com/chart/toptv/"],
    ]);
    for (const link of links) {
      expect(link).toHaveAttribute("target", "_blank");
    }
    expect(within(page).getAllByRole("listitem")).toHaveLength(3);
  });

  it("Should show the footer", () => {
    renderApp("/imdb-lists");
    expect(document.querySelector("footer")).not.toBeNull();
  });
});
