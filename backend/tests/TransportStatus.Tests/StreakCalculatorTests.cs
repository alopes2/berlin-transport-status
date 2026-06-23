using TransportStatus.Domain;

namespace TransportStatus.Tests;

public sealed class StreakCalculatorTests
{
    private static readonly DateOnly TrackingDate = new(2026, 6, 22);

    [Fact]
    public void New_clean_day_keeps_completed_streak_at_zero()
    {
        var result = StreakCalculator.Apply(
            current: null,
            observation: Observation.Clean("BVG", TrackingDate),
            trackingDate: TrackingDate);

        Assert.Equal(0, result.CurrentStreakDays);
        Assert.Equal(0, result.RecordDays);
        Assert.Equal(TodayStatus.Perfect, result.TodayStatus);
    }

    [Fact]
    public void First_issue_today_resets_the_displayed_streak_immediately()
    {
        var current = CompanyState.CreateForTest(
            company: "DB",
            berlinDate: TrackingDate,
            currentStreakDays: 5,
            recordDays: 8,
            todayHadIssue: false);

        var result = StreakCalculator.Apply(
            current,
            Observation.WithIssues("DB", TrackingDate, 2),
            TrackingDate);

        Assert.Equal(0, result.CurrentStreakDays);
        Assert.Equal(8, result.RecordDays);
        Assert.Equal(TodayStatus.Issues, result.TodayStatus);
        Assert.Equal(2, result.ActiveIssueCount);
    }

    [Fact]
    public void Crossing_midnight_finalizes_the_previous_clean_day()
    {
        var current = CompanyState.CreateForTest(
            company: "BVG",
            berlinDate: TrackingDate,
            currentStreakDays: 2,
            recordDays: 2,
            todayHadIssue: false);

        var result = StreakCalculator.Apply(
            current,
            Observation.Clean("BVG", TrackingDate.AddDays(1)),
            TrackingDate);

        Assert.Equal(3, result.CurrentStreakDays);
        Assert.Equal(3, result.RecordDays);
        Assert.Equal(TrackingDate.AddDays(1), result.BerlinDate);
    }

    [Fact]
    public void Unknown_previous_day_never_increments_the_streak()
    {
        var current = CompanyState.CreateForTest(
            company: "DB",
            berlinDate: TrackingDate,
            currentStreakDays: 4,
            recordDays: 6,
            todayHadIssue: false,
            dataStatus: DataStatus.Unavailable);

        var result = StreakCalculator.Apply(
            current,
            Observation.Clean("DB", TrackingDate.AddDays(1)),
            TrackingDate);

        Assert.Equal(0, result.CurrentStreakDays);
        Assert.Equal(6, result.RecordDays);
    }

    [Fact]
    public void Missed_calendar_day_breaks_the_streak()
    {
        var current = CompanyState.CreateForTest(
            company: "BVG",
            berlinDate: TrackingDate,
            currentStreakDays: 5,
            recordDays: 7,
            todayHadIssue: false);

        var result = StreakCalculator.Apply(
            current,
            Observation.Clean("BVG", TrackingDate.AddDays(2)),
            TrackingDate);

        Assert.Equal(0, result.CurrentStreakDays);
        Assert.Equal(7, result.RecordDays);
    }
}

