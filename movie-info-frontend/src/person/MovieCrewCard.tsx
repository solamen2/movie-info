import CreditCard from "./CreditCard";
import { type PersonMovieCrew } from "./personTypes";

interface MovieCrewCardProps {
  credit: PersonMovieCrew;
}

function MovieCrewCard({ credit }: MovieCrewCardProps) {
  return (
    <CreditCard
      testId="movie-crew-card"
      posterPath={credit.posterPath}
      title={credit.title}
      originalTitle={credit.originalTitle}
      role={credit.job}
      date={credit.releaseDate}
    />
  );
}

export default MovieCrewCard;
