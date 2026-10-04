import PersonCard from "../shared/PersonCard";
import { type TvSeriesCreator } from "./tvSeriesTypes";

interface CreatorCardProps {
  creator: TvSeriesCreator;
  onClick: () => void;
}

function CreatorCard({ creator, onClick }: CreatorCardProps) {
  return (
    <PersonCard
      testId="creator-card"
      name={creator.name}
      originalName={creator.originalName}
      profilePath={creator.profilePath}
      details={[]}
      onClick={onClick}
    />
  );
}

export default CreatorCard;
