namespace Posts.Api;

public static class Constants
{
    public const double MaxFileSizeInBytes = 100 * 1024 * 1024; // TODO - Move to configuration options
    public static readonly string[] SupportedImageFormats = { ".png", ".jpg", ".bmp" }; // TODO - Move to configuration options
}