// A piece of an award's credit text: plain text, or the name of one of its
// nominees (to be linked).
export interface NamePart {
  text: string;
  nominee: string | null;
}

// The spellings under which a nominee may appear in the credit text: the
// dataset drops the comma before a suffix in nominee names ("Cuba Gooding
// Jr.") that the credit text keeps ("Cuba Gooding, Jr.").
function spellings(nominee: string): string[] {
  const withComma = nominee.replace(/ (Jr|Sr)\.?$/, ", $1.");
  return withComma === nominee ? [nominee] : [nominee, withComma];
}

// Splits an award's credit text (e.g. "Emma Thomas, Charles Roven and
// Christopher Nolan, Producers") into the nominee names in it and the text
// between them, keeping the text as announced. A nominee missing from the
// text is left out (its name is still visible, just not marked). When there
// is no credit text at all (some special awards), the nominees are listed.
export function splitNameIntoNominees(
  name: string,
  nominees: string[],
): NamePart[] {
  const uniqueNominees = [...new Set(nominees.filter((n) => n !== ""))];
  if (name === "") {
    return uniqueNominees.flatMap((nominee, i) =>
      i === 0
        ? [{ text: nominee, nominee }]
        : [
            { text: ", ", nominee: null },
            { text: nominee, nominee },
          ],
    );
  }

  // Longest names first, so a name contained in another ("Tom Hanks" in
  // "Tom Hanks Jr.") can't claim the longer name's text.
  const matches: { start: number; end: number; nominee: string }[] = [];
  for (const nominee of [...uniqueNominees].sort(
    (a, b) => b.length - a.length,
  )) {
    for (const spelling of spellings(nominee)) {
      let from = 0;
      let found = false;
      while (!found) {
        const start = name.indexOf(spelling, from);
        if (start < 0) break;
        const end = start + spelling.length;
        if (!matches.some((m) => start < m.end && end > m.start)) {
          matches.push({ start, end, nominee });
          found = true;
        }
        from = start + 1;
      }
      if (found) break;
    }
  }
  matches.sort((a, b) => a.start - b.start);

  const parts: NamePart[] = [];
  let position = 0;
  for (const match of matches) {
    if (match.start > position) {
      parts.push({ text: name.slice(position, match.start), nominee: null });
    }
    parts.push({
      text: name.slice(match.start, match.end),
      nominee: match.nominee,
    });
    position = match.end;
  }
  if (position < name.length) {
    parts.push({ text: name.slice(position), nominee: null });
  }
  return parts;
}

// e.g. 1 -> "1st", 22 -> "22nd", 96 -> "96th"
// Writing awards are credited as "Written by X", "Screenplay by X", "Written
// for the screen by X", … which says nothing under a screenplay category's
// heading, so the Oscars By Year page drops that lead-in. A credit naming
// several roles ("Story by X; Screenplay by Y") keeps them all.
const WRITING_CREDIT_LEAD_IN =
  /^(?:written|screenplay|story|adapted)\b[^;]*?\bby\s+/i;

export function stripWritingCredit(name: string): string {
  const match = WRITING_CREDIT_LEAD_IN.exec(name);
  if (match === null) return name;
  const rest = name.slice(match[0].length);
  return /\bby\b/i.test(rest) ? name : rest;
}

export function ordinal(n: number): string {
  const lastTwo = n % 100;
  if (lastTwo >= 11 && lastTwo <= 13) return `${String(n)}th`;
  switch (n % 10) {
    case 1:
      return `${String(n)}st`;
    case 2:
      return `${String(n)}nd`;
    case 3:
      return `${String(n)}rd`;
    default:
      return `${String(n)}th`;
  }
}

// The id of a year's section on the Oscars By Year page, for jumping to it
export function oscarYearElementId(ceremony: number): string {
  return `oscars-year-${String(ceremony)}`;
}
