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
        path="/movie/:itemId"
        element={<SuggestionSearch panelKind="movie" />}
      />
      <Route
        path="/tvseries/:itemId"
        element={<SuggestionSearch panelKind="tvseries" />}
      />
      <Route
        path="/person/:itemId"
        element={<SuggestionSearch panelKind="person" />}
      />
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}

export default AppRoutes;
