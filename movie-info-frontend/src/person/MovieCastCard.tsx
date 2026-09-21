import CreditCard from "./CreditCard";
import { type PersonMovieCast } from "./personTypes";

interface MovieCastCardProps {
  credit: PersonMovieCast;
}

function MovieCastCard({ credit }: MovieCastCardProps) {
  return (
    <CreditCard
      testId="movie-cast-card"
      posterPath={credit.posterPath}
      title={credit.title}
      originalTitle={credit.originalTitle}
      role={credit.character}
      date={credit.releaseDate}
    />
  );
}

export default MovieCastCard;
