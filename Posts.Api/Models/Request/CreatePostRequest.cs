using System.ComponentModel.DataAnnotations;

namespace Posts.Api.Models.Request;

public class CreatePostRequest : IValidatableObject
{
    public IFormFile? Image { get; set; }
    public string Filename => Image?.FileName ?? string.Empty;

    [Required(ErrorMessage = "The caption is required")]
    [MaxLength(100, ErrorMessage = "The caption is too long")]
    public string Caption { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Image == null) yield return new ValidationResult("The image is invalid");

        if (Image.Length > Constants.MaxFileSizeInBytes)
        {
            yield return new ValidationResult("The image is too large");
        }

        if (!Constants.SupportedImageFormats.Any(s => s.Equals(Path.GetExtension(Filename), StringComparison.OrdinalIgnoreCase)))
        {
            yield return new ValidationResult("The image is in invalid format");
        }
    }
}