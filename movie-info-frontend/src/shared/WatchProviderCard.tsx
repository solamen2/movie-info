import { type WatchProvider } from "./sharedTypes";
import { getTmdbImageUrl } from "../utilities/utilities";
import "./shared.css";

interface WatchProviderCardProps {
  provider: WatchProvider;
}

function WatchProviderCard({ provider }: WatchProviderCardProps) {
  const logoUrl = getTmdbImageUrl(provider.logoPath, "w92");

  return (
    <div className="watch-provider-card" data-testid="watch-provider-card">
      {logoUrl ? (
        <img
          src={logoUrl}
          alt={`${provider.providerName} logo`}
          loading="lazy"
          className="watch-provider-logo"
        />
      ) : (
        <div className="watch-provider-logo watch-provider-logo-placeholder">
          ?
        </div>
      )}
      <p className="watch-provider-name">{provider.providerName}</p>
    </div>
  );
}

export default WatchProviderCard;
