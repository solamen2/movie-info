import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import LoginUser from "./LoginUser";
import RegisterUser from "./RegisterUser";
import MovieSearch from "./MovieSearch";
import "./App.css";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginUser />} />
        <Route path="/register" element={<RegisterUser />} />
        <Route path="/search" element={<MovieSearch />} />
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
