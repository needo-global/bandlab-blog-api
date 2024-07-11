using Posts.Infrastructure.Abstract;

namespace Posts.Infrastructure;

public class ImageProcessingService : IImageProcessingService
{
    public async Task<byte[]> ConvertAsync(byte[] image)
    {
        // TODO - Implement efficient conversion to jpg and resizing
        return image; 
    }
}