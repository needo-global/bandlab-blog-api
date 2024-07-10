using Posts.Domain;
using Posts.Domain.Abstract;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Posts.Infrastructure.Repository.Entity;

namespace Posts.Infrastructure.Repository.Command;

public class PostCommandRepository : IPostCommandRepository
{
    private readonly IDynamoDBContext _context;

    private const string PostPkPrefix = "POST#";
    private const string PostSkPrefix = "POST#";
    private const string CommentSkPrefix = "COMMENT#";

    protected readonly DynamoDBOperationConfig _dbConfig = new()
    {
        Conversion = DynamoDBEntryConversion.V2,
        OverrideTableName = "bandlab-posts-api-develop-posts-data"// TODO Remove hard coding of table name, use Options
    };

    public PostCommandRepository(IDynamoDBContext context)
    {
        _context = context;
    }

    public async Task AddPostAsync(Post post, string imageUrl)
    {
        var postEntity = new PostEntity
        {
            PK = $"{PostPkPrefix}{post.Id}",
            SK = $"{PostSkPrefix}#{post.Id}",
            Id = post.Id,
            Image = post.Image,
            OriginalImage = imageUrl,
            Caption = post.Caption,
            Creator = post.Creator,
            CreatedAt = post.CreatedAt
        };

        await _context.SaveAsync(postEntity, _dbConfig);
    }

    public async Task<Post?> GetPostAsync(string postId)
    {
        var entity = await _context.LoadAsync<PostEntity>($"{PostPkPrefix}{postId}", $"{PostSkPrefix}#{postId}", _dbConfig);

        return entity == null ? null : new Post(entity.Id, entity.Image, entity.Creator, entity.CreatedAt);
    }

    public async Task PostCommentAsync(string postId, Comment comment)
    {
        var commentEntity = new CommentEntity
        {
            PK = $"{PostPkPrefix}{postId}",
            SK = $"{CommentSkPrefix}#{comment.CreatedAt.Ticks}{comment.Id}",
            Id = comment.Id,
            Content = comment.Content,
            Creator = comment.Creator,
            CreatedAt = comment.CreatedAt
        };

        await _context.SaveAsync(commentEntity, _dbConfig);
    }

    public async Task<Comment?> GetPostCommentAsync(string postId, string commentId)
    {
        var entity = await _context.LoadAsync<CommentEntity>($"{PostPkPrefix}{postId}", $"{CommentSkPrefix}#{commentId}", _dbConfig);

        return entity == null ? null : new Comment(entity.Id, entity.Creator, entity.Content, entity.CreatedAt);
    }

    public async Task DeletePostCommentAsync(string postId, string commentId)
    {
        await _context.DeleteAsync<CommentEntity>($"{PostPkPrefix}{postId}", $"{CommentSkPrefix}#{commentId}", _dbConfig);
    }
}