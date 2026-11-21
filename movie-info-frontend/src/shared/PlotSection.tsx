import { useState } from "react";
import Collapsible from "./Collapsible";
import OptionToggle from "./OptionToggle";
import { displayText } from "../utilities/utilities";
import "./shared.css";

export interface PlotSource {
  // Where the summary comes from, e.g. "TMDB"
  name: string;
  // Paragraphs are separated by newlines
  text: string | null | undefined;
  // Set when the source's summary could not be looked up: the source still
  // gets its button, which shows this message in place of a summary
  error?: string;
}

interface PlotSectionProps {
  // "Plot" or "Overview"
  title: string;
  // In the order their buttons are shown; the first one with a summary is
  // the one shown to begin with.
  sources: PlotSource[];
}

// A detail panel's plot summaries, one per source, in a single section with
// a button per source to switch between them. Sources without a summary get
// no button (unless looking it up failed: see PlotSource.error), and without
// any button at all there is no section.
function PlotSection({ title, sources }: PlotSectionProps) {
  const [selectedName, setSelectedName] = useState<string | null>(null);
  const available = sources.filter(
    (s) => s.error !== undefined || displayText(s.text) !== "—",
  );
  if (available.length === 0) return null;
  const shown = available.find((s) => s.name === selectedName) ?? available[0];

  return (
    <Collapsible title={title}>
      <OptionToggle
        label={`${title} source`}
        options={available.map((s) => s.name)}
        selected={shown.name}
        onSelect={setSelectedName}
      />
      {shown.error !== undefined ? (
        <p className="detail-empty">{shown.error}</p>
      ) : (
        displayText(shown.text)
          .split("\n")
          .filter((paragraph) => paragraph.trim() !== "")
          .map((paragraph, i) => (
            <p key={i} className="detail-prose">
              {paragraph}
            </p>
          ))
      )}
    </Collapsible>
  );
}

export default PlotSection;
