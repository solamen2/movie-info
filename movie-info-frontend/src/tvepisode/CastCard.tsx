import PersonCard from "../shared/PersonCard";
import { type TvEpisodeCast } from "./tvEpisodeTypes";

interface CastCardProps {
  cast: TvEpisodeCast;
  onClick: () => void;
}

function CastCard({ cast, onClick }: CastCardProps) {
  return (
    <PersonCard
      testId="cast-card"
      name={cast.name}
      originalName={cast.originalName}
      profilePath={cast.profilePath}
      details={[cast.character]}
      onClick={onClick}
    />
  );
}

export default CastCard;
