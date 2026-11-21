import { useEffect, useState } from "react";
import OptionToggle from "./OptionToggle";
import {
  applyTheme,
  getStoredTheme,
  onThemeChangedElsewhere,
  storeTheme,
  THEMES,
  type Theme,
} from "../utilities/theme";

const THEME_LABELS: Record<Theme, string> = {
  auto: "Auto",
  light: "Light",
  dark: "Dark",
};

// "Theme: Auto | Light | Dark", in the footer. Picking one applies it at once
// and saves it in the browser for the next visit; Auto follows the OS
// setting.
function ThemeToggle() {
  const [theme, setTheme] = useState(getStoredTheme);

  useEffect(() => {
    applyTheme(theme);
  }, [theme]);

  useEffect(() => onThemeChangedElsewhere(setTheme), []);

  return (
    <div className="theme-toggle">
      <span aria-hidden="true">Theme:</span>
      <OptionToggle
        label="Theme"
        options={[...THEMES]}
        labelOf={(option) => THEME_LABELS[option as Theme]}
        selected={theme}
        onSelect={(option) => {
          const picked = option as Theme;
          storeTheme(picked);
          setTheme(picked);
        }}
      />
    </div>
  );
}

export default ThemeToggle;
