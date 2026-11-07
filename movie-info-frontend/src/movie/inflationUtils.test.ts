import { describe, expect, it } from "vitest";
import inflationDataJson1 from "../../tests/mocks/data/inflationData1.json" with { type: "json" };
import type { BlsTimeSeries, Inflation } from "./inflationTypes";
import {
  EARLIEST_INFLATION_DATE,
  adjustForInflation,
  clampYearMonth,
  formatYearMonth,
  getIndexValue,
  latestAvailable,
  latestAvailableFor,
  parseYearMonth,
  seriesFor,
  toYearMonth,
} from "./inflationUtils";

// The mock series: a month's value is (year - 1900) * 10 + month, except that
// Oct 2025 has no value, Nov 2025 is 1263 (not 1261) and the months after
// Sep 2026 aren't published yet (see tests/mocks/handlers.ts)
const inflation: Inflation = inflationDataJson1;

function series(rows: BlsTimeSeries["rows"]): BlsTimeSeries {
  return {
    seriesId: "TEST",
    seriesTitle: "Test series",
    isSeasonallyAdjusted: false,
    rows,
  };
}

describe("parseYearMonth", () => {
  it("Should read the month and year of an ISO date", () => {
    expect(parseYearMonth("2016-09-23")).toEqual({ year: 2016, month: 9 });
    expect(parseYearMonth("1913-01-01")).toEqual({ year: 1913, month: 1 });
    expect(parseYearMonth(" 2016-12 ")).toEqual({ year: 2016, month: 12 });
  });

  it.each([
    ["no date", null],
    ["an empty date", ""],
    ["N/A", "N/A"],
    ["just a year", "2016"],
    ["a month out of range", "2016-13-01"],
    ["a month of zero", "2016-00-01"],
    ["another format", "09/23/2016"],
  ])("Should return null for %s", (_, isoDate) => {
    expect(parseYearMonth(isoDate)).toBeNull();
  });
});

describe("formatYearMonth", () => {
  it("Should show the month's abbreviation and the year", () => {
    expect(formatYearMonth({ year: 2026, month: 9 })).toBe("Sep 2026");
    expect(formatYearMonth({ year: 1913, month: 1 })).toBe("Jan 1913");
  });
});

describe("toYearMonth", () => {
  it("Should take the month and year from a date", () => {
    expect(toYearMonth(new Date(2026, 10, 7))).toEqual({
      year: 2026,
      month: 11,
    });
  });
});

describe("clampYearMonth", () => {
  const earliest = { year: 1913, month: 1 };
  const latest = { year: 2026, month: 9 };

  it("Should leave a month within the range alone", () => {
    expect(clampYearMonth({ year: 2016, month: 9 }, earliest, latest)).toEqual({
      year: 2016,
      month: 9,
    });
    expect(clampYearMonth(earliest, earliest, latest)).toEqual(earliest);
    expect(clampYearMonth(latest, earliest, latest)).toEqual(latest);
  });

  it("Should move a month before the range to its start", () => {
    expect(clampYearMonth({ year: 1912, month: 12 }, earliest, latest)).toEqual(
      earliest,
    );
    expect(clampYearMonth({ year: 1905, month: 6 }, earliest, latest)).toEqual(
      earliest,
    );
  });

  it("Should move a month after the range to its end", () => {
    expect(clampYearMonth({ year: 2026, month: 10 }, earliest, latest)).toEqual(
      latest,
    );
    expect(clampYearMonth({ year: 2030, month: 1 }, earliest, latest)).toEqual(
      latest,
    );
  });
});

describe("latestAvailable", () => {
  it("Should return the last month with a value", () => {
    expect(latestAvailable(inflation.seasonallyAdjusted)).toEqual({
      year: 2026,
      month: 9,
    });
    expect(latestAvailable(inflation.notSeasonallyAdjusted)).toEqual({
      year: 2026,
      month: 9,
    });
  });

  it("Should skip a whole year without values", () => {
    expect(
      latestAvailable(
        series([
          { year: 2025, months: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12] },
          { year: 2026, months: Array<null>(12).fill(null) },
        ]),
      ),
    ).toEqual({ year: 2025, month: 12 });
  });

  it("Should return null for a series without values", () => {
    expect(latestAvailable(series([]))).toBeNull();
    expect(
      latestAvailable(
        series([{ year: 2026, months: Array<null>(12).fill(null) }]),
      ),
    ).toBeNull();
  });
});

describe("seriesFor", () => {
  it("Should use the seasonally adjusted series when both months are from 1947 on", () => {
    expect(
      seriesFor(inflation, { year: 1947, month: 1 }, { year: 2026, month: 9 }),
    ).toBe(inflation.seasonallyAdjusted);
    expect(
      seriesFor(inflation, { year: 2016, month: 9 }, { year: 1950, month: 6 }),
    ).toBe(inflation.seasonallyAdjusted);
  });

  it("Should use the not seasonally adjusted series when either month is before 1947", () => {
    expect(
      seriesFor(inflation, { year: 1946, month: 12 }, { year: 2026, month: 9 }),
    ).toBe(inflation.notSeasonallyAdjusted);
    expect(
      seriesFor(inflation, { year: 2016, month: 9 }, { year: 1940, month: 1 }),
    ).toBe(inflation.notSeasonallyAdjusted);
    expect(
      seriesFor(inflation, { year: 1913, month: 1 }, { year: 1920, month: 1 }),
    ).toBe(inflation.notSeasonallyAdjusted);
  });
});

