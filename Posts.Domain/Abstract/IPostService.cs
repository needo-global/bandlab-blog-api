namespace Posts.Domain.Abstract;

public interface IPostService
{
    Task<string> AddPostAsync(string userId, string caption, string fileName, byte[] content);
    Task<string> PostCommentAsync(string userId, string postId, string content);
    Task DeletePostCommentAsync(string userId, string postId, string commentId);
    Task<IList<Post>> GetPostsByPaging(string lastPostToken);
}