namespace Posts.Infrastructure.Abstract;

public interface IStorage
{
    Task<string> WriteAsync(string fileName, byte[] content);

    Task<byte[]> ReadAsync(string fileName);
}