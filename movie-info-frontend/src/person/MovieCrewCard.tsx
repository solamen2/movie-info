import CreditCard from "./CreditCard";
import { type PersonMovieCrew } from "./personTypes";

interface MovieCrewCardProps {
  credit: PersonMovieCrew;
  onClick: () => void;
}

function MovieCrewCard({ credit, onClick }: MovieCrewCardProps) {
  return (
    <CreditCard
      testId="movie-crew-card"
      posterPath={credit.posterPath}
      title={credit.title}
      originalTitle={credit.originalTitle}
      role={credit.job}
      date={credit.releaseDate}
      onClick={onClick}
    />
  );
}

export default MovieCrewCard;
