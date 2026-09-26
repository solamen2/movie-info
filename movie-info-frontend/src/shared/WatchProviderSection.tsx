import Collapsible from "./Collapsible";
import HorizontalList from "./HorizontalList";
import WatchProviderCard from "./WatchProviderCard";
import { type WatchProvider } from "./sharedTypes";

interface WatchProviderSectionProps {
  title: string;
  providers: WatchProvider[];
}

// A collapsed row of watch provider cards, in TMDB's display priority order.
function WatchProviderSection({ title, providers }: WatchProviderSectionProps) {
  const sorted = [...providers].sort(
    (a, b) => a.displayPriority - b.displayPriority,
  );
  return (
    <Collapsible title={title} count={providers.length}>
      <HorizontalList>
        {sorted.map((p) => (
          <WatchProviderCard key={p.id} provider={p} />
        ))}
      </HorizontalList>
    </Collapsible>
  );
}

export default WatchProviderSection;
