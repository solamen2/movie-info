import { test as testBase } from "@playwright/test"
import { type AnyHandler } from "msw"
import { defineNetworkFixture, type NetworkFixture } from "@msw/playwright"
import { handlers } from "./mocks/handlers.ts"

interface Fixtures {
  handlers: Array<AnyHandler>
  network: NetworkFixture
}

export const test = testBase.extend<Fixtures>({
  // Initial list of the network handlers.
  handlers: [handlers, { option: true }],

  // A fixture you use to control the network in your tests.
  network: [
    async ({ context, handlers }, use) => {
      const network = defineNetworkFixture({
        context,
        handlers,
      })

      await network.enable()
      await use(network)
      await network.disable()
    },
    { auto: true },
  ],
})