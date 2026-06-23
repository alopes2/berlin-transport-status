using TransportStatus.Sources;

namespace TransportStatus.Tests;

public sealed class DbRegioJsonParserTests
{
    [Fact]
    public void Converts_each_regional_message_to_an_alert()
    {
        const string json = """
        {
          "messages": [
            {
              "id": "regional-1",
              "head": "RE 2: Disruption",
              "lineNames": ["RE 2"]
            }
          ]
        }
        """;

        var alerts = DbRegioJsonParser.Parse(json);

        var alert = Assert.Single(alerts);
        Assert.Equal("regional-1", alert.Id);
        Assert.Equal("RE 2", alert.AffectedLine);
    }
}

