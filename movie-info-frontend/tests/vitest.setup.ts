import { beforeAll, afterEach, afterAll } from "vitest";
import { server } from "./mocks/node.ts";
import "@testing-library/jest-dom/vitest";
 
beforeAll(() => server.listen({onUnhandledRequest: 'error'}));
afterEach(() => server.resetHandlers());
afterAll(() => server.close());