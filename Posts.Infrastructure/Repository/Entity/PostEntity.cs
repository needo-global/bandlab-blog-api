using Amazon.DynamoDBv2.DataModel;

namespace Posts.Infrastructure.Repository.Entity;

internal class PostEntity : BaseEntity
{
    internal const string PostsByCommentCountIndex = "PostsByCommentCountIndex";

    [DynamoDBHashKey]
    public string PK { get; set; }
    public string Caption { get; set; }
    public string Image { get; set; }
    public string OriginalImage { get; set; }
    public int CommentCount { get; set; }
    public IList<CommentEntity>? RecentComments { get; set; }
}