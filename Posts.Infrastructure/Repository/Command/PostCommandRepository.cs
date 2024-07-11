using System.Text.Json;
using Posts.Domain;
using Posts.Domain.Abstract;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Posts.Infrastructure.Repository.Entity;
using Amazon.DynamoDBv2.DocumentModel;

namespace Posts.Infrastructure.Repository.Command;

public class PostCommandRepository : IPostCommandRepository
{
    private readonly IDynamoDBContext _context;

    private const string PostPkPrefix = "POST#";
    private const string PostSkPrefix = "POST#";
    private const string CommentPkPrefix = "COMMENT#";
    private const string CommentSkPrefix = "COMMENT#";

    private readonly DynamoDBOperationConfig _dbConfig = new()
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
        var postEntity = CreatePostEntity(post);
        postEntity.OriginalImage = imageUrl;
        await _context.SaveAsync(postEntity, _dbConfig);
    }

    public async Task PostCommentAsync(string postId, Comment comment)
    {
        var commentEntity = new CommentEntity
        {
            PK = $"{CommentPkPrefix}{postId}",
            SK = $"{CommentSkPrefix}{comment.Id}",
            Id = comment.Id,
            Type = Constants.Comment,
            PostId = postId,
            Content = comment.Content,
            Creator = comment.Creator,
            CreatedAt = comment.CreatedAt
        };

        await _context.SaveAsync(commentEntity, _dbConfig);
    }

    public async Task DeletePostCommentAsync(string postId, string commentId)
    {
        await _context.DeleteAsync<CommentEntity>($"{CommentPkPrefix}{postId}", $"{CommentSkPrefix}{commentId}", _dbConfig);
    }

    public async Task UpdatePostCommentsInfoAsync(string postId, int commentCountIncrement, int latestCommentsCount)
    {
        var postEntity = await _context.LoadAsync<PostEntity>($"{PostPkPrefix}{postId}", $"{PostSkPrefix}{postId}", _dbConfig);

        postEntity.CommentCount += commentCountIncrement;

        var qf = new QueryFilter();
        qf.AddCondition(nameof(CommentEntity.PK), QueryOperator.Equal, $"{CommentPkPrefix}{postId}");

        var queryConfig = new QueryOperationConfig
        {
            Filter = qf,
            Select = SelectValues.AllAttributes,
            Limit = 2,
            BackwardSearch = true
        };

        var recentComments = await _context.FromQueryAsync<CommentEntity>(queryConfig, _dbConfig).GetRemainingAsync();

        postEntity.RecentComments = JsonSerializer.Serialize(recentComments);
        postEntity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveAsync(postEntity, _dbConfig);
    }

    public async Task UpdatePostAsync(Post post)
    {
        var postEntity = CreatePostEntity(post);
        postEntity.OriginalImage = post.Image;
        postEntity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveAsync(postEntity, _dbConfig);
    }

    private PostEntity CreatePostEntity(Post post)
    {
        return new PostEntity
        {
            PK = $"{PostPkPrefix}{post.Id}",
            SK = $"{PostSkPrefix}{post.Id}",
            Id = post.Id,
            Type = Constants.Post,
            Image = post.Image,
            Caption = post.Caption,
            Creator = post.Creator,
            CreatedAt = post.CreatedAt,
            CommentCount = 0,
            RecentComments = null
        };
    }
}