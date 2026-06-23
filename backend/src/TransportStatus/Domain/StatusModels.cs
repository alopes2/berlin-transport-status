using System.Text.Json.Serialization;

namespace TransportStatus.Domain;

[JsonConverter(typeof(JsonStringEnumConverter<DataStatus>))]
public enum DataStatus
{
    Current,
    Stale,
    Unavailable
}

[JsonConverter(typeof(JsonStringEnumConverter<TodayStatus>))]
public enum TodayStatus
{
    Perfect,
    Issues,
    Unknown
}

public record TransportAlert(
    string Id,
    string Source,
    string Title,
    string? AffectedLine);

public record Observation(
    string Company,
    DateOnly BerlinDate,
    DateTimeOffset CheckedAt,
    DataStatus DataStatus,
    IReadOnlyList<TransportAlert> Alerts)
{
    public static Observation Clean(string company, DateOnly date) =>
        new(company, date, DateTimeOffset.UtcNow, DataStatus.Current, []);

    public static Observation WithIssues(string company, DateOnly date, int count) =>
        new(
            company,
            date,
            DateTimeOffset.UtcNow,
            DataStatus.Current,
            Enumerable.Range(1, count)
                .Select(index => new TransportAlert(
                    $"{company}-{index}",
                    company,
                    $"Issue {index}",
                    null))
                .ToArray());

    public static Observation Unavailable(string company, DateOnly date) =>
        new(company, date, DateTimeOffset.UtcNow, DataStatus.Unavailable, []);
}

public record CompanyState(
    string Company,
    DateOnly BerlinDate,
    DateOnly TrackingSince,
    int CurrentStreakDays,
    int RecordDays,
    bool TodayHadIssue,
    TodayStatus TodayStatus,
    DataStatus DataStatus,
    int ActiveIssueCount,
    DateTimeOffset LastCheckedAt)
{
    public static CompanyState CreateForTest(
        string company,
        DateOnly berlinDate,
        int currentStreakDays,
        int recordDays,
        bool todayHadIssue,
        DataStatus dataStatus = DataStatus.Current) =>
        new(
            company,
            berlinDate,
            berlinDate,
            currentStreakDays,
            recordDays,
            todayHadIssue,
            todayHadIssue ? TodayStatus.Issues : TodayStatus.Perfect,
            dataStatus,
            todayHadIssue ? 1 : 0,
            DateTimeOffset.UtcNow);
}

public record CompanyStatusResponse(
    string Company,
    string TodayStatus,
    int CurrentStreakDays,
    int RecordDays,
    string TrackingSince,
    int ActiveIssueCount,
    string DataStatus,
    DateTimeOffset LastCheckedAt);

public record StatusResponse(
    DateTimeOffset GeneratedAt,
    IReadOnlyList<CompanyStatusResponse> Companies);

