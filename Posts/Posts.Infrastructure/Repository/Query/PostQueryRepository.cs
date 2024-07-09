using Posts.Domain;
using Posts.Domain.Abstract;

namespace Posts.Infrastructure.Repository.Query;

public class PostQueryRepository : IPostQueryRepository
{
    public async Task<IList<Post>> GetPostsByPaging(string lastPageToken)
    {
        throw new NotImplementedException();
    }
}