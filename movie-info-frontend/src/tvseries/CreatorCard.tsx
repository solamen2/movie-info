import PersonCard from "../shared/PersonCard";
import { type PanelCardProps } from "../shared/usePanelCards";
import { type TvSeriesCreator } from "./tvSeriesTypes";

interface CreatorCardProps extends PanelCardProps {
  creator: TvSeriesCreator;
}

function CreatorCard({ creator, ...cardProps }: CreatorCardProps) {
  return (
    <PersonCard
      testId="creator-card"
      name={creator.name}
      originalName={creator.originalName}
      profilePath={creator.profilePath}
      details={[]}
      {...cardProps}
    />
  );
}

export default CreatorCard;
