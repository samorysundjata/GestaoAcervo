import { defineConfig, devices } from '@playwright/test';

/**
 * E2E against the SPA on :4200 and the API on :5000 (browser calls localhost).
 * Start the stack first: `docker compose up -d --build --wait` from the repo root
 * (or `ng serve` + API). CI does not use webServer; it brings Compose up in the workflow.
 */
export default defineConfig({
  testDir: './e2e',
  fullyParallel: false,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  workers: 1,
  timeout: 60_000,
  expect: { timeout: 15_000 },
  reporter: [['list'], ['html', { open: 'never' }]],
  use: {
    baseURL: 'http://localhost:4200',
    trace: 'on-first-retry',
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
});
