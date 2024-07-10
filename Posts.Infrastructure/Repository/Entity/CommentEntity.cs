namespace Posts.Infrastructure.Repository.Entity;

internal class CommentEntity : BaseEntity
{
    public string Id { get; set; }
    public string Content { get; set; }
    public string Creator { get; set; }
    public DateTime CreatedAt { get; set; }
}