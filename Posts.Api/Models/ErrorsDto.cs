namespace Posts.Api.Models;

public class ErrorsDto(IList<ErrorDto> errors)
{
    public IList<ErrorDto> Errors { get; set; } = errors;
}