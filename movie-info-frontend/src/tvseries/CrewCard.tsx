import PersonCard from "../shared/PersonCard";
import { type PanelCardProps } from "../shared/usePanelCards";
import { type TvSeriesCrew, displayEpisodeCount } from "./tvSeriesTypes";

interface CrewCardProps extends PanelCardProps {
  crew: TvSeriesCrew;
}

function CrewCard({ crew, ...cardProps }: CrewCardProps) {
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
      {...cardProps}
    />
  );
}

export default CrewCard;
