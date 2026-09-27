import { act, render } from "@testing-library/react";
import { createMemoryRouter, RouterProvider } from "react-router-dom";
import AppRoutes from "../src/AppRoutes";

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
