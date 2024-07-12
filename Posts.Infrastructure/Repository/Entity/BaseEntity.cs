using Amazon.DynamoDBv2.DataModel;

namespace Posts.Infrastructure.Repository.Entity;

internal class BaseEntity
{
    [DynamoDBHashKey]
    public virtual string PK { get; set; }
    [DynamoDBRangeKey]
    public string SK { get; set; }
    public string Id { get; set; }
    public virtual string Type { get; set; }
    public string Creator { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}