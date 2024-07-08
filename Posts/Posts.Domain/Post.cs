namespace Posts.Domain;

public class Post
{
    public string Id { get; set; }
    public string Caption { get; set; }
    public string Image { get; set; }
    public string Creator { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<Comment> Comments { get; set; }
}