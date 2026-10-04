import PersonCard from "../shared/PersonCard";
import { type PanelCardProps } from "../shared/usePanelCards";
import { type TvEpisodeCast } from "./tvEpisodeTypes";

interface CastCardProps extends PanelCardProps {
  cast: TvEpisodeCast;
}

function CastCard({ cast, ...cardProps }: CastCardProps) {
  return (
    <PersonCard
      testId="cast-card"
      name={cast.name}
      originalName={cast.originalName}
      profilePath={cast.profilePath}
      details={[cast.character]}
      {...cardProps}
    />
  );
}

export default CastCard;
