using Posts.Domain;
using Posts.Domain.Abstract;
using Posts.Domain.Exceptions;

namespace Posts.Services;

public class PostService(IPostCommandRepository postCommandRepository) : IPostService
{
    public async Task DeletePostCommentAsync(string userId, string postId, string commentId)
    {
        var comment = await postCommandRepository.GetPostCommentAsync(postId);

        if (comment == null) throw new NotFoundException("comment not found");

        if (!comment.Creator.Equals(userId, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Not the owner of the comment");

        await postCommandRepository.DeletePostCommentAsync(postId, commentId);
    }

    public async Task<string> PostCommentAsync(string userId, string postId, string content)
    {
        var post = await postCommandRepository.GetPostAsync(postId);

        if (post == null) throw new NotFoundException("post not found");

        var comment = new Comment(postId, userId, content);

        await postCommandRepository.PostCommentAsync(postId, comment);

        return comment.Id;
    }
}