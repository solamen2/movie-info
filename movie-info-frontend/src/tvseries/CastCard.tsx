import PersonCard from "../shared/PersonCard";
import { type TvSeriesCast, displayEpisodeCount } from "./tvSeriesTypes";

interface CastCardProps {
  cast: TvSeriesCast;
  // Whether to show how many episodes the cast member appears in (default
  // true); see TvSeriesCastCollapsible for why it can be hidden.
  showEpisodeCount?: boolean;
  onClick: () => void;
}

function CastCard({ cast, showEpisodeCount = true, onClick }: CastCardProps) {
  const characters = cast.characters.filter((c) => c !== "").join(", ");

  return (
    <PersonCard
      testId="cast-card"
      name={cast.name}
      originalName={cast.originalName}
      profilePath={cast.profilePath}
      details={[
        characters,
        showEpisodeCount && cast.totalEpisodeCount > 0
          ? displayEpisodeCount(cast.totalEpisodeCount)
          : "",
      ]}
      onClick={onClick}
    />
  );
}

export default CastCard;
