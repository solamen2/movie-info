import { act, render, waitFor } from "@testing-library/react";
import { expect } from "vitest";
import { createMemoryRouter, RouterProvider } from "react-router-dom";
import AppRoutes from "../src/AppRoutes";

export type AppRouter = ReturnType<typeof createMemoryRouter>;

// Mounts the real app routes in an in-memory router so component tests can
// start at any URL, read the URL the app navigated to, and drive the history
// like the browser's back / forward buttons.
export function renderApp(initialEntry = "/search") {
  const router = createMemoryRouter([{ path: "*", element: <AppRoutes /> }], {
    initialEntries: [initialEntry],
  });
  const utils = render(<RouterProvider router={router} />);
  return {
    ...utils,
    router,
    currentUrl: () =>
      router.state.location.pathname + router.state.location.search,
    goBack: () => act(() => router.navigate(-1)),
    goForward: () => act(() => router.navigate(1)),
  };
}

// Resolves once a search has put exactly this many result cards on the page
// (the page itself only says anything about the number when it is zero).
export async function findResultCards(count: number): Promise<Element[]> {
  await waitFor(() => {
    expect(
      document.querySelectorAll("#search-card:not(.stand-in)"),
    ).toHaveLength(count);
  });
  return [...document.querySelectorAll("#search-card:not(.stand-in)")];
}
