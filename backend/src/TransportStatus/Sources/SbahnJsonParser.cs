using System.Text.Json;
using TransportStatus.Domain;

namespace TransportStatus.Sources;

public static class SbahnJsonParser
{
    public static IReadOnlyList<TransportAlert> Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        var consequences = document.RootElement
            .GetProperty("data")
            .GetProperty("consequences");
        var alerts = new List<TransportAlert>();

        foreach (var consequence in consequences.EnumerateArray())
        {
            var affectsToday = consequence
                .GetProperty("timeFrames")
                .EnumerateArray()
                .Any(frame => frame.GetString() == "TODAY");
            if (!affectsToday)
            {
                continue;
            }

            alerts.Add(new TransportAlert(
                consequence.GetProperty("id").GetString()
                    ?? throw new InvalidDataException("S-Bahn consequence has no id."),
                "SBAHN",
                consequence.GetProperty("title").GetString()
                    ?? "S-Bahn disruption",
                consequence.TryGetProperty("lines", out var lines)
                    && lines.GetArrayLength() > 0
                    && lines[0].TryGetProperty("title", out var line)
                        ? line.GetString()
                        : null));
        }

        return alerts;
    }
}

