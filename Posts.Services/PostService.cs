using Posts.Domain;
using Posts.Domain.Abstract;
using Posts.Domain.Exceptions;
using Posts.Infrastructure.Abstract;

namespace Posts.Services;

public class PostService : IPostService
{
    private readonly IPostCommandRepository _postCommandRepository;
    private readonly IPostQueryRepository _postQueryRepository;
    private readonly IStorage _storage;

    public PostService(IPostCommandRepository postCommandRepository, IPostQueryRepository postQueryRepository, IStorage storage)
    {
        _postCommandRepository = postCommandRepository;
        _postQueryRepository = postQueryRepository;
        _storage = storage;
    }

    public async Task<string> AddPostAsync(string userId, string caption, string fileName, byte[] content)
    {
        var path = $"/original/{fileName}";
        
        var url = await _storage.WriteAsync(path, content);

        var post = new Post(caption, null, userId);
        
        await _postCommandRepository.AddPostAsync(post, url);

        return post.Id;
    }

    public async Task<string> PostCommentAsync(string userId, string postId, string content)
    {
        var post = await _postCommandRepository.GetPostAsync(postId);

        if (post == null) throw new NotFoundException("post not found");

        var comment = new Comment(postId, userId, content);

        await _postCommandRepository.PostCommentAsync(postId, comment);

        return comment.Id;
    }

    public async Task DeletePostCommentAsync(string userId, string postId, string commentId)
    {
        var comment = await _postCommandRepository.GetPostCommentAsync(postId, commentId);

        if (comment == null) throw new NotFoundException("comment not found");

        if (!comment.Creator.Equals(userId, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Not the owner of the comment");

        await _postCommandRepository.DeletePostCommentAsync(postId, commentId);
    }

    public async Task<IList<Post>> GetPostsByPaging(string lastPostToken)
    {
        var posts = await _postQueryRepository.GetPostsByPaging(lastPostToken);
        return posts;
    }
}