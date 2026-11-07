import { useState } from "react";
import { displayMoney } from "../utilities/utilities";
import {
  EARLIEST_INFLATION_DATE,
  MONTH_NAMES,
  type YearMonth,
  adjustForInflation,
  clampYearMonth,
  compareYearMonths,
  formatYearMonth,
  latestAvailableFor,
  sameYearMonth,
  toYearMonth,
} from "./inflationUtils";
import { useInflationData } from "./useInflationData";
import "./movie.css";

const SEASONAL_ADJUSTMENT_URL =
  "https://en.wikipedia.org/wiki/Seasonal_adjustment";

// "(seasonally adj.)" or "(not seasonally adj.)" after an adjusted amount,
// linking to an explanation of what that means
function SeasonalAdjustmentLabel({
  isSeasonallyAdjusted,
}: {
  isSeasonallyAdjusted: boolean;
}) {
  return (
    <a
      href={SEASONAL_ADJUSTMENT_URL}
      target="_blank"
      rel="noopener"
      className="inflation-adjustment-label"
    >
      ({isSeasonallyAdjusted ? "seasonally adj." : "not seasonally adj."})
    </a>
  );
}

interface InflationAdjustedMoneyProps {
  // The amount in the dollars of the release month; more than zero
  amount: number;
  // The movie's release month and year
  releaseDate: YearMonth;
  // "budget" or "revenue": names the controls for assistive technology (and
  // tests), e.g. "budget-month"
  name: string;
}

// A movie's budget or revenue with its inflation adjustment: the amount with
// dropdowns for the month and year it is adjusted to (the release month and
// year to begin with, when the amount is shown as is; once changed, the
// adjusted amount is shown in bold, with a button to go back), and below it
// the amount in the dollars of the current month, or the latest month the
// inflation data has. Without the inflation data, just the amount, with a
// note in red when loading the data failed.
function InflationAdjustedMoney({
  amount,
  releaseDate,
  name,
}: InflationAdjustedMoneyProps) {
  const { data: inflation, error } = useInflationData();
  // The month and year picked, null while the release month and year stand
  const [selected, setSelected] = useState<YearMonth | null>(null);

  const latest =
    inflation === null ? null : latestAvailableFor(inflation, releaseDate);
  if (inflation === null || latest === null) {
    return (
      <>
        {displayMoney(amount)}
        {(error !== "" || inflation !== null) && (
          <>
            {" "}
            <span className="inflation-unavailable">
              (inflation data not available)
            </span>
          </>
        )}
      </>
    );
  }

  const earliest = EARLIEST_INFLATION_DATE;
  const release = clampYearMonth(releaseDate, earliest, latest);
  const target =
    selected === null ? release : clampYearMonth(selected, earliest, latest);
  const changed = !sameYearMonth(target, release);
  const adjusted = changed
    ? adjustForInflation(inflation, amount, release, target)
    : null;
  const now = clampYearMonth(toYearMonth(new Date()), earliest, latest);
  const nowAdjusted = adjustForInflation(inflation, amount, release, now);

  const years: number[] = [];
  for (let year = earliest.year; year <= latest.year; year++) {
    years.push(year);
  }
  const isAvailable = (date: YearMonth): boolean =>
    compareYearMonths(date, earliest) >= 0 &&
    compareYearMonths(date, latest) <= 0;

  return (
    <span className="inflation-money">
      <span className="inflation-money-line">
        {changed ? (
          <strong>
            {adjusted === null ? "—" : displayMoney(adjusted.amount)}
          </strong>
        ) : (
          displayMoney(amount)
        )}
        {adjusted !== null && (
          <SeasonalAdjustmentLabel
            isSeasonallyAdjusted={adjusted.isSeasonallyAdjusted}
          />
        )}
        {/* The dropdowns (and the reset button) stay together on one line,
            wherever the amount's width leaves them */}
        <span className="inflation-money-dates">
          <select
            aria-label={`${name}-month`}
            value={target.month}
            onChange={(e) => {
              setSelected({ year: target.year, month: Number(e.target.value) });
            }}
          >
            {MONTH_NAMES.map((monthName, i) => (
              <option
                key={monthName}
                value={i + 1}
                disabled={!isAvailable({ year: target.year, month: i + 1 })}
              >
                {monthName}
              </option>
            ))}
          </select>
          <select
            aria-label={`${name}-year`}
            value={target.year}
            onChange={(e) => {
              setSelected({
                year: Number(e.target.value),
                month: target.month,
              });
            }}
          >
            {years.map((year) => (
              <option key={year} value={year}>
                {year}
              </option>
            ))}
          </select>
          {changed && (
            <button
              type="button"
              className="inflation-reset-button"
              aria-label={`reset-${name}-date`}
              title="Back to the release month and year"
              onClick={() => {
                setSelected(null);
              }}
            >
              ×
            </button>
          )}
        </span>
      </span>
      <span className="inflation-money-now">
        {nowAdjusted === null ? (
          "—"
        ) : (
          <>
            {displayMoney(nowAdjusted.amount)} ({formatYearMonth(now)}){" "}
            <SeasonalAdjustmentLabel
              isSeasonallyAdjusted={nowAdjusted.isSeasonallyAdjusted}
            />
          </>
        )}
      </span>
    </span>
  );
}

export default InflationAdjustedMoney;
