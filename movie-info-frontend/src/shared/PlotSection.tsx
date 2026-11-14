import { useState } from "react";
import Collapsible from "./Collapsible";
import OptionToggle from "./OptionToggle";
import { displayText } from "../utilities/utilities";
import "./shared.css";

export interface PlotSource {
  // Where the summary comes from, e.g. "TMDB"
  name: string;
  text: string | null | undefined;
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
// no button, and without any summary at all there is no section.
function PlotSection({ title, sources }: PlotSectionProps) {
  const [selectedName, setSelectedName] = useState<string | null>(null);
  const available = sources.filter((s) => displayText(s.text) !== "—");
  if (available.length === 0) return null;
  const shown = available.find((s) => s.name === selectedName) ?? available[0];

  return (
    <Collapsible title={title}>
      <OptionToggle
        label={`${title.toLowerCase()} source`}
        options={available.map((s) => s.name)}
        selected={shown.name}
        onSelect={setSelectedName}
      />
      <p className="detail-prose">{displayText(shown.text)}</p>
    </Collapsible>
  );
}

export default PlotSection;
