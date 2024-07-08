using Posts.Domain.Abstract;
using Posts.Domain.Exceptions;
using System.Net;

namespace Posts.Domain;

public class PostService(IPostRepository postRepository) : IPostService
{
    public async Task DeletePostCommentAsync(string userId, string postId, string commentId)
    {
        var comment = await postRepository.GetPostCommentAsync(postId);

        if (comment == null) throw new NotFoundException("comment not found");

        if (!comment.Creator.Equals(userId, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Not the owner of the comment");

        await postRepository.DeletePostCommentAsync(postId, commentId);
    }
}