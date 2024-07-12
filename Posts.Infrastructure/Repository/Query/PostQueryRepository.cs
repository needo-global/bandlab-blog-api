using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Posts.Domain;
using Posts.Domain.Abstract;
using Posts.Infrastructure.Repository.Entity;
using Amazon.DynamoDBv2.DocumentModel;

namespace Posts.Infrastructure.Repository.Query;

public class PostQueryRepository : IPostQueryRepository
{
    private readonly IDynamoDBContext _context;

    private const string PostPkPrefix = "POST#";
    private const string PostSkPrefix = "POST#";
    private const string CommentSkPrefix = "COMMENT#";

    private readonly DynamoDBOperationConfig _dbConfig = new()
    {
        Conversion = DynamoDBEntryConversion.V2,
        OverrideTableName = "bandlab-posts-api-develop-posts-data"// TODO Remove hard coding of table name, use Options
    };

    public PostQueryRepository(IDynamoDBContext context)
    {
        _context = context;
    }

    public async Task<Post?> GetPostAsync(string postId)
    {
        var entity = await _context.LoadAsync<PostEntity>($"{PostPkPrefix}{postId}", $"{PostSkPrefix}{postId}", _dbConfig);

        return entity == null ? null : new Post(entity.Id, entity.Caption, entity.Image, entity.Creator, entity.CreatedAt);
    }

    public async Task<Comment?> GetPostCommentAsync(string postId, string commentId)
    {
        var entity = await _context.LoadAsync<CommentEntity>($"{PostPkPrefix}{postId}", $"{CommentSkPrefix}{commentId}", _dbConfig);

        return entity == null ? null : new Comment(entity.Id, entity.PostId, entity.Creator, entity.Content, entity.CreatedAt);
    }

    public async Task<IList<Post>> GetPostsByPaging(string lastPageToken)
    {
        //var qf = new QueryFilter();
        //qf.AddCondition(nameof(PostEntity.PK), QueryOperator.Equal, $"{CommentPkPrefix}{postId}");

        //var queryConfig = new QueryOperationConfig
        //{
        //    IndexName = PostEntity.PostsByCommentCountIndex,
        //    Filter = qf,
        //    Select = SelectValues.AllAttributes,
        //    Limit = 10,
        //    BackwardSearch = true
        //};

        //var posts = await _context.FromQueryAsync<CommentEntity>(queryConfig, _dbConfig).GetNextSetAsync();
        return null;
    }
}