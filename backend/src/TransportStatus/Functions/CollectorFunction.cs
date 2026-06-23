using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using TransportStatus.Domain;
using TransportStatus.Sources;
using TransportStatus.Storage;

namespace TransportStatus.Functions;

public class CollectorFunction
{
    private static readonly TimeZoneInfo BerlinTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");

    private readonly IReadOnlyList<IDisruptionSource> sources;
    private readonly IStatusRepository repository;
    private readonly DateOnly trackingDate;

    public CollectorFunction()
        : this(
            SourceFactory.Create(new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(20)
            }),
            new DynamoStatusRepository(
                new AmazonDynamoDBClient(),
                RequiredEnvironment("STATUS_TABLE")),
            DateOnly.Parse(
                Environment.GetEnvironmentVariable("TRACKING_SINCE")
                    ?? DateOnly.FromDateTime(DateTime.UtcNow).ToString("O")))
    {
    }

    internal CollectorFunction(
        IReadOnlyList<IDisruptionSource> sources,
        IStatusRepository repository,
        DateOnly trackingDate)
    {
        this.sources = sources;
        this.repository = repository;
        this.trackingDate = trackingDate;
    }

    public async Task HandleAsync(
        object _,
        ILambdaContext context)
    {
        var now = DateTimeOffset.UtcNow;
        var berlinDate = DateOnly.FromDateTime(
            TimeZoneInfo.ConvertTime(now, BerlinTimeZone).DateTime);

        foreach (var companyGroup in sources.GroupBy(source => source.Company))
        {
            Observation observation;
            try
            {
                var alertLists = await Task.WhenAll(
                    companyGroup.Select(source =>
                        source.GetActiveAlertsAsync(
                            context.RemainingTime > TimeSpan.FromSeconds(5)
                                ? CancellationToken.None
                                : new CancellationToken(true))));

                var alerts = alertLists
                    .SelectMany(alerts => alerts)
                    .DistinctBy(alert => alert.Id)
                    .ToArray();

                observation = new Observation(
                    companyGroup.Key,
                    berlinDate,
                    now,
                    DataStatus.Current,
                    alerts);
            }
            catch (Exception exception)
            {
                context.Logger.LogError(
                    $"Collection failed for {companyGroup.Key}: {exception.Message}");
                observation = new Observation(
                    companyGroup.Key,
                    berlinDate,
                    now,
                    DataStatus.Unavailable,
                    []);
            }

            try
            {
                var current = await repository.GetAsync(
                    companyGroup.Key,
                    CancellationToken.None);
                var next = StreakCalculator.Apply(current, observation, trackingDate);
                await repository.SaveAsync(next, CancellationToken.None);
            }
            catch (Exception exception)
            {
                context.Logger.LogError(
                    exception,
                    "Streak calculation failed for {companyGroupKey}",
                    companyGroup.Key);
            }
        }
    }

    private static string RequiredEnvironment(string key) =>
        Environment.GetEnvironmentVariable(key)
        ?? throw new InvalidOperationException($"{key} is required.");
}

