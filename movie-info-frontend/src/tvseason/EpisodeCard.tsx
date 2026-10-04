import Collapsible from "../shared/Collapsible";
import ExpandableCard from "../shared/ExpandableCard";
import TvEpisodePanel from "../tvepisode/TvEpisodePanel";
import {
  displayDate,
  displayRuntime,
  displayText,
  getTmdbImageUrl,
} from "../utilities/utilities";
import { type TvSeasonEpisode, displayEpisodeType } from "./tvSeasonTypes";

interface EpisodeCardProps {
  episode: TvSeasonEpisode;
  // The IMDB id and name of the series, needed by the episode panel the card
  // expands into. (The TMDB series id and season number come with the
  // episode.)
  tvSeriesImdbId: string;
  tvSeriesName: string;
  selected: boolean;
  deselecting: boolean;
  expanded: boolean;
  // Called when the Expand button is clicked; the caller navigates to the
  // episode's URL, which is what actually expands the card.
  onExpand: () => void;
}

function displayNames(people: { name: string }[]): string {
  return people.map((p) => p.name).join(", ");
}

function EpisodeCard({
  episode,
  tvSeriesImdbId,
  tvSeriesName,
  selected,
  deselecting,
  expanded,
  onExpand,
}: EpisodeCardProps) {
  const stillUrl = getTmdbImageUrl(episode.stillPath, "w185");
  const directors = displayNames(episode.directors);
  const writers = displayNames(episode.writers);

  return (
    <ExpandableCard
      className="episode-card"
      testId="episode-card"
      selected={selected}
      deselecting={deselecting}
      expanded={expanded}
    >
      {expanded ? (
        <TvEpisodePanel
          tmdbTvSeriesId={episode.tmdbTvSeriesId}
          seasonNumber={episode.seasonNumber}
          episodeNumber={episode.episodeNumber}
          tvSeriesImdbId={tvSeriesImdbId}
          tvSeriesName={tvSeriesName}
        />
      ) : (
        <>
          <div className="episode-card-top">
            {stillUrl ? (
              <img
                src={stillUrl}
                alt={episode.title}
                loading="lazy"
                className="episode-card-image"
              />
            ) : (
              <div className="episode-card-image episode-card-image-placeholder">
                No image
              </div>
            )}
            <div className="episode-card-details">
              <p className="episode-card-name">
                {String(episode.episodeNumber)}. {episode.title}
              </p>
              <p className="episode-card-secondary">
                {displayDate(episode.airDate)}
              </p>
              <p className="episode-card-secondary">
                {displayRuntime(episode.runtime)}
                {" · "}
                {displayEpisodeType(episode.episodeType)}
              </p>
              {directors && (
                <p className="episode-card-secondary">
                  Directed by {directors}
                </p>
              )}
              {writers && (
                <p className="episode-card-secondary">Written by {writers}</p>
              )}
            </div>
          </div>
          <Collapsible title="Overview">
            <p className="detail-prose">{displayText(episode.tmdbOverview)}</p>
          </Collapsible>
          <button
            type="button"
            className="episode-card-expand"
            onClick={onExpand}
          >
            Expand
          </button>
        </>
      )}
    </ExpandableCard>
  );
}

export default EpisodeCard;
