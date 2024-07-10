namespace Posts.Domain.Abstract;

public interface IPostCommandRepository
{
    Task AddPostAsync(Post post, string imageUrl);
    Task<Post?> GetPostAsync(string postId);
    Task<Comment?> GetPostCommentAsync(string postId, string commentId);
    Task PostCommentAsync(string postId, Comment comment);
    Task DeletePostCommentAsync(string postId, string commentId);
}