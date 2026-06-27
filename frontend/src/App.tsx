import { useEffect, useState } from 'react';
import { fetchStatus } from './api/status';
import StatusSection from './components/StatusSection';
import type { StatusResponse } from './types/status';

type LoadState =
  | { kind: 'loading' }
  | { kind: 'loaded'; data: StatusResponse }
  | { kind: 'error' };

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
];

function formatTrackingStartDate(value: string): string {
  return new Intl.DateTimeFormat('de-DE', {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
    timeZone: 'Europe/Berlin',
  }).format(new Date(`${value}T12:00:00+02:00`));
}

function formatUpdatedAtDate(value: string): string {
  const date = new Intl.DateTimeFormat('de-DE', {
    dateStyle: 'medium',
    timeZone: 'Europe/Berlin',
  }).format(new Date(value));

  const time = new Intl.DateTimeFormat('de-DE', {
    timeStyle: 'medium',
    timeZone: 'Europe/Berlin',
  }).format(new Date(value));

  return [date, time].join(' at ');
}

export default function App() {
  const [state, setState] = useState<LoadState>({ kind: 'loading' });

  useEffect(() => {
    const controller = new AbortController();
    fetchStatus(controller.signal)
      .then((data) => setState({ kind: 'loaded', data }))
      .catch((error: unknown) => {
        if (!(error instanceof DOMException && error.name === 'AbortError')) {
          setState({ kind: 'error' });
        }
      });
    return () => controller.abort();
  }, []);

  const updatedAt =
    state.kind === 'loaded'
      ? formatUpdatedAtDate(state.data.generatedAt)
      : null;

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
        </>
      ) : null}
      <footer>
        <p>
          Tracking since {formatTrackingStartDate('2026-06-23')}
          <span aria-hidden="true"> · </span>
          {updatedAt ? `Updated at ${updatedAt}` : null}
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
    </main>
  );
}
