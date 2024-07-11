using Posts.DatabaseEventListener.Handler.Abstract;
using Posts.Domain;
using Posts.Domain.Abstract;

namespace Posts.DatabaseEventListener.Handler;

internal class CommentProcessingStrategy : IHandlerStrategy
{
    private readonly IPostCommandRepository _postCommandRepository;

    public string Type => "COMMENT";

    public CommentProcessingStrategy(IPostCommandRepository postCommandRepository)
    {
        _postCommandRepository = postCommandRepository;
    }

    public async Task Process<T>(T input, string operation) where T : IEventInstance
    {
        var comment = input as Comment;

        await _postCommandRepository.UpdatePostCommentsInfoAsync(comment.PostId, operation.Equals("INSERT") ? 1 : -1, 2);
    }
}