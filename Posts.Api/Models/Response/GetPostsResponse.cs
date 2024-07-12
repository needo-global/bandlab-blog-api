namespace Posts.Api.Models.Response;

public class GetPostsResponse
{
    public IList<PostDto> Posts { get; }

    public string? LastPostToken => Posts?.LastOrDefault()?.Key;

    public GetPostsResponse(IList<PostDto> posts)
    {
        Posts = posts;
    }
}