describe("latestAvailableFor", () => {
  it("Should return the last month of the series the release month uses", () => {
    expect(
      latestAvailableFor(
        {
          seasonallyAdjusted: series([
            {
              year: 2026,
              months: [1, 2, 3, 4, 5, 6, 7, 8, null, null, null, null],
            },
          ]),
          notSeasonallyAdjusted: series([
            {
              year: 2026,
              months: [1, 2, 3, 4, 5, 6, 7, 8, 9, null, null, null],
            },
          ]),
        },
        { year: 2016, month: 9 },
      ),
    ).toEqual({ year: 2026, month: 8 });
    expect(
      latestAvailableFor(
        {
          seasonallyAdjusted: series([
            {
              year: 2026,
              months: [1, 2, 3, 4, 5, 6, 7, 8, null, null, null, null],
            },
          ]),
          notSeasonallyAdjusted: series([
            {
              year: 2026,
              months: [1, 2, 3, 4, 5, 6, 7, 8, 9, null, null, null],
            },
          ]),
        },
        { year: 1940, month: 1 },
      ),
    ).toEqual({ year: 2026, month: 9 });
  });
});

describe("getIndexValue", () => {
  it("Should return the month's value", () => {
    expect(
      getIndexValue(inflation.seasonallyAdjusted, { year: 2016, month: 9 }),
    ).toBe(1169);
    expect(
      getIndexValue(inflation.notSeasonallyAdjusted, { year: 1913, month: 1 }),
    ).toBe(131);
  });

  it("Should average the nearest months before and after one without a value", () => {
    // Oct 2025: Sep 2025 is 1259 and Nov 2025 is 1263
    expect(
      getIndexValue(inflation.seasonallyAdjusted, { year: 2025, month: 10 }),
    ).toBe(1261);
  });

  it("Should look past other months without values, across years", () => {
    const gappy = series([
      { year: 2024, months: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 100] },
      { year: 2025, months: Array<null>(12).fill(null) },
      {
        year: 2026,
        months: [
          200,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
        ],
      },
    ]);
    expect(getIndexValue(gappy, { year: 2025, month: 1 })).toBe(150);
    expect(getIndexValue(gappy, { year: 2025, month: 12 })).toBe(150);
  });

  it("Should use the one neighbour there is at either end of the series", () => {
    const ends = series([
      {
        year: 2025,
        months: [
          null,
          null,
          30,
          40,
          50,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
        ],
      },
    ]);
    expect(getIndexValue(ends, { year: 2025, month: 1 })).toBe(30);
    expect(getIndexValue(ends, { year: 2025, month: 12 })).toBe(50);
  });

  it("Should return null for a month outside the series or a series without values", () => {
    expect(
      getIndexValue(inflation.seasonallyAdjusted, { year: 1946, month: 12 }),
    ).toBeNull();
    expect(
      getIndexValue(inflation.seasonallyAdjusted, { year: 2027, month: 1 }),
    ).toBeNull();
    expect(
      getIndexValue(
        series([{ year: 2025, months: Array<null>(12).fill(null) }]),
        { year: 2025, month: 5 },
      ),
    ).toBeNull();
  });
});

describe("adjustForInflation", () => {
  it("Should scale the amount by the ratio of the two months' values", () => {
    // Sep 2016 is 1169, Jan 2020 is 1201
    expect(
      adjustForInflation(
        inflation,
        25000000,
        { year: 2016, month: 9 },
        { year: 2020, month: 1 },
      ),
    ).toEqual({
      amount: (25000000 * 1201) / 1169,
      isSeasonallyAdjusted: true,
    });
  });

  it("Should leave the amount alone when both months are the same", () => {
    expect(
      adjustForInflation(
        inflation,
        25000000,
        { year: 2016, month: 9 },
        { year: 2016, month: 9 },
      ),
    ).toEqual({ amount: 25000000, isSeasonallyAdjusted: true });
  });

  it("Should use the not seasonally adjusted series when either month is before 1947", () => {
    // Jan 1940 is 401 in the not seasonally adjusted series
    expect(
      adjustForInflation(
        inflation,
        25000000,
        { year: 2016, month: 9 },
        { year: 1940, month: 1 },
      ),
    ).toEqual({
      amount: (25000000 * 401) / 1169,
      isSeasonallyAdjusted: false,
    });
    expect(
      adjustForInflation(inflation, 100, EARLIEST_INFLATION_DATE, {
        year: 2026,
        month: 9,
      }),
    ).toEqual({ amount: (100 * 1269) / 131, isSeasonallyAdjusted: false });
  });

  it("Should adjust to a month without a value using its neighbours' average", () => {
    expect(
      adjustForInflation(
        inflation,
        25000000,
        { year: 2016, month: 9 },
        { year: 2025, month: 10 },
      ),
    ).toEqual({
      amount: (25000000 * 1261) / 1169,
      isSeasonallyAdjusted: true,
    });
  });

  it("Should return null when a month's value is unknown", () => {
    expect(
      adjustForInflation(
        inflation,
        25000000,
        { year: 2016, month: 9 },
        { year: 2027, month: 1 },
      ),
    ).toBeNull();
    expect(
      adjustForInflation(
        inflation,
        25000000,
        { year: 1912, month: 9 },
        { year: 2016, month: 1 },
      ),
    ).toBeNull();
  });
});
