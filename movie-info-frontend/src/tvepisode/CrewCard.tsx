import PersonCard from "../shared/PersonCard";
import { type PanelCardProps } from "../shared/usePanelCards";
import { type TvEpisodeCrew } from "./tvEpisodeTypes";

interface CrewCardProps extends PanelCardProps {
  crew: TvEpisodeCrew;
}

function CrewCard({ crew, ...cardProps }: CrewCardProps) {
  return (
    <PersonCard
      testId="crew-card"
      name={crew.name}
      originalName={crew.originalName}
      profilePath={crew.profilePath}
      details={[crew.job]}
      {...cardProps}
    />
  );
}

export default CrewCard;
