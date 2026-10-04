import PersonCard from "../shared/PersonCard";
import { type TvEpisodeGuestStar } from "./tvEpisodeTypes";

interface GuestStarCardProps {
  guestStar: TvEpisodeGuestStar;
  onClick: () => void;
}

function GuestStarCard({ guestStar, onClick }: GuestStarCardProps) {
  return (
    <PersonCard
      testId="guest-star-card"
      name={guestStar.name}
      originalName={guestStar.originalName}
      profilePath={guestStar.profilePath}
      details={[guestStar.character]}
      onClick={onClick}
    />
  );
}

export default GuestStarCard;
