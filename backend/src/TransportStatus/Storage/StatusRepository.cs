using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using TransportStatus.Domain;

namespace TransportStatus.Storage;

public interface IStatusRepository
{
    Task<CompanyState?> GetAsync(string company, CancellationToken cancellationToken);
    Task SaveAsync(CompanyState state, CancellationToken cancellationToken);
    Task<IReadOnlyList<CompanyState>> GetAllAsync(CancellationToken cancellationToken);
}

public sealed class DynamoStatusRepository(
    IAmazonDynamoDB dynamoDb,
    string tableName) : IStatusRepository
{
    public async Task<CompanyState?> GetAsync(
        string company,
        CancellationToken cancellationToken)
    {
        var response = await dynamoDb.GetItemAsync(
            new GetItemRequest
            {
                TableName = tableName,
                Key = Key(company)
            },
            cancellationToken);
        return response.Item.Count == 0 ? null : FromItem(response.Item);
    }

    public async Task SaveAsync(
        CompanyState state,
        CancellationToken cancellationToken)
    {
        var currentItem = ToItem(state, "CURRENT");
        var dailyItem = ToItem(state, $"DAY#{state.BerlinDate:O}");
        await dynamoDb.PutItemAsync(
            new PutItemRequest
            {
                TableName = tableName,
                Item = currentItem
            },
            cancellationToken);
        await dynamoDb.PutItemAsync(
            new PutItemRequest
            {
                TableName = tableName,
                Item = dailyItem
            },
            cancellationToken);
    }

    public async Task<IReadOnlyList<CompanyState>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var states = new List<CompanyState>();
        foreach (var company in new[] { "BVG", "DB" })
        {
            var state = await GetAsync(company, cancellationToken);
            if (state is not null)
            {
                states.Add(state);
            }
        }

        return states;
    }

    private static Dictionary<string, AttributeValue> Key(
        string company,
        string sortKey = "CURRENT") =>
        new()
        {
            ["pk"] = new AttributeValue($"COMPANY#{company}"),
            ["sk"] = new AttributeValue(sortKey)
        };

    private static Dictionary<string, AttributeValue> ToItem(
        CompanyState state,
        string sortKey)
    {
        var item = Key(state.Company, sortKey);
        item["company"] = new AttributeValue(state.Company);
        item["berlinDate"] = new AttributeValue(state.BerlinDate.ToString("O"));
        item["trackingSince"] = new AttributeValue(state.TrackingSince.ToString("O"));
        item["currentStreakDays"] = new AttributeValue { N = state.CurrentStreakDays.ToString() };
        item["recordDays"] = new AttributeValue { N = state.RecordDays.ToString() };
        item["todayHadIssue"] = new AttributeValue { BOOL = state.TodayHadIssue };
        item["todayStatus"] = new AttributeValue(state.TodayStatus.ToString());
        item["dataStatus"] = new AttributeValue(state.DataStatus.ToString());
        item["activeIssueCount"] = new AttributeValue { N = state.ActiveIssueCount.ToString() };
        item["lastCheckedAt"] = new AttributeValue(state.LastCheckedAt.ToString("O"));
        return item;
    }

    private static CompanyState FromItem(Dictionary<string, AttributeValue> item) =>
        new(
            item["company"].S,
            DateOnly.Parse(item["berlinDate"].S),
            DateOnly.Parse(item["trackingSince"].S),
            int.Parse(item["currentStreakDays"].N),
            int.Parse(item["recordDays"].N),
            item["todayHadIssue"].BOOL ?? false,
            Enum.Parse<TodayStatus>(item["todayStatus"].S),
            Enum.Parse<DataStatus>(item["dataStatus"].S),
            int.Parse(item["activeIssueCount"].N),
            DateTimeOffset.Parse(item["lastCheckedAt"].S));
}
