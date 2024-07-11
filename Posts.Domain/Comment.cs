using Posts.Domain.Abstract;

namespace Posts.Domain;

public class Comment : IEventInstance
{
    public string Id { get; }
    public string PostId { get; }
    public string Content { get; }
    public string Creator { get; }
    public DateTime CreatedAt { get; }

    public Comment(string id, string postId, string userId, string content, DateTime createdAt)
    {
        Id = id;
        Content = content;
        PostId = postId;
        Creator = userId;
        CreatedAt = createdAt;
    }

    public Comment(string postId, string userId, string content) : this(Ulid.NewUlid().ToString(), postId, userId, content, DateTime.UtcNow)
    {
    }
}