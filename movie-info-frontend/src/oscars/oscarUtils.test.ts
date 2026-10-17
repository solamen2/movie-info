import { describe, expect, it } from "vitest";
import {
  ordinal,
  oscarYearElementId,
  splitNameIntoNominees,
  stripWritingCredit,
} from "./oscarUtils";

describe("stripWritingCredit", () => {
  it.each([
    ["Written by Ryan Coogler", "Ryan Coogler"],
    ["Screenplay by Will Tracy", "Will Tracy"],
    ["Written for the screen by Cord Jefferson", "Cord Jefferson"],
    ["Written for the Screen by Guillermo del Toro", "Guillermo del Toro"],
    [
      "Adapted for the screen by Debra Granik & Anne Rosellini",
      "Debra Granik & Anne Rosellini",
    ],
    ["Story by Ben Hecht", "Ben Hecht"],
    [
      "Story and Screenplay by Jean-Charles Tacchella",
      "Jean-Charles Tacchella",
    ],
  ])("Should drop the lead-in of %j", (name, expected) => {
    expect(stripWritingCredit(name)).toBe(expected);
  });

  it.each([
    "Story by Nancy Dowd; Screenplay by Waldo Salt, Robert C. Jones",
    "Screenplay by Daniel Petrie, Jr.; Story by Danilo Bach, Daniel Petrie, Jr.",
    "Screenplay and Dialogue by George Bernard Shaw; Adaptation by W. P. Lipscomb",
    "Alan Ball",
    "Ring Lardner, Jr., Michael Kanin",
    "Screenplay - Justine Triet and Arthur Harari",
    "",
  ])("Should keep %j, which has several roles or no lead-in", (name) => {
    expect(stripWritingCredit(name)).toBe(name);
  });
});

describe("splitNameIntoNominees", () => {
  it("Should mark each nominee's name in the credit text", () => {
    expect(
      splitNameIntoNominees(
        "Emma Thomas, Charles Roven and Christopher Nolan, Producers",
        ["Emma Thomas", "Charles Roven", "Christopher Nolan"],
      ),
    ).toEqual([
      { text: "Emma Thomas", nominee: "Emma Thomas" },
      { text: ", ", nominee: null },
      { text: "Charles Roven", nominee: "Charles Roven" },
      { text: " and ", nominee: null },
      { text: "Christopher Nolan", nominee: "Christopher Nolan" },
      { text: ", Producers", nominee: null },
    ]);
  });

  it("Should keep text before the first nominee", () => {
    expect(
      splitNameIntoNominees("Music and Lyric by Diane Warren", [
        "Diane Warren",
      ]),
    ).toEqual([
      { text: "Music and Lyric by ", nominee: null },
      { text: "Diane Warren", nominee: "Diane Warren" },
    ]);
  });

  it("Should find a nominee whose suffix the credit text writes with a comma", () => {
    expect(
      splitNameIntoNominees("Cuba Gooding, Jr.", ["Cuba Gooding Jr."]),
    ).toEqual([{ text: "Cuba Gooding, Jr.", nominee: "Cuba Gooding Jr." }]);
  });

  it("Should not let a name claim part of a longer name", () => {
    expect(
      splitNameIntoNominees("Tom Hanks Jr. and Tom Hanks", [
        "Tom Hanks",
        "Tom Hanks Jr.",
      ]),
    ).toEqual([
      { text: "Tom Hanks Jr.", nominee: "Tom Hanks Jr." },
      { text: " and ", nominee: null },
      { text: "Tom Hanks", nominee: "Tom Hanks" },
    ]);
  });

  it("Should leave the text as is when the nominees aren't in it", () => {
    expect(splitNameIntoNominees("United Kingdom", [])).toEqual([
      { text: "United Kingdom", nominee: null },
    ]);
    expect(
      splitNameIntoNominees("Dr. John G. Frayne", ["John G. Frayne Sr."]),
    ).toEqual([{ text: "Dr. John G. Frayne", nominee: null }]);
  });

  it("Should list the nominees when there is no credit text", () => {
    expect(splitNameIntoNominees("", ["Gene Kelly"])).toEqual([
      { text: "Gene Kelly", nominee: "Gene Kelly" },
    ]);
    expect(splitNameIntoNominees("", ["A", "B", "A"])).toEqual([
      { text: "A", nominee: "A" },
      { text: ", ", nominee: null },
      { text: "B", nominee: "B" },
    ]);
    expect(splitNameIntoNominees("", [])).toEqual([]);
  });
});

describe("ordinal", () => {
  it("Should add the right suffix", () => {
    expect(
      [1, 2, 3, 4, 11, 12, 13, 21, 22, 23, 96, 100, 111].map(ordinal),
    ).toEqual([
      "1st",
      "2nd",
      "3rd",
      "4th",
      "11th",
      "12th",
      "13th",
      "21st",
      "22nd",
      "23rd",
      "96th",
      "100th",
      "111th",
    ]);
  });
});

describe("oscarYearElementId", () => {
  it("Should make an id from the ceremony number", () => {
    expect(oscarYearElementId(96)).toBe("oscars-year-96");
  });
});
