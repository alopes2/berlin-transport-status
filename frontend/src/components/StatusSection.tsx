import type { CompanyStatus } from '../types/status'

interface StatusSectionProps {
  status: CompanyStatus
}

function days(value: number): string {
  return `${value} ${value === 1 ? 'day' : 'days'}`
}

function statusHeading(status: CompanyStatus): string {
  if (status.dataStatus !== 'current' || status.todayStatus === 'unknown') {
    return `${status.company} status is unavailable`
  }

  return status.todayStatus === 'perfect'
    ? `${status.company} is PERFECT today`
    : `${status.company} has issues today`
}

export default function StatusSection({ status }: StatusSectionProps) {
  const unavailable =
    status.dataStatus !== 'current' || status.todayStatus === 'unknown'
  const issueLabel = `${status.activeIssueCount} active transport ${
    status.activeIssueCount === 1 ? 'issue' : 'issues'
  }`

  return (
    <section
      className={`status-section status-section--${status.company.toLowerCase()}`}
      aria-labelledby={`${status.company}-heading`}
    >
      <div className="status-section__rail" aria-hidden="true" />
      <div className="status-section__body">
        <div className="status-section__live">
          <span
            className={`status-dot status-dot--${status.todayStatus}`}
            aria-hidden="true"
          />
          <h2 id={`${status.company}-heading`}>{statusHeading(status)}</h2>
        </div>

        {unavailable ? (
          <p className="status-section__unknown">
            We will never count missing data as a perfect day.
          </p>
        ) : (
          <>
            <p className="status-section__lead">It has been operating for</p>
            <p className="status-section__days">{days(status.currentStreakDays)}</p>
            <p className="status-section__lead">without any issues</p>
            {status.todayStatus === 'issues' ? (
              <p className="status-section__issues">{issueLabel}</p>
            ) : null}
          </>
        )}

        <p className="status-section__record">
          Last record is {days(status.recordDays)}
        </p>
      </div>
    </section>
  )
}

