import CreditCard from "./CreditCard";
import { type PanelCardProps } from "../shared/usePanelCards";
import { type PersonMovieCast } from "./personTypes";

interface MovieCastCardProps extends PanelCardProps {
  credit: PersonMovieCast;
}

function MovieCastCard({ credit, ...cardProps }: MovieCastCardProps) {
  return (
    <CreditCard
      testId="movie-cast-card"
      posterPath={credit.posterPath}
      title={credit.title}
      originalTitle={credit.originalTitle}
      role={credit.character}
      date={credit.releaseDate}
      {...cardProps}
    />
  );
}

export default MovieCastCard;
