using Posts.Domain.Abstract;

namespace Posts.Domain;

public class Post : IEventInstance
{
    public string Id { get; }
    public string Caption { get; }
    public string Image { get; private set; }
    public string Creator { get; }
    public DateTime CreatedAt { get; }
    public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();

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

    public void SetConvertedImage(string convertedImageUrl)
    {
        Image = convertedImageUrl;
    }
}