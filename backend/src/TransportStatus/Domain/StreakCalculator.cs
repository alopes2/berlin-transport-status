namespace TransportStatus.Domain;

public static class StreakCalculator
{
    public static CompanyState Apply(
        CompanyState? current,
        Observation observation,
        DateOnly trackingDate)
    {
        if (current is null)
        {
            return NewState(observation, trackingDate);
        }

        var dayDifference = observation.BerlinDate.DayNumber - current.BerlinDate.DayNumber;
        var completedStreak = current.CurrentStreakDays;

        if (dayDifference == 1)
        {
            completedStreak =
                current.DataStatus == DataStatus.Current && !current.TodayHadIssue
                    ? current.CurrentStreakDays + 1
                    : 0;
        }
        else if (dayDifference > 1)
        {
            completedStreak = 0;
        }

        var hasIssues = observation.Alerts.Count > 0;
        var unavailable = observation.DataStatus != DataStatus.Current;
        var displayedStreak = hasIssues || unavailable ? 0 : completedStreak;
        var record = Math.Max(current.RecordDays, displayedStreak);

        return current with
        {
            BerlinDate = observation.BerlinDate,
            CurrentStreakDays = displayedStreak,
            RecordDays = record,
            TodayHadIssue = hasIssues || (dayDifference == 0 && current.TodayHadIssue),
            TodayStatus = unavailable
                ? TodayStatus.Unknown
                : hasIssues || (dayDifference == 0 && current.TodayHadIssue)
                    ? TodayStatus.Issues
                    : TodayStatus.Perfect,
            DataStatus = observation.DataStatus,
            ActiveIssueCount = observation.Alerts.Count,
            LastCheckedAt = observation.CheckedAt
        };
    }

    private static CompanyState NewState(Observation observation, DateOnly trackingDate)
    {
        var unavailable = observation.DataStatus != DataStatus.Current;
        var hasIssues = observation.Alerts.Count > 0;
        return new CompanyState(
            observation.Company,
            observation.BerlinDate,
            trackingDate,
            0,
            0,
            hasIssues,
            unavailable
                ? TodayStatus.Unknown
                : hasIssues
                    ? TodayStatus.Issues
                    : TodayStatus.Perfect,
            observation.DataStatus,
            observation.Alerts.Count,
            observation.CheckedAt);
    }
}

