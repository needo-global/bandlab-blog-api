namespace Posts.Api.Models;

public class ErrorDto(string code, string message)
{
    public string Code { get; } = code;
    public string Message { get; } = message;
}