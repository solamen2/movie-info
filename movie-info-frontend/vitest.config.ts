import { defineConfig, mergeConfig } from "vitest/config";
import viteConfig from "./vite.config";
import react from "@vitejs/plugin-react";

export default mergeConfig(
  viteConfig,
  defineConfig({
    plugins: [react()],
    test: {
      globals: true, // Provided for React Testing Library to automatically perform cleanup *sigh*
      environment: "jsdom",
      reporters:
        process.env.GITHUB_ACTIONS === "true"
          ? ["default", "github-actions"]
          : ["default"],
      setupFiles: ["./tests/vitest.setup.ts"],
      include: [
        "src/**/*.test.{js,jsx,ts,tsx}", // This avoids running Playwright tests on accident
      ],
    },
  }),
);
