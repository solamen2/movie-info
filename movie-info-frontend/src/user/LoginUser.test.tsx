import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { MemoryRouter } from "react-router-dom";
import LoginUser from "./LoginUser";

// screen.logTestingPlaygroundURL();
describe("LoginUser", () => {
  describe("When using good login info", () => {
    it("Should log in user successfully", () => {
      render(
        <MemoryRouter>
          <LoginUser />
        </MemoryRouter>,
      ); // MemoryRouter required to use navigate()
      const email = screen.getByLabelText("Email");
      fireEvent.change(email, { target: { value: "test@example.com" } });
      const password = screen.getByLabelText("Password");
      fireEvent.change(password, { target: { value: "Pass123!@#" } });
      const loginButton = screen.getByRole("button", { name: "Login" });
      fireEvent.click(loginButton);
    });
  });
  describe("When the login page is shown", () => {
    it("Should credit the sources of the app's data, each linking to its site", () => {
      render(
        <MemoryRouter>
          <LoginUser />
        </MemoryRouter>,
      );
      const credits = screen.getByText(/^Uses data from:/);

      expect(
        [...credits.querySelectorAll("a")].map((a) => [
          a.querySelector("img")?.alt ?? a.textContent,
          a.href,
        ]),
      ).toEqual([
        ["IMDb", "https://www.imdb.com/"],
        ["TMDB", "https://www.themoviedb.org/"],
        ["OMDb API", "https://www.omdbapi.com/"],
        ["Wikipedia", "https://www.wikipedia.org/"],
        ["Wikidata", "https://www.wikidata.org/"],
        ["U.S. Bureau of Labor Statistics", "https://www.bls.gov/"],
      ]);
    });
  });
  describe("When using a bad username for login", () => {
    it("Should show error 'Login failed. Please check your credentials and try again.'", async () => {
      render(
        <MemoryRouter>
          <LoginUser />
        </MemoryRouter>,
      );
      const email = screen.getByLabelText("Email");
      fireEvent.change(email, { target: { value: "error" } });
      const password = screen.getByLabelText("Password");
      fireEvent.change(password, { target: { value: "error" } });
      const loginButton = screen.getByRole("button", { name: "Login" });
      fireEvent.click(loginButton);
      const loginFailed = await screen.findByText(
        "Login failed. Please check your credentials and try again.",
      );
      expect(loginFailed).toBeInTheDocument();
    });
  });
});
