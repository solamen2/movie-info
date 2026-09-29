import CreditCard from "./CreditCard";
import { type PersonTvSeriesCrew } from "./personTypes";

interface TvSeriesCrewCardProps {
  credit: PersonTvSeriesCrew;
}

function TvSeriesCrewCard({ credit }: TvSeriesCrewCardProps) {
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
    />
  );
}

export default TvSeriesCrewCard;
