import { render } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import TitleStatus from "./TitleStatus";

function renderedText(status: string | null) {
  const { container } = render(
    <TitleStatus status={status} usualStatus="Released" />,
  );
  return container.textContent;
}

describe("TitleStatus", () => {
  it("Should show an unusual status in parentheses and lowercase", () => {
    expect(renderedText("In Production")).toBe("(in production)");
    expect(renderedText("Canceled")).toBe("(canceled)");
  });

  it.each([
    ["the usual status", "Released"],
    ["the usual status in another case", "released"],
    ["an empty status", ""],
    ["a blank status", "  "],
    ["no status", null],
  ])("Should show nothing for %s", (_, status) => {
    expect(renderedText(status)).toBe("");
  });
});
