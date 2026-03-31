import { fireEvent, render, screen } from '@testing-library/react'
import { describe, expect, it } from "vitest";
import { MemoryRouter } from "react-router-dom"
import RegisterUser from "./RegisterUser";

// screen.logTestingPlaygroundURL();
describe("RegisterUser", async () => {
  describe("When using good registration info", async () => {
    it("Should register user successfully", async () => {
      render(<MemoryRouter><RegisterUser /></MemoryRouter>);  // MemoryRouter required to use navigate()
      const email = screen.getByLabelText("Email");
      fireEvent.change(email, { target: { value: "test@example.com" }});
      const password = screen.getByLabelText("Password");
      fireEvent.change(password, { target: { value: "Pass123!@#" }});
      const registerButton = screen.getByRole("button", { name: "register" });
      await fireEvent.click(registerButton);
    });
  });
  describe("When using a bad email for registration", async () => {
    it("Should show error 'Email <email> is invalid.'", async () => {
      render(<MemoryRouter><RegisterUser /></MemoryRouter>);
      const email = screen.getByLabelText("Email");
      fireEvent.change(email, { target: { value: "error" }});
      const password = screen.getByLabelText("Password");
      fireEvent.change(password, { target: { value: "error" }});
      const registerButton = screen.getByRole("button", { name: "register" });
      await fireEvent.click(registerButton);
      const registrationFailed = await screen.findByText("Email 'error' is invalid.");
      expect(registrationFailed).toBeInTheDocument();
    });
  });
});