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
      const loginButton = screen.getByRole("button", { name: "login" });
      fireEvent.click(loginButton);
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
      const loginButton = screen.getByRole("button", { name: "login" });
      fireEvent.click(loginButton);
      const loginFailed = await screen.findByText(
        "Login failed. Please check your credentials and try again.",
      );
      expect(loginFailed).toBeInTheDocument();
    });
  });
});
