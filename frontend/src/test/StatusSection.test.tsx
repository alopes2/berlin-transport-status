import { render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import StatusSection from '../components/StatusSection'

describe('StatusSection', () => {
  it('uses singular grammar for one day', () => {
    render(
      <StatusSection
        status={{
          company: 'BVG',
          todayStatus: 'perfect',
          currentStreakDays: 1,
          recordDays: 1,
          trackingSince: '2026-06-22',
          activeIssueCount: 0,
          dataStatus: 'current',
          lastCheckedAt: '2026-06-22T09:58:00Z',
        }}
      />,
    )

    expect(screen.getByText('1 day')).toBeInTheDocument()
    expect(screen.getByText('Last record is 1 day')).toBeInTheDocument()
  })
})

