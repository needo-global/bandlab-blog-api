namespace Posts.Domain.Abstract;

public interface IPostQueryRepository
{
    Task<IList<Post>> GetPostsByPaging(string lastPageToken);
}