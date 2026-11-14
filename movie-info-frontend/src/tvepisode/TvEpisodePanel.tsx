import { hasImdbId, imdbTitleUrl } from "../movie/movieTypes";
import Collapsible from "../shared/Collapsible";
import DetailFacts, { type DetailFact } from "../shared/DetailFacts";
import HorizontalList from "../shared/HorizontalList";
import ExternalLinkRow from "../shared/ExternalLinkRow";
import ImdbLinkRow from "../shared/ImdbLinkRow";
import { knownForActorsFact, peopleFact } from "../shared/linkFacts";
import PlotSection from "../shared/PlotSection";
import { separatedValues } from "../shared/separatedValues";
import { useDetailData } from "../shared/useDetailData";
import { usePanelCards } from "../shared/usePanelCards";
import {
  displayEpisodeType,
  displaySeasonEpisode,
} from "../tvseason/tvSeasonTypes";
import {
  displayDate,
  displayGenres,
  displayRuntime,
  displayText,
  getTmdbImageUrl,
} from "../utilities/utilities";
import CastCard from "./CastCard";
import GuestStarCard from "./GuestStarCard";
import { tvEpisodeGoogleSearchUrl } from "./tvEpisodeTypes";
import "../shared/shared.css";

interface TvEpisodePanelProps {
  tmdbTvSeriesId: number;
  seasonNumber: number;
  episodeNumber: number;
  // For the link to the series' own IMDB page
  tvSeriesImdbId: string;
  // For the Google search offered when the episode has no IMDB page
  tvSeriesName: string;
}

// Shows one episode of a TV series, loaded from /api/tvepisode with the same
// query parameters that select it in the URL. Its cast and guest star
// cards open the person's own panel in place of the whole TV series panel
// (see usePanelCards).
function TvEpisodePanel({
  tmdbTvSeriesId,
  seasonNumber,
  episodeNumber,
  tvSeriesImdbId,
  tvSeriesName,
}: TvEpisodePanelProps) {
  const params = new URLSearchParams({
    tmdbTvSeriesId: String(tmdbTvSeriesId),
    seasonNumber: String(seasonNumber),
    episodeNumber: String(episodeNumber),
  });
  const { data: tvEpisode, error } = useDetailData(
    `/api/tvepisode?${params.toString()}`,
    "TV episode",
  );
  const cards = usePanelCards({
    cardsIn: {
      cast: tvEpisode?.cast,
      "guest-stars": tvEpisode?.guestStars,
    },
  });

  if (error) {
    return (
      <div className="detail-panel">
        <p className="error-message">{error}</p>
      </div>
    );
  }

  if (!tvEpisode) {
    return (
      <div className="detail-panel">
        <p className="detail-loading">Loading TV episode details…</p>
      </div>
    );
  }

  const stillUrl = getTmdbImageUrl(tvEpisode.stillPath, "w300");
  const sortedCast = [...tvEpisode.cast].sort(
    (a, b) => a.billedOrder - b.billedOrder,
  );
  const sortedGuestStars = [...tvEpisode.guestStars].sort(
    (a, b) => a.billedOrder - b.billedOrder,
  );
  const castSelected = cards.isSelectedIn("cast");
  const guestStarSelected = cards.isSelectedIn("guest-stars");

  const facts: DetailFact[] = [
    ["Air Date", displayDate(tvEpisode.airDate)],
    [
      "Runtime",
      separatedValues([
        displayRuntime(tvEpisode.omdbAverageEpisodeRuntimeNumber),
        displayRuntime(tvEpisode.runtime),
      ]),
    ],
    ["Rated", displayText(tvEpisode.rated)],
    ["Type", displayEpisodeType(tvEpisode.episodeType)],
    peopleFact("Director", tvEpisode.directors),
    peopleFact("Writer", tvEpisode.writers),
    knownForActorsFact(
      tvEpisode.knownForActors,
      [
        { cardType: "cast", people: tvEpisode.cast },
        { cardType: "guest-stars", people: tvEpisode.guestStars },
      ],
      cards,
    ),
    ["Genres", displayGenres("", tvEpisode.omdbGenres)],
  ];

  return (
    <div
      className={`detail-panel fly-origin${cards.hasSelection ? " has-child-selection" : ""}`}
      data-testid="tv-episode-panel"
    >
      <div className="detail-panel-top">
        {stillUrl ? (
          <img
            src={stillUrl}
            alt={tvEpisode.title}
            className="detail-poster detail-still"
          />
        ) : (
          <div className="detail-poster detail-still card-image-placeholder">
            No image
          </div>
        )}
        <div className="detail-panel-heading">
          <h2 className="detail-title">{tvEpisode.title}</h2>
          <p className="detail-subtitle">
            {displaySeasonEpisode(
              tvEpisode.seasonNumber,
              tvEpisode.episodeNumber,
            )}
          </p>
          {hasImdbId(tvEpisode.imdbId) ? (
            <ImdbLinkRow
              url={imdbTitleUrl(tvEpisode.imdbId)}
              rating={tvEpisode.imdbRating}
              votes={tvEpisode.imdbVotes}
            />
          ) : (
            // TMDB knows of no IMDB page for the episode (so there is no OMDB
            // rating either); offer a Google search for it instead
            <ExternalLinkRow
              url=""
              searchUrl={tvEpisodeGoogleSearchUrl(
                tvSeriesName,
                tvEpisode.seasonNumber,
                tvEpisode.episodeNumber,
                tvEpisode.title,
              )}
              label="IMDB:"
            />
          )}
          <ExternalLinkRow
            url={imdbTitleUrl(tvSeriesImdbId)}
            label="TV Series IMDB:"
            copyButton={false}
          />
          <DetailFacts facts={facts} />
        </div>
      </div>

      <div className="detail-sections">
        <Collapsible
          title="Cast"
          count={tvEpisode.cast.length}
          {...cards.sectionProps("cast")}
        >
          <HorizontalList hasSelection={castSelected}>
            {sortedCast.map((c) => (
              <CastCard
                key={c.id}
                cast={c}
                {...cards.cardProps("cast", "person", c.tmdbId)}
              />
            ))}
          </HorizontalList>
        </Collapsible>
        <Collapsible
          title="Guest Stars"
          count={tvEpisode.guestStars.length}
          {...cards.sectionProps("guest-stars")}
        >
          <HorizontalList hasSelection={guestStarSelected}>
            {sortedGuestStars.map((g) => (
              <GuestStarCard
                key={g.id}
                guestStar={g}
                {...cards.cardProps("guest-stars", "person", g.tmdbId)}
              />
            ))}
          </HorizontalList>
        </Collapsible>
        <PlotSection
          title="Plot"
          sources={[
            { name: "OMDB", text: tvEpisode.omdbOverview },
            { name: "TMDB", text: tvEpisode.tmdbOverview },
          ]}
        />
      </div>
    </div>
  );
}

export default TvEpisodePanel;
