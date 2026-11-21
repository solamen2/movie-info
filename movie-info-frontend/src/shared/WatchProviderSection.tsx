import { useState } from "react";
import Collapsible from "./Collapsible";
import HorizontalList from "./HorizontalList";
import OptionToggle from "./OptionToggle";
import WatchProviderCard from "./WatchProviderCard";
import { type WatchProvider } from "./sharedTypes";

interface WatchProviderSectionProps {
  flatrate: WatchProvider[];
  rent: WatchProvider[];
  buy: WatchProvider[];
}

// A detail panel's "Where to Watch" section: the title's watch providers as a
// row of cards, in TMDB's display priority order, with a button per way of
// watching (stream, rent, buy) to switch between them. Ways without any
// provider get no button, and without any provider at all there is no section.
function WatchProviderSection({
  flatrate,
  rent,
  buy,
}: WatchProviderSectionProps) {
  const [selectedWay, setSelectedWay] = useState<string | null>(null);
  const available = [
    { way: "Stream", providers: flatrate },
    { way: "Rent", providers: rent },
    { way: "Buy", providers: buy },
  ].filter(({ providers }) => providers.length > 0);
  if (available.length === 0) return null;
  const shown = available.find((a) => a.way === selectedWay) ?? available[0];
  const sorted = [...shown.providers].sort(
    (a, b) => a.displayPriority - b.displayPriority,
  );

  return (
    <Collapsible title="Where to Watch">
      <OptionToggle
        label="Way to watch"
        options={available.map((a) => a.way)}
        selected={shown.way}
        onSelect={setSelectedWay}
      />
      <HorizontalList label="Where to watch">
        {sorted.map((p) => (
          <WatchProviderCard key={p.id} provider={p} />
        ))}
      </HorizontalList>
    </Collapsible>
  );
}

export default WatchProviderSection;
