import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import LoginUser from "./user/LoginUser";
import RegisterUser from "./user/RegisterUser";
import SuggestionSearch from "./suggestion/SuggestionSearch";
import "./App.css";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginUser />} />
        <Route path="/register" element={<RegisterUser />} />
        <Route path="/search" element={<SuggestionSearch />} />
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
