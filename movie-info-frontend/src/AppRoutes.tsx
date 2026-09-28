import { Routes, Route, Navigate } from "react-router-dom";
import LoginUser from "./user/LoginUser";
import RegisterUser from "./user/RegisterUser";
import SuggestionSearch from "./suggestion/SuggestionSearch";

function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<LoginUser />} />
      <Route path="/register" element={<RegisterUser />} />
      <Route path="/search" element={<SuggestionSearch />} />
      <Route
        path="/movie/:imdbId"
        element={<SuggestionSearch panelKind="movie" />}
      />
      <Route
        path="/tvseries/:imdbId"
        element={<SuggestionSearch panelKind="tvseries" />}
      />
      <Route
        path="/person/:imdbId"
        element={<SuggestionSearch panelKind="person" />}
      />
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}

export default AppRoutes;
