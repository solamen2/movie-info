// Mirrors MovieInfoBackend's Oscar view models (OscarsViewModel.Years and
// MovieViewModel.Oscars / PersonViewModel.Oscars) as serialized by
// System.Text.Json: camelCase properties.

export interface OscarFilm {
  title: string;
  // null when the dataset has no IMDB id for the film
  imdbId: string | null;
}

export interface OscarNominee {
  name: string;
  // null when the dataset has no IMDB id for the person (or company)
  imdbId: string | null;
}

// One nomination (or win): a film and / or the people honored, in one
// category at one ceremony.
export interface OscarAward {
  ceremony: number;
  // The year of the films honored, e.g. "1927/28" or "2023"
  year: string;
  // As shown, e.g. "Best Picture" or "Honorary Award"
  category: string;
  // The category's name at the time when it differs from the modern one,
  // e.g. "Outstanding Production"; otherwise null
  originalCategory: string | null;
  // A special (non-competitive) award, listed after the categories
  isSpecial: boolean;
  winner: boolean;
  films: OscarFilm[];
  // The credit as announced, e.g. "Emma Thomas, Charles Roven and
  // Christopher Nolan, Producers"; "" for some special awards
  name: string;
  // The individual people (or companies) in name
  nominees: OscarNominee[];
  // Acting awards: the character played, one per film
  characters: string[];
  // Original Song awards: the song's title
  song: string | null;
  note: string | null;
  // The text of special awards
  citation: string | null;
}

// The awards in one category, or in one subsection of a category split that
// year, at one ceremony: the winner(s) first, then the other nominees
export interface OscarCategorySubsection {
  // The subsection's title when the category was split that year, e.g.
  // "Color" or "Best Sound Mixing"; otherwise null
  section: string | null;
  // The name(s) at the time when differing from the modern one
  originalCategory: string | null;
  entries: OscarAward[];
}

// One category at one ceremony: its awards as one undivided subsection, or
// in a titled subsection each in the years the category was split (e.g. into
// black-and-white and color)
export interface OscarCategory {
  category: string;
  subsections: OscarCategorySubsection[];
}

// One ceremony: its categories in display order, then its special awards
export interface OscarYear {
  ceremony: number;
  year: string;
  categories: OscarCategory[];
  specialAwards: OscarAward[];
}
