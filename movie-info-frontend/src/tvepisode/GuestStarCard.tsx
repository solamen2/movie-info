import PersonCard from "../shared/PersonCard";
import { type PanelCardProps } from "../shared/usePanelCards";
import { type TvEpisodeGuestStar } from "./tvEpisodeTypes";

interface GuestStarCardProps extends PanelCardProps {
  guestStar: TvEpisodeGuestStar;
}

function GuestStarCard({ guestStar, ...cardProps }: GuestStarCardProps) {
  return (
    <PersonCard
      testId="guest-star-card"
      name={guestStar.name}
      originalName={guestStar.originalName}
      profilePath={guestStar.profilePath}
      details={[guestStar.character]}
      {...cardProps}
    />
  );
}

export default GuestStarCard;
