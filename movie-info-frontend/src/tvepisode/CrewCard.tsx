import PersonCard from "../shared/PersonCard";
import { type TvEpisodeCrew } from "./tvEpisodeTypes";

interface CrewCardProps {
  crew: TvEpisodeCrew;
  onClick: () => void;
}

function CrewCard({ crew, onClick }: CrewCardProps) {
  return (
    <PersonCard
      testId="crew-card"
      name={crew.name}
      originalName={crew.originalName}
      profilePath={crew.profilePath}
      details={[crew.job]}
      onClick={onClick}
    />
  );
}

export default CrewCard;
