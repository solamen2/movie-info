import "../shared/shared.css";

// IMDB's own pages for its ranked lists (not user-made lists). The top and
// bottom 1000 movies only exist as groups in its title search; the top 250 TV
// shows have a chart page.
const IMDB_LISTS: [string, string][] = [
  [
    "IMDB Top 1000 Movies",
    "https://www.imdb.com/search/title/?groups=top_1000&sort=user_rating,desc",
  ],
  [
    "IMDB Bottom 1000 Movies",
    "https://www.imdb.com/search/title/?groups=bottom_1000&sort=user_rating,asc",
  ],
  ["IMDB Top 250 TV Shows", "https://www.imdb.com/chart/toptv/"],
];

// Links to IMDB's ranked lists, which open on IMDB itself.
function ImdbLists() {
  return (
    <div className="reference-page" data-testid="imdb-lists-page">
      <h1>IMDB Lists</h1>
      <ul className="reference-list">
        {IMDB_LISTS.map(([text, url]) => (
          <li key={url}>
            <a href={url} target="_blank" rel="noopener">
              {text}
            </a>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default ImdbLists;
