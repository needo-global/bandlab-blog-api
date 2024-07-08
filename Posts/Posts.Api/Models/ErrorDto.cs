namespace Posts.Api.Models;

public class ErrorDto
{
    public string Code { get; }
    public string Message { get; }

    public ErrorDto(string code, string message)
    {
        Code = code;
        Message = message;
    }
}