namespace Posts.Domain;

public class Comment(string postId, string userId, string content)
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PostId { get; set; } = postId;
    public string Content { get; set; } = content;
    public string Creator { get; set; } = userId;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}