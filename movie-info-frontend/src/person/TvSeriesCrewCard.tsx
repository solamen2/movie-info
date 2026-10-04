import CreditCard from "./CreditCard";
import { type PersonTvSeriesCrew } from "./personTypes";

interface TvSeriesCrewCardProps {
  credit: PersonTvSeriesCrew;
  onClick: () => void;
}

function TvSeriesCrewCard({ credit, onClick }: TvSeriesCrewCardProps) {
  return (
    <CreditCard
      testId="tv-series-crew-card"
      posterPath={credit.posterPath}
      title={credit.name}
      originalTitle={credit.originalName}
      role={credit.job}
      date={credit.firstCreditAirDate ?? credit.firstAirDate}
      dateLabel={
        credit.firstCreditAirDate ? "First appearance" : "First air date"
      }
      episodeCount={credit.episodeCount}
      onClick={onClick}
    />
  );
}

export default TvSeriesCrewCard;
