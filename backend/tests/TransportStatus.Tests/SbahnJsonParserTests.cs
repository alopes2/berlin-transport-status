using TransportStatus.Sources;

namespace TransportStatus.Tests;

public class SbahnJsonParserTests
{
  [Fact]
  public void Includes_only_consequences_affecting_today()
  {
    const string json = """
        {
          "data": {
            "consequences": [
              {
                "id": "today-1",
                "title": "Replacement service",
                "timeFrames": ["NOW", "TODAY"],
                "lines": [{ "title": "S7" }]
              },
              {
                "id": "future-1",
                "title": "Future construction",
                "timeFrames": ["FUTURE"],
                "lines": [{ "title": "S2" }]
              }
            ]
          }
        }
        """;

    var alerts = SbahnJsonParser.Parse(json);

    var alert = Assert.Single(alerts);
    Assert.Equal("today-1", alert.Id);
    Assert.Equal("S7", alert.AffectedLine);
  }
}

