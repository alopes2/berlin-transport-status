export type Company = 'BVG' | 'DB'
export type TodayStatus = 'perfect' | 'issues' | 'unknown'
export type DataStatus = 'current' | 'stale' | 'unavailable'

export interface CompanyStatus {
  company: Company
  todayStatus: TodayStatus
  currentStreakDays: number
  recordDays: number
  trackingSince: string
  activeIssueCount: number
  dataStatus: DataStatus
  lastCheckedAt: string
}

export interface StatusResponse {
  generatedAt: string
  companies: CompanyStatus[]
}

