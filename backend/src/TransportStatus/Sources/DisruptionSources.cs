using System.Net.Http.Json;
using TransportStatus.Domain;

namespace TransportStatus.Sources;

public interface IDisruptionSource
{
    string Company { get; }
    Task<IReadOnlyList<TransportAlert>> GetActiveAlertsAsync(
        CancellationToken cancellationToken);
}

public class SbahnDisruptionSource(HttpClient httpClient) : IDisruptionSource
{
    private const string Query = """
        query Consequences($language: Language!) {
          consequences(language: $language) {
            id
            timeFrames
            title
            lines { title }
          }
        }
        """;

    public string Company => "DB";

    public async Task<IReadOnlyList<TransportAlert>> GetActiveAlertsAsync(
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.sbahn.berlin/construction-api/query");
        request.Headers.UserAgent.ParseAdd(
            "BerlinTransportStatus/1.0 (+https://github.com/)");
        request.Headers.Accept.ParseAdd("application/json");
        request.Content = JsonContent.Create(new
        {
            query = Query,
            variables = new { language = "GERMAN" }
        });
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return SbahnJsonParser.Parse(json);
    }
}

public class BvgDisruptionSource(
    HttpClient httpClient,
    Uri uri) : IDisruptionSource
{
    public string Company => "BVG";

    public async Task<IReadOnlyList<TransportAlert>> GetActiveAlertsAsync(
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.UserAgent.ParseAdd(
            "BerlinTransportStatus/1.0 (+https://github.com/)");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return BvgJsonParser.Parse(json);
    }
}

public class DbRegioDisruptionSource(HttpClient httpClient)
    : IDisruptionSource
{
    private static readonly TimeZoneInfo BerlinTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");

    public string Company => "DB";

    public async Task<IReadOnlyList<TransportAlert>> GetActiveAlertsAsync(
        CancellationToken cancellationToken)
    {
        var berlinDate = DateOnly.FromDateTime(
            TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, BerlinTimeZone).DateTime);
        var date = berlinDate.ToString("yyyy-MM-dd");
        var uri =
            "https://www.dbregio-berlin-brandenburg.de/service/dbmsg/"
            + "db-regio-no/Fahren/Baustellen-und-Stoerungen/"
            + $"?dateFrom={date}&timeFrom=00:00&dateTo={date}&timeTo=23:59";
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.UserAgent.ParseAdd(
            "BerlinTransportStatus/1.0 (+https://github.com/)");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return DbRegioJsonParser.Parse(json);
    }
}

public static class SourceFactory
{
    public static IReadOnlyList<IDisruptionSource> Create(HttpClient client) =>
    [
        new BvgDisruptionSource(
            client,
            new Uri(
                "https://www.bvg.de/disruption-reports-service/disruptions/v1/de")),
        new SbahnDisruptionSource(client),
        new DbRegioDisruptionSource(client)
    ];
}
