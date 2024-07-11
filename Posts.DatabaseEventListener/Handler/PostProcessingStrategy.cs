using Posts.DatabaseEventListener.Handler.Abstract;
using Posts.Domain.Abstract;
using Posts.Domain;

namespace Posts.DatabaseEventListener.Handler;

internal class PostProcessingStrategy : IHandlerStrategy
{
    public string Type => "POST";

    public async Task Process<T>(T input) where T : IEventInstance
    {
        var comment = input as Post;
    }
}