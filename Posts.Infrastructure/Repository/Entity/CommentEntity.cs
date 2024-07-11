namespace Posts.Infrastructure.Repository.Entity;

internal class CommentEntity : BaseEntity
{
    public string PostId { get; set; }
    public string Content { get; set; }
}