namespace Posts.Domain.Abstract;

public interface IPostRepository
{
    Task<Post> GetPostCommentAsync(string postId);
    Task DeletePostCommentAsync(string postId, string commentId);
}