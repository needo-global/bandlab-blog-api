namespace Posts.Api.Models
{
    public class PostDto
    {
        public string Id { get; set; }
        public string Caption { get; set; }
        public string Image { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<CommentDto> Comments { get; set; }
    }
}
