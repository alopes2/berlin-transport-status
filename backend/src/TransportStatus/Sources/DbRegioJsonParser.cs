using System.Text.Json;
using TransportStatus.Domain;

namespace TransportStatus.Sources;

public static class DbRegioJsonParser
{
    public static IReadOnlyList<TransportAlert> Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        if (!document.RootElement.TryGetProperty("messages", out var messages))
        {
            throw new InvalidDataException("DB Regio response has no messages array.");
        }

        return messages
            .EnumerateArray()
            .Select(message => new TransportAlert(
                message.GetProperty("id").GetString()
                    ?? throw new InvalidDataException("DB Regio message has no id."),
                "DBREGIO",
                message.GetProperty("head").GetString()
                    ?? "DB Regio disruption",
                message.TryGetProperty("lineNames", out var lines)
                    && lines.GetArrayLength() > 0
                        ? lines[0].GetString()
                        : null))
            .ToArray();
    }
}

