using Posts.DatabaseEventListener.Handler.Abstract;
using Posts.Domain.Abstract;
using Posts.Domain;
using Posts.Infrastructure.Abstract;

namespace Posts.DatabaseEventListener.Handler;

internal class PostProcessingStrategy : IHandlerStrategy
{
    private readonly IStorage _storage;
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IPostCommandRepository _postCommandRepository;

    public string Type => "POST";

    public PostProcessingStrategy(
        IStorage storage, 
        IImageProcessingService imageProcessingService, 
        IPostCommandRepository postCommandRepository)
    {
        _storage = storage;
        _imageProcessingService = imageProcessingService;
        _postCommandRepository = postCommandRepository;
    }

    public async Task Process<T>(T input, string operation) where T : IEventInstance
    {
        var post = input as Post;

        var originalImage = await _storage.ReadAsync($"original/{post.Id}");

        var convertedImage = await _imageProcessingService.ConvertAsync(originalImage);

        var convertedImageUrl = await _storage.WriteAsync($"converted/{post.Id}", convertedImage);

        post.SetConvertedImage(convertedImageUrl);

        await _postCommandRepository.UpdatePostAsync(post);
    }
}