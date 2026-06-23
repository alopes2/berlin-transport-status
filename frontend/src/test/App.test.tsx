import { render, screen, waitFor } from '@testing-library/react'
import { afterEach, describe, expect, it, vi } from 'vitest'
import App from '../App'
import type { StatusResponse } from '../types/status'

const response: StatusResponse = {
  generatedAt: '2026-06-22T10:00:00Z',
  companies: [
    {
      company: 'BVG',
      todayStatus: 'perfect',
      currentStreakDays: 3,
      recordDays: 7,
      trackingSince: '2026-06-22',
      activeIssueCount: 0,
      dataStatus: 'current',
      lastCheckedAt: '2026-06-22T09:58:00Z',
    },
    {
      company: 'DB',
      todayStatus: 'issues',
      currentStreakDays: 0,
      recordDays: 4,
      trackingSince: '2026-06-22',
      activeIssueCount: 2,
      dataStatus: 'current',
      lastCheckedAt: '2026-06-22T09:58:00Z',
    },
  ],
}

afterEach(() => {
  vi.unstubAllGlobals()
})

describe('App', () => {
  it('renders both company summaries returned by the API', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue({
        ok: true,
        json: () => Promise.resolve(response),
      }),
    )

    render(<App />)

    expect(screen.getByText('Checking Berlin…')).toBeInTheDocument()
    expect(await screen.findByText('BVG is PERFECT today')).toBeInTheDocument()
    expect(screen.getByText('DB has issues today')).toBeInTheDocument()
    expect(screen.getByText('3 days')).toBeInTheDocument()
    expect(screen.getByText('2 active transport issues')).toBeInTheDocument()
  })

  it('shows an unavailable state instead of claiming a clean day', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue({
        ok: true,
        json: () =>
          Promise.resolve({
            ...response,
            companies: [
              {
                ...response.companies[0],
                todayStatus: 'unknown',
                dataStatus: 'unavailable',
              },
            ],
          }),
      }),
    )

    render(<App />)

    expect(
      await screen.findByText('BVG status is unavailable'),
    ).toBeInTheDocument()
    expect(
      screen.getByText('We will never count missing data as a perfect day.'),
    ).toBeInTheDocument()
  })

  it('shows a retryable error when the status endpoint fails', async () => {
    vi.stubGlobal('fetch', vi.fn().mockRejectedValue(new Error('offline')))

    render(<App />)

    await waitFor(() => {
      expect(
        screen.getByText('Berlin status could not be loaded.'),
      ).toBeInTheDocument()
    })
  })
})

