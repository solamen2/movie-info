import { displayText, getTmdbImageUrl } from "../utilities/utilities";
import { type TvSeriesNetwork } from "./tvSeriesTypes";

interface NetworkCardProps {
  network: TvSeriesNetwork;
}

function NetworkCard({ network }: NetworkCardProps) {
  const logoUrl = getTmdbImageUrl(network.logoPath, "w92");

  return (
    <div className="network-card" data-testid="network-card">
      {logoUrl ? (
        <img
          src={logoUrl}
          alt={`${network.name} logo`}
          loading="lazy"
          className="network-logo"
        />
      ) : (
        <div className="network-logo network-logo-placeholder">No logo</div>
      )}
      <p className="network-name">{network.name}</p>
      <p className="network-country">{displayText(network.originCountry)}</p>
    </div>
  );
}

export default NetworkCard;
