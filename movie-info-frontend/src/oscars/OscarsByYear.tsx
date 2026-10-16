import { useState } from "react";
import Collapsible from "../shared/Collapsible";
import { useDetailData } from "../shared/useDetailData";
import OscarAwardLine, { CategoryAsterisk } from "./OscarAwardLine";
import type {
  OscarAward,
  OscarCategory,
  OscarCategorySubsection,
  OscarYear,
} from "./oscarTypes";
import { ordinal, oscarYearElementId } from "./oscarUtils";
import "./oscars.css";

function AwardList({
  awards,
  testId,
}: {
  awards: OscarAward[];
  testId: string;
}) {
  return (
    <ul className="oscar-award-list" data-testid={testId}>
      {awards.map((award, i) => (
        <li key={i}>
          <OscarAwardLine award={award} context="year" />
        </li>
      ))}
    </ul>
  );
}

// The awards of one category (or of one subsection of it): the winner(s),
// with the other nominees behind a collapsed "Nominees" section
function CategorySubsection({
  subsection,
}: {
  subsection: OscarCategorySubsection;
}) {
  const winners = subsection.entries.filter((e) => e.winner);
  const nominees = subsection.entries.filter((e) => !e.winner);
  return (
    <>
      <AwardList awards={winners} testId="oscar-winners" />
      {nominees.length > 0 && (
        // Lazy, since the page has thousands of nominees that are mostly
        // never looked at
        <Collapsible title="Nominees" count={nominees.length} lazy>
          <AwardList awards={nominees} testId="oscar-nominees" />
        </Collapsible>
      )}
    </>
  );
}

// One category of one year. A category split that year (e.g. into
// black-and-white and color) has a titled subsection for each half.
function CategorySection({ category }: { category: OscarCategory }) {
  const split = category.subsections.length > 1;
  const whole = category.subsections[0];
  return (
    <div className="oscar-category" data-testid="oscar-category">
      <h3 className="oscar-category-title">
        {category.category}
        {!split && whole.originalCategory !== null && (
          <CategoryAsterisk originalCategory={whole.originalCategory} />
        )}
      </h3>
      {split ? (
        category.subsections.map((subsection, i) => (
          <div
            key={i}
            className="oscar-category-subsection"
            data-testid="oscar-category-subsection"
          >
            <h4 className="oscar-category-subsection-title">
              {subsection.section}
              {subsection.originalCategory !== null && (
                <CategoryAsterisk
                  originalCategory={subsection.originalCategory}
                />
              )}
            </h4>
            <CategorySubsection subsection={subsection} />
          </div>
        ))
      ) : (
        <CategorySubsection subsection={whole} />
      )}
    </div>
  );
}

// One ceremony as a collapsed section. Its contents are only rendered while
// it is open, since there are nearly a hundred years of awards on the page.
function YearSection({
  year,
  open,
  onOpenChange,
}: {
  year: OscarYear;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  return (
    <details
      className="collapsible oscar-year"
      id={oscarYearElementId(year.ceremony)}
      data-testid="oscar-year"
      open={open}
      onToggle={(e) => {
        onOpenChange(e.currentTarget.open);
      }}
    >
      <summary>
        <span className="chevron" aria-hidden="true" />
        <h2 className="oscar-year-heading">
          {year.year}
          <span className="oscar-year-ceremony">
            {ordinal(year.ceremony)} Academy Awards
          </span>
        </h2>
      </summary>
      <div className="collapsible-body">
        {open && (
          <>
            {year.categories.map((category, i) => (
              <CategorySection key={i} category={category} />
            ))}
            {year.specialAwards.length > 0 && (
              <div
                className="oscar-category"
                data-testid="oscar-special-awards"
              >
                <h3 className="oscar-category-title">Special Awards</h3>
                <AwardList awards={year.specialAwards} testId="oscar-winners" />
              </div>
            )}
          </>
        )}
      </div>
    </details>
  );
}

// Every ceremony's Academy Award winners and nominees, most recent first,
// in the app's fixed list of categories (see OscarCategoriesHelper on the
// backend) followed by the year's special awards. Each year is a section
// that starts collapsed; "Expand All" / "Collapse All" open and close them
// all, and "Jump To" scrolls to a year and opens it.
function OscarsByYear() {
  const { data: years, error } = useDetailData("/api/oscars", "Oscars");
  // The ceremonies whose sections are open
  const [openYears, setOpenYears] = useState<ReadonlySet<number>>(new Set());

  function setYearOpen(ceremony: number, open: boolean) {
    setOpenYears((previous) => {
      if (previous.has(ceremony) === open) return previous;
      const next = new Set(previous);
      if (open) {
        next.add(ceremony);
      } else {
        next.delete(ceremony);
      }
      return next;
    });
  }

  function jumpTo(ceremony: string) {
    setYearOpen(Number(ceremony), true);
    document
      .getElementById(oscarYearElementId(Number(ceremony)))
      ?.scrollIntoView();
  }

  return (
    <div className="oscars-page" data-testid="oscars-page">
      <h1>Oscars By Year</h1>
      {error && <p className="error-message">{error}</p>}
      {!years && !error && (
        <p className="detail-loading">Loading the Oscars…</p>
      )}
      {years && (
        <>
          <div className="oscars-toolbar">
            <label className="oscars-jump">
              Jump To:
              <select
                aria-label="jump-to-year"
                defaultValue=""
                onChange={(e) => {
                  jumpTo(e.target.value);
                }}
              >
                <option value="" disabled>
                  Select a year
                </option>
                {years.map((year) => (
                  <option key={year.ceremony} value={year.ceremony}>
                    {year.year}
                  </option>
                ))}
              </select>
            </label>
            <button
              type="button"
              onClick={() => {
                setOpenYears(new Set(years.map((year) => year.ceremony)));
              }}
            >
              Expand All
            </button>
            <button
              type="button"
              onClick={() => {
                setOpenYears(new Set());
              }}
            >
              Collapse All
            </button>
          </div>
          {years.map((year) => (
            <YearSection
              key={year.ceremony}
              year={year}
              open={openYears.has(year.ceremony)}
              onOpenChange={(open) => {
                setYearOpen(year.ceremony, open);
              }}
            />
          ))}
        </>
      )}
    </div>
  );
}

export default OscarsByYear;
