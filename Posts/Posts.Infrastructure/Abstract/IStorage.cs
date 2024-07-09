namespace Posts.Infrastructure.Abstract;

public interface IStorage
{
    Task WriteAsync(string containerName, string fileName, byte[] content);
}