using Posts.Infrastructure.Abstract;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Posts.Infrastructure;

public class ImageProcessingService : IImageProcessingService
{
    private const int ResizeWidth = 600;
    private const int ResizeHeight = 600;

    public async Task<byte[]> ConvertAsync(byte[] imageData)
    {
        using (var image = Image.Load(imageData))
        {
            using (var outStream = new MemoryStream())
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Position = AnchorPositionMode.Center,
                    Size = new Size(ResizeWidth, ResizeHeight)
                }));

                await image.SaveAsJpegAsync(outStream);

                return outStream.ToArray();
            }
        }
    }
}