import PersonCard from "../shared/PersonCard";
import { type TvSeriesCrew, displayEpisodeCount } from "./tvSeriesTypes";

interface CrewCardProps {
  crew: TvSeriesCrew;
  onClick: () => void;
}

function CrewCard({ crew, onClick }: CrewCardProps) {
  const jobs = crew.jobs.filter((j) => j !== "").join(", ");

  return (
    <PersonCard
      testId="crew-card"
      name={crew.name}
      originalName={crew.originalName}
      profilePath={crew.profilePath}
      details={[
        jobs,
        crew.totalEpisodeCount > 0
          ? displayEpisodeCount(crew.totalEpisodeCount)
          : "",
      ]}
      onClick={onClick}
    />
  );
}

export default CrewCard;
