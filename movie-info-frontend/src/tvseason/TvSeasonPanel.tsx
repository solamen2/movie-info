import { type ReactNode, useEffect, useState } from "react";
import { useLocation, useNavigate, useSearchParams } from "react-router-dom";
import { imdbTitleUrl } from "../movie/movieTypes";
import Collapsible from "../shared/Collapsible";
import HorizontalList from "../shared/HorizontalList";
import ImdbRow from "../shared/ImdbRow";
import WatchProviderSection from "../shared/WatchProviderSection";
import { useCardSelection } from "../shared/useCardSelection";
import NetworkCard from "../tvseries/NetworkCard";
import {
  displayDate,
  displayText,
  getIntParam,
  getTmdbImageUrl,
  withQueryParams,
} from "../utilities/utilities";
import EpisodeCard from "./EpisodeCard";
import { type TvSeason } from "./tvSeasonTypes";
import "../shared/shared.css";
import "./tvseason.css";

interface TvSeasonPanelProps {
  tmdbTvSeriesId: number;
  seasonNumber: number;
  // For the link to the series' own IMDB page
  tvSeriesImdbId: string;
}

// Shows one season of a TV series, loaded from /api/tvseason with the same
// query parameters that select it in the URL. An episode within it is selected
// via the URL's `?episodeNumber=…` query parameter, so browser back / forward
// and directly loaded URLs work for episodes too.
function TvSeasonPanel({
  tmdbTvSeriesId,
  seasonNumber,
  tvSeriesImdbId,
}: TvSeasonPanelProps) {
  const [tvSeason, setTvSeason] = useState<TvSeason | null>(null);
  const [error, setError] = useState("");
  const [searchParams] = useSearchParams();
  const location = useLocation();
  const navigate = useNavigate();
  const selectedEpisodeNumber = getIntParam(searchParams, "episodeNumber");
  const episodeSelection = useCardSelection({
    targetId:
      selectedEpisodeNumber === null ? null : String(selectedEpisodeNumber),
    opensPanel: true,
    hasCardFor: (id) =>
      tvSeason?.episodes.some((e) => String(e.episodeNumber) === id) ?? false,
  });

  useEffect(() => {
    const controller = new AbortController();

    async function loadTvSeason() {
      try {
        const params = new URLSearchParams({
          tmdbTvSeriesId: String(tmdbTvSeriesId),
          seasonNumber: String(seasonNumber),
        });
        const response = await fetch(`/api/tvseason?${params.toString()}`, {
          signal: controller.signal,
        });
        if (!response.ok) {
          setError(
            `Loading TV season failed with status ${String(response.status)}. Please try again.`,
          );
          return;
        }
        const data = (await response.json()) as TvSeason | null; // TODO: Maybe someday make this validation more robust
        if (data == null) {
          setError("No TV season details were found.");
          return;
        }
        setTvSeason(data);
      } catch {
        if (!controller.signal.aborted) {
          setError("An unexpected error occurred while loading the TV season.");
        }
      }
    }

    void loadTvSeason();
    return () => {
      controller.abort();
    };
  }, [tmdbTvSeriesId, seasonNumber]);

  if (error) {
    return (
      <div className="detail-panel">
        <p className="error-message">{error}</p>
      </div>
    );
  }

  if (!tvSeason) {
    return (
      <div className="detail-panel">
        <p className="detail-loading">Loading TV season details…</p>
      </div>
    );
  }

  const posterUrl = getTmdbImageUrl(tvSeason.posterPath, "w500");
  const sortedEpisodes = [...tvSeason.episodes].sort(
    (a, b) => a.episodeNumber - b.episodeNumber,
  );

  const facts: [string, ReactNode][] = [
    ["Season Number", displayText(tvSeason.seasonNumber)],
    ["Number of Episodes", displayText(tvSeason.numberOfEpisodes)],
    ["First Air Date", displayDate(tvSeason.firstAirDate)],
  ];

  return (
    <div
      className={`detail-panel fly-origin${episodeSelection.selectedId !== null ? " has-child-selection" : ""}`}
      data-testid="tv-season-panel"
    >
      <div className="detail-panel-top">
        {posterUrl ? (
          <img src={posterUrl} alt={tvSeason.name} className="detail-poster" />
        ) : (
          <div className="detail-poster card-image-placeholder">No image</div>
        )}
        <div className="detail-panel-heading">
          <h2 className="detail-title">{tvSeason.name}</h2>
          <ImdbRow
            imdbUrl={imdbTitleUrl(tvSeriesImdbId)}
            label="TV Series IMDB:"
            copyButton={false}
          />
          <dl className="detail-facts">
            {facts.map(([label, value]) => (
              <div className="detail-fact" key={label}>
                <dt>{label}</dt>
                <dd>{value}</dd>
              </div>
            ))}
          </dl>
        </div>
      </div>

      <div className="detail-sections">
        <Collapsible
          title="Episodes"
          count={tvSeason.episodes.length}
          forceOpen={episodeSelection.selectedId !== null}
          childSelected={episodeSelection.selectedId !== null}
        >
          <HorizontalList hasSelection={episodeSelection.selectedId !== null}>
            {sortedEpisodes.map((e) => {
              const episodeId = String(e.episodeNumber);
              return (
                <EpisodeCard
                  key={e.id}
                  episode={e}
                  tvSeriesImdbId={tvSeriesImdbId}
                  selected={episodeSelection.selectedId === episodeId}
                  expanded={episodeSelection.expandedId === episodeId}
                  deselecting={
                    episodeSelection.previouslySelectedId === episodeId &&
                    episodeSelection.selectedId !== episodeId
                  }
                  onExpand={() => {
                    void navigate(
                      withQueryParams(location.pathname, location.search, {
                        tmdbTvSeriesId: String(tmdbTvSeriesId),
                        seasonNumber: String(seasonNumber),
                        episodeNumber: episodeId,
                      }),
                    );
                  }}
                />
              );
            })}
          </HorizontalList>
        </Collapsible>
        <Collapsible title="Overview (TMDB)">
          <p className="detail-prose">{displayText(tvSeason.tmdbOverview)}</p>
        </Collapsible>
        <Collapsible title="Networks" count={tvSeason.networks.length}>
          <HorizontalList>
            {tvSeason.networks.map((n) => (
              <NetworkCard key={n.id} network={n} />
            ))}
          </HorizontalList>
        </Collapsible>
        <WatchProviderSection
          title="Where to Stream"
          providers={tvSeason.watchProvidersFlatrate}
        />
        <WatchProviderSection
          title="Where to Rent"
          providers={tvSeason.watchProvidersRent}
        />
        <WatchProviderSection
          title="Where to Buy"
          providers={tvSeason.watchProvidersBuy}
        />
      </div>
    </div>
  );
}

export default TvSeasonPanel;
