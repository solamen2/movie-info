import { test as testBase } from "@playwright/test"
import { type AnyHandler } from "msw"
import { defineNetworkFixture, type NetworkFixture } from "@msw/playwright"
import { handlers } from "./mocks/handlers.ts"
import { config } from "../src/movieinfo.config.ts"

interface Fixtures {
  handlers: Array<AnyHandler>
  network: NetworkFixture
}

export const test = 
  config.useMockHttpCalls
  ? testBase.extend<Fixtures>({
      // Initial list of the network handlers.
      handlers: [handlers, { option: true }],

      // A fixture you use to control the network in your tests.
      // Access the network fixture in your tests and use it as the `setupWorker()` API.
      // No more disrupted context between processes.
      // NOTE: This would be slightly tricky to implement in this app's tests due to the conditional export here, so it's not used currently
      network: [
        async ({ context, handlers }, use) => {
          const network = defineNetworkFixture({
            context,
            handlers,
          });

          await network.enable();
          await use(network);
          await network.disable();
        },
        { auto: true },
      ],
    })
  : testBase;