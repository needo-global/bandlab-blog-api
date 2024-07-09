namespace Posts.Domain;

public class Post(string caption, string imageUrl, string userId)
{
    public string Id { get; set; } = Ulid.NewUlid().ToString();
    public string Caption { get; set; } = caption;
    public string Image { get; set; } = imageUrl;
    public string Creator { get; set; } = userId;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();
}