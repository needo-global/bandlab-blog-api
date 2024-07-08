namespace Posts.Domain.Abstract;

public interface IPostService
{
    Task DeletePostCommentAsync(string userId, string postId, string commentId);
    Task<string> PostCommentAsync(string userId, string postId, string content);
}