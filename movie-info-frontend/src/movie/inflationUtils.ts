import type { BlsTimeSeries, Inflation } from "./inflationTypes";

// A month of a year; `month` is 1 (January) to 12 (December).
export interface YearMonth {
  year: number;
  month: number;
}

// The not seasonally adjusted series starts here, so earlier dates are
// adjusted as if they were this one.
export const EARLIEST_INFLATION_DATE: YearMonth = { year: 1913, month: 1 };

// The seasonally adjusted series starts in January of this year.
export const FIRST_SEASONALLY_ADJUSTED_YEAR = 1947;

export const MONTH_NAMES = [
  "Jan",
  "Feb",
  "Mar",
  "Apr",
  "May",
  "Jun",
  "Jul",
  "Aug",
  "Sep",
  "Oct",
  "Nov",
  "Dec",
];

export function compareYearMonths(a: YearMonth, b: YearMonth): number {
  return a.year - b.year || a.month - b.month;
}

export function sameYearMonth(a: YearMonth, b: YearMonth): boolean {
  return compareYearMonths(a, b) === 0;
}

export function clampYearMonth(
  date: YearMonth,
  earliest: YearMonth,
  latest: YearMonth,
): YearMonth {
  if (compareYearMonths(date, earliest) < 0) return earliest;
  if (compareYearMonths(date, latest) > 0) return latest;
  return date;
}

// The month and year of an ISO date such as a movie's release date
// ("2016-09-23"); null when there is no date or it can't be read.
export function parseYearMonth(
  isoDate: string | null | undefined,
): YearMonth | null {
  if (isoDate == null) return null;
  const match = /^(\d{4})-(\d{2})(?:-\d{2})?$/.exec(isoDate.trim());
  if (!match) return null;
  const [year, month] = match.slice(1).map(Number);
  return month >= 1 && month <= 12 ? { year, month } : null;
}

// e.g. "Sep 2026"
export function formatYearMonth(date: YearMonth): string {
  return `${MONTH_NAMES[date.month - 1]} ${String(date.year)}`;
}

// The month and year of a Date, in the user's own time zone
export function toYearMonth(date: Date): YearMonth {
  return { year: date.getFullYear(), month: date.getMonth() + 1 };
}

// The series' months in order, with their values (null where there is none)
function monthsOf(
  series: BlsTimeSeries,
): { date: YearMonth; value: number | null }[] {
  return series.rows.flatMap((row) =>
    row.months.map((value, i) => ({
      date: { year: row.year, month: i + 1 },
      value,
    })),
  );
}

// The last month the series has a value for; null when it has none at all.
export function latestAvailable(series: BlsTimeSeries): YearMonth | null {
  const months = monthsOf(series);
  for (let i = months.length - 1; i >= 0; i--) {
    if (months[i].value !== null) return months[i].date;
  }
  return null;
}

// The series that a figure from one month, adjusted to another, is adjusted
// with: the seasonally adjusted series when both months are within it,
// otherwise the not seasonally adjusted one (which goes back to 1913). Both
// index values then come from the same series, so their ratio means something.
export function seriesFor(
  inflation: Inflation,
  from: YearMonth,
  to: YearMonth,
): BlsTimeSeries {
  return from.year >= FIRST_SEASONALLY_ADJUSTED_YEAR &&
    to.year >= FIRST_SEASONALLY_ADJUSTED_YEAR
    ? inflation.seasonallyAdjusted
    : inflation.notSeasonallyAdjusted;
}

// The latest month a figure from `from` can be adjusted to: the last one in
// the series that would be used for any recent month (see seriesFor). Null
// when the series has no values at all.
export function latestAvailableFor(
  inflation: Inflation,
  from: YearMonth,
): YearMonth | null {
  return latestAvailable(
    from.year >= FIRST_SEASONALLY_ADJUSTED_YEAR
      ? inflation.seasonallyAdjusted
      : inflation.notSeasonallyAdjusted,
  );
}

// The series' index value for a month. A month the series has no value for
// (e.g. Oct 2025) gets the average of the nearest months before and after it
// that have one, or just the one of those there is. Null when the month is
// outside the series or the series has no values at all.
export function getIndexValue(
  series: BlsTimeSeries,
  date: YearMonth,
): number | null {
  const months = monthsOf(series);
  const at = months.findIndex((m) => sameYearMonth(m.date, date));
  if (at < 0) return null;
  if (months[at].value !== null) return months[at].value;

  let before: number | null = null;
  for (let i = at - 1; i >= 0 && before === null; i--) {
    before = months[i].value;
  }
  let after: number | null = null;
  for (let i = at + 1; i < months.length && after === null; i++) {
    after = months[i].value;
  }
  if (before !== null && after !== null) return (before + after) / 2;
  return before ?? after;
}

export interface AdjustedAmount {
  amount: number;
  // Whether the seasonally adjusted series was used (see seriesFor)
  isSeasonallyAdjusted: boolean;
}

// An amount of money from one month in the dollars of another. Null when
// either month's index value is unknown.
export function adjustForInflation(
  inflation: Inflation,
  amount: number,
  from: YearMonth,
  to: YearMonth,
): AdjustedAmount | null {
  const series = seriesFor(inflation, from, to);
  const fromValue = getIndexValue(series, from);
  const toValue = getIndexValue(series, to);
  if (fromValue === null || toValue === null || fromValue === 0) return null;
  return {
    amount: (amount * toValue) / fromValue,
    isSeasonallyAdjusted: series.isSeasonallyAdjusted,
  };
}
