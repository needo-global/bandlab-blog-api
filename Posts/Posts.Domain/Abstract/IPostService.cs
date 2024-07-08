namespace Posts.Domain.Abstract;

public interface IPostService
{
    Task DeletePostCommentAsync(string userId, string postId, string commentId);
}