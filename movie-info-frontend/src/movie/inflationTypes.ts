// Mirrors MovieInfoBackend's InflationDataModel (and the BLS data models in
// it) as serialized by System.Text.Json: camelCase properties.

// One year of a Bureau of Labor Statistics time series
export interface BlsTimeSeriesRow {
  year: number;
  // The index value of each month, Jan to Dec; null where the series has
  // none: not published yet, or never collected (e.g. Oct 2025, during the
  // lapse in appropriations)
  months: (number | null)[];
}

export interface BlsTimeSeries {
  seriesId: string;
  seriesTitle: string;
  isSeasonallyAdjusted: boolean;
  // By year ascending
  rows: BlsTimeSeriesRow[];
}

// The two Consumer Price Index series (CPI-U, all items, U.S. city average)
// that dollar amounts are adjusted for inflation with
export interface Inflation {
  // CUSR0000SA0, from Jan 1947
  seasonallyAdjusted: BlsTimeSeries;
  // CUUR0000SA0, from Jan 1913
  notSeasonallyAdjusted: BlsTimeSeries;
}
