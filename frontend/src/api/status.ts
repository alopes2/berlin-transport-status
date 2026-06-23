import type { StatusResponse } from '../types/status'

const API_URL = import.meta.env.VITE_API_URL ?? '/api/status'

export async function fetchStatus(signal?: AbortSignal): Promise<StatusResponse> {
  const response = await fetch(API_URL, {
    headers: { Accept: 'application/json' },
    signal,
  })

  if (!response.ok) {
    throw new Error(`Status request failed with ${response.status}`)
  }

  return response.json() as Promise<StatusResponse>
}

