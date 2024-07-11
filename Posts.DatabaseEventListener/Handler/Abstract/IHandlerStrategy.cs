using Posts.Domain.Abstract;

namespace Posts.DatabaseEventListener.Handler.Abstract;

internal interface IHandlerStrategy
{
    string Type { get; }

    Task Process<T>(T input) where T : IEventInstance;
}