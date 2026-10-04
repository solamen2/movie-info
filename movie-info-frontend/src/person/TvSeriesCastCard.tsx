import CreditCard from "./CreditCard";
import { type PanelCardProps } from "../shared/usePanelCards";
import { type PersonTvSeriesCast } from "./personTypes";

interface TvSeriesCastCardProps extends PanelCardProps {
  credit: PersonTvSeriesCast;
}

function TvSeriesCastCard({ credit, ...cardProps }: TvSeriesCastCardProps) {
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
      {...cardProps}
    />
  );
}

export default TvSeriesCastCard;
