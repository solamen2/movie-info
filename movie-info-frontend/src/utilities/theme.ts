// The app's theme: light, dark, or "auto" to follow the OS setting. The CSS
// does the theming itself (see the tokens in index.css): this only records a
// picked theme, in localStorage, and puts it on <html> as data-theme. With
// nothing picked there is no attribute, and the CSS follows the OS setting,
// including when that changes while the app is open.
export const THEMES = ["auto", "light", "dark"] as const;
export type Theme = (typeof THEMES)[number];

// Also read by the script in index.html, which applies a picked theme before
// the first paint.
export const THEME_STORAGE_KEY = "theme";

function isTheme(value: unknown): value is Theme {
  return THEMES.includes(value as Theme);
}

// Storage can be unavailable (e.g. blocked by the browser's privacy
// settings); the theme then still applies, for as long as the page is open.
export function getStoredTheme(): Theme {
  try {
    const stored = localStorage.getItem(THEME_STORAGE_KEY);
    return isTheme(stored) ? stored : "auto";
  } catch {
    return "auto";
  }
}

export function storeTheme(theme: Theme) {
  try {
    if (theme === "auto") {
      localStorage.removeItem(THEME_STORAGE_KEY);
    } else {
      localStorage.setItem(THEME_STORAGE_KEY, theme);
    }
  } catch {
    // Not saved, see getStoredTheme
  }
}

export function applyTheme(theme: Theme) {
  const root = document.documentElement;
  if (theme === "auto") {
    delete root.dataset.theme;
  } else {
    root.dataset.theme = theme;
  }

  // The browser's own UI around the page takes its colour from the
  // theme-color <meta> whose media query matches (see index.html): the OS
  // setting picks between the two, unless a picked theme makes one of them
  // match always and the other never.
  for (const meta of document.querySelectorAll<HTMLMetaElement>(
    "meta[data-theme-color]",
  )) {
    const metaTheme = meta.dataset.themeColor ?? "";
    if (theme === "auto") {
      meta.media = `(prefers-color-scheme: ${metaTheme})`;
    } else {
      meta.media = theme === metaTheme ? "all" : "not all";
    }
  }
}

// Calls back with the new theme when it is picked in another tab or window of
// the app, so that every one of them changes together. Returns the function
// that stops listening.
export function onThemeChangedElsewhere(callback: (theme: Theme) => void) {
  function handleStorage(event: StorageEvent) {
    // A null key means the whole storage was cleared
    if (event.key === THEME_STORAGE_KEY || event.key === null) {
      callback(getStoredTheme());
    }
  }
  window.addEventListener("storage", handleStorage);
  return () => {
    window.removeEventListener("storage", handleStorage);
  };
}
