using TransportStatus.Sources;

namespace TransportStatus.Tests;

public sealed class BvgJsonParserTests
{
    [Fact]
    public void Includes_traffic_messages_and_excludes_elevator_messages()
    {
        const string json = """
        {
          "elements": [
            {
              "id": "traffic-1",
              "messageType": "TRAFFIC",
              "lines": [{ "subway": [{ "name": "U2" }] }],
              "content": [{ "headline": "Ersatzverkehr" }]
            },
            {
              "id": "lift-1",
              "messageType": "ELEVATOR",
              "lines": [{ "subway": [{ "name": "U8" }] }],
              "content": [{ "headline": "Aufzug außer Betrieb" }]
            }
          ]
        }
        """;

        var alerts = BvgJsonParser.Parse(json);

        var alert = Assert.Single(alerts);
        Assert.Equal("traffic-1", alert.Id);
        Assert.Equal("U2", alert.AffectedLine);
        Assert.Equal("Ersatzverkehr", alert.Title);
    }
}

