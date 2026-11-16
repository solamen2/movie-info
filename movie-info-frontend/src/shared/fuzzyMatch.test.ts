import { describe, expect, it } from "vitest";
import { fuzzyMatches } from "./fuzzyMatch";

describe("fuzzyMatches", () => {
  const texts = ["Zoë Saldaña", "Lt. Nyota Uhura"];

  it("Should match everything with a blank filter", () => {
    expect(fuzzyMatches("", texts)).toBe(true);
    expect(fuzzyMatches("   ", texts)).toBe(true);
  });

  it("Should match substrings regardless of case and accents", () => {
    expect(fuzzyMatches("SALDANA", texts)).toBe(true);
    expect(fuzzyMatches("zoe", texts)).toBe(true);
    expect(fuzzyMatches("uhu", texts)).toBe(true);
  });

  it("Should need every word of the filter to match, in any of the texts", () => {
    expect(fuzzyMatches("uhura zoe", texts)).toBe(true);
    expect(fuzzyMatches("zoe kirk", texts)).toBe(false);
  });

  it("Should tolerate a typo in words of five letters or more, and two in words of nine or more", () => {
    expect(fuzzyMatches("saldna", texts)).toBe(true);
    expect(fuzzyMatches("nyoat", texts)).toBe(true);
    expect(fuzzyMatches("zoe saldanna", texts)).toBe(true);
    expect(fuzzyMatches("sladnaa", texts)).toBe(false);
  });

  it("Should count two swapped neighboring letters as one typo", () => {
    expect(fuzzyMatches("sladana", texts)).toBe(true);
    expect(fuzzyMatches("uhrua", texts)).toBe(true);
    expect(fuzzyMatches(" zoe sadlanaa", ["Zoë Saldaña"])).toBe(false);
  });

  it("Should need short words to appear exactly", () => {
    expect(fuzzyMatches("zoa", texts)).toBe(false);
    expect(fuzzyMatches("uhra", texts)).toBe(false);
  });
});
