import { useEffect, useState } from 'react'
import { fetchStatus } from './api/status'
import StatusSection from './components/StatusSection'
import type { StatusResponse } from './types/status'

type LoadState =
  | { kind: 'loading' }
  | { kind: 'loaded'; data: StatusResponse }
  | { kind: 'error' }

const SOURCE_LINKS = [
  {
    label: 'BVG',
    href: 'https://www.bvg.de/de/verbindungen/stoerungsmeldungen',
  },
  {
    label: 'S-Bahn Berlin',
    href: 'https://sbahn.berlin/fahren/bauen-stoerung/',
  },
  {
    label: 'DB Regio',
    href: 'https://www.dbregio-berlin-brandenburg.de/db-regio-no/Fahren/Baustellen-und-Stoerungen/',
  },
]

function formatDate(value: string): string {
  return new Intl.DateTimeFormat('en-GB', {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
    timeZone: 'Europe/Berlin',
  }).format(new Date(`${value}T12:00:00+02:00`))
}

function relativeUpdate(value: string): string {
  const elapsedMinutes = Math.max(
    0,
    Math.floor((Date.now() - new Date(value).getTime()) / 60_000),
  )
  if (elapsedMinutes < 1) return 'Updated just now'
  return `Updated ${elapsedMinutes} ${elapsedMinutes === 1 ? 'minute' : 'minutes'} ago`
}

export default function App() {
  const [state, setState] = useState<LoadState>({ kind: 'loading' })

  useEffect(() => {
    const controller = new AbortController()
    fetchStatus(controller.signal)
      .then((data) => setState({ kind: 'loaded', data }))
      .catch((error: unknown) => {
        if (!(error instanceof DOMException && error.name === 'AbortError')) {
          setState({ kind: 'error' })
        }
      })
    return () => controller.abort()
  }, [])

  return (
    <main>
      <header className="site-header">
        <h1>Berlin, still moving?</h1>
        <div className="site-header__line" aria-hidden="true" />
      </header>

      {state.kind === 'loading' ? (
        <div className="message" role="status">
          Checking Berlin…
        </div>
      ) : null}

      {state.kind === 'error' ? (
        <div className="message message--error" role="alert">
          <strong>Berlin status could not be loaded.</strong>
          <span>Try again in a few minutes.</span>
        </div>
      ) : null}

      {state.kind === 'loaded' ? (
        <>
          <div className="status-list">
            {state.data.companies.map((status) => (
              <StatusSection key={status.company} status={status} />
            ))}
          </div>
          <footer>
            <p>
              Tracking since{' '}
              {formatDate(state.data.companies[0]?.trackingSince ?? '2026-06-22')}
              <span aria-hidden="true"> · </span>
              {relativeUpdate(state.data.generatedAt)}
            </p>
            <nav aria-label="Disruption data sources">
              {SOURCE_LINKS.map((source, index) => (
                <span key={source.label}>
                  {index > 0 ? <span aria-hidden="true"> · </span> : null}
                  <a href={source.href} target="_blank" rel="noreferrer">
                    {source.label}
                  </a>
                </span>
              ))}
            </nav>
          </footer>
        </>
      ) : null}
    </main>
  )
}

