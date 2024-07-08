using System.ComponentModel.DataAnnotations;

namespace Posts.Api.Models.Request;

public class CreatePostCommentRequest : IValidatableObject
{
    public string Content { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        throw new NotImplementedException();
    }
}