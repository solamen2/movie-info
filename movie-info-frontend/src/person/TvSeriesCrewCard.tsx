import CreditCard from "./CreditCard";
import { type PanelCardProps } from "../shared/usePanelCards";
import { type PersonTvSeriesCrew } from "./personTypes";

interface TvSeriesCrewCardProps extends PanelCardProps {
  credit: PersonTvSeriesCrew;
}

function TvSeriesCrewCard({ credit, ...cardProps }: TvSeriesCrewCardProps) {
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
      {...cardProps}
    />
  );
}

export default TvSeriesCrewCard;
