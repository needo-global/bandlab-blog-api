using System.Drawing;
using System.Drawing.Imaging;
using Posts.Infrastructure.Abstract;

namespace Posts.Infrastructure;

public class ImageProcessingService : IImageProcessingService
{
    public async Task<byte[]> ConvertAsync(byte[] image)
    {
        var stream = new MemoryStream(image);
        Image img = new Bitmap(stream);

        var converted = new MemoryStream();
        img.Save(converted, ImageFormat.Jpeg);

        return converted.ToArray(); // TODO - Implement efficient resizing
    }
}