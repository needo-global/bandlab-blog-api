using Posts.Domain.Abstract;

namespace Posts.Domain;

public class Post : IEventInstance
{
    public string Id { get; }
    public string Caption { get; }
    public string Image { get; private set; }
    public string Creator { get; }
    public DateTime CreatedAt { get; }
    public IList<Comment> Comments { get; set; } = new List<Comment>();
    public int? CommentCount { get; }

    public Post(string caption, string imageUrl, string userId) : this(Ulid.NewUlid().ToString(), caption, imageUrl, userId, DateTime.UtcNow)
    {
    }

    public Post(string id, string caption, string imageUrl, string userId, DateTime createdAt)
    {
        Id = id;
        Caption = caption;
        Image = imageUrl;
        Creator = userId;
        CreatedAt = createdAt;
    }

    public Post(string id, string caption, string imageUrl, string userId, DateTime createdAt, IList<Comment> comments, int? commentCount = null)
    : this(id, caption, imageUrl, userId, createdAt)
    {
        Comments = comments;
        CommentCount = commentCount;
    }

    public void SetConvertedImage(string convertedImageUrl)
    {
        Image = convertedImageUrl;
    }
}