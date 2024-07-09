namespace Posts.Api.Models.Response;

public class GetPostsResponse(IList<PostDto> posts)
{
    public IList<PostDto> Posts { get; set; } = posts;

    public string? LastPostToken { get; set; } = posts?.LastOrDefault()?.Id;
}