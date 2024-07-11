using Amazon.Lambda.DynamoDBEvents;
using Posts.Domain;

namespace Posts.DatabaseEventListener.Extensions;

public static class Mapper
{
    public static Post ToPost(this Dictionary<string, DynamoDBEvent.AttributeValue> record)
    {
        return new Post(
            record.SafeRetrieveValue("Id"),
            record.SafeRetrieveValue("Caption"),
            record.SafeRetrieveValue("Image"),
            record.SafeRetrieveValue("Creator"),
            record.SafeRetrieveDate("CreatedAt"));
    }

    public static Comment ToComment(this Dictionary<string, DynamoDBEvent.AttributeValue> record)
    {
        return new Comment(
            record.SafeRetrieveValue("Id"),
            record.SafeRetrieveValue("PostId"),
            record.SafeRetrieveValue("Creator"),
            record.SafeRetrieveValue("Content"),
            record.SafeRetrieveDate("CreatedAt"));
    }

    public static string SafeRetrieveValue(this Dictionary<string, DynamoDBEvent.AttributeValue> record, string attributeName)
    {
        return record.ContainsKey(attributeName)
            ? record[attributeName].S
            : string.Empty;
    }

    public static DateTime SafeRetrieveDate(this Dictionary<string, DynamoDBEvent.AttributeValue> record, string attributeName)
    {
        return record.ContainsKey(attributeName)
            ? DateTime.Parse(record[attributeName].S)
            : new DateTime();
    }
}