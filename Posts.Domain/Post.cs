namespace Posts.Domain;

public class Post
{
    public string Id { get; }
    public string Caption { get; }
    public string Image { get; }
    public string Creator { get; }
    public DateTime CreatedAt { get; }
    public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();

    public Post(string caption, string imageUrl, string userId) : this(caption, imageUrl, userId, DateTime.UtcNow)
    {
    }

    public Post(string caption, string imageUrl, string userId, DateTime createdAt)
    {
        Id = Ulid.NewUlid().ToString();
        Caption = caption;
        Image = imageUrl;
        Creator = userId;
        CreatedAt = createdAt;
    }
}