namespace Posts.Domain.Abstract;

public interface IPostQueryRepository
{
    Task<Post?> GetPostAsync(string postId);
    Task<Comment?> GetPostCommentAsync(string postId, string commentId);
    Task<IList<Post>> GetPostsByPaging(string? lastPageToken);
}