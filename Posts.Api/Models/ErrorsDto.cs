namespace Posts.Api.Models;

public class ErrorsDto
{
    public IList<ErrorDto> Errors { get; }

    public ErrorsDto(IList<ErrorDto> errors)
    {
        Errors = errors;
    }
}