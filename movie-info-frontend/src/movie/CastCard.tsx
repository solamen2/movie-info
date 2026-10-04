import PersonCard from "../shared/PersonCard";
import { type MovieCast } from "./movieTypes";

interface CastCardProps {
  cast: MovieCast;
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
