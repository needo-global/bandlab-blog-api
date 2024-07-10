namespace Posts.Domain;

public class Comment
{
    public string Id { get; }
    public string PostId { get; }
    public string Content { get; }
    public string Creator { get; }
    public DateTime CreatedAt { get; }

    public Comment(string postId, string userId, string content)
    {
        Id = Guid.NewGuid().ToString();
        Content = content;
        PostId = postId;
        Creator = userId;
        CreatedAt = DateTime.UtcNow;
    }
}