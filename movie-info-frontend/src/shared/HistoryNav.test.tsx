import { fireEvent, render, screen } from "@testing-library/react";
import { MemoryRouter, useLocation } from "react-router-dom";
import { expect, test } from "vitest";
import HistoryNav from "./HistoryNav";

function CurrentPath() {
  return <p>{useLocation().pathname}</p>;
}

function renderAt(entries: string[], index: number) {
  render(
    <MemoryRouter initialEntries={entries} initialIndex={index}>
      <HistoryNav />
      <CurrentPath />
    </MemoryRouter>,
  );
}

test("The back button goes back through the history", async () => {
  renderAt(["/search", "/oscars"], 1);

  fireEvent.click(screen.getByRole("button", { name: "Back" }));
  expect(await screen.findByText("/search")).toBeInTheDocument();
});

function scrollTo(y: number) {
  Object.defineProperty(window, "scrollY", { value: y, configurable: true });
  fireEvent.scroll(window);
}

test("Scrolling down tucks the back button away, and scrolling up 20px or reaching the top brings it back", () => {
  renderAt(["/search"], 0);
  const button = screen.getByRole("button", { name: "Back" });
  fireEvent.touchStart(window);
  fireEvent.touchMove(window);

  scrollTo(19);
  expect(button).not.toHaveClass("is-tucked-away");
  scrollTo(300);
  expect(button).toHaveClass("is-tucked-away");

  scrollTo(281);
  expect(button).toHaveClass("is-tucked-away");
  scrollTo(280);
  expect(button).not.toHaveClass("is-tucked-away");

  scrollTo(400);
  expect(button).toHaveClass("is-tucked-away");
  scrollTo(395);
  scrollTo(0);
  expect(button).not.toHaveClass("is-tucked-away");
});

test("Scrolling done by the app, not by a dragging finger, leaves the back button alone", () => {
  renderAt(["/search"], 0);
  const button = screen.getByRole("button", { name: "Back" });

  // A tap, then the app scrolls
  fireEvent.touchStart(window);
  scrollTo(300);
  expect(button).not.toHaveClass("is-tucked-away");

  // The user's own scrolling is measured from where the app left the page
  fireEvent.touchMove(window);
  scrollTo(319);
  expect(button).not.toHaveClass("is-tucked-away");
  scrollTo(320);
  expect(button).toHaveClass("is-tucked-away");
});

test("The login page has no back button", () => {
  renderAt(["/login"], 0);

  expect(screen.queryByRole("button", { name: "Back" })).toBeNull();
});
