import CreditCard from "./CreditCard";
import { type PersonTvSeriesCast } from "./personTypes";

interface TvSeriesCastCardProps {
  credit: PersonTvSeriesCast;
}

function TvSeriesCastCard({ credit }: TvSeriesCastCardProps) {
  return (
    <CreditCard
      testId="tv-series-cast-card"
      posterPath={credit.posterPath}
      title={credit.name}
      originalTitle={credit.originalName}
      role={credit.character}
      date={credit.firstCreditAirDate ?? credit.firstAirDate}
      episodeCount={credit.episodeCount}
    />
  );
}

export default TvSeriesCastCard;
