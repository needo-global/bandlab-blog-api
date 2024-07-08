namespace Posts.Domain.Abstract;

public interface IPostCommandRepository
{
    Task<Post> GetPostCommentAsync(string postId);
    Task DeletePostCommentAsync(string postId, string commentId);
    Task<Post> GetPostAsync(string postId);
    Task PostCommentAsync(string postId, Comment comment);
}