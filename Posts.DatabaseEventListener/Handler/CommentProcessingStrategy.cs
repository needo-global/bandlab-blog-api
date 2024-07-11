using Posts.DatabaseEventListener.Handler.Abstract;
using Posts.Domain;
using Posts.Domain.Abstract;

namespace Posts.DatabaseEventListener.Handler;

internal class CommentProcessingStrategy : IHandlerStrategy
{
    public string Type => "COMMENT";

    public async Task Process<T>(T input) where T : IEventInstance
    {
        var comment = input as Comment;
    }
}