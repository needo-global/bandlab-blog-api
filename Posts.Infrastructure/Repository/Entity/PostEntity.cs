namespace Posts.Infrastructure.Repository.Entity;

internal class PostEntity : BaseEntity
{
    public string Id { get; set; }
    public string Caption { get; set; }
    public string Image { get; set; }

    public string OriginalImage { get; set; }
    public string Creator { get; set; }
    public DateTime CreatedAt { get; set; }
}