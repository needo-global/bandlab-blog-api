namespace Posts.Api.Models;

public class CommentDto
{
    public string Id { get; set; }
    public string Content { get; set; }
    public string Creator { get; set; }
    public DateTime CreatedAt { get; set; }
}