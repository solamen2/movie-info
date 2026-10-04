import CreditCard from "./CreditCard";
import { type PersonMovieCast } from "./personTypes";

interface MovieCastCardProps {
  credit: PersonMovieCast;
  onClick: () => void;
}

function MovieCastCard({ credit, onClick }: MovieCastCardProps) {
  return (
    <CreditCard
      testId="movie-cast-card"
      posterPath={credit.posterPath}
      title={credit.title}
      originalTitle={credit.originalTitle}
      role={credit.character}
      date={credit.releaseDate}
      onClick={onClick}
    />
  );
}

export default MovieCastCard;
