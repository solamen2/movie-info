import PersonCard from "../shared/PersonCard";
import { type MovieCrew } from "./movieTypes";

interface CrewCardProps {
  crew: MovieCrew;
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
