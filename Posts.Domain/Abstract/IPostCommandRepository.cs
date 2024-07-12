namespace Posts.Domain.Abstract;

public interface IPostCommandRepository
{
    Task AddPostAsync(Post post, string imageUrl);
    Task PostCommentAsync(string postId, Comment comment);
    Task DeletePostCommentAsync(string postId, string commentId);
    Task UpdatePostCommentsInfoAsync(string postId, int commentCountIncrement, int latestCommentsCount);
    Task UpdatePostAsync(Post post, string imageUrl);
}