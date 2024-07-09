namespace Posts.Domain.Abstract;

public interface IPostService
{
    Task<string> AddPostAsync(string userId, string caption, string fileName, byte[] stream);
    Task<string> PostCommentAsync(string userId, string postId, string content);
    Task DeletePostCommentAsync(string userId, string postId, string commentId);
}