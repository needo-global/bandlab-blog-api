namespace Posts.Infrastructure.Abstract;

public interface IImageProcessingService
{
    Task<byte[]> ConvertAsync(byte[] image);
}