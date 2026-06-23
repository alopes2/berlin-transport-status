using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TransportStatus.Domain;

namespace TransportStatus.Sources;

public static partial class HtmlIssueParser
{
    private static readonly string[] AccessibilityTerms =
    [
        "lift",
        "elevator",
        "escalator",
        "aufzug",
        "rolltreppe"
    ];

    private static readonly string[] IssueTerms =
    [
        "delay",
        "delays",
        "cancel",
        "replacement service",
        "disruption",
        "closed",
        "interruption",
        "verspät",
        "ausfall",
        "ersatzverkehr",
        "störung",
        "unterbrochen",
        "gesperrt",
        "bauarbeiten"
    ];

    public static IReadOnlyList<TransportAlert> Parse(string html, string source)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return [];
        }

        var decoded = WebUtility.HtmlDecode(TagRegex().Replace(html, "\n"));
        var lines = decoded
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(NormalizeWhitespace)
            .Where(line => line.Length >= 8)
            .Where(IsTransportIssue)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return lines
            .Select(line => new TransportAlert(
                CreateId(source, line),
                source,
                line,
                LineRegex().Match(line) is { Success: true } match
                    ? match.Value.ToUpperInvariant()
                    : null))
            .ToArray();
    }

    private static bool IsTransportIssue(string line)
    {
        var lower = line.ToLowerInvariant();
        return !AccessibilityTerms.Any(lower.Contains)
            && IssueTerms.Any(lower.Contains)
            && LineRegex().IsMatch(line);
    }

    private static string NormalizeWhitespace(string value) =>
        WhitespaceRegex().Replace(value, " ").Trim();

    private static string CreateId(string source, string value)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{source}:{value}"));
        return Convert.ToHexString(hash)[..16].ToLowerInvariant();
    }

    [GeneratedRegex("<[^>]+>", RegexOptions.Singleline)]
    private static partial Regex TagRegex();

    [GeneratedRegex("\\s+")]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex("\\b(?:U|S|RE|RB)\\s?\\d{1,2}\\b", RegexOptions.IgnoreCase)]
    private static partial Regex LineRegex();
}
