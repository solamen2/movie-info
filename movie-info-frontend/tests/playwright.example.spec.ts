import { http, HttpResponse } from 'msw'
import { test } from './playwright.setup.ts'

test('displays the user dashboard', async ({ network, page }) => {
  // Access the network fixture and use it as the `setupWorker()` API.
  // No more disrupted context between processes.
  network.use(
    http.get('/user', () => {
      return HttpResponse.json({
        id: 'abc-123',
        firstName: 'John',
        lastName: 'Maverick',
      })
    }),
  )

  //await page.goto('/dashboard')
});