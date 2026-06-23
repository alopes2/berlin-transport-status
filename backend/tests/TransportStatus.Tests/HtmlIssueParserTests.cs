using TransportStatus.Sources;

namespace TransportStatus.Tests;

public sealed class HtmlIssueParserTests
{
    [Fact]
    public void Extracts_transport_disruptions_and_ignores_accessibility_outages()
    {
        var html = File.ReadAllText(Fixture("bvg-mixed.html"));

        var alerts = HtmlIssueParser.Parse(html, "BVG");

        var alert = Assert.Single(alerts);
        Assert.Equal("U2", alert.AffectedLine);
        Assert.Contains("replacement service", alert.Title, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Deduplicates_repeated_alert_content()
    {
        var html = File.ReadAllText(Fixture("sbahn-duplicates.html"));

        var alerts = HtmlIssueParser.Parse(html, "SBAHN");

        Assert.Single(alerts);
    }

    [Fact]
    public void Empty_page_has_no_active_transport_alerts()
    {
        var alerts = HtmlIssueParser.Parse("<html><body>No current disruptions</body></html>", "DBREGIO");

        Assert.Empty(alerts);
    }

    private static string Fixture(string name) =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", name);
}

