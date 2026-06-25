import type { StatusResponse } from '../types/status';

const API_URL = import.meta.env.VITE_API_URL ?? '/api';

export async function fetchStatus(
  signal?: AbortSignal,
): Promise<StatusResponse> {
  const statusUrl = API_URL + '/status';
  const response = await fetch(statusUrl, {
    headers: { Accept: 'application/json' },
    signal,
  });

  if (!response.ok) {
    throw new Error(`Status request failed with ${response.status}`);
  }

  return response.json() as Promise<StatusResponse>;
}
