import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import TvSeriesCrewCard from "./TvSeriesCrewCard";
import { type PersonTvSeriesCrew } from "./personTypes";

const credit: PersonTvSeriesCrew = {
  id: "c1",
  backdropPath: null,
  tmdbId: 90002,
  originalName: "Example Show",
  popularity: 1,
  posterPath: null,
  firstAirDate: "1997-03-10",
  name: "Example Show",
  department: "Production",
  episodeCount: 22,
  firstCreditAirDate: "2001-10-02",
  job: "Executive Producer",
};

describe("TvSeriesCrewCard", () => {
  it("Should label the date of the person's first credit as their first appearance", () => {
    render(<TvSeriesCrewCard credit={credit} />);
    expect(screen.getByTestId("tv-series-crew-card").textContent).toBe(
      "No imageExample ShowExecutive Producer22 episodesFirst appearance:Oct 2, 2001",
    );
  });

  it("Should label the date the series first aired as such when the credit has no date of its own", () => {
    render(
      <TvSeriesCrewCard credit={{ ...credit, firstCreditAirDate: null }} />,
    );
    expect(screen.getByTestId("tv-series-crew-card").textContent).toContain(
      "22 episodesFirst air date:Mar 10, 1997",
    );
  });
});
