using Posts.Domain;
using Posts.Domain.Abstract;

namespace Posts.Infrastructure.Repository;

public class PostRepository : IPostRepository
{
    public async Task<Post> GetPostCommentAsync(string postId)
    {
        throw new NotImplementedException();
    }

    public async Task DeletePostCommentAsync(string postId, string commentId)
    {
        throw new NotImplementedException();
    }
}