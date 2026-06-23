using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using TransportStatus.Domain;
using TransportStatus.Storage;

namespace TransportStatus.Functions;

public class StatusApiFunction
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    private readonly IStatusRepository repository;

    public StatusApiFunction()
        : this(
            new DynamoStatusRepository(
                new AmazonDynamoDBClient(),
                Environment.GetEnvironmentVariable("STATUS_TABLE")
                    ?? throw new InvalidOperationException("STATUS_TABLE is required.")))
    {
    }

    internal StatusApiFunction(IStatusRepository repository)
    {
        this.repository = repository;
    }

    public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(
        APIGatewayHttpApiV2ProxyRequest _,
        ILambdaContext context)
    {
        try
        {
            var states = await repository.GetAllAsync(CancellationToken.None);
            var response = new StatusResponse(
                DateTimeOffset.UtcNow,
                [.. states.Select(Map)]);
            return Json(HttpStatusCode.OK, response);
        }
        catch (Exception exception)
        {
            context.Logger.LogError(exception, "Status read failed");
            return Json(
                HttpStatusCode.ServiceUnavailable,
                new { message = "Status data is temporarily unavailable." });
        }
    }

    private static CompanyStatusResponse Map(CompanyState state) =>
        new(
            state.Company,
            state.TodayStatus.ToString().ToLowerInvariant(),
            state.CurrentStreakDays,
            state.RecordDays,
            state.TrackingSince.ToString("O"),
            state.ActiveIssueCount,
            state.DataStatus.ToString().ToLowerInvariant(),
            state.LastCheckedAt);

    private static APIGatewayHttpApiV2ProxyResponse Json(
        HttpStatusCode status,
        object body) =>
        new()
        {
            StatusCode = (int)status,
            Body = JsonSerializer.Serialize(body, JsonOptions),
            Headers = new Dictionary<string, string>
            {
                ["content-type"] = "application/json; charset=utf-8",
                ["cache-control"] = "public, max-age=30"
            }
        };
}

