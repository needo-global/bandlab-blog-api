using Amazon.DynamoDBv2.DataModel;

namespace Posts.Infrastructure.Repository.Entity;

internal class PostEntity : BaseEntity
{
    internal const string PostsByCommentCountIndex = "PostsByCommentCountIndex";

    [DynamoDBHashKey]
    public override string PK { get; set; }
    public string Caption { get; set; }
    public string Image { get; set; }
    public string OriginalImage { get; set; }
    [DynamoDBGlobalSecondaryIndexHashKey(PostsByCommentCountIndex)]
    public override string Type { get; set; }
    [DynamoDBGlobalSecondaryIndexRangeKey(PostsByCommentCountIndex)]
    public int CommentCount { get; set; }
    public string? RecentComments { get; set; }
}