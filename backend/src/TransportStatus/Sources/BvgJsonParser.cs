using System.Text.Json;
using TransportStatus.Domain;

namespace TransportStatus.Sources;

public static class BvgJsonParser
{
    public static IReadOnlyList<TransportAlert> Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        if (!document.RootElement.TryGetProperty("elements", out var elements))
        {
            throw new InvalidDataException("BVG response has no elements array.");
        }

        var alerts = new List<TransportAlert>();
        foreach (var element in elements.EnumerateArray())
        {
            if (!element.TryGetProperty("messageType", out var messageType)
                || messageType.GetString() != "TRAFFIC")
            {
                continue;
            }

            var id = element.GetProperty("id").GetString()
                ?? throw new InvalidDataException("BVG alert has no id.");
            var title = element.TryGetProperty("content", out var content)
                && content.GetArrayLength() > 0
                && content[0].TryGetProperty("headline", out var headline)
                    ? headline.GetString() ?? "BVG transport disruption"
                    : "BVG transport disruption";
            alerts.Add(new TransportAlert(
                id,
                "BVG",
                title,
                FirstLine(element)));
        }

        return alerts;
    }

    private static string? FirstLine(JsonElement element)
    {
        if (!element.TryGetProperty("lines", out var lineGroups))
        {
            return null;
        }

        foreach (var group in lineGroups.EnumerateArray())
        {
            foreach (var transportType in group.EnumerateObject())
            {
                if (transportType.Value.GetArrayLength() > 0
                    && transportType.Value[0].TryGetProperty("name", out var name))
                {
                    return name.GetString();
                }
            }
        }

        return null;
    }
}

