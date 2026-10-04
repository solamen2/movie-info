import PersonCard from "../shared/PersonCard";
import { type PanelCardProps } from "../shared/usePanelCards";
import { type TvSeriesCast, displayEpisodeCount } from "./tvSeriesTypes";

interface CastCardProps extends PanelCardProps {
  cast: TvSeriesCast;
  // Whether to show how many episodes the cast member appears in (default
  // true); see TvSeriesCastCollapsible for why it can be hidden.
  showEpisodeCount?: boolean;
}

function CastCard({
  cast,
  showEpisodeCount = true,
  ...cardProps
}: CastCardProps) {
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
      {...cardProps}
    />
  );
}

export default CastCard;
