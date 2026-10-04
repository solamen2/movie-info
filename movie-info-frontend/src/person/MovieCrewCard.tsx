import CreditCard from "./CreditCard";
import { type PanelCardProps } from "../shared/usePanelCards";
import { type PersonMovieCrew } from "./personTypes";

interface MovieCrewCardProps extends PanelCardProps {
  credit: PersonMovieCrew;
}

function MovieCrewCard({ credit, ...cardProps }: MovieCrewCardProps) {
  return (
    <CreditCard
      testId="movie-crew-card"
      posterPath={credit.posterPath}
      title={credit.title}
      originalTitle={credit.originalTitle}
      role={credit.job}
      date={credit.releaseDate}
      {...cardProps}
    />
  );
}

export default MovieCrewCard;
