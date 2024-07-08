using Posts.Domain;
using Posts.Domain.Abstract;

namespace Posts.Infrastructure.Repository;

public class PostCommandRepository : IPostCommandRepository
{
    public async Task<Post> GetPostCommentAsync(string postId)
    {
        throw new NotImplementedException();
    }

    public async Task DeletePostCommentAsync(string postId, string commentId)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> GetPostAsync(string postId)
    {
        throw new NotImplementedException();
    }

    public async Task PostCommentAsync(string postId, Comment comment)
    {
        throw new NotImplementedException();
    }
}