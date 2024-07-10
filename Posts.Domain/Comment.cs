namespace Posts.Domain;

public class Comment
{
    public string Id { get; }
    public string PostId { get; }
    public string Content { get; }
    public string Creator { get; }
    public DateTime CreatedAt { get; }

    public Comment(string postId, string userId, string content, DateTime createdAt)
    {
        Id = Guid.NewGuid().ToString();
        Content = content;
        PostId = postId;
        Creator = userId;
        CreatedAt = createdAt;
    }

    public Comment(string postId, string userId, string content) : this(postId, userId, content, DateTime.UtcNow)
    {
    }
}