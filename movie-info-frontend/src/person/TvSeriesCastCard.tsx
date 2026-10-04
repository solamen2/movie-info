import CreditCard from "./CreditCard";
import { type PersonTvSeriesCast } from "./personTypes";

interface TvSeriesCastCardProps {
  credit: PersonTvSeriesCast;
  onClick: () => void;
}

function TvSeriesCastCard({ credit, onClick }: TvSeriesCastCardProps) {
  return (
    <CreditCard
      testId="tv-series-cast-card"
      posterPath={credit.posterPath}
      title={credit.name}
      originalTitle={credit.originalName}
      role={credit.character}
      date={credit.firstCreditAirDate ?? credit.firstAirDate}
      dateLabel={
        credit.firstCreditAirDate ? "First appearance" : "First air date"
      }
      episodeCount={credit.episodeCount}
      onClick={onClick}
    />
  );
}

export default TvSeriesCastCard;
