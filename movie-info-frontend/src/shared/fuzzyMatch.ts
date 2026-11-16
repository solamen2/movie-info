// Case-insensitive, accent-insensitive, typo-tolerant matching for the
// free-text filters above lists of people (see FilterablePeopleList).

// Lower case, without diacritics ("Zoë" → "zoe")
function normalize(text: string): string {
  return text.normalize("NFD").replace(/\p{M}/gu, "").toLowerCase();
}

// How many typos a word of the filter may have and still match: short words
// must appear exactly, since nearly anything is within a typo of them.
function allowedTypos(word: string): number {
  if (word.length < 5) return 0;
  if (word.length < 9) return 1;
  return 2;
}

// The fewest typos (a character inserted, dropped or replaced, or two
// neighbors swapped) that turn `word` into some substring of `text`: Sellers'
// algorithm, with the swap of the optimal string alignment distance.
function substringDistance(word: string, text: string): number {
  // Row i holds, per end position in the text, the distance from the first i
  // characters of the word to the nearest substring ending there
  let beforePrevious: number[] = [];
  let previous = new Array<number>(text.length + 1).fill(0);
  for (let i = 1; i <= word.length; i++) {
    const current = [i];
    for (let j = 1; j <= text.length; j++) {
      let distance = Math.min(
        previous[j - 1] + (word[i - 1] === text[j - 1] ? 0 : 1),
        previous[j] + 1,
        current[j - 1] + 1,
      );
      if (
        i > 1 &&
        j > 1 &&
        word[i - 1] === text[j - 2] &&
        word[i - 2] === text[j - 1]
      ) {
        distance = Math.min(distance, beforePrevious[j - 2] + 1);
      }
      current.push(distance);
    }
    beforePrevious = previous;
    previous = current;
  }
  return Math.min(...previous);
}

// Whether every word of the filter appears in at least one of the texts, as
// a substring give or take a typo or two for longer words ("hanks", "hnaks"
// and "tom hank" all match "Tom Hanks"). A blank filter matches everything.
export function fuzzyMatches(filter: string, texts: string[]): boolean {
  const words = normalize(filter).split(/\s+/).filter(Boolean);
  if (words.length === 0) return true;
  const candidates = texts.map(normalize);
  return words.every((word) =>
    candidates.some(
      (text) =>
        text.includes(word) ||
        substringDistance(word, text) <= allowedTypos(word),
    ),
  );
}
