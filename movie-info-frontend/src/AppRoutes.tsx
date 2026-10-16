import { Routes, Route, Navigate } from "react-router-dom";
import LoginUser from "./user/LoginUser";
import RegisterUser from "./user/RegisterUser";
import SuggestionSearch from "./suggestion/SuggestionSearch";
import OscarsByYear from "./oscars/OscarsByYear";
import Footer from "./shared/Footer";

function AppRoutes() {
  return (
    <>
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
        <Route path="/oscars" element={<OscarsByYear />} />
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
      <Footer />
    </>
  );
}

export default AppRoutes;
