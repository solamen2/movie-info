import PersonCard from "../shared/PersonCard";
import { type PanelCardProps } from "../shared/usePanelCards";
import { type MovieCast } from "./movieTypes";

interface CastCardProps extends PanelCardProps {
  cast: MovieCast;
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
