using Posts.Domain;
using Posts.Domain.Abstract;
using Posts.Domain.Exceptions;
using Posts.Infrastructure.Abstract;
using Posts.Infrastructure.Repository.Query;

namespace Posts.Services;

public class PostService(IPostCommandRepository postCommandRepository, IPostQueryRepository postQueryRepository, IStorage storage) : IPostService
{
    public async Task<string> AddPostAsync(string userId, string caption, string fileName, byte[] stream)
    {
        var bucketName = "band-lab-post-images"; // TODO - Move to configuration options
        var path = $"/original/{fileName}";
        
        await storage.WriteAsync(bucketName, path, stream);

        var post = new Post(caption, null, userId);

        var url = $"https://s3.amazonaws.com/{bucketName}{path}";
        await postCommandRepository.AddPostAsync(post, url);

        return post.Id;
    }

    public async Task<string> PostCommentAsync(string userId, string postId, string content)
    {
        var post = await postCommandRepository.GetPostAsync(postId);

        if (post == null) throw new NotFoundException("post not found");

        var comment = new Comment(postId, userId, content);

        await postCommandRepository.PostCommentAsync(postId, comment);

        return comment.Id;
    }

    public async Task DeletePostCommentAsync(string userId, string postId, string commentId)
    {
        var comment = await postCommandRepository.GetPostCommentAsync(postId);

        if (comment == null) throw new NotFoundException("comment not found");

        if (!comment.Creator.Equals(userId, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Not the owner of the comment");

        await postCommandRepository.DeletePostCommentAsync(postId, commentId);
    }

    public async Task<IList<Post>> GetPostsByPaging(string lastPostToken)
    {
        var posts = await postQueryRepository.GetPostsByPaging(lastPostToken);
        return posts;
    }
}